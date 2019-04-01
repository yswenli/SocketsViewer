/****************************************************************************
*项目名称：SocketsViewer.Libs
*CLR 版本：4.0.30319.42000
*机器名称：WENLI-PC
*命名空间：SocketsViewer.Libs
*类 名 称：TransferHelper
*版 本 号：V1.0.0.0
*创建人： yswenli
*电子邮箱：wenguoli_520@qq.com
*创建时间：2019/4/1 13:26:12
*描述：
*=====================================================================
*修改时间：2019/4/1 13:26:12
*修 改 人： yswenli
*版 本 号： V1.0.0.0
*描    述：
*****************************************************************************/
using SocketsViewer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SocketsViewer.Libs
{
    public class TransferHelper : IDisposable
    {
        WinCapHelper winCapHelper;

        public Dictionary<string, long> Total
        {
            get;
            private set;
        } = new Dictionary<string, long>();

        Dictionary<string, long> _old = new Dictionary<string, long>();

        bool isAlived = true;

        Thread _thread;

        public Dictionary<string, long> Speed
        {
            get;
            private set;
        } = new Dictionary<string, long>();

        public TransferHelper()
        {
            winCapHelper = WinCapHelper.WinCapInstance;
            winCapHelper.OnLogs += WinCapHelper_OnLogs;
            winCapHelper.Filter = "TCP,UDP";
            winCapHelper.Listen();

            _thread = new Thread(new ThreadStart(() =>
            {
                while (isAlived)
                {
                    List<KeyValuePair<string, long>> list = new List<KeyValuePair<string, long>>();

                    try
                    {
                        list = Total.ToList();
                    }
                    catch { }

                    foreach (var item in list)
                    {
                        if (!_old.ContainsKey(item.Key))
                        {
                            _old[item.Key] = 0;
                        }

                        Speed[item.Key] = item.Value - _old[item.Key];

                        _old[item.Key] = item.Value;
                    }
                    Thread.Sleep(1000);
                }
            }));
            _thread.IsBackground = true;
            _thread.Start();
        }

        private void WinCapHelper_OnLogs(TransferInfo transfer)
        {
            var sip = transfer.SourceIP + ":" + transfer.SourcePort;

            if (Total.ContainsKey(sip))
            {
                Total[sip] += transfer.Length;
            }
            else
            {
                Total[sip] = transfer.Length;
            }

            LogHelper.WriteLine($"{transfer.Protocol}\t{sip}\t{transfer.TargetIP + ":" + transfer.TargetPort}\t{Encoding.ASCII.GetString(transfer.Data)}");
        }

        public void Dispose()
        {
            isAlived = false;
            winCapHelper.StopAll();
            LogHelper.Close();
        }
    }
}
