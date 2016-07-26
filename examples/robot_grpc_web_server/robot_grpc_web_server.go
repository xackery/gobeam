package main

import (
	"fmt"
	beam "github.com/xackery/gobeam"
	"golang.org/x/net/context"
	"google.golang.org/grpc"
	"log"
	"net"
	"time"
)

var delay = (1 * time.Second)

type robotService struct {
	session *beam.Session
}

type streamSession struct {
	reportChan chan *beam.Report
}

func main() {

	var err error
	//Create a chatbot session.
	chatbot := beam.Session{
		Debug:           false,
		UseCookies:      true,
		TimeoutDuration: 2 * time.Second,
	}

	//subscribe to events
	chatbot.AddHandler(userJoin)
	chatbot.AddHandler(userLeave)
	chatbot.AddHandler(userChat)

	//Log in
	log.Println("[Chatbot] Logging in")
	err = chatbot.Login()
	if err != nil {
		log.Println("[Chatbot] Error logging in:", err.Error())
		return
	}

	//Create a robot session, copy auth data from chatbot
	robot := beam.Session{
		Debug:        false,
		UseCookies:   true,
		Cookies:      chatbot.Cookies,
		LoginPayload: chatbot.LoginPayload,
		CsrfToken:    chatbot.CsrfToken,
	}

	//Make a robot service provider, and a channel
	rs := &robotService{
		session: &robot,
	}

	//subscribe to robot events
	robot.AddHandler(errorRobot)

	// Open chatbot websocket
	log.Println("[Chatbot] Connecting")
	err = chatbot.Open()
	if err != nil {
		log.Println("[Chatbot] Error opening winsock:", err.Error())
		return
	}
	log.Println("[Chatbot] Success!")

	//Open robot websocket
	log.Println("[Robot] Connecting")
	err = robot.OpenRobot()
	if err != nil {
		log.Println("[Robot] Error opening winsock:", err)
		return
	}
	log.Println("[Robot] Success!")

	//Start grpc listener
	addr := "127.0.0.1:50051"
	log.Println("[gRPC] Listening on", addr)
	lis, err := net.Listen("tcp", addr)
	if err != nil {
		log.Println("[gRPC] Failed to listen:", err.Error())
		return
	}

	s := grpc.NewServer()
	beam.RegisterRobotServiceServer(s, rs)
	go s.Serve(lis)

	webbot := &WebServer{
		Robot:   &robot,
		Chatbot: &chatbot,
	}
	go webbot.ListenAndServe("127.0.0.1:1234")

	/*chanResp, err := chatbot.Channels(0)
	if err != nil {
		fmt.Println("Error getting channel repsonse", err.Error())
	}
	fmt.Println(chanResp)
	*/
	// Simple way to keep program running until any key press.
	var input string
	fmt.Scanln(&input)
	return
}

func userJoin(s *beam.Session, m *beam.UserJoinEvent) {
	log.Printf("[Chatbot] %s joined the channel.\n", m.Username)
}

func userLeave(s *beam.Session, m *beam.UserLeaveEvent) {
	log.Printf("[Chatbot] %s left the channel.\n", m.Username)
}

func userChat(s *beam.Session, m *beam.ChatMessageEvent) {
	//Whisper will set a target
	if m.Target != "" {
		if len(m.Message.Messages) < 1 {
			fmt.Printf("[Chatbot]  [%d] %s WHISPER=> %s (No Message?)\n", m.UserId, m.Target, m.Username)
			return
		}
		log.Printf("[Chatbot] [%d] %s WHISPER=> %s: %s\n", m.UserId, m.Username, m.Target, m.Message.Messages[0].Text)
		return
	}
	if len(m.Message.Messages) < 1 {
		fmt.Printf("[Chatbot] [%d] %s (No Message?)\n", m.UserId, m.Username)
		return
	}
	log.Printf("[Chatbot] [%d] %s: %s\n", m.UserId, m.Username, m.Message.Messages[0].Text)
}

func errorRobot(s *beam.Session, m *beam.ErrorEvent) {
	log.Printf("[Robot] Error: %s\n", m)
}

func (ss *streamSession) reportRobot(s *beam.Session, m *beam.ReportEvent) {
	//Flush any old reports (used when a client isn't actively consuming them)
	for len(ss.reportChan) > 0 {
		<-ss.reportChan
	}
	//Add new report to buffer
	ss.reportChan <- m.Report
}

func (rs *robotService) ProgressUpdate(ctx context.Context, req *beam.ProgressUpdateRequest) (resp *beam.ProgressUpdateResponse, err error) {
	err = rs.session.ProgressUpdate(req.ProgressUpdate)
	return
}

//Stream the report output to a connected RPC client
func (rs *robotService) StreamReport(req *beam.StreamRequest, stream beam.RobotService_StreamReportServer) (err error) {

	fmt.Println("[gRPC] Client connected")
	//Create a stream session
	ss := &streamSession{
		reportChan: make(chan *beam.Report, 10),
	}
	//subscribe to report events
	rs.session.AddHandler(ss.reportRobot)

	for {
		newReport := <-ss.reportChan
		if err := stream.Send(newReport); err != nil {
			return err
		}
	}
}
