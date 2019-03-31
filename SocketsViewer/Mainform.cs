using SocketsViewer.Libs;
using SocketsViewer.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SocketsViewer
{
    public partial class Mainform : Form
    {

        List<RowInfo> _rowInfos = new List<RowInfo>();

        public Mainform()
        {
            InitializeComponent();
        }

        public delegate void Action();

        private void Mainform_Load(object sender, EventArgs e)
        {
            var td = new Thread(new ThreadStart(new Action(() =>
            {
                while (true)
                {
                    SystemInfo systemInfo = new SystemInfo();

                    var cpu = $"核心：{systemInfo.ProcessorCount}个 使用率：{systemInfo.CpuLoad.ToString("0.00")}%";

                    var mem = $"{StringExtention.ToFormatString(systemInfo.MemoryAvailable)}/{StringExtention.ToFormatString(systemInfo.PhysicalMemory)}";

                    var sb = new StringBuilder();

                    var hds = systemInfo.GetLogicalDrives();

                    foreach (var item in hds)
                    {
                        sb.Append($"{item.Name}盘 {StringExtention.ToFormatString(item.FreeSpace)}/{StringExtention.ToFormatString(item.Size)} \t");
                    }

                    this.Invoke(new Action(() =>
                    {
                        label8.Text = cpu;
                        label9.Text = mem;
                        label10.Text = sb.ToString();
                    }));

                    Thread.Sleep(1000);
                }
            })));

            td.IsBackground = true;
            td.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var pName = textBox1.Text;

            var lIp = textBox2.Text;

            var lPort = textBox3.Text;

            var rIp = textBox4.Text;

            var rPort = textBox5.Text;

            dataGridView1.Rows.Clear();


            var tps = NetProcessAPI.GetAllTcpConnections();


            foreach (var p in tps)
            {
                if (!string.IsNullOrEmpty(pName) && ProcessAPI.GetProcessNameByPID(p.owningPid).IndexOf(pName, StringComparison.OrdinalIgnoreCase) == -1)
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(lIp) && p.LocalAddress.ToString() != lIp)
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(lPort) && p.LocalPort.ToString() != lPort)
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(rIp) && p.RemoteAddress.ToString() != rIp)
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(rPort) && p.RemotePort.ToString() != rPort)
                {
                    continue;
                }

                dataGridView1.Rows.Add(new object[] { p.owningPid.ToString(), ProcessAPI.GetIcon(p.owningPid, true), ProcessAPI.GetProcessNameByPID(p.owningPid), "TCP", p.LocalAddress.ToString(), p.LocalPort.ToString(), p.RemoteAddress.ToString(), p.RemotePort.ToString() });
            }

            var ups = NetProcessAPI.GetAllUdpConnections();

            if (!string.IsNullOrEmpty(rIp) || !string.IsNullOrEmpty(rPort))
            {
                return;
            }

            foreach (var p in ups)
            {
                if (!string.IsNullOrEmpty(pName) && ProcessAPI.GetProcessNameByPID(p.owningPid).IndexOf(pName, StringComparison.OrdinalIgnoreCase) == -1)
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(lIp) && p.LocalAddress.ToString() != lIp)
                {
                    continue;
                }
                if (!string.IsNullOrEmpty(lPort) && p.LocalPort.ToString() != lPort)
                {
                    continue;
                }

                dataGridView1.Rows.Add(new object[] { p.owningPid.ToString(), ProcessAPI.GetIcon(p.owningPid, true), ProcessAPI.GetProcessNameByPID(p.owningPid), "UDP", p.LocalAddress.ToString(), p.LocalPort.ToString(), "", "" });
            }

            for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
            {
                DataGridViewRow r = this.dataGridView1.Rows[i];
                r.HeaderCell.Value = string.Format("{0}", i + 1);
            }
            this.dataGridView1.Refresh();
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(button1, null);
            }
        }

        private void textBox2_KeyUp(object sender, KeyEventArgs e)
        {
            textBox1_KeyUp(sender, e);
        }

        private void textBox3_KeyUp(object sender, KeyEventArgs e)
        {
            textBox1_KeyUp(sender, e);
        }

        private void textBox4_KeyUp(object sender, KeyEventArgs e)
        {
            textBox1_KeyUp(sender, e);
        }

        private void textBox5_KeyUp(object sender, KeyEventArgs e)
        {
            textBox1_KeyUp(sender, e);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            button1_Click(button1, null);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            timer1.Enabled = checkBox1.Checked;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                var pName = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();

                SystemInfo systemInfo = new SystemInfo();

                var list = systemInfo.GetProcessInfo(pName);

                StringBuilder sb = new StringBuilder();

                sb.AppendLine($"pid：{list[0].Id}\r\n");

                sb.AppendLine($"pName：{pName}\r\n");

                sb.AppendLine($"live：{StringExtention.ToFormatString(list[0].TotalMilliseconds)}\r\n");

                sb.AppendLine($"mem：{StringExtention.ToFormatString(list[0].WorkingSet64)}\r\n");

                sb.AppendLine($"path：{list[0].FileName}\r\n");

                label12.Text = sb.ToString();
            }
            catch { }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/yswenli/SocketsViewer");
        }
    }
}
