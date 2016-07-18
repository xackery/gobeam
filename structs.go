//Heavily inspired/derived from discordgo https://github.com/bwmarrin/discordgo

package gobeam

import (
	"encoding/json"
	"github.com/gorilla/websocket"
	"net/http"
	"reflect"
	"sync"
)

type LoginPayload struct {
	Level  int `json:"level,omitempty"`
	Social struct {
		Verified []interface{} `json:"verified,omitempty"`
	} `json:"social,omitempty"`
	ID          int    `json:"id,omitempty"`
	Username    string `json:"username,omitempty"`
	Email       string `json:"email,omitempty"`
	Verified    bool   `json:"verified,omitempty"`
	Experience  int    `json:"experience,omitempty"`
	Sparks      int    `json:"sparks,omitempty"`
	AvatarURL   int    `json:"avatarUrl,omitempty"`
	AllowEmail  bool   `json:"allowEmail,omitempty"`
	Bio         int    `json:"bio,omitempty"`
	PrimaryTeam int    `json:"primaryTeam,omitempty"`
	CreatedAt   string `json:"createdAt,omitempty"`
	UpdatedAt   string `json:"updatedAt,omitempty"`
	DeletedAt   int    `json:"deletedAt,omitempty"`
	Groups      []struct {
		ID   int    `json:"id,omitempty"`
		Name string `json:"name,omitempty"`
	} `json:"groups,omitempty"`
	Channel struct {
		ID                   uint32 `json:"id,omitempty"`
		UserID               int    `json:"userId,omitempty"`
		Token                string `json:"token,omitempty"`
		Online               bool   `json:"online,omitempty"`
		Featured             bool   `json:"featured,omitempty"`
		Partnered            bool   `json:"partnered,omitempty"`
		TranscodingProfileID int    `json:"transcodingProfileId,omitempty"`
		Suspended            bool   `json:"suspended,omitempty"`
		Name                 string `json:"name,omitempty"`
		Audience             string `json:"audience,omitempty"`
		ViewersTotal         int    `json:"viewersTotal,omitempty"`
		ViewersCurrent       int    `json:"viewersCurrent,omitempty"`
		NumFollowers         int    `json:"numFollowers,omitempty"`
		Description          int    `json:"description,omitempty"`
		TypeID               int    `json:"typeId,omitempty"`
		Interactive          bool   `json:"interactive,omitempty"`
		TetrisGameID         int    `json:"tetrisGameId,omitempty"`
		Ftl                  int    `json:"ftl,omitempty"`
		HasVod               bool   `json:"hasVod,omitempty"`
		LanguageID           int    `json:"languageId,omitempty"`
		CoverID              int    `json:"coverId,omitempty"`
		ThumbnailID          int    `json:"thumbnailId,omitempty"`
		BadgeID              int    `json:"badgeId,omitempty"`
		HosteeID             int    `json:"hosteeId,omitempty"`
		CreatedAt            string `json:"createdAt,omitempty"`
		UpdatedAt            string `json:"updatedAt,omitempty"`
		DeletedAt            int    `json:"deletedAt,omitempty"`
	} `json:"channel,omitempty"`
	HasTwoFactor bool `json:"hasTwoFactor,omitempty"`
	TwoFactor    struct {
		Enabled     bool `json:"enabled,omitempty"`
		CodesViewed bool `json:"codesViewed,omitempty"`
	} `json:"twoFactor,omitempty"`
	Preferences struct {
		ChatTimestamps       bool   `json:"chat:timestamps,omitempty"`
		ChatSoundsPlay       string `json:"chat:sounds:play,omitempty"`
		ChatWhispers         bool   `json:"chat:whispers,omitempty"`
		ChatSoundsHTML5      bool   `json:"chat:sounds:html5,omitempty"`
		ChatChromakey        bool   `json:"chat:chromakey,omitempty"`
		ChatLurkmode         bool   `json:"chat:lurkmode,omitempty"`
		ChannelMatureAllowed bool   `json:"channel:mature:allowed,omitempty"`
		ChannelNotifications struct {
			Ids        []string `json:"ids,omitempty"`
			Transports []string `json:"transports,omitempty"`
		} `json:"channel:notifications,omitempty"`
		ChannelPlayer struct {
			Vod  string `json:"vod,omitempty"`
			Rtmp string `json:"rtmp,omitempty"`
			Ftl  string `json:"ftl,omitempty"`
		} `json:"channel:player,omitempty"`
		ChatTagging      bool `json:"chat:tagging,omitempty"`
		ChatColors       bool `json:"chat:colors,omitempty"`
		ChatSoundsVolume int  `json:"chat:sounds:volume,omitempty"`
	} `json:"preferences,omitempty"`
}

// A Session represents a connection to the Discord API.
type Session struct {
	Type    string //Session Type
	Cookies []*http.Cookie
	sync.RWMutex
	endpoints    []string
	authKey      string //ws authkey
	LoginPayload *LoginPayload
	config       *GoBeamConfig
	// General configurable settings.

	// Authentication token for this session
	Token string

	// Debug for printing JSON request/responses
	Debug bool

	// Should the session reconnect the websocket on errors.
	ShouldReconnectOnError bool

	// Should the session request compressed websocket data.
	Compress bool

	// Should state tracking be enabled.
	// State tracking is the best way for getting the the users
	// active guilds and the members of the guilds.
	StateEnabled bool

	// Exposed but should not be modified by User.

	// Whether the Data Websocket is ready
	DataReady bool

	// Whether the Voice Websocket is ready
	VoiceReady bool

	// Whether the UDP Connection is ready
	UDPReady bool

	// Managed state object, updated internally with events when
	// StateEnabled is true.
	State *State

	handlersMu sync.RWMutex
	// This is a mapping of event struct to a reflected value
	// for event handlers.
	// We store the reflected value instead of the function
	// reference as it is more performant, instead of re-reflecting
	// the function each event.
	handlers map[interface{}][]reflect.Value

	// The websocket connection.
	wsConn *websocket.Conn

	// When nil, the session is not listening.
	listening chan interface{}
}

type NewUser struct {
	Username string   `json:"username"`
	roles    []string `json:"roles"`
	id       int      `json:"id"`
}

// An Event provides a basic initial struct for all websocket event.
type Event struct {
	Type      string          `json:"type,omitempty"`
	Event     string          `json:"event,omitempty"`
	Method    string          `json:"method,omitempty"`
	Id        int             `json:"id,omitempty"`
	Arguments []interface{}   `json:"arguments,omitempty"`
	Error     string          `json:"error,omitempty"`
	Data      json.RawMessage `json:"data,omitempty"` //json.RawMessage `json:"data"`
}

type ChatMessage struct {
	Channel   int      `json:"channel"`
	Id        string   `json:"id"`
	Username  string   `json:"user_name"`
	UserId    int      `json:"user_id"`
	UserRoles []string `json:"user_roles"`
	Message   struct {
		Messages []ChatMessageDetail `json:"message"`
	} `json:"message"`
	Meta string `json:"meta"`
}

type ChatMessageDetail struct {
	Type string `json:"type"`
	Data string `json:"data"`
	Text string `json:"text"`
}

// A State contains the current known state.
// As discord sends this in a READY blob, it seems reasonable to simply
// use that struct as the data store.
type State struct {
	sync.RWMutex
	//	Ready
	MaxMessageCount int
}
