using AntLib.Communication;
using AntLib.Communication.Client;

internal class Program
{
    private static void Main(string[] args)
    {
        string clientName = "AntLibDemoClient";
        string serverIp = "127.0.0.1";
        int port = 1515;
        bool isParallel = true;
        int threads = Environment.ProcessorCount;
        Console.WriteLine($"Hello, World! I am {clientName}. Ready to work");
        AntLibClient client = new AntLibClient(clientName, isParallel, threads);
        client.OnMessageReceive += PrineMessage;
        client.ConnectToServer(serverIp, port);
        Console.ReadKey();
    }

    private static void PrineMessage(Message msg)
    {
        Console.WriteLine($"Message from {msg.SenderName} with command {Enum.GetName(msg.Command.GetType() ,msg.Command)}");
    }
}