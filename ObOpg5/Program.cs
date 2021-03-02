using System;
using System.Net;

namespace ObOpg5
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server(4646, IPAddress.Loopback);
            server.Listen();
        }
    }
}
