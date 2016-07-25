//Heavily inspired/derived from discordgo https://github.com/bwmarrin/discordgo

package gobeam

import (
	"bytes"
	//"compress/zlib"
	//"encoding/binary"
	"encoding/json"
	"errors"
	"fmt"
	"github.com/golang/protobuf/proto"
	"github.com/gorilla/websocket"
	"io"
	"io/ioutil"
	"log"
	"net/http"
	"reflect"
	"time"
)

type handshakeProperties struct {
	OS              string `json:"$os"`
	Browser         string `json:"$browser"`
	Device          string `json:"$device"`
	Referer         string `json:"$referer"`
	ReferringDomain string `json:"$referring_domain"`
}

type handshakeData struct {
	Version        int                 `json:"v"`
	Token          string              `json:"token"`
	Properties     handshakeProperties `json:"properties"`
	LargeThreshold int                 `json:"large_threshold"`
	Compress       bool                `json:"compress"`
}

type handshakeOp struct {
	Op   int           `json:"op"`
	Data handshakeData `json:"d"`
}

// Open opens a websocket connection to Beam.
func (s *Session) Open() (err error) {

	s.Lock()
	defer func() {
		if err != nil {
			s.Unlock()
		}
	}()

	if s.wsConn != nil {
		err = errors.New("Web socket already opened.")
		return
	}

	s.Type = "Chat"
	// Get the gateway to use for the Websocket connection
	g, err := s.Gateway()
	if err != nil {
		return
	}

	header := http.Header{}
	header.Add("accept-encoding", "zlib")

	// TODO: See if there's a use for the http response.
	// conn, response, err := websocket.DefaultDialer.Dial(session.Gateway, nil)
	s.wsConn, _, err = websocket.DefaultDialer.Dial(g, header)
	if err != nil {
		return
	}

	/*err = s.wsConn.WriteJSON(handshakeOp{2, handshakeData{3, s.Token, handshakeProperties{runtime.GOOS, "beamgo v" + VERSION, "", "", ""}, 250, s.Compress}})
	if err != nil {
		return
	}*/

	args := make([]interface{}, 3)
	args[0] = s.LoginPayload.Channel.ID
	args[1] = s.LoginPayload.ID
	args[2] = s.authKey

	err = s.wsConn.WriteJSON(&Event{
		Type:      "method",
		Method:    "auth",
		Arguments: args,
	})
	if err != nil {
		return
	}

	// Create listening outside of listen, as it needs to happen inside the mutex
	// lock.
	s.listening = make(chan interface{})
	go s.listen(s.wsConn, s.listening)

	s.Unlock()

	s.initialize()
	s.handle(&ConnectEvent{})

	if !s.isReplyMonitorAlive {
		go s.replyMonitor()
	}

	return
}

func (s *Session) OpenRobot() (err error) {

	s.Lock()
	defer func() {
		if err != nil {
			s.Unlock()
		}
	}()

	if s.wsConn != nil {
		err = errors.New("Web socket already opened.")
		return
	}

	s.Type = "Interactive"

	// Get the gateway to use for the Websocket connection
	g, err := s.TetrisGateway()
	if err != nil {
		return
	}
	if s.Debug {
		log.Println("[Robot] Websocket connection")
	}

	header := http.Header{}
	header.Add("accept-encoding", "zlib")

	resp := &http.Response{}
	//fmt.Println("Dialing ", g, "and key", s.authKey, resp)

	s.wsConn, resp, err = websocket.DefaultDialer.Dial(fmt.Sprintf("%s/robot", g), header)

	if resp.StatusCode != 200 {
		defer func() {
			err := resp.Body.Close()
			if err != nil {
				fmt.Println("error closing resp body")
			}
		}()

		var response []byte
		response, err = ioutil.ReadAll(resp.Body)
		if err != nil {
			return
		}
		err = fmt.Errorf("Failed to establish websocket (%d): %s", resp.StatusCode, string(response))
	}

	hs := &Handshake{
		Channel:   s.LoginPayload.Channel.ID,
		StreamKey: s.authKey,
	}

	bhs, err := proto.Marshal(hs)
	if err != nil {
		return
	}

	//the mysterious 0x08? unsure it's usage, but it makes things work on handshake
	bPayload := []byte{0x00, 0x08, byte(len(bhs))}
	for _, b := range bhs {
		bPayload = append(bPayload, b)
	}

	err = s.wsConn.WriteMessage(websocket.BinaryMessage, bPayload)
	if err != nil {
		return
	}

	// Create listening outside of listen, as it needs to happen inside the mutex
	// lock.
	s.listening = make(chan interface{})
	go s.listen(s.wsConn, s.listening)

	s.Unlock()

	s.initialize()
	s.handle(&ConnectEvent{})
	if !s.isReplyMonitorAlive {
		go s.replyMonitor()
	}

	return
}

