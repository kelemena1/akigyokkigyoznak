using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using System.IO;
using static System.Net.WebRequestMethods;
using File = System.IO.File;
using System.Media;
using System.Net.Sockets;


namespace Snake
{
    internal class Program
    {
        static Random rnd = new Random();
        static int ScreenX = Console.WindowWidth;
        static int ScreenY = Console.WindowHeight;
        static char[,] screen = new char[ScreenX, ScreenY];
        static int almaSzam = 5;
        static List<Point> Kigyo = new List<Point>();
        static char irany;
        static bool end = false;
        static int time = 10000;
        static int score = 0;
        static int level = 1;
        //static SoundPlayer muzsika = new SoundPlayer(@"C:\Users\Adam\OneDrive\Asztali gép\GH\akigyokkigyoznak\Snake\Snake\bin\Debug\Sounds\Background.wav");
        static SoundPlayer sbsound = new SoundPlayer(@"C:\Users\Felhasználó\Desktop\SnakeSajat\Snake\bin\Debug\Sounds\Nulla.wav");
        static SoundPlayer scoresound = new SoundPlayer(@"C:\Users\Felhasználó\Desktop\SnakeSajat\Snake\bin\Debug\Sounds\Mario.wav");
        static SoundPlayer wallsound = new SoundPlayer(@"C:\Users\Felhasználó\Desktop\SnakeSajat\Snake\bin\Debug\Sounds\Villamcsapva.wav");
        static int kigyoSzama = 0;
        static StreamReader r;
        static StreamWriter w;


        static void PalyaKeszites(char[,] screen)
        {
            int x = screen.GetLength(0);
            int y = screen.GetLength(1);
            screen[0, 0] = '╔';
            for (int i = 0; i < x - 2; i++)
            {
                screen[i + 1, 0] = '═';
                screen[i + 1, y - 2] = '═';
            }
            screen[x - 1, 0] = '╗';
            for (int i = 0; i < y - 3; i++)
            {
                screen[0, i + 1] = '║';
                screen[x - 1, i + 1] = '║';
            }
            screen[0, y - 2] = '╚';
            screen[x - 1, y - 2] = '╝';

        }

        static void Kirajzolas(char[,] screen)
        {
            for (int i = 0; i < ScreenX; i++)
            {
                for (int j = 0; j < ScreenY; j++)
                {
                    Console.SetCursorPosition(i, j);
                    Console.Write(screen[i, j]);
                }
            }
            Console.SetCursorPosition(0, 0);
        }

        static void KigyoStart(List<Point> kigyo)
        {
            int vKozep = ScreenX / 2;
            int yKozep = ScreenY / 2;
            kigyo.Add(new Point(vKozep - 2, yKozep));
            kigyo.Add(new Point(vKozep - 1, yKozep));
            kigyo.Add(new Point(vKozep, yKozep));
            kigyo.Add(new Point(vKozep + 1, yKozep));
            kigyo.Add(new Point(vKozep + 2, yKozep));
            kigyo.Add(new Point(vKozep + 3, yKozep));
        }

        static void AdatKuldes()
        {
            string kigyoAdatai = "KigyoAdat|" + kigyoSzama.ToString();
            for (int i = 0; i < Kigyo.Count; i++)
            {
                string kigyoTest = $"{Kigyo[i].X}-{Kigyo[i].Y}";
                kigyoAdatai += $";{kigyoTest}";
            }
            w.WriteLine(kigyoAdatai);
            w.Flush();
        }

