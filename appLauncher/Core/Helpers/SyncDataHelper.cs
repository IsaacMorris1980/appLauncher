//using System.Net;
//using System.Net.Sockets;
//using System.Text;
//using System.Threading.Tasks;

//namespace appLauncher.Core.Helpers
//{
//    public struct Received
//    {
//        public IPEndPoint Sender;
//        public string Message;
//    }

//    abstract class UdpBase : UdpClient
//    {
//        //   protected UdpClient Client;

//        protected UdpBase() : base()
//        {
//            // Client = new UdpClient();
//        }

//        public async Task<Received> Receive()
//        {
//            UdpReceiveResult result = await Client.ReceiveAsync();
//            return new Received()
//            {
//                Message = Encoding.UTF8.GetString(bytes: result.Buffer, index: 0, count: result.Buffer.Length),
//                Sender = result.RemoteEndPoint
//            };
//        }
//    }

//    //Server
//    class UdpListener : UdpBase
//    {
//        private IPEndPoint _listenOn;

//        public UdpListener() : this(new IPEndPoint(IPAddress.Any, 32123))
//        {
//        }

//        public UdpListener(IPEndPoint endpoint)
//        {
//            _listenOn = endpoint;
//            Client = new UdpClient(_listenOn);
//        }

//        public async void Reply(string message, IPEndPoint endpoint)
//        {
//            var datagram = Encoding.UTF8.GetBytes(message);
//            await Client.SendAsync(datagram, datagram.Length, endpoint);
//        }

//    }

//    //Client
//    class UdpUser : UdpBase
//    {

//        private UdpUser() { }

//        public static async Task<UdpUser> ConnectTo(IPEndPoint hostname, int port)
//        {
//            //  IPEndPoint ipep = new IPEndPoint(IPAddress.Broadcast,)
//            var connection = new UdpUser();
//            connection.Client.Client.Connect(hostname);
//            connection.Client.Client.Send(new byte[1]);
//            return connection;
//        }

//        public async void Send(string message)
//        {
//            var datagram = Encoding.UTF8.GetBytes(message);
//            await Client.SendAsync(datagram, datagram.Length, end);
//        }

//    }


//}
