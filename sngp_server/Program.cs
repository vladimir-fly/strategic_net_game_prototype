using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace sngp_server
{
    internal enum EMessageType
    {
        PlaygroundSizeRequest = 0,
        UnitsDataRequest = 1,
        MoveDataRequest = 2
    }

    internal class Program
    {
        private const int port = 8000;
        private const byte playgroundSize = 10;
        private static byte[] units = {01, 23, 44, 17, 79};

        static void Main()
        {
            var playground = new Playground(playgroundSize, units.ToList());

            TcpListener server = null;
            try
            {
                var localAddr = IPAddress.Parse("127.0.0.1");
                server = new TcpListener(localAddr, port);
                server.Start();

                while (true)
                {
                    Console.WriteLine("Waiting client... ");
                    var client = server.AcceptTcpClient();
                    Console.WriteLine("Client connected. Parse request...");
                    using (var stream = client.GetStream())
                    {
                        var requestData = new byte[6];
                        stream.Read(requestData, 0, requestData.Length);
                        Console.WriteLine($"Request id is: {requestData[0]}");

                        var responseData = new byte[0];
                        switch (requestData[0])
                        {
                            case (byte) EMessageType.PlaygroundSizeRequest:
                                responseData = new []{playgroundSize};
                                Console.WriteLine($"Playground size is {responseData.FirstOrDefault()}");
                                break;
                            case (byte) EMessageType.UnitsDataRequest:
                                responseData = units;
                                Console.WriteLine($"Response is {string.Join(", ", units)}");
                                break;
                            case (byte) EMessageType.MoveDataRequest:
                                responseData = new []{playground.MoveUnit(requestData[1], requestData[2])};
                                Console.WriteLine($"UnitId = {requestData[1]}, NodeId = {requestData[2]}");
                                Console.WriteLine($"Move direction is {responseData[0]}");
                                break;
                        }
                        stream.Write(responseData, 0, responseData.Length);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Server error: {e.Message}");
            }
            finally
            {
                server?.Stop();
            }
        }
    }
}