// Close closes a websocket and stops all listening/heartbeat goroutines.
func (s *Session) Close() (err error) {
	s.Lock()

	s.DataReady = false

	if s.listening != nil {
		close(s.listening)
		s.listening = nil
	}

	if s.wsConn != nil {
		err = s.wsConn.Close()
		s.wsConn = nil
	}

	s.Unlock()

	s.handle(&DisconnectEvent{})

	return
}

// listen polls the websocket connection for events, it will stop when
// the listening channel is closed, or an error occurs.
func (s *Session) listen(wsConn *websocket.Conn, listening <-chan interface{}) {
	for {
		messageType, message, err := wsConn.ReadMessage()
		/*if s.Debug {
			fmt.Printf("[%s] New message %d: % x (%d)\n", s.Type, messageType, string(message), len(message))
		}*/
		if err != nil {
			// Detect if we have been closed manually. If a Close() has already
			// happened, the websocket we are listening on will be different to the
			// current session.
			s.RLock()
			sameConnection := s.wsConn == wsConn
			s.RUnlock()
			if sameConnection {
				// There has been an error reading, Close() the websocket so that
				// OnDisconnect is fired.
				err := s.Close()
				if err != nil {
					fmt.Println("error closing session connection: ", err)
				}

				// Attempt to reconnect, with expenonential backoff up to 10 minutes.
				if s.ShouldReconnectOnError {
					wait := time.Duration(1)
					for {
						if s.Open() == nil {
							return
						}
						<-time.After(wait * time.Second)
						wait *= 2
						if wait > 600 {
							wait = 600
						}
					}
				}
			}
			return
		}

		select {
		case <-listening:
			return
		default:
			go s.event(messageType, message)
		}
	}
}

type heartbeatOp struct {
	Op   int `json:"op"`
	Data int `json:"d"`
}

// heartbeat sends regular heartbeats to Beam so it knows the client
// is still connected.  If you do not send these heartbeats Beam will
// disconnect the websocket connection after a few seconds.
func (s *Session) heartbeat(wsConn *websocket.Conn, listening <-chan interface{}, i time.Duration) {

	if listening == nil || wsConn == nil {
		return
	}

	s.Lock()
	s.DataReady = true
	s.Unlock()

	var err error
	ticker := time.NewTicker(i * time.Millisecond)
	for {
		err = wsConn.WriteJSON(heartbeatOp{1, int(time.Now().Unix())})
		if err != nil {
			fmt.Println("Error sending heartbeat:", err)
			return
		}

		select {
		case <-ticker.C:
			// continue loop and send heartbeat
		case <-listening:
			return
		}
	}
}

type updateStatusGame struct {
	Name string `json:"name"`
}

type updateStatusData struct {
	IdleSince *int              `json:"idle_since"`
	Game      *updateStatusGame `json:"game"`
}

type updateStatusOp struct {
	Op   int              `json:"op"`
	Data updateStatusData `json:"d"`
}

