using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model
{
    public class SocketManager
    {
        public delegate void ClientReceivedHandler(string data);
        public delegate void ClientDisconnectedHandler();
        public event ClientReceivedHandler Received;
        public event ClientDisconnectedHandler Disconnected;

        public IPEndPoint Ip { get; private set; }


        public Socket _socket;

        public Socket Socket
        {
            get => _socket;
            set => _socket = value;
        }

        public SocketManager(Socket accepted)
        {
            _socket = accepted;
            Ip = (IPEndPoint)_socket.RemoteEndPoint;
            Parallel.Invoke(() => { _socket.BeginReceive(new byte[] { 0 }, 0, 0, 0, Callback, null); });
        }

        void Callback(IAsyncResult ar)
        {
                try
                {
                    _socket.EndReceive(ar);
                    var buffer = new byte[_socket.ReceiveBufferSize];
                    var rec = _socket.Receive(buffer, buffer.Length, 0);
                    if (rec < buffer.Length)
                    {
                        Array.Resize(ref buffer, rec);
                    }
                    Parallel.Invoke(() => { Received?.Invoke(ByteArrayFormatter.DeserializeMessage(buffer, rec));  });
                    _socket.BeginReceive(new byte[] { 0 }, 0, 0, 0, Callback, null);

                }
                catch (Exception)
                {
                    Close();
                    if (Disconnected != null)
                    {
                        Parallel.Invoke(() => { Disconnected(); });
                    }
                }
        }

        public void Send(string data)
        {
            Parallel.Invoke(() =>
            {

                var buffer = ByteArrayFormatter.Format(data);
                _socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, ar => _socket.EndSend(ar), buffer);

            });

        }

        public void Close()
        {
            Parallel.Invoke(() =>
            {
                _socket.Dispose();
                _socket.Close();
                _socket = null;
            });

        }
    }
}
