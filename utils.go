//Heavily inspired/derived from discordgo https://github.com/bwmarrin/discordgo

package beam

import (
	"bytes"
	"encoding/json"
	"fmt"
)

// printEvent prints out a WSAPI event.
func printEvent(sessionType string, e *Event) {
	fmt.Println(fmt.Sprintf("[%s] Event. Type: %s, Method: %s Error: %s, Data: %s", sessionType, e.Type, e.Method, e.Error, e.Data))
	//printJSON(e.Data)
}

// printJSON is a helper function to display JSON data in a easy to read format.
func printJSON(body []byte) {
	var prettyJSON bytes.Buffer
	error := json.Indent(&prettyJSON, body, "", "\t")
	if error != nil {
		fmt.Print("JSON parse error: ", error)
	}
	fmt.Println(string(prettyJSON.Bytes()))
}
