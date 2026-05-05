using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Loader;

namespace KeyAuth
{
    public partial class UserInfo : Form
    {
        public UserInfo()
        {
            InitializeComponent();
            Drag.MakeDraggable(this);

            this.Load += Main_Load;
        }

        public static bool SubExist(string name, int len)
        {
            for (var i = 0; i < len; i++)
            {
                if (Login.KeyAuthApp.user_data.subscriptions[i].subscription == name)
                {
                    return true;
                }
            }
            return false;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void CloseButton_Click_1(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private async void Main_Load(object sender, EventArgs e)
        {
            UserName.Text = $"{Login.KeyAuthApp.user_data.username}";
            IP.Text = $"IP: {Login.KeyAuthApp.user_data.ip}";
            HWID.Text = $"HWID: {Login.KeyAuthApp.user_data.hwid}";
            Expires.Text = $"Expires: {Login.KeyAuthApp.user_data.subscriptions[0].expiration}";
            Time.Text = $"Time Left: {Login.KeyAuthApp.expirydaysleft()}";
            /*userDataField.Items.Clear();
            userDataField.Items.Add($"License: {Login.KeyAuthApp.user_data.subscriptions[0].key}");
            userDataField.Items.Add($"Expires: {Login.KeyAuthApp.user_data.subscriptions[0].expiration}");
            userDataField.Items.Add($"Subscription: {Login.KeyAuthApp.user_data.subscriptions[0].subscription}");
            userDataField.Items.Add($"IP: {Login.KeyAuthApp.user_data.ip}");
            userDataField.Items.Add($"HWID: {Login.KeyAuthApp.user_data.hwid}");
            userDataField.Items.Add($"Creation Date: {Login.KeyAuthApp.user_data.CreationDate}");
            userDataField.Items.Add($"Last Login: {Login.KeyAuthApp.user_data.LastLoginDate}");
            userDataField.Items.Add($"Time Left: {Login.KeyAuthApp.expirydaysleft()}");*/
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Main main = new Main();
            this.Close();
            main.Show();
        }
    }
}
