using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace NaNiT
{
    public interface IServer
    {
        TcpClient ThisTcpClient { get; set; }

        void Disconnect();
    }
}