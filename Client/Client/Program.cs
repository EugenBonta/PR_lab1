using System.Net.Sockets;
using System.Text;

namespace Client;

internal static class Client
{
    static TcpClient? _tcpClient;
    static NetworkStream _clientStream;
    static string _clientName;

    static void Main()
    {
        const string serverIp = "127.0.0.1";
        Console.Write("Introdu numele: ");
        _clientName = Console.ReadLine();

        _tcpClient = new TcpClient(serverIp, 1234);
        _clientStream = _tcpClient.GetStream();

        Console.WriteLine("Conectat la server. Incepe sa scrii mesaje.");

        Thread receiveThread = new Thread(ReceiveMessages);
        receiveThread.Start();

        while (true)
        {
            string? message = Console.ReadLine();

            byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes($"{_clientName}: {message}");
            _clientStream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
            _clientStream.Flush();
        }
    }

    private static void ReceiveMessages()
    {
        try
        {
            while (true)
            {
                byte[] receivedMessage = new byte[4096];
                int bytesRead = _clientStream.Read(receivedMessage, 0, receivedMessage.Length);
                if (bytesRead > 0)
                {
                    string message = Encoding.ASCII.GetString(receivedMessage, 0, bytesRead);
                    Console.WriteLine(message);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Eroare de comunicare: " + ex.Message);
        }
    }
}