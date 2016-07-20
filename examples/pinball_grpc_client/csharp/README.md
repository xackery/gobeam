Robot gRPC Client
---

This is an example of a gRPC client script in CSharp. It is an endpoint that consumes data from robot_grpc_server to poll keypresses.

It is made specifically for the original nintendo game Pinball, as it uses ReadProcessMemory to detect if the game is at the main menu (and auto presses the Enter key to simulate a Start press).


Needs:
* FCEUX
* Install-Package Google.Protobuf -Pre
* Install-Package Grpc
* Install-Package Grpc.Tools
* A running copy of robot_grpc_server.