// UpdateStatus is used to update the authenticated user's status.
// If idle>0 then set status to idle.  If game>0 then set game.
// if otherwise, set status to active, and no game.
func (s *Session) UpdateStatus(idle int, game string) (err error) {
	s.RLock()
	defer s.RUnlock()
	if s.wsConn == nil {
		return errors.New("No websocket connection exists.")
	}

	var usd updateStatusData
	if idle > 0 {
		usd.IdleSince = &idle
	}
	if game != "" {
		usd.Game = &updateStatusGame{game}
	}

	err = s.wsConn.WriteJSON(updateStatusOp{3, usd})

	return
}

// Front line handler for all Websocket Events.  Determines the
// event type and passes the message along to the next handler.

// event is the front line handler for all events.  This needs to be
// broken up into smaller functions to be more idiomatic Go.
// Events will be handled by any implemented handler in Session.
// All unhandled events will then be handled by OnEvent.
func (s *Session) event(messageType int, message []byte) {
	var err error
	var reader io.Reader
	reader = bytes.NewBuffer(message)

	//fmt.Printf("[%s] Parsing Event\n", s.Type)

	var e *Event

	if s.Type == "Interactive" {
		/*if s.Debug {
			fmt.Printf("[%s] Wire messageType %d (%d len)\n", s.Type, messageType, len(message))
		}*/

		if len(message) < 1 {
			if s.Debug {
				fmt.Printf("[%s] discarded wire", s.Type)
			}
			return
		}

		switch messageType {
		case 0: //Hanshake, should never see
			break
		case 1: //hanshake ack
			//fmt.Printf("[%s] handshake ack!\n", s.Type)
			break
		case 2: //report
			/*
				if s.Debug {
					fmt.Printf("[%s] report: % x\n", s.Type, string(message))
				}
			*/
			if len(message) < 5 {
				return
			}
			report := &Report{}
			err = proto.Unmarshal(message[1:], report)
			if err != nil {
				fmt.Println("Error unmarshalling report", err.Error())
				return
			}
			s.handle(&ReportEvent{Report: report})
			return
		case 3: //error
			fmt.Printf("[%s] error\n", s.Type)
			errorResp := &Error{}
			if len(message) < 5 {
				return
			}
			err = proto.Unmarshal(message[1:], errorResp)
			if err != nil {
				fmt.Println("Error unmarshalling error", err.Error())
				return
			}
			s.handle(&ErrorEvent{Error: errorResp})
			break
		case 4: //progress update
			fmt.Printf("[%s] progress update\n", s.Type)
			break
		default:
			fmt.Printf("[%s] unknown messagetype: %d\n", s.Type, messageType)
			break
		}

		//fmt.Printf("[%s] dump: %s\n", s.Type, string(bData))
		return
	}

	//Chat events are handled here
	decoder := json.NewDecoder(reader)

	if err = decoder.Decode(&e); err != nil {
		fmt.Printf("[%s] jsonDecode event: %s\n", s.Type, err.Error())
		return
	}

	if s.Debug {
		printEvent(s.Type, e)
	}

	//On reply, send it to eventReplyChan to be consumed
	if e.Type == "reply" {
		s.eventReplyChan <- e
	}

	if e != nil && e.Type == "event" {
		i := eventToInterface[e.Event]
		if s.Debug {
			fmt.Println("Event translated to", e.Event)
		}
		if i != nil {
			// Create a new instance of the event type.
			i = reflect.New(reflect.TypeOf(i)).Interface()

			// Attempt to unmarshal our event.
			// If there is an error we should handle the event itself.
			if err = unmarshal(e.Data, i); err != nil {
				fmt.Printf("[%s] Unable to unmarshal event data.\n", s.Type)
				i = e
			}
		} else {
			fmt.Printf("[%s] Unknown event.\n", s.Type)
			i = e
		}

		s.handle(i)
	}
	return
}

func (s *Session) GetTransactionId() (tx int) {
	s.lastTransactionId++
	tx = s.lastTransactionId
	return
}

type TransactionBuffer struct {
	TransactionId      int
	TransactionChannel chan Event
	TransactionTimeout time.Time
}

