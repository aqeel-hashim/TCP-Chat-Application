using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    class Listener
    {
        private Socket _socket;

        public bool Listening { get; private set; }

        public int Port { get; private set; }

        public Listener(int port)
        {
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

        public delegate void SocketAcceptHandler(Socket e);
        public event SocketAcceptHandler SocketAccepted;

        void Callback(IAsyncResult ar) 
        {
            try
            {
                var s = _socket.EndAccept(ar);
                SocketAccepted?.Invoke(s);
                _socket.BeginAccept(Callback, null);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
