package main

import (
	"encoding/json"
	"fmt"
	beam "github.com/xackery/gobeam"
	"io"
	"io/ioutil"
	"log"
	"net/http"
	"strings"
)

type WebServer struct {
	Debug   bool
	Chatbot *beam.Session
	Robot   *beam.Session
}

type Rest struct {
	Status  int64       `json:"status"`
	Message string      `json:"message,omitempty"`
	Data    interface{} `json:"data,omitempty"`
}

func (ws *WebServer) ListenAndServe(addr string) (err error) {
	http.HandleFunc("/", ws.RouteHandler)
	if ws.Debug {
		log.Println("[Webserver] Listening on", addr)
	}
	err = http.ListenAndServe(addr, nil)
	log.Println("[Webserver] Crash error:", err.Error())
	return
}

//Handle various routes
func (ws *WebServer) RouteHandler(w http.ResponseWriter, r *http.Request) {
	var err error

	w.Header().Set("Access-Control-Allow-Origin", "*")

	rest := &Rest{
		Status:  0,
		Message: "Unhandled Error",
	}

	if strings.Contains(r.RequestURI, "/api/") && r.Method != "POST" {
		rest.Message = "Invalid request method."
		rest.Data = nil
		rest.Status = 0
		err = json.NewEncoder(w).Encode(rest)
		if err != nil {
			fmt.Println("Error decoding rest:", err.Error)
		}
		return
	}

	if len(r.RequestURI) == 0 {
		r.RequestURI = "/"
	}
	if r.RequestURI[len(r.RequestURI)-1:len(r.RequestURI)] != "/" {
		r.RequestURI += "/"
	}
	r.RequestURI = strings.ToLower(r.RequestURI)

	switch r.RequestURI {
	case "/":
		if r.RequestURI == "/" {
			r.RequestURI = "/index.html"
		}
		http.FileServer(http.Dir("www/")).ServeHTTP(w, r)
		return
	case "/api/msg/":
		err = ws.Msg(w, r, rest)
	case "/api/channels/":
		err = ws.Channels(w, r, rest)
	default:
		if !strings.Contains("/api/", r.RequestURI) {
			http.FileServer(http.Dir("www/")).ServeHTTP(w, r)
			return
		}
		err = fmt.Errorf("Endpoint not found")
		break
	}

	//All errors for every route is handled inside the rest struct
	if err != nil {
		rest.Status = 0
		rest.Message = err.Error()
		rest.Data = nil
	} else {
		rest.Status = 1
		if rest.Message == "" {
			rest.Message = "Completed successfully."
		}
	}

	err = json.NewEncoder(w).Encode(rest)
	if err != nil {
		fmt.Println("Error decoding rest:", err.Error)
	}
	//fmt.Println("Index")
}

func (ws *WebServer) Msg(w http.ResponseWriter, r *http.Request, rest *Rest) (err error) {
	if ws.Chatbot == nil {
		err = fmt.Errorf("No chatbot found")
		return
	}
	if ws.Chatbot.Type != "Chat" {
		err = fmt.Errorf("Chatbot not initialized")
		return
	}
	body, err := ws.readBody(r.Body)
	if err != nil {
		return
	}
	if len(body) == 0 {
		err = fmt.Errorf("No data supplied")
		return
	}
	type MsgRequest struct {
		Message string `json:"message"`
	}
	msgReq := &MsgRequest{}

	err = json.Unmarshal([]byte(body), msgReq)
	if err != nil {
		return
	}
	if len(msgReq.Message) < 2 {
		err = fmt.Errorf("Message needs to be at least 3 characters long")
		return
	}

	resp, err := ws.Chatbot.Msg(msgReq.Message)
	if err != nil {
		return
	}
	rest.Status = 1
	rest.Message = "Success"
	rest.Data = resp
	return
}

func (ws *WebServer) Channels(w http.ResponseWriter, r *http.Request, rest *Rest) (err error) {
	if ws.Chatbot == nil {
		err = fmt.Errorf("No chatbot found")
		return
	}
	if ws.Chatbot.Type != "Chat" {
		err = fmt.Errorf("Chatbot not initialized")
		return
	}
	body, err := ws.readBody(r.Body)
	if err != nil {
		return
	}
	type ChannelsRequest struct {
		ChannelId uint32 `json:"channelId"`
	}

	chanReq := &ChannelsRequest{}

	var cid uint32
	if len(body) > 0 {

		err = json.Unmarshal([]byte(body), chanReq)
		if err != nil {
			return
		}
		cid = chanReq.ChannelId
	}
	if cid < 1 {
		cid = 0
	}

	resp, err := ws.Chatbot.Channels(cid)
	if err != nil {
		return
	}
	rest.Status = 1
	rest.Message = "Success"
	rest.Data = resp
	return
}

/*
Read the body of a http request and convert it to string
*/
func (ws *WebServer) readBody(r io.ReadCloser) (body string, err error) {
	data, err := ioutil.ReadAll(r)
	if err != nil {
		return
	}
	body = string(data)
	return
}
