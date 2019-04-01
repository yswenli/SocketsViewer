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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var key = comboBox1.Items[comboBox1.SelectedIndex].ToString();

            if (!string.IsNullOrEmpty(key))
            {
                textBox1.Text = LogHelper.ReadAll(_logList[key]);
            }
        }
    }
}
