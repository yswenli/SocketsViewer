using System;
using System.Collections.Generic;
using System.Text;

namespace SocketsViewer.Libs
{
    public static class StringExtention
    {
        public static string ToFormatString(float num)
        {
            if (num < 1024)
            {
                return $"{num.ToString("0.00")}B";
            }
            else if (num < 1024 * 1024)
            {
                return $"{(num / 1024).ToString("0.00")}K";
            }
            else if (num < 1024 * 1024 * 1024)
            {
                return $"{(num / 1024 / 1024).ToString("0.00")}M";
            }
            else
            {
                return $"{(num / (1024 * 1024 * 1024)).ToString("0.00")}G";
            }
        }


        public static string ToFormatString(double num)
        {
            if (num < 1000)
            {
                return $"{num.ToString("0.00")}ms";
            }
            else if (num < 1000 * 60)
            {
                return $"{(num / 1000).ToString("0.00")}s";
            }
            else if (num < 1000*60*60)
            {
                return $"{(num / 1000 / 60).ToString("0.00")}m";
            }
            else
            {
                return $"{(num / (1000 * 60 * 60)).ToString("0.00")}h";
            }
        }

    }
}
