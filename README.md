# eNetwork2
New version of the eNetwork library. Fast, easy to use and lightweight library for TCP Networking with .NET

## More informations !

- Easy to use
- Only TCP
- Send byte array with dynamic size
- Compatible with all .NET languages
- Asynchronous

## How to use ?

#### • eServer

```csharp
//Import the library
using eNetwork2;

//Declare new eServer with the port
eServer server = new eServer(2048);

//Add events
server.OnDataReceived += server_OnDataReceived;
server.OnClientConnected += server_OnClientConnected;
server.OnClientDisconnected += server_OnClientDisconnected;

void server_OnClientConnected(TcpClient client)
{
}

void server_OnClientDisconnected(TcpClient client)
{
}

void server_OnDataReceived(TcpClient client, byte[] buffer)
{
}

//Start the server
server.Start();

//Stop the server
server.Stop();

//Send data
server.SendToAll(buffer); //Buffer must be an byte array
server.SendToAllExcept(client, buffer); //Client must be a TcpClient and buffer an byte array
```

#### • eClient

```csharp
//Import the library
using eNetwork2;

//Declare new eClient with the IP and the port
eClient client = new eClient("127.0.0.1", 2048);

//Add events
client.OnDataReceived += client_OnDataReceived;
client.OnClientConnected += client_OnConnected;
client.OnClientDisconnected += client_OnDisconnected;

void client_OnConnected()
{
}

void client_OnDisconnected()
{
}

void client_OnDataReceived(byte[] data)
{
}

//Connect to the server
client.Connect();

//Disconnect
client.Disconnect();

//Send data
client.Send(buffer); //Buffer must be an byte array
```