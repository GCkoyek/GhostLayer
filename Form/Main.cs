using Loader;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Media;
using System.Text;
using System.Runtime.InteropServices;
using MetroFramework.Controls;
using Microsoft.Win32;
using Siticone.Desktop.UI.WinForms;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Security.Cryptography;

namespace KeyAuth
{
    public partial class Main : Form
    {
        /*
        * 
        * WATCH THIS VIDEO TO SETUP APPLICATION: https://www.youtube.com/watch?v=RfDTdiBq4_o
        * 
	     * READ HERE TO LEARN ABOUT KEYAUTH FUNCTIONS https://github.com/KeyAuth/KeyAuth-CSHARP-Example#keyauthapp-instance-definition
		 *
        */


        public Main()
        {
            InitializeComponent();
            Drag.MakeDraggable(this);
        }

        private Logs logWindow;
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

        public static string RandomId(int length)
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string result = "";
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                result += chars[random.Next(chars.Length)];
            }

            return result;
        }

        private string RandomIdprid(int length)
        {
            const string digits = "0123456789";
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var random = new Random();
            var id = new char[length];
            int dashIndex = 5;
            int letterIndex = 17;
            for (int i = 0; i < length; i++)
            {
                if (i == dashIndex)
                {
                    id[i] = '-';
                    dashIndex += 6;
                }
                else if (i == letterIndex)
                {
                    id[i] = letters[random.Next(letters.Length)];
                }
                else if (i == letterIndex + 1)
                {
                    id[i] = letters[random.Next(letters.Length)];
                }
                else
                {
                    id[i] = digits[random.Next(digits.Length)];
                }
            }
            return new string(id);
        }

        private async void checkSessionBtn_Click_1(object sender, EventArgs e)
        {
            await Login.KeyAuthApp.check();
            MessageBox.Show(Login.KeyAuthApp.response.message);
        }
        private async void CloseButton_Click(object sender, EventArgs e)
        {
            await Login.KeyAuthApp.logout(); // ends the sessions once the application closes
            Environment.Exit(0);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            UserInfo userInfo = new UserInfo();
            userInfo.Show();
            this.Hide();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            string discordInvite = "https://discord.gg/twoj_link"; // <-- Zmieñ na swój link
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = discordInvite,
                    UseShellExecute = true // Wa¿ne w .NET Core i nowszych
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nie uda³o siê otworzyæ linku Discord: " + ex.Message);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (logWindow == null || logWindow.IsDisposed)
            {
                logWindow = new Logs();
                logWindow.Show();
            }
            else
            {
                logWindow.Show();
            }
        }

        /*private void SaveLogs(string id, string logBefore, string logAfter)
        {
            string logsFolderPath = Path.Combine(Application.StartupPath, "Logs");
            if (!Directory.Exists(logsFolderPath))
                Directory.CreateDirectory(logsFolderPath);

            string logFileName = Path.Combine(logsFolderPath, $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt");
            string logEntryBefore = $"{DateTime.Now:HH:mm:ss}: ID {id} -  {logBefore}";
            string logEntryAfter = $"{DateTime.Now:HH:mm:ss}: ID {id} -  {logAfter}";

            File.AppendAllText(logFileName, logEntryBefore + Environment.NewLine);
            File.AppendAllText(logFileName, logEntryAfter + Environment.NewLine);

            AppendLogEntryToWindow(logEntryBefore, logEntryAfter);
        }

        private void AppendLogEntryToWindow(string logEntryBefore, string logEntryAfter)
        {
            if (logWindow != null && !logWindow.IsDisposed)
            {
                logWindow.AddLogEntry(logEntryBefore, logEntryAfter);
            }
        }
        public void AddLogEntry(string logEntryBefore, string logEntryAfter)
        {
            richTextBoxLogs.SelectionColor = Color.Gray;
            richTextBoxLogs.AppendText(logEntryBefore + Environment.NewLine);

            richTextBoxLogs.SelectionColor = Color.White;
            richTextBoxLogs.AppendText(logEntryAfter + Environment.NewLine);
        }*/

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string originalName;
                string newName = "SecHex-" + RandomId(7);
                using (RegistryKey computerNameKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\ComputerName\\ComputerName", true))
                {
                    if (computerNameKey != null)
                    {
                        originalName = computerNameKey.GetValue("ComputerName").ToString();

                        computerNameKey.SetValue("ComputerName", newName);
                        computerNameKey.SetValue("ActiveComputerName", newName);
                        computerNameKey.SetValue("ComputerNamePhysicalDnsDomain", "");
                    }
                    else
                    {
                        //ShowNotification("ComputerName key not found.", NotificationType.Error);
                        return;
                    }
                }
                using (RegistryKey activeComputerNameKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\ComputerName\\ActiveComputerName", true))
                {
                    if (activeComputerNameKey != null)
                    {
                        activeComputerNameKey.SetValue("ComputerName", newName);
                        activeComputerNameKey.SetValue("ActiveComputerName", newName);
                        activeComputerNameKey.SetValue("ComputerNamePhysicalDnsDomain", "");
                    }
                    else
                    {
                        //ShowNotification("ActiveComputerName key not found.", NotificationType.Error);
                        return;
                    }
                }
                using (RegistryKey hostnameKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\Tcpip\\Parameters", true))
                {
                    if (hostnameKey != null)
                    {
                        hostnameKey.SetValue("Hostname", newName);
                        hostnameKey.SetValue("NV Hostname", newName);
                    }
                    else
                    {
                        //ShowNotification("Hostname key not found.", NotificationType.Error);
                        return;
                    }
                }
                using (RegistryKey interfacesKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\Tcpip\\Parameters\\Interfaces", true))
                {
                    if (interfacesKey != null)
                    {
                        foreach (string interfaceName in interfacesKey.GetSubKeyNames())
                        {
                            using (RegistryKey interfaceKey = interfacesKey.OpenSubKey(interfaceName, true))
                            {
                                if (interfaceKey != null)
                                {
                                    interfaceKey.SetValue("Hostname", newName);
                                    interfaceKey.SetValue("NV Hostname", newName);
                                }
                            }
                        }
                    }
                }

                string logBefore = "ComputerName - Before: " + originalName;
                string logAfter = "ComputerName - After: " + newName;
                //SaveLogs("pcname", logBefore, logAfter);
                //ShowNotification("PC-Name successfully spoofed.", NotificationType.Success);

            }
            catch (Exception ex)
            {
               
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                using (RegistryKey productKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion", true))
                {
                    if (productKey != null)
                    {
                        string originalProductId = productKey.GetValue("ProductId")?.ToString();

                        string newProductId = RandomIdprid(20);
                        productKey.SetValue("ProductId", newProductId);

                        string logBefore = "Product ID - Before: " + originalProductId;
                        string logAfter = "Product ID - After: " + newProductId;
                        //SaveLogs("product", logBefore, logAfter);

                        //ShowNotification("Product ID successfully spoofed.", NotificationType.Success);
                    }
                    else
                    {
                        //ShowNotification("Product registry key not found.", NotificationType.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                //ShowNotification("An error occurred while changing the Product ID: " + ex.Message, NotificationType.Error);
            }
        }
    }
}