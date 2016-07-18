//Heavily inspired/derived from discordgo https://github.com/bwmarrin/discordgo

package gobeam

import ()

var eventToInterface = map[string]interface{}{
	"Connect":     ConnectEvent{},     //Chat and Interactive
	"Disconnect":  DisconnectEvent{},  //Chat and Interactive
	"ChatMessage": ChatMessageEvent{}, //Chat
	"UserJoin":    UserJoinEvent{},    //Chat
	"UserLeave":   UserLeaveEvent{},   //Chat
	"Report":      ReportEvent{},      //Interactive
	"Error":       ErrorEvent{},       //Interactive
}

type ConnectEvent struct{}

type DisconnectEvent struct{}

type ChatMessageEvent struct {
	*ChatMessage
}

type UserJoinEvent struct {
	*NewUser
}

type UserLeaveEvent struct {
	*NewUser
}

type ReportEvent struct {
	*Report
}

type ErrorEvent struct {
	*Error
}
