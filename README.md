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

//Declare new eServer with the main port and the request port
eServer server = new eServer(2048, 4096);

//Add events
server.OnDataReceived += server_OnDataReceived;
server.OnRequestReceived += server_OnRequestReceived;
server.OnClientConnected += server_OnClientConnected;
server.OnClientDisconnected += server_OnClientDisconnected;

void server_OnClientConnected(eSClient client)
{
}

void server_OnClientDisconnected(eSClient client)
{
}

void server_OnDataReceived(eSClient client, byte[] buffer)
{
}

void server_OnRequestReceived(TcpClient client, byte[] buffer)
{
}

//Start the server
server.Start();

//Stop the server
server.Stop();

//Send data
server.SendTo(buffer, client); //Client must be a TcpClient or an eSClient and buffer an byte array
server.SendToAll(buffer); //Buffer must be an byte array
server.SendToAllExcept(buffer, client); //Client must be a TcpClient and buffer an byte array

//Get TcpClient from an ID
int id = server.GetIDFromTcpClient(client); //Client must be a TcpClient

//Get client list
List<eSClient> list = server.GetClientList();
```

#### • eClient

```csharp
//Import the library
using eNetwork2;

//Declare new eClient with the IP, the main port and the request port
eClient client = new eClient("127.0.0.1", 2048, 4096);

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

void client_OnDataReceived(byte[] buffer)
{
}

//Connect to the server
client.Connect();

//Disconnect
client.Disconnect();

//Send data
client.Send(buffer); //Buffer must be an byte array

//Send request
byte[] response = client.SendRequest(buffer); //Buffer must be an byte array

//Get ID
int id = client.GetID();
```