//one thread of this
func (s *Session) replyMonitor() {
	s.eventReplyChan = make(chan *Event)
	s.transactionSubChan = make(chan TransactionBuffer)
	subscribers := []TransactionBuffer{}
	if s.Debug {
		fmt.Println("Initialized reply monitor")
	}
	//watch for events and subscribers
	for {
		select {
		case sub := <-s.transactionSubChan:
			if s.Debug {
				fmt.Println("Added subscriber ", sub.TransactionId)
			}
			//add new subscribers
			subscribers = append(subscribers, sub)
			break
		case event := <-s.eventReplyChan:
			if s.Debug {
				fmt.Println("Consuming event, checking for subs", len(subscribers))
			}
			//find any subscribers for event
			for i, sub := range subscribers {
				//Cleanup any subscribers who did not get response fast enough
				if sub.TransactionTimeout.Before(time.Now()) {
					//Remove subscription
					subscribers[i] = subscribers[len(subscribers)-1]
					subscribers = subscribers[:len(subscribers)-1]
					continue
				}
				if s.Debug {
					fmt.Println(event.Id, "vs ", sub.TransactionId)
				}
				if event.Id == sub.TransactionId {
					//add event to transaction
					sub.TransactionChannel <- *event
					//Remove subscription
					subscribers[i] = subscribers[len(subscribers)-1]
					subscribers = subscribers[:len(subscribers)-1]
					break
				}
			}
			break
		}
	}
	s.isReplyMonitorAlive = false
}

// Send a Chat Message. An active winsock connection must be active.
func (s *Session) Msg(message string) (chatMessage *ChatMessage, err error) {
	s.RLock()
	defer s.RUnlock()
	tx := s.GetTransactionId()

	if s.wsConn == nil {
		err = errors.New("No websocket connection exists.")
		return
	}

	if s.Type != "Chat" {
		err = fmt.Errorf("Invalid session type, needs to be chat: %s\n", s.Type)
		return
	}

	//Make a channel to inform of us of the reply
	msgChan := make(chan Event, 10)
	s.transactionSubChan <- TransactionBuffer{
		TransactionId:      tx,
		TransactionChannel: msgChan,
		TransactionTimeout: time.Now().Add(3 * time.Second),
	}

	evt := &Event{
		Type:   "method",
		Method: "msg",
		Id:     tx,
	}

	evt.Arguments = append(evt.Arguments, message)
	err = s.wsConn.WriteJSON(evt)
	//wait for the reply, or timeout
	select {
	case reply := <-msgChan:
		//If error exists, return that
		if len(reply.Error) > 0 {
			err = fmt.Errorf("%s", reply.Error)
			return
		}
		//Try to parse reply as chat message
		chatMessage = &ChatMessage{}
		err = json.Unmarshal(reply.Data, chatMessage)
		if err != nil {
			return
		}
		break
	case <-time.After(10 * time.Second):
		err = fmt.Errorf("Response timeout on transaction %d", tx)
		break
	}
	return
}

//Whisper a user
func (s *Session) Whisper(username string, message string) (err error) {
	s.RLock()
	defer s.RUnlock()

	if s.wsConn == nil {
		return errors.New("No websocket connection exists.")
	}

	if s.Type != "Chat" {
		err = fmt.Errorf("Invalid session type, needs to be chat: %s\n", s.Type)
		return
	}

	evt := &Event{
		Type:   "method",
		Method: "whisper",
		Id:     5, //TODO: Make a unique transaction number requester
	}

	evt.Arguments = append(evt.Arguments, username)
	evt.Arguments = append(evt.Arguments, message)

	err = s.wsConn.WriteJSON(evt)
	return
}

//Casts a vote for the current poll, arguments is the vote option index.
func (s *Session) Vote(option int) (err error) {
	s.RLock()
	defer s.RUnlock()

	if s.wsConn == nil {
		return errors.New("No websocket connection exists.")
	}

	if s.Type != "Chat" {
		err = fmt.Errorf("Invalid session type, needs to be chat: %s\n", s.Type)
		return
	}

	evt := &Event{
		Type:   "method",
		Method: "vote",
		Id:     3, //TODO: Make a unique transaction number requester
	}
	evt.Arguments = append(evt.Arguments, option)

	err = s.wsConn.WriteJSON(evt)
	return
}

