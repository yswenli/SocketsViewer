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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace SocketsViewer.Libs
{
    public static class LogHelper
    {
        static string _filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().FullName), "Logs");

        static string _fileName = string.Empty;

        static FileStream _writeStream;

        static StreamWriter _streamWriter;

        static FileStream _readStream;

        static StreamReader _streamReader;

        static LogHelper()
        {
            if (!Directory.Exists(_filePath))
            {
                Directory.CreateDirectory(_filePath);
            }

            _fileName = Path.Combine(_filePath, $"SocketsViewerLog-{DateTime.Now.ToString("yyyyMMddHHmm")}.log");

            _writeStream = File.Open(_fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);

            _streamWriter = new StreamWriter(_writeStream);

            _readStream = File.Open(_fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            _streamReader = new StreamReader(_readStream);
        }


        public static void WriteLine(string msg)
        {
            _streamWriter.Write($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} {msg}");
        }

        public static string ReadLine()
        {
            return _streamReader.ReadLine();
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
            StringBuilder sb = new StringBuilder();

            using (var fs = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                fs.Position = 0;

                var bytes = new byte[1024];

                int count = 0;

                do
                {
                    count = fs.Read(bytes, 0, bytes.Length);

                    if (count == 0)
                    {
                        break;
                    }
                    else
                    {
                        fs.Position += count;
                    }
                    sb.Append(Encoding.ASCII.GetString(bytes, 0, count));
                }
                while (count > 0);

            }
            return sb.ToString();
        }

        public static void Close()
        {
            try
            {
                _streamWriter.Flush();
                _streamWriter.Close();
                _streamReader.Close();
            }
            catch { }

        }
    }
}
