using System.Net;
using System.Net.Sockets;
using System.Text;

class Server
{
    private const int DEFAULT_BUFLEN = 512;
    private const int PORT = 27015;

    static void Main()
    {
        Console.Title = "SERVER";
        Console.OutputEncoding = Encoding.UTF8;

        var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        var endPoint = new IPEndPoint(IPAddress.Any, PORT);
        listener.Bind(endPoint);
        listener.Listen(10);

        Console.WriteLine("Сервер запущен и ждёт подключения...");

        var client = listener.Accept();
        Console.WriteLine("Клиент подключился!");

        while (true)
        {
            var buffer = new byte[DEFAULT_BUFLEN];
            int bytesReceived = client.Receive(buffer);
            if (bytesReceived == 0) break;

            string message = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
            Console.WriteLine($"Получено от клиента: {message}");

            string response;
            if (int.TryParse(message, out int number))
            {
                response = (number + 1).ToString();
            }
            else
            {
                response = "Ошибка: ожидалось целое число";
            }

            byte[] responseBytes = Encoding.UTF8.GetBytes(response);
            client.Send(responseBytes);
            Console.WriteLine($"Ответ отправлен: {response}");
        }

        Console.WriteLine("Сервер завершает работу.");
        client.Close();
        listener.Close();
    }
}
