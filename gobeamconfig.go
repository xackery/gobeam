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

	fi, err := os.Stat(path)
	if fi == nil {
		//write an empty file, since it doesn't exist
		for {
			fmt.Printf("Username: ")
			_, err = fmt.Scanln(&c.Username)
			if len(c.Username) < 3 {
				fmt.Println("Username must be greater than 3 characters")
				continue
			}
			if err != nil {
				fmt.Println(err.Error())
				continue
			}
			break
		}
		for {
			fmt.Printf("Password: ")
			_, err = fmt.Scanln(&c.Password)
			if len(c.Password) < 3 {
				fmt.Println("Password must be greater than 3 characters")
				continue
			}
			if err != nil {
				fmt.Println(err.Error())
				continue
			}
			break
		}
		fmt.Println("Saved credentials to", path)
		err = c.Save(path)
		return
	}

	f, err := os.Open(path)

	if err != nil {
		if err != os.ErrNotExist {
			err = fmt.Errorf("Error opening config: %s", err.Error())
			return
		}
		err = fmt.Errorf("Unknown error opening config: %s", err.Error())
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
