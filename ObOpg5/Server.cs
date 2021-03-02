using Newtonsoft.Json;
using ObOpg1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ObOpg5
{
    class Server
    {
        public int Port { get; private set; }
        public IPAddress Address { get; private set; }
        private TcpListener Listener { get; set; }

        private static List<Beer> Data = new List<Beer>()
        {
            new Beer(2,"GUFA", 10,50),
            new Beer(3,"THEA", 5, 2),
            new Beer(4, "KOKA", 9999, 99)
        };

        public Server(int port, IPAddress address)
        {
            Port = port;
            Address = address;
            Listener = new TcpListener(Address, Port);
        }

        private void Start()
        {
            Listener.Start();
        }

        public void Listen()
        {
            Start();

            while (true)
            {
                Console.WriteLine("Waiting for connection");
                TcpClient client = Listener.AcceptTcpClient();
                new Thread(() => HandleClient(client)).Start();
                Console.WriteLine("Connected");
            }

        }

        private void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream);
            writer.AutoFlush = true;

            string command = reader.ReadLine();
            string data = reader.ReadLine();
            try
            {

                switch (command.ToUpper())
                {
                    case "HENTALLE":
                        List<Beer> beers = GetAll();
                        string s = JsonConvert.SerializeObject(beers);
                        writer.WriteLine(s);
                        break;
                    case "HENT":
                        Beer beer = GetById(int.Parse(data));
                        writer.WriteLine(JsonConvert.SerializeObject(beer));
                        break;
                    case "GEM":
                        Save(JsonConvert.DeserializeObject<Beer>(data));
                        break;
                    default:
                        writer.WriteLine("Invalid Command Name");
                        break;
                }
            }
            catch (Exception ex)
            {
                writer.WriteLine("Exception Occured\n" + ex.Message);
                client.Close();
            }

            client.Close();
        }

        private List<Beer> GetAll()
        {
            return Data;
        }

        private Beer GetById(int id)
        {
            return Data.Single((b) => b.Id == id);
        }
        private void Save(Beer beer)
        {
            Data.Add(beer);
        }

    }
}
