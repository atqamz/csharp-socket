using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace SocketClientDemo
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
            ExecuteClient();
        }

        private static void ExecuteClient()
        {
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11000);

            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                sender.Connect(localEndPoint);

                Console.WriteLine("Socket connected to -> {0} ", sender.RemoteEndPoint?.ToString());

                byte[] messageSent = Encoding.ASCII.GetBytes("Test Client<EOF>");
                int byteSent = sender.Send(messageSent);

                byte[] messageReceived = new byte[1024];

                // receive mahasiswa object from server and print it
                int byteRecv = sender.Receive(messageReceived);
                string json = Encoding.ASCII.GetString(messageReceived, 0, byteRecv);
                Mahasiswa mhs = JsonConvert.DeserializeObject<Mahasiswa>(json);
                Console.WriteLine("NIM: {0}, Nama: {1}", mhs.NIM, mhs.Nama);

                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }
        }
    }
}