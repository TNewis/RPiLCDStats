using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using RPiLCDClientConsole.Services;

namespace RPiLCDClientConsole
{
    public class PCClient
    {
        private static ILoggingService _loggingService = new LogToConsoleService();

        public static string Output;

        private const int port = 13202;

        private static ManualResetEvent _connectDone = new ManualResetEvent(false);
        private static ManualResetEvent _sendDone = new ManualResetEvent(false);
        private static ManualResetEvent _receiveDone = new ManualResetEvent(false);

        public static event EventHandler<UpdateRecievedEventArgs> RaiseUpdateRecievedEvent;
        protected static UpdateRecievedEventArgs _args = new UpdateRecievedEventArgs();

        private static string response = String.Empty;

        private static Socket _clientSocket;

        private static bool _connecting = false;
        private static int _connectingWait = 0;

        public void OpenConnection()
        {
            Task.Run(() =>
            {
                StartClient();
            });
        }

        public void SendUpdate(string message)
        {
            if (_clientSocket != null)
            {
                if (_clientSocket.Connected)
                {
                    SendMessage(message);
                    return;
                }
            }
            OpenConnection();
        }

        public void CloseConnection()
        {
            _clientSocket.Shutdown(SocketShutdown.Both);
            _clientSocket.Close();
        }

        public static void SetLoggingService(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        private static void StartClient()
        {
            if (_connecting && _connectingWait < 10)
            {
                _loggingService.LogMessage(".");
                _connectingWait++;
                return;
            }
            else if (!(_clientSocket == null))
            {
                if (_clientSocket.Connected)
                {
                    _loggingService.LogWarning("Connection already exists. Aborting new connection attempt.");
                    return;
                }

            }

            _connectingWait = 0;
            _connecting = true;
            try
            {
                IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                if (_clientSocket == null)
                {
                    _clientSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                }

                _clientSocket.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), _clientSocket);

                _connectDone.WaitOne();
            }
            catch (Exception e)
            {
                _loggingService.LogError(e.ToString());
            }
            finally
            {
                _connecting = false;
            }
        }

        private static void SendMessage(string message)
        {
            try
            {
                Send(_clientSocket, message + "<EOF>");

                var m1Time = DateTime.Now;
                _sendDone.WaitOne();
                var m2Time = DateTime.Now;

                Receive(_clientSocket);
                var m3Time=DateTime.Now;
                _receiveDone.WaitOne();
            }
            catch (Exception e)
            {
                _loggingService.LogError(e.ToString());
            }
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;

                client.EndConnect(ar);

                _loggingService.LogMessage(("Socket connected to "+ client.RemoteEndPoint.ToString()));

                _connectDone.Set();
            }
            catch (SocketException se)
            {
                if (se.ErrorCode == 10061)
                {
                    _loggingService.LogError("ConnectCallback: Target actively refused connection. Is the server running?");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception e)
            {
                _loggingService.LogError(e.ToString());
            }
        }

        private static void Receive(Socket client)
        {
            try
            {
                DataContract state = new DataContract
                {
                    workSocket = client
                };
                client.BeginReceive(state.buffer, 0, DataContract.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);

            }
            catch (Exception e)
            {
                _loggingService.LogError(e.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            String content = string.Empty;

            try
            {
                DataContract state = (DataContract)ar.AsyncState;
                Socket client = state.workSocket;

                int bytesRead = 0;
                try
                {
                    bytesRead = client.EndReceive(ar);
                }
                catch (SocketException se)
                {
                    if (se.ErrorCode == 10054)
                    {
                        _loggingService.LogError("Connection forcibly closed by host.");
                    }
                    else
                    {
                        throw;
                    }
                }
                if (bytesRead > 0)
                {
                    state.stringBuilder.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    content = state.stringBuilder.ToString();
                    if (content.IndexOf("<EOF>") > -1)
                    {
                        _receiveDone.Set();

                        Output = content;
                        if (!Output.StartsWith("<EOF>"))
                        {
                            _args.UpdateString = Output;
                            OnUpdateRecieved(_args);
                        }
                    }
                    else
                    {
                        client.BeginReceive(state.buffer, 0, DataContract.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                    }
                }
                else
                {
                    if (state.stringBuilder.Length > 1)
                    {
                        response = state.stringBuilder.ToString();
                    }
                    _receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                _loggingService.LogError(e.ToString());
            }
        }

        private static void Send(Socket client, String data)
        {
            try
            {
                byte[] byteData = Encoding.ASCII.GetBytes(data);
                client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client);
            }
            catch (SocketException se)
            {
                if (se.ErrorCode == 10053)
                {
                    _loggingService.LogError("Established connection aborted. Did the server shutdown?");
                }
                else
                {
                    _loggingService.LogError(se.Message);
                }
            }

        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                int bytesSent = client.EndSend(ar);
                _loggingService.LogMessage("Sent "+ bytesSent +" bytes to server.");
                _sendDone.Set();
            }
            catch (Exception e)
            {
                _loggingService.LogError(e.ToString());
            }
        }

        private static void OnUpdateRecieved(UpdateRecievedEventArgs e)
        {
            RaiseUpdateRecievedEvent?.Invoke(null, e); ;
        }
    }
    public class UpdateRecievedEventArgs : EventArgs
    {
        public string UpdateString { get; set; }
    }

}
