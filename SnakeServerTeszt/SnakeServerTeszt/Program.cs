using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;

namespace SnakeServerTeszt
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TcpClient client= new TcpClient("176.63.30.223", 50000);
            StreamReader r=new StreamReader(client.GetStream(),Encoding.UTF8);
            StreamWriter w=new StreamWriter(client.GetStream(),Encoding.UTF8);
            bool end = false;
            string message=r.ReadLine();
            if (message == "Start") 
            {
                while(message!="Stop")
                {
                    message= r.ReadLine();
                    Console.WriteLine(message);
                }
            }
            while (!end) 
            {
                Console.Write("Parancs: ");
                string command=Console.ReadLine();
                w.WriteLine(command);
                w.Flush();
                message=r.ReadLine();
                Console.WriteLine(message);
                if (message=="Exit....")
                {
                    end = true;
                }

            }
           
        }
    }
}
