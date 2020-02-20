using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace RPiLCDServerConsole
{
    public class Server
    {
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public Server()
        {
        }

        public static void StartListening()
        {
            IPAddress ipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 13202);
 
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
 
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(1);

                while (true)
                {
                    allDone.Reset();

                    Console.WriteLine("Waiting for a connection on "+ ipAddress.ToString() +":"+ localEndPoint.Port.ToString());
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

                    allDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();

        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                allDone.Set();

                Socket listener = (Socket)ar.AsyncState;
                Socket handler = listener.EndAccept(ar);
                DataContract state = new DataContract();

                state.workSocket = handler;
                handler.BeginReceive(state.buffer, 0, DataContract.BufferSize, 0, new AsyncCallback(ReadCallback), state);

            } catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            String content = string.Empty;
            DataContract state = (DataContract)ar.AsyncState;
            Socket handler = state.workSocket;

            int bytesRead = handler.EndReceive(ar);
            if (bytesRead > 0)
            {  
                state.stringBuilder.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                content = state.stringBuilder.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    Console.WriteLine("Read {0} bytes from socket. \n Data : {1}", content.Length, content);
                    state.stringBuilder.Clear();
                    content = "TestServer<EOF>";

                    Send(handler, content);
                    handler.BeginReceive(state.buffer, 0, DataContract.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                }
                else
                {
                    handler.BeginReceive(state.buffer, 0, DataContract.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                }
            }
        }

        private static void Send(Socket handler, String data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

    }

}