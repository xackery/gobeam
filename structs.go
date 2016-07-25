//Heavily inspired/derived from discordgo https://github.com/bwmarrin/discordgo

package gobeam

import (
	"encoding/json"
	"github.com/gorilla/websocket"
	"net/http"
	"reflect"
	"sync"
	"time"
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
	AvatarURL   string `json:"avatarUrl,omitempty"`
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
		ID                       uint32 `json:"id,omitempty"`
		UserID                   int    `json:"userId,omitempty"`
		Token                    string `json:"token,omitempty"`
		Online                   bool   `json:"online,omitempty"`
		Featured                 bool   `json:"featured,omitempty"`
		Partnered                bool   `json:"partnered,omitempty"`
		TranscodingProfileID     int    `json:"transcodingProfileId,omitempty"`
		Suspended                bool   `json:"suspended,omitempty"`
		Name                     string `json:"name,omitempty"`
		Audience                 string `json:"audience,omitempty"`
		ViewersTotal             int    `json:"viewersTotal,omitempty"`
		ViewersCurrent           int    `json:"viewersCurrent,omitempty"`
		NumFollowers             int    `json:"numFollowers,omitempty"`
		NumSubscribers           int    `json:"numSubscribers,omitempty"`
		MaxConcurrentSubscribers int    `json:"maxConcurrentSubscribers,omitempty"`
		Description              string `json:"description,omitempty"`
		TypeID                   int    `json:"typeId,omitempty"`
		Interactive              bool   `json:"interactive,omitempty"`
		TetrisGameID             int    `json:"tetrisGameId,omitempty"`
		Ftl                      int    `json:"ftl,omitempty"`
		HasVod                   bool   `json:"hasVod,omitempty"`
		LanguageID               int    `json:"languageId,omitempty"`
		CoverID                  int    `json:"coverId,omitempty"`
		ThumbnailID              int    `json:"thumbnailId,omitempty"`
		BadgeID                  int    `json:"badgeId,omitempty"`
		HosteeID                 int    `json:"hosteeId,omitempty"`
		CreatedAt                string `json:"createdAt,omitempty"`
		UpdatedAt                string `json:"updatedAt,omitempty"`
		DeletedAt                int    `json:"deletedAt,omitempty"`
	} `json:"channel,omitempty"`
	TwoFactor struct {
		Enabled     bool `json:"enabled,omitempty"`
		CodesViewed bool `json:"codesViewed,omitempty"`
	} `json:"twoFactor,omitempty"`
	HasTwoFactor bool `json:"hasTwoFactor,omitempty"`
	Preferences  struct {
		ChatSoundsPlay       string `json:"chat:sounds:play,omitempty"`
		ChatSoundsHTML5      bool   `json:"chat:sounds:html5,omitempty"`
		ChatTimestamps       bool   `json:"chat:timestamps,omitempty"`
		ChatWhispers         bool   `json:"chat:whispers,omitempty"`
		ChatChromakey        bool   `json:"chat:chromakey,omitempty"`
		ChatLurkmode         bool   `json:"chat:lurkmode,omitempty"`
		ChannelNotifications struct {
			Ids        []string `json:"ids,omitempty"`
			Transports []string `json:"transports,omitempty"`
		} `json:"channel:notifications,omitempty"`
		ChannelMatureAllowed bool `json:"channel:mature:allowed,omitempty"`
		ChannelPlayer        struct {
			Vod  string `json:"vod,omitempty"`
			Rtmp string `json:"rtmp,omitempty"`
			Ftl  string `json:"ftl,omitempty"`
		} `json:"channel:player,omitempty"`
		ChatTagging      bool `json:"chat:tagging,omitempty"`
		ChatColors       bool `json:"chat:colors,omitempty"`
		ChatSoundsVolume int  `json:"chat:sounds:volume,omitempty"`
	} `json:"preferences,omitempty"`
}

