using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Client
{
    private const int DEFAULT_BUFLEN = 512;
    private const int PORT = 27015;

    static void Main()
    {
        Console.Title = "CLIENT";
        Console.OutputEncoding = Encoding.UTF8;

        var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        var endPoint = new IPEndPoint(IPAddress.Loopback, PORT);

        try
        {
            clientSocket.Connect(endPoint);
        }
        catch
        {
            Console.WriteLine("Сервер не найден. Попытка запуска...");

            Process.Start(new ProcessStartInfo
            {
                FileName = "Server.exe", // ⚠️ Путь к .exe сервера, если они в одной папке, оставь как есть
                UseShellExecute = true
            });

            Thread.Sleep(2000); // Подождать запуск сервера

            clientSocket.Connect(endPoint);
        }

        Console.WriteLine("Подключено к серверу.");

        while (true)
        {
            Console.Write("Введите целое число (или 'exit' для выхода): ");
            string input = Console.ReadLine();

            if (input == "exit")
            {
                clientSocket.Shutdown(SocketShutdown.Send);
                clientSocket.Close();
                break;
            }

            byte[] msgBytes = Encoding.UTF8.GetBytes(input);
            clientSocket.Send(msgBytes);

            var buffer = new byte[DEFAULT_BUFLEN];
            int bytesReceived = clientSocket.Receive(buffer);
            string response = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
            Console.WriteLine($"Ответ от сервера: {response}");
        }
    }
}
