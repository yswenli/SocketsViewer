/****************************************************************************
*项目名称：SocketsViewer.Libs
*CLR 版本：4.0.30319.42000
*机器名称：WENLI-PC
*命名空间：SocketsViewer.Libs
*类 名 称：NetWorkController
*版 本 号：V1.0.0.0
*创建人： yswenli
*电子邮箱：wenguoli_520@qq.com
*创建时间：2019/4/1 11:07:46
*描述：
*=====================================================================
*修改时间：2019/4/1 11:07:46
*修 改 人： yswenli
*版 本 号： V1.0.0.0
*描    述：
*****************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Tamir.IPLib;
using Tamir.IPLib.Protocols;

namespace SocketsViewer.Libs
{
    public static class NetWorkController
    {
        static PcapDeviceList _devicelist;

        static NetWorkController()
        {
            _devicelist = SharpPcap.GetAllDevices();
        }

        static NetworkDevice _default = null;
        public static NetworkDevice Default
        {
            get
            {
                if (_default == null)
                {
                    var devicelist = SharpPcap.GetAllDevices();
                    foreach (PcapDevice device in devicelist)
                    {
                        if (!device.PcapDescription.ToLower().Contains("vmware") && !device.PcapDescription.ToLower().Contains("virtual"))
                        {
                            _default = (NetworkDevice)device;
                        }
                    }
                }
                return _default;
            }
        }

        public static NetworkDevice Get(string name)
        {
            try
            {
                foreach (PcapDevice device in _devicelist)
                {
                    if (device.PcapDescription == name)
                    {
                        return (NetworkDevice)device;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "获取网卡信息失败");
            }
            return null;
        }

        public static PcapDeviceList GetList()
        {
            return _devicelist;
        }

        public static List<string> GetNameList()
        {
            var result = new List<string>();
            try
            {
                foreach (PcapDevice device in _devicelist)
                {
                    if (!device.PcapDescription.ToLower().Contains("vmware") && !device.PcapDescription.ToLower().Contains("virtual"))
                    {
                        result.Add(device.PcapDescription);
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "获取网卡信息失败");
            }
            return result;
        }

        public static void ShowList(ComboBox cb)
        {
            var list = GetNameList();
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    cb.Items.Add(item);
                }
            }
        }

        public static string ConverToMac(string name, string ip)
        {
            var dev = Get(name);
            if (dev != null)
            {
                ARP arp = new ARP(dev.PcapName);
                //arp.LocalIP = dev.IpAddress;
                //arp.LocalMAC = dev.MacAddress;
                arp.LocalIP = "192.168.1.130";
                arp.LocalMAC = "D8-CB-8A-96-82-A0";
                return arp.Resolve(ip);
            }
            return null;
        }

        public static void ConverToMacAsync(string name, string ip, Action<string> callBack = null)
        {
            new Thread(() =>
            {
                try
                {
                    var result = ConverToMac(name, ip);
                    callBack?.Invoke(result);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "转换mac失败");
                }

            })
            { IsBackground = true }.Start();
        }
    }
}