// A Session represents a connection to the Beam API.
type Session struct {
	// Debug for printing JSON request/responses
	Debug bool
	//Timeout for calls
	TimeoutDuration time.Duration

	Type    string //Session Type
	Cookies []*http.Cookie
	sync.RWMutex
	endpoints           []string
	authKey             string //ws authkey
	LoginPayload        *LoginPayload
	config              *GoBeamConfig
	CsrfToken           string
	UseCookies          bool
	lastTransactionId   int
	eventReplyChan      chan *Event
	transactionSubChan  chan TransactionBuffer
	isReplyMonitorAlive bool
	// General configurable settings.

	// Authentication token for this session
	Token string

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

type User struct {
	Id          int      `json:"id"`
	UserId      int      `json:"user"` //Used by UserUpdate
	Username    string   `json:"username"`
	Roles       []string `json:"roles"`
	Permissions []string `json:"permissions"`
}

//All chat websocket events are handled with this.
type Event struct {
	Type      string          `json:"type,omitempty"`
	Event     string          `json:"event,omitempty"`
	Method    string          `json:"method,omitempty"`
	Id        int             `json:"id,omitempty"`
	Arguments []interface{}   `json:"arguments,omitempty"`
	Error     string          `json:"error,omitempty"`
	Data      json.RawMessage `json:"data,omitempty"` //json.RawMessage `json:"data"`
}

//Handles websocket events of chatmessage
type ChatMessage struct {
	Channel   int      `json:"channel"`
	Id        string   `json:"id"`
	Username  string   `json:"user_name"`
	UserId    int      `json:"user_id"`
	UserRoles []string `json:"user_roles"`
	Message   struct {
		Messages []ChatMessageDetail `json:"message"`
	} `json:"message"`
	Meta   ChatMeta `json:"meta,omitempty"`
	Target string   `json:"target,omitempty"` //Used by whisper
}

//Part of chatmessage
type ChatMeta struct {
	Whisper bool `json:"whisper,omitempty"` //Omitted when whisper is not true
	Me      bool `json:"me,omitempty"`      //This is you do an event
}

//Part of chatmessage
type ChatMessageDetail struct {
	Type     string `json:"type"`
	Data     string `json:"data"`
	Text     string `json:"text"`
	Path     string `json:"path"`
	Url      string `json:"url"`
	Source   string `json:"source"`
	Pack     string `json:"pack"`
	Coords   Coords `json:"coords"`
	Username string `json:"username"` //Used when tagging
	Id       int    `json:"id"`       //Used when tagging
}

//Part of chatmessage
type Coords struct {
	X      float64 `json:"x"`
	Y      float64 `json:"y"`
	Width  float64 `json:"width"`
	Height float64 `float:"height"`
}

//Used by PollStart and PollEnd
type Poll struct {
	Q         string   `json:"q"`       //Question being asked
	Answers   []string `json:"answers"` //Possible options for answers
	Author    Author   `json:"author"`
	Duration  int      `json:"duration"`
	EndsAt    int      `json:"endsAt"`
	Voters    int      `json:"voters"`
	Responses struct {
		Good int `json:"good"`
		Bad  int `json:"bad"`
	} `json:"responses"`
}

//Used by Poll
type Author struct {
	Username  string   `json:"user_name"`
	UserId    int      `json:"user_id"`
	UserRoles []string `json:"user_roles"`
}

type Channel struct {
	ID                   int    `json:"id,omitempty"`
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
	Description          string `json:"description,omitempty"`
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
	Thumbnail            int    `json:"thumbnail,omitempty"`
	Cover                struct {
		Meta struct {
			Small string `json:"small,omitempty"`
		} `json:"meta,omitempty"`
		ID         int    `json:"id,omitempty"`
		Type       string `json:"type,omitempty"`
		Relid      int    `json:"relid,omitempty"`
		URL        string `json:"url,omitempty"`
		Store      string `json:"store,omitempty"`
		RemotePath string `json:"remotePath,omitempty"`
		CreatedAt  string `json:"createdAt,omitempty"`
		UpdatedAt  string `json:"updatedAt,omitempty"`
	} `json:"cover,omitempty"`
	Badge int `json:"badge,omitempty"`
	Type  struct {
		ID             int    `json:"id,omitempty"`
		Name           string `json:"name,omitempty"`
		Parent         string `json:"parent,omitempty"`
		Description    string `json:"description,omitempty"`
		Source         string `json:"source,omitempty"`
		ViewersCurrent int    `json:"viewersCurrent,omitempty"`
		CoverURL       string `json:"coverUrl,omitempty"`
		Online         int    `json:"online,omitempty"`
	} `json:"type,omitempty"`
	Cache       []interface{} `json:"cache,omitempty"`
	Preferences struct {
		CostreamAllow                 string `json:"costream:allow,omitempty"`
		Sharetext                     string `json:"sharetext,omitempty"`
		ChannelLinksClickable         bool   `json:"channel:links:clickable,omitempty"`
		ChannelLinksAllowed           bool   `json:"channel:links:allowed,omitempty"`
		ChannelSlowchat               int    `json:"channel:slowchat,omitempty"`
		ChannelNotifyFollow           bool   `json:"channel:notify:follow,omitempty"`
		ChannelNotifyFollowmessage    string `json:"channel:notify:followmessage,omitempty"`
		ChannelNotifyHostedBy         string `json:"channel:notify:hostedBy,omitempty"`
		ChannelNotifyHosting          string `json:"channel:notify:hosting,omitempty"`
		ChannelNotifySubscribemessage string `json:"channel:notify:subscribemessage,omitempty"`
		ChannelNotifySubscribe        bool   `json:"channel:notify:subscribe,omitempty"`
		ChannelPartnerSubmail         string `json:"channel:partner:submail,omitempty"`
		ChannelPlayerMuteOwn          bool   `json:"channel:player:muteOwn,omitempty"`
		ChannelTweetEnabled           bool   `json:"channel:tweet:enabled,omitempty"`
		ChannelTweetBody              string `json:"channel:tweet:body,omitempty"`
	} `json:"preferences,omitempty"`
	User struct {
		Level  int `json:"level,omitempty"`
		Social struct {
			Verified []interface{} `json:"verified,omitempty"`
		} `json:"social,omitempty"`
		ID          int    `json:"id,omitempty"`
		Username    string `json:"username,omitempty"`
		Verified    bool   `json:"verified,omitempty"`
		Experience  int    `json:"experience,omitempty"`
		Sparks      int    `json:"sparks,omitempty"`
		AvatarURL   string `json:"avatarUrl,omitempty"`
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
			ID                   int    `json:"id,omitempty"`
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
			Description          string `json:"description,omitempty"`
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
	} `json:"user,omitempty"`
}

//When a timeout event is received
type UserTimeout struct {
	User     User `json:"user"`
	Duration int
}

// A State contains the current known state.
// As Beam sends this in a READY blob, it seems reasonable to simply
// use that struct as the data store.
type State struct {
	sync.RWMutex
	//	Ready
	MaxMessageCount int
}
