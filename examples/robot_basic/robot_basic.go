package main

import (
	"fmt"
	beam "github.com/xackery/gobeam"
	"log"
	"time"
)

func main() {

	//Create a robot session
	robot := beam.Session{
		UseCookies: true,
		Debug:      true,
	}

	//subscribe to robot events
	//robot.AddHandler(reportRobot)
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

	id := uint32(0)
	isFired := true
	//cooldown := uint32(1000)
	progress := float64(0.1)

	jid := uint32(9)
	jangle := float64(0.1)
	intensity := float64(0.1)

	for {
		time.Sleep(1500 * time.Millisecond)
		progress += 0.1
		if progress > 1 {
			progress = 0
		}
		jangle += 0.1
		if jangle > 1 {
			jangle = 0
		}
		intensity += 0.1
		if intensity > 1 {
			intensity = 1
		}

		err = robot.ProgressUpdate(&beam.ProgressUpdate{
			Tactile: []*beam.ProgressUpdate_TactileUpdate{
				&beam.ProgressUpdate_TactileUpdate{
					Id:    &id,
					Fired: &isFired,
					//	Cooldown: &cooldown,
					Progress: &progress,
				},
			},
			Joystick: []*beam.ProgressUpdate_JoystickUpdate{
				&beam.ProgressUpdate_JoystickUpdate{
					Id:        &jid,
					Angle:     &jangle,
					Intensity: &intensity,
				},
			},
		})
		if err != nil {
			fmt.Println("error sending progress update:", err.Error())
		}
	}
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
