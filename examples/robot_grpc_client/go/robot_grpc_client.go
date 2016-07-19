package main

import (
	"fmt"
	beam "github.com/xackery/gobeam"
	"golang.org/x/net/context"
	"google.golang.org/grpc"
	"io"
	"log"
	"time"
)

var delay = (1 * time.Second)

type robotService struct {
	reportChan chan *beam.Report
}

func main() {
	//Start grpc connection
	addr := "127.0.0.1:50051"
	log.Println("[gRPC] Connecting to", addr)
	conn, err := grpc.Dial(addr, grpc.WithInsecure())
	if err != nil {
		log.Println("[gRPC] Failed to connect: %v", err.Error())
		return
	}
	defer conn.Close()
	c := beam.NewRobotServiceClient(conn)

	//Get Report, poll them
	stream, err := c.StreamReport(context.Background(), &beam.StreamRequest{})
	if err != nil {
		fmt.Printf("[gRPC] Could not get report: %v\n", err)
		return
	}
	for {
		report, err := stream.Recv()
		if err == io.EOF {
			break
		}
		if err != nil {
			log.Fatalf("%v = _, %v", c, err)
		}
		log.Println(report)
	}
	return
}
