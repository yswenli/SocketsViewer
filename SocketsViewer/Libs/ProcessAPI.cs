using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace SocketsViewer.Libs
{
    public class ProcessAPI
    {
        [DllImport("Shell32.dll")]
        private static extern int SHGetFileInfo
        (
        string pszPath,
        uint dwFileAttributes,
        out SHFILEINFO psfi,
        uint cbfileInfo,
        SHGFI uFlags
        );

        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            public SHFILEINFO(bool b)
            {
                hIcon = IntPtr.Zero; iIcon = 0; dwAttributes = 0; szDisplayName = ""; szTypeName = "";
            }
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.LPStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.LPStr, SizeConst = 80)]
            public string szTypeName;
        };

        private enum SHGFI
        {
            SmallIcon = 0x00000001,
            LargeIcon = 0x00000000,
            Icon = 0x00000100,
            DisplayName = 0x00000200,
            Typename = 0x00000400,
            SysIconIndex = 0x00004000,
            UseFileAttributes = 0x00000010
        }

        public static Icon GetIcon(string strPath, bool bSmall)
        {
            SHFILEINFO info = new SHFILEINFO(true);
            int cbFileInfo = Marshal.SizeOf(info);
            SHGFI flags;
            if (bSmall)
                flags = SHGFI.Icon | SHGFI.SmallIcon | SHGFI.UseFileAttributes;
            else
                flags = SHGFI.Icon | SHGFI.LargeIcon | SHGFI.UseFileAttributes;

            SHGetFileInfo(strPath, 256, out info, (uint)cbFileInfo, flags);
            return Icon.FromHandle(info.hIcon);
        }



        public static Icon GetIcon(int pid, bool bSmall)
        {
            try
            {
                var p = GetProcessByPID(pid);

                return GetIcon(p.MainModule.FileName, bSmall);
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public static Process GetProcessByPID(int processID)
        {
            try
            {
                return Process.GetProcessById(processID); 
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public static string GetProcessNameByPID(int processID)
        {
            //could be an error here if the process die before we can get his name
            try
            {
                Process p = GetProcessByPID(processID);
                return p?.ProcessName;
            }
            catch (Exception ex)
            {
                return "Unknown";
            }
        }
    }
}
