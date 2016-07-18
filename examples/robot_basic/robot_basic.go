package main

import (
	"fmt"
	beam "github.com/xackery/gobeam"
	"log"
)

func main() {

	//Create a robot session
	robot := beam.Session{
		Debug: false,
	}

	//subscribe to robot events
	robot.AddHandler(reportRobot)
	robot.AddHandler(errorRobot)

	//Log in
	log.Println("[Robot] Logging in")
	err := robot.Login()
	if err != nil {
		log.Println("[Chatbot] Error logging in:", err.Error())
		return
	}

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

func reportRobot(s *beam.Session, m *beam.ReportEvent) {
	log.Printf("[Robot] Report: %s\n", m)
}

func errorRobot(s *beam.Session, m *beam.ErrorEvent) {
	log.Printf("[Robot] Error: %s\n", m)
}
