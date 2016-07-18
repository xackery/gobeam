//Heavily inspired/derived from discordgo https://github.com/bwmarrin/discordgo

package gobeam

import (
	"reflect"
)

const VERSION = "0.01.0"

// validateHandler takes an event handler func, and returns the type of event.
// eg.
//     Session.validateHandler(func (s *beamgo.Session, m *beamgo.MessageCreate))
//     will return the reflect.Type of *beamgo.MessageCreate
func (s *Session) validateHandler(handler interface{}) reflect.Type {
	handlerType := reflect.TypeOf(handler)

	if handlerType.NumIn() != 2 {
		panic("Unable to add event handler, handler must be of the type func(*beamgo.Session, *beamgo.EventType).")
	}

	if handlerType.In(0) != reflect.TypeOf(s) {
		panic("Unable to add event handler, first argument must be of type *beamgo.Session.")
	}

	eventType := handlerType.In(1)

	// Support handlers of type interface{}, this is a special handler, which is triggered on every event.
	if eventType.Kind() == reflect.Interface {
		eventType = nil
	}

	return eventType
}

// AddHandler allows you to add an event handler that will be fired anytime
// the Beam WSAPI event that matches the interface fires.
// eventToInterface in events.go has a list of all the Beam WSAPI events
// and their respective interface.
// eg:
//     Session.AddHandler(func(s *beamgo.Session, m *beamgo.MessageCreate) {
//     })
//
// or:
//     Session.AddHandler(func(s *beamgo.Session, m *beamgo.PresenceUpdate) {
//     })
// The return value of this method is a function, that when called will remove the
// event handler.
func (s *Session) AddHandler(handler interface{}) func() {
	s.initialize()

	eventType := s.validateHandler(handler)

	s.handlersMu.Lock()
	defer s.handlersMu.Unlock()

	h := reflect.ValueOf(handler)

	handlers := s.handlers[eventType]
	if handlers == nil {
		handlers = []reflect.Value{}
	}
	s.handlers[eventType] = append(handlers, h)

	// This must be done as we need a consistent reference to the
	// reflected value, otherwise a RemoveHandler method would have
	// been nice.
	return func() {
		s.handlersMu.Lock()
		defer s.handlersMu.Unlock()

		handlers := s.handlers[eventType]
		for i, v := range handlers {
			if h == v {
				s.handlers[eventType] = append(handlers[:i], handlers[i+1:]...)
				return
			}
		}
	}
}

// handle calls any handlers that match the event type and any handlers of
// interface{}.
func (s *Session) handle(event interface{}) {
	s.handlersMu.RLock()
	defer s.handlersMu.RUnlock()

	if s.handlers == nil {
		return
	}

	handlerParameters := []reflect.Value{reflect.ValueOf(s), reflect.ValueOf(event)}

	if handlers, ok := s.handlers[reflect.TypeOf(event)]; ok {
		for _, handler := range handlers {
			handler.Call(handlerParameters)
		}
	}

	if handlers, ok := s.handlers[nil]; ok {
		for _, handler := range handlers {
			handler.Call(handlerParameters)
		}
	}
}

// initialize adds all internal handlers and state tracking handlers.
func (s *Session) initialize() {
	s.handlersMu.Lock()
	if s.handlers != nil {
		s.handlersMu.Unlock()
		return
	}

	s.handlers = map[interface{}][]reflect.Value{}
	s.handlersMu.Unlock()

	s.AddHandler(s.onEvent)
	s.AddHandler(s.State.onInterface)
}

// onEvent handles events that are unhandled or errored while unmarshalling
func (s *Session) onEvent(se *Session, e *Event) {
	printEvent(s.Type, e)
}
