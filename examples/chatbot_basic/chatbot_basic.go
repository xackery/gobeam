package main

import (
	"fmt"
	beam "github.com/xackery/gobeam"
	"log"
	"time"
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

	// Open chatbot websocket
	log.Println("[Chatbot] Connecting")
	err = chatbot.Open()
	if err != nil {
		log.Println("[Chatbot] Error opening winsock:", err.Error())
		return
	}
	log.Println("[Chatbot] Success!")
	time.Sleep(1 * time.Second)

	//Send a message
	err = chatbot.Msg("Hello, world!")
	if err != nil {
		log.Println("[Chatbot] Error sending message:", err.Error())
		return
	}

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