        static void AdatFogadas()
        {
            w.WriteLine($"send|{kigyoSzama}");
            w.Flush();
            string message = r.ReadLine();
            if (message == "Start")
            {
                while (message != "Stop")
                {
                    message = r.ReadLine();
                    if (message != "Stop")
                    {
                        List<Point> egyKigyo = new List<Point>();
                        string[] adatok = message.Split(';');
                        int egyKigyoSzama = int.Parse(adatok[0]);
                        for (int i = 1; i < adatok.Length; i++)
                        {
                            string[] koordinatak = adatok[i].Split('-');
                            Point koordinata = new Point(int.Parse(koordinatak[0]), int.Parse(koordinatak[1]));
                            egyKigyo.Add(koordinata);
                        }
                        KigyoKirajzolas(egyKigyo, false, egyKigyoSzama);
                    }
                }
            }
            w.WriteLine("almak");
            w.Flush();
            message = r.ReadLine();
            string[] adat = message.Split(';');
            Console.ForegroundColor = ConsoleColor.Red;
            for (int i = 0; i < adat.Length - 1; i++)
            {
                string[] koordinata = adat[i].Split('-');
                int x = int.Parse(koordinata[0]);
                int y = int.Parse(koordinata[1]);
                screen[x, y] = '♥';
                Console.SetCursorPosition(x, y);
                Console.Write('♥');
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
        static void KigyoKirajzolas(List<Point> Kigyo, bool sajat, int kigyoSzama)
        {
            if (sajat)
            {
                AdatKuldes();
                Thread szal = new Thread(AdatFogadas);
                szal.Start();
                szal.Join();
                //AdatFogadas();
            }
            for (int i = 0; i < Kigyo.Count; i++)
            {
                Console.ForegroundColor = (ConsoleColor)(kigyoSzama % 16);
                Console.SetCursorPosition(Kigyo[i].X, Kigyo[i].Y);
                if (i == 0)
                {
                    Console.Write("☺");
                    if (sajat)
                    {
                        screen[Kigyo[i].X, Kigyo[i].Y] = '☺';
                    }
                }
                else if (i <= Kigyo.Count - 3)
                {
                    Console.Write("o");
                    if (sajat)
                    {
                        screen[Kigyo[i].X, Kigyo[i].Y] = 'o';
                    }
                }
                else if (i == Kigyo.Count - 2)
                {
                    Console.Write("°");
                    if (sajat)
                    {
                        screen[Kigyo[i].X, Kigyo[i].Y] = '°';
                    }
                }
                else
                {
                    Console.Write(" ");
                    if (sajat)
                    {
                        screen[Kigyo[i].X, Kigyo[i].Y] = ' ';
                    }
                }
            }
        }
        static void TerepTargyKirakas(char item,int db,ConsoleColor color)
        {
            Console.ForegroundColor = color;
            int szamlalo = 0;
            int x, y;
            do
            {
                x = rnd.Next(1, ScreenX);
                y = rnd.Next(1, ScreenY - 1);
                if (screen[x, y] == 0)
                {
                    screen[x, y] = item;
                    Console.SetCursorPosition(x, y);
                    Console.Write(item);
                    szamlalo++;
                }
            } while (szamlalo != db);


            Console.ForegroundColor = ConsoleColor.White;
        }

        static void BillenyuzetFigyeles()
        {
            ConsoleKeyInfo keyInfo;
            do
            {
                keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.A || keyInfo.Key == ConsoleKey.LeftArrow && irany != 'j')
                {
                    irany = 'b';
                }
                if (keyInfo.Key == ConsoleKey.W || keyInfo.Key == ConsoleKey.UpArrow && irany != 'l')
                {
                    irany = 'f';
                }
                if (keyInfo.Key == ConsoleKey.D || keyInfo.Key == ConsoleKey.RightArrow && irany != 'b')
                {
                    irany = 'j';
                }
                if (keyInfo.Key == ConsoleKey.S || keyInfo.Key == ConsoleKey.DownArrow && irany != 'f')
                {
                    irany = 'l';
                }
                Console.SetCursorPosition(10, 10);
                //Console.Write(irany);
            } while (!end);
        }

        static void KigyoMozgatas()
        {
            
            
            while (!end)
            {
                
                int x = 0;
                int y = 0;
                switch (irany)
                {
                    case 'b':
                        {
                            x = Kigyo[0].X - 1;
                            y = Kigyo[0].Y;
                            Point ujFej = new Point(x, y);
                            if (x > 0 && !Kigyo.Contains(ujFej))
                            {
                                Kigyo.Insert(0, ujFej);
                            }
                            else
                            {
                                end = true;

                            }
                            break;
                        }
                    case 'j':
                        {
                            x = Kigyo[0].X + 1;
                            y = Kigyo[0].Y;
                            Point ujFej = new Point(x, y);
                            if (x < ScreenX - 1 && !Kigyo.Contains(ujFej))
                            {
                                Kigyo.Insert(0, ujFej);
                            }
                            else
                            {
                                end = true;
                            }
                            break;
                        }
                    case 'f':
                        {
                            x = Kigyo[0].X;
                            y = Kigyo[0].Y - 1;
                            Point ujFej = new Point(x, y);
                            if (y > 0 && !Kigyo.Contains(ujFej))
                            {
                                Kigyo.Insert(0, ujFej);
                            }
                            else
                            {
                                end = true;
                            }
                            break;
                        }
                    case 'l':
                        {
                            x = Kigyo[0].X;
                            y = Kigyo[0].Y + 1;
                            Point ujFej = new Point(x, y);
                            if (y < ScreenY - 2 && !Kigyo.Contains(ujFej))
                            {
                                Kigyo.Insert(0, ujFej);
                            }
                            else
                            {
                                end = true;
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
                if (screen[x,y]=='■') 
                {
                    wallsound.Play();
                    end= true;

                }
                else if (screen[x, y] != '♥')
                {
                    Console.SetCursorPosition(Kigyo[Kigyo.Count - 1].X, Kigyo[Kigyo.Count - 1].Y);
                    Console.Write(" ");
                    Kigyo.RemoveAt(Kigyo.Count - 1);
                }
                else
                {
                    w.WriteLine($"almaEves|{x}-{y}");
                    w.Flush();
                    score += 1;
                    scoresound.Play();
                    time += 500;
                    almaSzam --;
                    if (almaSzam == 0)
                    {
                        almaSzam = 5+level;
                        level++;
                        time = 10000;
                        //TerepTargyKirakas('♥',almaSzam,ConsoleColor.Red);
                        TerepTargyKirakas('■',almaSzam,ConsoleColor.Cyan);
                    }
                }
                time -= 10;
                KigyoKirajzolas(Kigyo, true, kigyoSzama);
                Informacio();
                if (irany=='j' || irany=='b')
                {
                    System.Threading.Thread.Sleep(100);
                }
                else
                {
                    System.Threading.Thread.Sleep(100);
                }
                w.WriteLine($"Exit|{kigyoSzama}");
                w.Flush();
            }
            Console.SetCursorPosition(ScreenX / 2 - 5, ScreenY / 2);
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor= ConsoleColor.White;
            //Háttérzene
            




            Console.WriteLine("Game Over!");
            sbsound.Play();
            Console.ReadKey();
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.BackgroundColor= ConsoleColor.White;
            Console.Write("Kérem a játékos neved: ");
            string player = Console.ReadLine();
            string filename = "scoreboard.txt";
            if (File.Exists(filename))
            {
                string[] linesRead = File.ReadAllLines(filename);
                bool playerExists = false;
                for (int i = 0; i < linesRead.Length; i++)
                {
                    string[] parts = linesRead[i].Split(' ');
                    if (parts[0] == player)
                    {
                        playerExists = true;
                        int oldScore = int.Parse(parts[1]);
                        if (score > oldScore)
                        {
                            linesRead[i] = player + " " + score;
                        }
                        break;
                    }
                }
                // Új player ha nincs
                if (!playerExists)
                {
                    Array.Resize(ref linesRead, linesRead.Length + 1);
                    linesRead[linesRead.Length - 1] = player + " " + score;
                }
                // Fájl írása
                File.WriteAllLines(filename, linesRead);
            }
            else
            {
                // Fájl ellenőrzés:
                string[] linesToWrite = { player + " " + score };
                File.WriteAllLines(filename, linesToWrite);
            }

            // Konzolra írás
    
                string[] sorsz = File.ReadAllLines(filename);
                var sortedLines = sorsz.OrderByDescending(line => int.Parse(line.Split(' ')[1]));

                // Csak a top 10
                int counter = 1;
                foreach (string line in sortedLines)
                {
                    if (counter > 10)
                    {
                        break;
                    }
                    string[] kiirato = line.Split(' ');
                Console.WriteLine($"Helyezés:{counter}\tName: {kiirato[0]}\tHighest Score:{kiirato[1]}");
                    counter++;
                }
            

            Console.WriteLine();
        }
        static void Informacio()
        {
            Console.SetCursorPosition(0, ScreenY - 1);
            Console.Write($"Level: {level}");
            Console.SetCursorPosition(ScreenX/2-6, ScreenY - 1);
            Console.Write($"Score: {score}");
            Console.SetCursorPosition(ScreenX-10, ScreenY - 1);
            Console.Write($"Time: {time/100:D3}");
        }
        static void Main(string[] args)
        {
            TcpClient client = new TcpClient("176.63.30.223", 50000);
            r = new StreamReader(client.GetStream(), Encoding.UTF8);
            w = new StreamWriter(client.GetStream(), Encoding.UTF8);
            string message = r.ReadLine();
            int szamlalo = 0;
            if (message == "Start")
            {
                while (message != "Stop")
                {
                    if (szamlalo == 2)
                    {
                        kigyoSzama = int.Parse(r.ReadLine());
                    }
                    message = r.ReadLine();
                    szamlalo++;
                }
            }
            //
            int a = ScreenX;
            int b = ScreenY;
            PalyaKeszites(screen);
            Kirajzolas(screen);
            KigyoStart(Kigyo);
            //KigyoKirajzolas();
            //TerepTargyKirakas('♥',almaSzam,ConsoleColor.Red);
            irany = 'b';
            Console.CursorVisible = false;
            Thread szal2 = new Thread(KigyoMozgatas);
            Thread szal1 = new Thread(BillenyuzetFigyeles);
        
            szal1.Start();
            szal2.Start();
            szal1.Join();
            szal2.Join();
            Console.ReadKey();
        }
    }
}