//Times a User out, they cannot send messages until the duration is over.
func (s *Session) Timeout(username string, duration string) (err error) {
	s.RLock()
	defer s.RUnlock()

	if s.wsConn == nil {
		return errors.New("No websocket connection exists.")
	}

	if s.Type != "Chat" {
		err = fmt.Errorf("Invalid session type, needs to be chat: %s\n", s.Type)
		return
	}

	evt := &Event{
		Type:   "method",
		Method: "timeout",
		Id:     4, //TODO: Make a unique transaction number requester
	}
	evt.Arguments = append(evt.Arguments, username)
	evt.Arguments = append(evt.Arguments, duration)

	err = s.wsConn.WriteJSON(evt)
	return
}

// Request previous messages from this chat from before you joined.
// The argument controls how many messages are requested. 100 is the maximum.
func (s *Session) History(messageCount int) (err error) {
	if messageCount > 100 {
		err = fmt.Errorf("messageCount must be less than 101")
		return
	}
	if messageCount < 1 {
		messageCount = 1
	}
	s.RLock()
	defer s.RUnlock()

	if s.wsConn == nil {
		return errors.New("No websocket connection exists.")
	}

	if s.Type != "Chat" {
		err = fmt.Errorf("Invalid session type, needs to be chat: %s\n", s.Type)
		return
	}

	evt := &Event{
		Type:   "method",
		Method: "history",
		Id:     1, //TODO: Make a unique transaction number requester
	}
	evt.Arguments = append(evt.Arguments, messageCount)

	err = s.wsConn.WriteJSON(evt)
	return
}

//Start a giveaway in the channel.
func (s *Session) Giveaway() (err error) {
	s.RLock()
	defer s.RUnlock()

	if s.wsConn == nil {
		return errors.New("No websocket connection exists.")
	}

	if s.Type != "Chat" {
		err = fmt.Errorf("Invalid session type, needs to be chat: %s\n", s.Type)
		return
	}

	evt := &Event{
		Type:   "method",
		Method: "giveaway",
		Id:     1, //TODO: Make a unique transaction number requester
	}

	err = s.wsConn.WriteJSON(evt)
	return
}

//A ping method, used in environments where websocket implementations do not natively support pings.
func (s *Session) Ping() (err error) {
	s.RLock()
	defer s.RUnlock()

	if s.wsConn == nil {
		return errors.New("No websocket connection exists.")
	}

	if s.Type != "Chat" {
		err = fmt.Errorf("Invalid session type, needs to be chat: %s\n", s.Type)
		return
	}

	evt := &Event{
		Type:   "method",
		Method: "ping",
		Id:     1, //TODO: Make a unique transaction number requester
	}

	err = s.wsConn.WriteJSON(evt)
	return
}

//A prog event may be sent up periodically at the behest of the Robot. It contains an array of objects for multiple controls on the frontend.
func (s *Session) ProgressUpdate(prg *ProgressUpdate) (err error) {
	if prg == nil {
		err = fmt.Errorf("Empty progress sent")
		return
	}

	if s.wsConn == nil {
		err = fmt.Errorf("No websocket connection exists.")
		return
	}
	if s.Type != "Interactive" {
		err = fmt.Errorf("Invalid session type, needs to be interactive: %s\n", s.Type)
		return
	}

	s.RLock()
	defer s.RUnlock()

	bhs, err := proto.Marshal(prg)
	if err != nil {
		return
	}

	//the mysterious 0x08? unsure it's usage, but it makes things work on handshake
	bPayload := []byte{0x04}
	for _, b := range bhs {
		bPayload = append(bPayload, b)
	}
	if s.Debug {
		fmt.Printf("sending ProgressUpdate message % x\n", bPayload)
	}
	err = s.wsConn.WriteMessage(websocket.BinaryMessage, bPayload)
	if err != nil {
		return
	}
	return
}
