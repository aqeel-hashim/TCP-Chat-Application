using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.Client
{
    public class SocketManager
    {
        readonly Socket _s;
        public delegate void ReceivedEventHandler(string received);
        public event ReceivedEventHandler Received = delegate { };
        bool _connected;

        public SocketManager()
        {
            _s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect(string ip, int port)
        {
            Parallel.Invoke(() =>
            {

                try
                {
                    var ep = new IPEndPoint(IPAddress.Parse(ip), port);
                    _s.BeginConnect(ep, ConnectCallback, _s);
                }
                catch { }

            });


        }
        public void Close()
        {
            _s.Dispose();
            _s.Close();
        }

        void ConnectCallback(IAsyncResult ar)
        {
            Parallel.Invoke(() =>
            {

                _s.EndConnect(ar);
                _connected = true;
                var buffer = new byte[_s.ReceiveBufferSize];
                _s.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReadCallback, buffer);

            });
        }

        private void ReadCallback(IAsyncResult ar)
        {

            Console.WriteLine("Message Socket: Start");
            try
            {
                Parallel.Invoke(() =>
                {

                    var buffer = (byte[])ar.AsyncState;
                    var rec = _s.EndReceive(ar);
                    if (rec != 0)
                    {
                        var message = ByteArrayFormatter.DeserializeMessage(buffer, rec);
                        Console.WriteLine("Message Socket: " + message);
                        Received(message);
                    }
                    else
                    {
                        _connected = false;
                        Close();
                        return;
                    }
                    _s.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReadCallback, buffer);

                });
            }
            catch (AggregateException ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
           

        }

        public void Send(string data)
        {
            Parallel.Invoke(() =>
            {
                try
                {
                    var dataReceived = ByteArrayFormatter.Format(data);
                    _s.BeginSend(dataReceived, 0, dataReceived.Length, SocketFlags.None, SendCallback, dataReceived);
                }
                catch (Exception ex) { Console.WriteLine(ex.StackTrace); }

            });

        }
        void SendCallback(IAsyncResult ar)
        {
            Parallel.Invoke(() => { _s.EndSend(ar); });

        }
    }
}
