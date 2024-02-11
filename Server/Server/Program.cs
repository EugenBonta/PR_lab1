using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server;

static class Server
{
    static TcpListener? _tcpListener;
    static List<TcpClient> _clients = new();

    static void Main()
    {
        _tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 1234);
        _tcpListener.Start();

        Console.WriteLine("Serverul a pornit. Asteapta conexiuni...");

        while (true)
        {
            TcpClient client = _tcpListener.AcceptTcpClient();
            _clients.Add(client);

            Thread clientThread = new Thread(HandleClientComm);
            clientThread.Start(client);
        }
    }

    private static void HandleClientComm(object? clientObj)
    {
        TcpClient tcpClient = (TcpClient)clientObj;
        NetworkStream clientStream = tcpClient.GetStream();

        byte[] message = new byte[4096];
        int bytesRead;

        while (true)
        {
            try
            {
                bytesRead = clientStream.Read(message, 0, 4096);
            }
            catch
            {
                break;
            }

            if (bytesRead == 0)
                break;

            string clientMessage = Encoding.ASCII.GetString(message, 0, bytesRead);
            Console.WriteLine(clientMessage);

            Broadcast(clientMessage);
        }

        _clients.Remove(tcpClient);
        tcpClient.Close();
    }

    private static void Broadcast(string message)
    {
        foreach (TcpClient client in _clients)
        {
            NetworkStream clientStream = client.GetStream();
            byte[] broadcastMessage = Encoding.ASCII.GetBytes(message);
            clientStream.Write(broadcastMessage, 0, broadcastMessage.Length);
            clientStream.Flush();
        }
    }
}