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

        TransferHelper _transferHelper;

        public Mainform()
        {
            InitializeComponent();
        }

        private void Mainform_Load(object sender, EventArgs e)
        {
            _transferHelper = new TransferHelper();

            var td = new Thread(new ThreadStart(new Action(() =>
            {
                while (!this.IsDisposed)
                {
                    SystemInfo systemInfo = new SystemInfo();

                    var cpu = $"核心数：{systemInfo.ProcessorCount}个 使用率：{systemInfo.CpuLoad}%";

                    var mem = $"{StringExtention.ToFormatString(systemInfo.MemoryAvailable)}/{StringExtention.ToFormatString(systemInfo.PhysicalMemory)}";

                    var sb = new StringBuilder();

                    var hds = systemInfo.GetLogicalDrives();

                    foreach (var item in hds)
                    {
                        sb.Append($"{item.Name}盘 {StringExtention.ToFormatString(item.FreeSpace)}/{StringExtention.ToFormatString(item.Size)} \t");
                    }

                    try
                    {
                        this.Invoke(new Action(() =>
                        {
                            label8.Text = cpu;
                            label9.Text = mem;
                            label10.Text = sb.ToString();
                        }));
                    }
                    catch { }

                    Thread.Sleep(1000);
                }
            })));

            td.IsBackground = true;
            td.Start();


            NetMonitor();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;

            var pName = textBox1.Text;

            var lIp = textBox2.Text;

            var lPort = textBox3.Text;

            var rIp = textBox4.Text;

            var rPort = textBox5.Text;


            var tps = NetProcessAPI.GetAllTcpConnections();

            var ups = NetProcessAPI.GetAllUdpConnections();

            dataGridView1.Rows.Clear();

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


            if (!string.IsNullOrEmpty(rIp) || !string.IsNullOrEmpty(rPort))
            {
                button1.Enabled = true;
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

            button1.Enabled = true;
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

        #region choose

        bool _choosed = false;

        string _chooseName = string.Empty;

        string _chooseIpPort = string.Empty;

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                _chooseName = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();

                _chooseIpPort = dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString() + ":" + dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString();

                _choosed = true;
            }
            catch { }
        }

        private void NetMonitor()
        {
            Thread td = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    if (_choosed)
                    {
                        var speed = 0L;

                        if (_transferHelper.Speed.ContainsKey(_chooseIpPort))
                        {
                            speed = _transferHelper.Speed[_chooseIpPort];
                        }

                        var speedStr = StringExtention.ToFormatString(speed);

                        var total = 0L;

                        if (_transferHelper.Total.ContainsKey(_chooseIpPort))
                        {
                            total = _transferHelper.Total[_chooseIpPort];
                        }

                        var totalStr = StringExtention.ToFormatString(total);

                        SystemInfo systemInfo = new SystemInfo();

                        var list = systemInfo.GetProcessInfo(_chooseName);

                        StringBuilder sb = new StringBuilder();

                        sb.AppendLine($"pid：{list[0].Id}\r\n");

                        sb.AppendLine($"pName：{_chooseName}\r\n");

                        sb.AppendLine($"active：{StringExtention.ToFormatString(list[0].TotalMilliseconds)}\r\n");

                        sb.AppendLine($"mem：{StringExtention.ToFormatString(list[0].WorkingSet64)}\r\n");

                        sb.AppendLine($"path：{list[0].FileName}\r\n");

                        sb.AppendLine($"net：{speedStr}/{totalStr}");

                        try
                        {
                            this.Invoke(new Action(() =>
                            {
                                label12.Text = sb.ToString();
                            }));
                        }
                        catch { }
                    }
                    Thread.Sleep(100);
                }
            }));

            td.IsBackground = true;
            td.Start();
        }

        #endregion



        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/yswenli/SocketsViewer");
        }

        private void 网络监控日志ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new LogForm().ShowDialog(this);
        }


        private void Mainform_FormClosed(object sender, FormClosedEventArgs e)
        {
            _transferHelper.Dispose();
        }

        
    }
}
