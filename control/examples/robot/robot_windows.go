//Works on windows only, due to w32 binding
package main

import (
	"fmt"
	beam "github.com/xackery/gobeam"
	rawr "github.com/xackery/w32/user"
	"log"
	"time"
)

var delay = (1 * time.Second)

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

	//Create a robot session, copy auth data from chatbot
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
	log.Printf("[Chatbot] %s joined the channel.\n", m.Username)
}

func userLeave(s *beam.Session, m *beam.UserJoinEvent) {
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

func reportRobot(s *beam.Session, m *beam.ReportEvent) {
	log.Printf("[Robot] Report: %s\n", m)
	numButtons := 2
	if len(m.Tactile) < numButtons {
		fmt.Println("Number of buttons < m.Tactile")
		return
	}
	for _, key := range m.Tactile {
		if key.GetId() == 0 && key.GetHolding() == 1 {
			PressLeftPaddle()
			fmt.Println("[Robot] Press Left")
		}
		if key.GetId() == 2 && key.GetHolding() == 1 {
			PressRightPaddle()
			fmt.Println("[Robot] Press Right")
		}
		if key.GetId() == 1 && key.GetHolding() == 1 {
			PressStartButton()
			fmt.Println("[Robot] Press Start")
		}
	}
	return
}

func errorRobot(s *beam.Session, m *beam.ErrorEvent) {
	log.Printf("[Robot] Error: %s\n", m)
}

func PressLeftPaddle() {
	go sendKey(0x4B, delay) //a key
}

func PressRightPaddle() {
	go sendKey(0x4D, delay) //d key
}

func PressStartButton() {
	go sendKey(0x48, delay) //w key
}

func sendKey(keyCode uint16, duration time.Duration) (err error) {
	input := rawr.INPUT{
		Type: rawr.INPUT_KEYBOARD,
		Ki: rawr.KEYBDINPUT{
			WScan:   keyCode,
			DwFlags: 0x0008, //hold key
			Time:    0,
		},
	}
	inputs := []rawr.INPUT{}
	inputs = append(inputs, input)
	rawr.SendInput(inputs)
	time.Sleep(duration)
	inputs[0].Ki.DwFlags = 0x0002 | 0x0008 // release key
	//w32.ASendInput(inputs)
	time.Sleep(1 * time.Second)
	return
}
