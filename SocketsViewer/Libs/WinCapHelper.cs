/****************************************************************************
*项目名称：SocketsViewer.Libs
*CLR 版本：4.0.30319.42000
*机器名称：WENLI-PC
*命名空间：SocketsViewer.Libs
*类 名 称：WinCapHelper
*版 本 号：V1.0.0.0
*创建人： yswenli
*电子邮箱：wenguoli_520@qq.com
*创建时间：2019/4/1 11:06:35
*描述：
*=====================================================================
*修改时间：2019/4/1 11:06:35
*修 改 人： yswenli
*版 本 号： V1.0.0.0
*描    述：
*****************************************************************************/
using SocketsViewer.Model;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tamir.IPLib;
using Tamir.IPLib.Packets;

namespace SocketsViewer.Libs
{
    public class WinCapHelper
    {
        private static object syncObj = new object();
        private static WinCapHelper _capInstance;
        public static WinCapHelper WinCapInstance
        {
            get
            {
                if (null == _capInstance)
                {
                    lock (syncObj)
                    {
                        if (null == _capInstance)
                        {
                            _capInstance = new WinCapHelper();
                        }
                    }
                }
                return _capInstance;
            }
        }



        public event Action<TransferInfo> OnLogs;


        /// <summary>  
        /// 过滤条件关键字  
        /// </summary>  
        public string Filter { get; set; } = "TCP,UDP";


        private WinCapHelper()
        {


        }


        public void Listen()
        {
            Task.Factory.StartNew(() =>
            {
                ////遍历网卡  
                foreach (PcapDevice device in NetWorkController.GetList())
                {
                    ////分别启动监听，指定包的处理函数  
                    device.PcapOnPacketArrival += device_OnPacketArrival;
                    device.PcapOpen(true, 1000);
                    device.PcapStartCapture();
                }
            });
        }


        /// <summary>  
        /// 打印包信息，组合包太复杂了，所以直接把hex字符串打出来了  
        /// </summary>  
        /// <param name="p"></param>  
        /// <param name="transferInfo"></param>  
        private void PrintPacket(Packet p, out TransferInfo transferInfo)
        {
            transferInfo = null;

            if (p != null)
            {
                string s = p.ToString();

                if (!string.IsNullOrEmpty(Filter))
                {
                    var filted = false;

                    var filters = Filter.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var item in filters)
                    {
                        if (s.Contains(item))
                        {
                            filted = true;
                        }
                    }

                    if (!filted)
                        return;

                    var arr = p.ToString().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                    transferInfo = new TransferInfo()
                    {
                        Protocol = s.Contains("TCP") ? "Tcp" : "Udp",
                        Length = p.PcapHeader.PacketLength,
                        Data = p.Data
                    };

                    if (transferInfo.Protocol == "Tcp")
                    {
                        var tcpPacket = (Tamir.IPLib.Packets.TCPPacket)p;

                        transferInfo.SourceIP = tcpPacket.SourceAddress;
                        transferInfo.SourcePort = tcpPacket.SourcePort;

                        transferInfo.TargetIP = tcpPacket.DestinationAddress;
                        transferInfo.TargetPort = tcpPacket.DestinationPort;
                    }
                    else
                    {
                        var udpPacket = (Tamir.IPLib.Packets.UDPPacket)p;

                        transferInfo.SourceIP = udpPacket.SourceAddress;
                        transferInfo.SourcePort = udpPacket.SourcePort;

                        transferInfo.TargetIP = udpPacket.DestinationAddress;
                        transferInfo.TargetPort = udpPacket.DestinationPort;
                    }
                }
            }
        }


        /// <summary>  
        /// 接收到包的处理函数  
        /// </summary>  
        /// <param name="sender"></param>  
        /// <param name="e"></param>  
        private void device_OnPacketArrival(object sender, Packet packet)
        {
            TransferInfo transferInfo;

            PrintPacket(packet, out transferInfo);

            if (transferInfo != null)
            {
                try
                {
                    OnLogs(transferInfo);
                }
                catch { }
            }
        }


        public void StopAll()
        {
            foreach (PcapDevice device in NetWorkController.GetList())
            {
                try
                {
                    if (device.PcapOpened)
                    {
                        device.PcapStopCapture();
                    }
                }
                catch { }
            }
        }


    }
}
