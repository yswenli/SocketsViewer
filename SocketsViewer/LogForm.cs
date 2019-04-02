/****************************************************************************
*项目名称：SocketsViewer
*CLR 版本：4.0.30319.42000
*机器名称：WENLI-PC
*命名空间：SocketsViewer
*类 名 称：LogForm
*版 本 号：V1.0.0.0
*创建人： yswenli
*电子邮箱：wenguoli_520@qq.com
*创建时间：2019/4/1 16:42:58
*描述：
*=====================================================================
*修改时间：2019/4/1 16:42:58
*修 改 人： yswenli
*版 本 号： V1.0.0.0
*描    述：
*****************************************************************************/
using SocketsViewer.Libs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace SocketsViewer
{
    public partial class LogForm : Form
    {

        Dictionary<string, string> _logList;

        public LogForm()
        {
            InitializeComponent();
        }

        private void LogForm_Load(object sender, EventArgs e)
        {
            _logList = LogHelper.GetLogList();

            foreach (var item in _logList)
            {
                comboBox1.Items.Add(item.Key);
            }
            comboBox1.SelectedIndex = 0;
            comboBox1_SelectedIndexChanged(comboBox1, null);

            //richTextBox1.SelectionBullet = true;
            //richTextBox1.SelectionBackColor = System.Drawing.Color.Blue;
            //richTextBox1.SelectionColor = System.Drawing.Color.White;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var key = comboBox1.Items[comboBox1.SelectedIndex].ToString();

            if (!string.IsNullOrEmpty(key))
            {
                richTextBox1.Text = LogHelper.ReadAll(_logList[key]);
                toolStripStatusLabel1.Text = $"{key}已加载完毕，{richTextBox1.Text.Length}字用时：{stopwatch.ElapsedMilliseconds}ms";
            }
            stopwatch.Stop();
        }


        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(null, null);
            }
        }


        int _searchOffset = 0;

        string _searchKey = string.Empty;

        private void button1_Click(object sender, EventArgs e)
        {
            var searchKey = textBox1.Text;

            if (!string.IsNullOrEmpty(searchKey))
            {
                if (_searchKey != searchKey)
                {
                    _searchKey = searchKey;
                    richTextBox1.Text = richTextBox1.Text;
                }
                Search(richTextBox1.Text, searchKey);
            }
            else
            {
                richTextBox1.Text = richTextBox1.Text;
            }
        }


        private void Search(string str, string searchKey)
        {
            var index = str.IndexOf(searchKey, searchKey.Length + _searchOffset);

            if (_searchOffset == 0 && index == -1)
            {
                MessageBox.Show("未能找到内容:" + searchKey);
                return;
            }

            _searchOffset = index;

            if (_searchOffset == -1)
            {
                _searchOffset = 0;
                Search(str, searchKey);
            }

            richTextBox1.Focus();
            richTextBox1.Select(_searchOffset, searchKey.Length);
            richTextBox1.ScrollToCaret();
        }


    }
}
