//Heavily inspired/derived from discordgo https://github.com/bwmarrin/discordgo

package gobeam

import (
	"errors"
)

// ErrNilState is returned when the state is nil.
var ErrNilState = errors.New("State not instantiated, please use beam.New() or assign Session.State.")

// NewState creates an empty state.
func NewState() *State {
	return &State{
	/*Ready: Ready{
		PrivateChannels: []*Channel{},
		Guilds:          []*Guild{},
	},*/
	}
}

func (s *State) OnUserJoin(user *NewUser) error {
	if s == nil {
		return ErrNilState
	}

	//s.Lock()
	//defer s.Unlock()
	//todo: Member management here
	return nil
}

// onInterface handles all events related to states.
func (s *State) onInterface(se *Session, i interface{}) (err error) {
	if s == nil {
		return ErrNilState
	}
	if !se.StateEnabled {
		return nil
	}

	switch t := i.(type) {
	case *UserJoinEvent:
		err = s.OnUserJoin(t.NewUser)
	}

	return
}
