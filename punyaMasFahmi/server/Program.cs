using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace SocketServerDemo
{
    class Mahasiswa
    {
        public string NIM { get; set; }
        public string Nama { get; set; }

        public Mahasiswa(string nim, string nama)
        {
            NIM = nim;
            Nama = nama;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            ExecuteServer();
        }

        static void ExecuteServer()
        {
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11000);

            Socket listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                while (true)
                {
                    Console.WriteLine("Waiting connection ... ");

                    Socket clientSocket = listener.Accept();
                    byte[] bytes = new byte[1024];
                    string data = "";

                    while (true)
                    {
                        int numByte = clientSocket.Receive(bytes);

                        data += Encoding.ASCII.GetString(bytes, 0, numByte);

                        if (data.IndexOf("<EOF>") > -1)
                            break;
                    }

                    // make new mahasiswa object and send to client
                    Mahasiswa mhs = new Mahasiswa("5220600014", "Fahmi");
                    string json = JsonConvert.SerializeObject(mhs);
                    byte[] message = Encoding.ASCII.GetBytes(json);
                    clientSocket.Send(message);

                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        // method to send message to client
        public static void Send(Socket handler, String data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            handler.Send(byteData);
        }
    }
}
