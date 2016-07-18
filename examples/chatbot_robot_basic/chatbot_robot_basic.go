package main

import (
	"fmt"
	beam "github.com/xackery/gobeam"
	"log"
)

func main() {
	//Create a chatbot session.
	chatbot := beam.Session{
		Debug: false,
	}

	//subscribe to events
	chatbot.AddHandler(userJoin)
	chatbot.AddHandler(userLeave)
	chatbot.AddHandler(userChat)

	//Log in
	log.Println("[Chatbot] Logging in")
	err := chatbot.Login()
	if err != nil {
		log.Println("[Chatbot] Error logging in:", err.Error())
		return
	}

	//Create a interactive session, copy auth data from chatbot
	robot := beam.Session{
		Debug:        false,
		Cookies:      chatbot.Cookies,
		LoginPayload: chatbot.LoginPayload,
	}

	//subscribe to robot events
	robot.AddHandler(reportRobot)
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

	// Simple way to keep program running until any key press.
	var input string
	fmt.Scanln(&input)
	return
}

func userJoin(s *beam.Session, m *beam.UserJoinEvent) {
	log.Println("[Chatbot] %s joined the channel.\n", m.Username)
}

func userLeave(s *beam.Session, m *beam.UserJoinEvent) {
	log.Printf("[Chatbot] %s left the channel.\n", m.Username)
}

func userChat(s *beam.Session, m *beam.ChatMessageEvent) {
	log.Printf("[Chatbot] %s: %s\n", m.Username, m.Message.Messages[0].Text)
}

func reportRobot(s *beam.Session, m *beam.ReportEvent) {
	log.Printf("[Robot] Report: %s\n", m)
}

func errorRobot(s *beam.Session, m *beam.ErrorEvent) {
	log.Printf("[Robot] Error: %s\n", m)
}
