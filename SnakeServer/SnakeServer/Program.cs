using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;

namespace SnakeServer
{
    internal class Program
    {
        static string ipv4 = "192.168.0.53"; //192.168.60.16
        static int port = 50000;
        static IPAddress ip = null;
        static TcpListener listener = null;
        public static int cn = 0;

        public static List<ClientCom> clients = new List<ClientCom>();
        public static List<Thread> clientThreads = new List<Thread>();
        public static Dictionary<int,List<Point>> kigyok=new Dictionary<int, List<Point>>();
        public static Dictionary<int, List<Point>> korabbiKigyok = new Dictionary<int, List<Point>>();

        public static List<Point> almak=new List<Point>();
        public static int level = 0;

        static void Main(string[] args)
        {
            ip = IPAddress.Parse(ipv4);
            listener = new TcpListener(ip, port);
            listener.Start();
            Console.WriteLine($"Cím: {ip}\nPort: {port}");
            while (true)
            {
                if (listener.Pending())
                {
                    TcpClient client = listener.AcceptTcpClient();
                    ClientCom clientc = new ClientCom(client);
                    Thread thread = new Thread(clientc.ComStart);
                    clients.Add(clientc);
                    clientThreads.Add(thread);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{cn+1}. kliens csatlakozott.");
                    cn=clients.Count;
                    thread.Start();
                }
                
            }
        }
    }
}
