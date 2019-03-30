using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace SocketsViewer.Model
{
    public class RowInfo
    {
        public string PID
        {
            get;set;
        }

        public Icon PIcon
        {
            get;set;
        }

        public string PName
        {
            get;set;
        }

        public string Type
        {
            get;set;
        }

        public string LocalAddress
        {
            get;set;
        }

        public string LocalPort
        {
            get;set;
        }

        public string RemoteAddress
        {
            get;set;
        }

        public string RemotePort
        {
            get;set;
        }
    }
}
