using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Domain.Model;
using Domain.Repository;

namespace Data.Model
{
    public class Listner
    {
        public delegate void SocketAcceptHandler(UserEntity e);
        public event SocketAcceptHandler SocketAccepted;

        private IConnectionListener _connectionListener;

        private Socket _socket;

        public bool Listening { get; private set; }

        public int Port { get; private set; }

        public IConnectionListener ConnectionListener
        {
            get => _connectionListener;
            set => _connectionListener = value;
        }

        public Listner(int port, IConnectionListener connectionListener)
        {
            _connectionListener = connectionListener;
            Port = port;
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start() //Creates a new socket
        {
            if (Listening) return;
            Parallel.Invoke(() =>
            {
                _socket.Bind(new IPEndPoint(0, Port)); //Binds it with the servers IP(purticular end point)
                _socket.Listen(0);
                _socket.BeginAccept(Callback, null); //Starts a thread or a asyncronus task
                Listening = true;
            });

        }

        public void Stop()
        {
            if (!Listening) return;
            Parallel.Invoke(() =>
            {

                if (_socket.Connected)
                    _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            });


        }

        void Callback(IAsyncResult ar)
        {
            try
            {
                var s = _socket.EndAccept(ar);
                SocketAccepted?.Invoke(new UserEntity(new User((s.RemoteEndPoint as IPEndPoint)?.Address.ToString(), ""), new SocketManager(s)));
                _socket.BeginAccept(Callback, null);
                _connectionListener.Accept(new User((s.RemoteEndPoint as IPEndPoint)?.Address.ToString(), ""));

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                //MessageBox.Show(ex.Message);
            }
        }
    }
}
