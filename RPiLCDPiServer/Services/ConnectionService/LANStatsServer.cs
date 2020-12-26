using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using RPiLCDShared.Services;

namespace RPiLCDPiServer.Services.ConnectionService
{
    class LANStatsServer : ConnectionService
    {
        private static ILoggingService _loggingService;

        public static ManualResetEvent allDone = new ManualResetEvent(false);

        private static Socket _listener;
        private static readonly StringBuilder _response = new StringBuilder();

        public override void SetLoggingService(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        public override void OpenConnection()
        {
            StartListening();
        }

        public override void CloseConnection()
        {
            StopListening();
        }

        public override void IncludeInResponse(string response)
        {
            IncludeInResponseStatic(response);
        }

        public static void IncludeInResponseStatic(string response)
        {
            _response.Append(response);
        }

        public static void StopListening()
        {
            if (_listener == null)
            {
                return;
            }

            _listener.Shutdown(SocketShutdown.Both);
            _listener.Close();
        }

        public static void StartListening()
        {
            IPAddress ipAddress = IPAddress.Parse("192.168.0.10");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 13202);

            _listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                _listener.Bind(localEndPoint);
                _listener.Listen(10);

                while (true)
                {
                    allDone.Reset();

                    _loggingService.LogMessage("Waiting for a connection on " + ipAddress.ToString() + ":" + localEndPoint.Port.ToString());
                    _listener.BeginAccept(new AsyncCallback(AcceptCallback), _listener);

                    allDone.WaitOne();
                }
            }
            catch (SocketException se)
            {
                _loggingService.LogError(se.ErrorCode + ": " + se.Message);
            }
            catch (Exception e)
            {
                _loggingService.LogError(e.Message);
            }

        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                allDone.Set();

                Socket listener = (Socket)ar.AsyncState;
                Socket handler = listener.EndAccept(ar);
                DataContract state = new DataContract { workSocket = handler };
                handler.BeginReceive(state.buffer, 0, DataContract.BufferSize, 0, new AsyncCallback(ReadCallback), state);

            }
            catch (SocketException se)
            {
                _loggingService.LogError(se.ErrorCode + ": " + se.Message);
            }
            catch (Exception e)
            {
                _loggingService.LogError(e.Message);
            }
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            String content = string.Empty;
            DataContract state = (DataContract)ar.AsyncState;
            Socket handler = state.workSocket;

            int bytesRead = 0;
            try
            {
                bytesRead = handler.EndReceive(ar);
            }
            catch (SocketException se)
            {
                if (se.ErrorCode == 104)
                {
                    _loggingService.LogMessage(se.ErrorCode + ": Sending shutdown trigger.");
                    _loggingService.LogTrigger(LogTriggerTypes.ConnectionClosed);
                }
                _loggingService.LogError(se.ErrorCode + ": " + se.Message);
            }

            if (bytesRead > 0)
            {
                state.stringBuilder.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                content = state.stringBuilder.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    _args.UpdateString = content;
                    OnUpdateRecieved(_args);

                    state.stringBuilder.Clear();

                    content = _response.ToString() + "<EOF>";
                    Send(handler, content);
                    _response.Clear();
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
            try
            {
                byte[] byteData = Encoding.ASCII.GetBytes(data);
                handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
            }
            catch (SocketException se)
            {
                _loggingService.LogError(se.ErrorCode + ": " + se.Message);
            }
            catch (Exception e)
            {
                _loggingService.LogError(e.Message);
            }

        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;
                int bytesSent = handler.EndSend(ar);
                //Console.WriteLine("Sent {0} bytes to client.", bytesSent);
            }
            catch (SocketException se)
            {
                _loggingService.LogError(se.ErrorCode + ": " + se.Message);
            }
            catch (Exception e)
            {
                _loggingService.LogError(e.Message);
            }
        }
    }
}
