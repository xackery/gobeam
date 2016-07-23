Robot gRPC Client
---

This is an example of a gRPC client script in CSharp. It is an endpoint that consumes data from robot_grpc_server to poll keypresses.

It is made generically to handle most games and input types.

Needs:
* Install-Package Google.Protobuf -Pre
* Install-Package Grpc
* Install-Package Grpc.Tools
* A running copy of robot_grpc_server.
