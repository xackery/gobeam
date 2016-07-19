//Heavily inspired/derived from discordgo https://github.com/bwmarrin/discordgo

package gobeam

import ()

var eventToInterface = map[string]interface{}{
	"Connect":       ConnectEvent{},       //Chat and Interactive
	"Disconnect":    DisconnectEvent{},    //Chat and Interactive
	"ChatMessage":   ChatMessageEvent{},   //Chat
	"DeleteMessage": DeleteMessageEvent{}, //Chat
	"PurgeMessage":  PurgeMessageEvent{},  //Chat
	"ClearMessages": ClearMessagesEvent{}, //Chat
	"UserJoin":      UserJoinEvent{},      //Chat
	"UserLeave":     UserLeaveEvent{},     //Chat
	"UserUpdate":    UserUpdateEvent{},    //Chat
	"UserTimeout":   UserTimeoutEvent{},   //Chat
	"PollStart":     PollStartEvent{},     //Chat
	"PollEnd":       PollEndEvent{},       //Chat
	"Report":        ReportEvent{},        //Interactive
	"Error":         ErrorEvent{},         //Interactive
}

type ConnectEvent struct{}

type DisconnectEvent struct{}

type ChatMessageEvent struct {
	*ChatMessage
}

//Only contains an ID
type DeleteMessageEvent struct {
	*ChatMessage
}

//Only contains a userid
type PurgeMessageEvent struct {
	*ChatMessage
}

//No data usually contained
type ClearMessagesEvent struct {
	*ChatMessage
}

type UserJoinEvent struct {
	*User
}

type UserLeaveEvent struct {
	*User
}

type UserUpdateEvent struct {
	*User
}

type UserTimeoutEvent struct {
	*UserTimeout
}

type PollStartEvent struct {
	*Poll
}

type PollEndEvent struct {
	*Poll
}

type ReportEvent struct {
	*Report
}

type ErrorEvent struct {
	*Error
}
