using BioMetrixCore.Model;
using BioMetrixCore.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BioMetrixCore
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }


        private void FormMain_Load(object sender, EventArgs e)
        {

            var obj = DBAccess.Sql.GetObjectCollection<Attn_tblDeviceInfo>("select * from Attn_tblDeviceInfo", true);
            foreach (Attn_tblDeviceInfo di in obj)
            {
                //var zx = di;
                tabControl1.TabPages.Add(di.IP, di.IP);
                var t1 = tabControl1.TabPages[di.IP];
                var zk = new UCZkService(di);
                t1.Controls.Add(zk);
            }
            notifyIcon1.Icon = this.Icon;
            notifyIcon1.Text = $"{obj.Count} Devices connected.";
        }

        private void buttonShowDevCtrl_Click(object sender, EventArgs e)
        {
            //textBoxSL
            var form = new Master();
            form.ShowDialog();
        }
    }
}
