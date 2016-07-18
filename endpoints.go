//Heavily inspired/derived from discordgo https://github.com/bwmarrin/discordgo
package beam

var (
	//STATUS = "https://beam.pro/api/v1/system/health"
	BEAM = "https://beam.pro"
	API  = BEAM + "/api/v1/"

	TETRIS   = API + "tetris/"
	USERS    = API + "users/"
	CHANNELS = API + "channels/"
	GATEWAY  = API + "chats/"

	AUTH  = API + "oauth/"
	LOGIN = USERS + "login"
)
