/****************************************************************************
*项目名称：SocketsViewer.Libs
*CLR 版本：4.0.30319.42000
*机器名称：WENLI-PC
*命名空间：SocketsViewer.Libs
*类 名 称：LogHelper
*版 本 号：V1.0.0.0
*创建人： yswenli
*电子邮箱：wenguoli_520@qq.com
*创建时间：2019/4/1 15:07:55
*描述：
*=====================================================================
*修改时间：2019/4/1 15:07:55
*修 改 人： yswenli
*版 本 号： V1.0.0.0
*描    述：
*****************************************************************************/
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace SocketsViewer.Libs
{
    public static class LogHelper
    {
        static string _filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().FullName), "Logs");

        static bool _isClosed = false;

        static ConcurrentQueue<string> _logs;

        static LogHelper()
        {
            if (!Directory.Exists(_filePath))
            {
                Directory.CreateDirectory(_filePath);
            }

            _logs = new ConcurrentQueue<string>();


            var td = new Thread(new ThreadStart(() =>
            {
                while (!_isClosed)
                {
                    var fileName = Path.Combine(_filePath, $"SocketsViewerLog-{DateTime.Now.ToString("yyyyMMddHHmm")}.log");

                    using (var writeStream = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                    {
                        using (var streamWriter = new StreamWriter(writeStream))
                        {
                            while (!_logs.IsEmpty)
                            {
                                if (_logs.TryDequeue(out string msg))
                                {
                                    streamWriter.Write($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} {msg}");
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                    Thread.Sleep(500);
                }
            }));
            td.IsBackground = true;
            td.Start();
        }


        public static void WriteLine(string msg)
        {
            _logs.Enqueue(msg);
        }

        public static Dictionary<string, string> GetLogList()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            var files = Directory.GetFiles(_filePath);

            foreach (var item in files)
            {
                dic[Path.GetFileName(item)] = item;
            }

            return dic;
        }

        public static string ReadAll(string fileName)
        {
            using (var fs = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var bytes = new byte[fs.Length];

                fs.Read(bytes, 0, bytes.Length);

                for (int i = 0; i < bytes.Length; i++)
                {
                    if (bytes[i] == 0)
                    {
                        bytes[i] = 32;
                    }
                }

                return Encoding.ASCII.GetString(bytes);
            }
        }

        public static void Close()
        {
            _isClosed = true;
        }
    }
}
