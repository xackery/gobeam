package gobeam

import (
	"encoding/json"
	"fmt"
	"os"
)

type GoBeamConfig struct {
	isLoaded bool
	Username string `json:"username`
	Password string `json:"password"`
}

var config *GoBeamConfig

func (c *GoBeamConfig) Load(path string) (err error) {
	if c.isLoaded {
		return
	}

	f, err := os.Open(path)
	if err != nil {
		if err != os.ErrNotExist {
			err = fmt.Errorf("Error opening config: %s", err.Error())
			return
		}
		//write an empty file, since it doesn't exist
		c.Save(path)
		return
	}
	//info, err := f.Stat()
	//fmt.Println("Size", info.Size())

	dec := json.NewDecoder(f)
	err = dec.Decode(c)
	if err != nil {
		err = fmt.Errorf("Error decoding config: %s", err.Error())
	}

	err = f.Close()
	if err != nil {
		err = fmt.Errorf("Failed to close config: %s", err.Error())
		return
	}

	return
}

func (c *GoBeamConfig) Save(path string) (err error) {

	f, err := os.Create(path)
	if err != nil {
		return
	}
	bData, err := json.MarshalIndent(c, "", "	")
	if err != nil {
		return
	}
	_, err = f.Write(bData)
	if err != nil {
		return
	}
	return
}
