using Loader;
using System;
using System.Diagnostics;
using System.Net;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;

namespace KeyAuth
{
    public partial class Login : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
            int nWidthEllipse, int nHeightEllipse);

        private Timer animationTimer;
        private Button hoveredButton;
        private Color startColor = Color.FromArgb(70, 70, 70);
        private Color targetColor = Color.FromArgb(0, 120, 255); // neon blue
        private int animationStep = 0;
        private const int maxSteps = 10;
        private bool isHovering = false;

        private void ApplyRoundedCorners(Control ctrl, int radius)
        {
            ctrl.Region = Region.FromHrgn(CreateRoundRectRgn(
                0, 0, ctrl.Width, ctrl.Height, radius, radius
            ));

            ctrl.Resize += (s, e) =>
            {
                ctrl.Region = Region.FromHrgn(CreateRoundRectRgn(
                    0, 0, ctrl.Width, ctrl.Height, radius, radius
                ));
            };
        }

        public static api KeyAuthApp = new api(
             name: "Spoofer",
             ownerid: "1jQWJMrERE",
             version: "1.0"
        );

        public Login()
        {
            InitializeComponent();
            Drag.MakeDraggable(this);

            InitControlsStyle();

            animationTimer = new Timer();
            animationTimer.Interval = 20;
            animationTimer.Tick += AnimateButtonColor;
        }

        private void InitControlsStyle()
        {
            var fields = new[] { usernameField, passwordField, emailField, keyField };
            foreach (var field in fields)
            {
                ApplyRoundedCorners(field, 15);
                field.BackColor = Color.FromArgb(45, 45, 45);
                field.ForeColor = Color.White;
                field.BorderStyle = BorderStyle.None;
                field.Font = new Font("Microsoft Sans Serif", 16);
            }

            var buttons = new[] { loginBtn, registerBtn };
            foreach (var btn in buttons)
            {
                
                btn.BackColor = startColor;
                btn.ForeColor = Color.White;
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.Font = new Font("Segoe UI", 12, FontStyle.Bold);

                btn.MouseEnter += Button_MouseEnter;
                btn.MouseLeave += Button_MouseLeave;
            }

            this.BackColor = Color.FromArgb(30, 30, 30);
        }

        private void Button_MouseEnter(object sender, EventArgs e)
        {
            hoveredButton = sender as Button;
            isHovering = true;
            animationStep = 0;
            animationTimer.Start();
        }

        private void Button_MouseLeave(object sender, EventArgs e)
        {
            isHovering = false;
            animationStep = 0;
            animationTimer.Start();
        }

        private void AnimateButtonColor(object sender, EventArgs e)
        {
            if (hoveredButton == null) return;

            animationStep++;
            float progress = animationStep / (float)maxSteps;

            if (isHovering)
            {
                hoveredButton.BackColor = InterpolateColor(startColor, targetColor, progress);
            }
            else
            {
                hoveredButton.BackColor = InterpolateColor(targetColor, startColor, progress);
            }

            if (animationStep >= maxSteps)
            {
                animationTimer.Stop();
                animationStep = 0;
                if (!isHovering)
                {
                    hoveredButton = null;
                }
            }
        }

        private Color InterpolateColor(Color from, Color to, float progress)
        {
            int r = (int)(from.R + (to.R - from.R) * progress);
            int g = (int)(from.G + (to.G - from.G) * progress);
            int b = (int)(from.B + (to.B - from.B) * progress);
            return Color.FromArgb(r, g, b);
        }

        #region Misc References
        public static bool SubExist(string name)
        {
            return KeyAuthApp.user_data.subscriptions.Exists(x => x.subscription == name);
        }

        static string random_string()
        {
            string str = "";
            Random random = new Random();
            for (int i = 0; i < 5; i++)
            {
                str += Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
            }
            return str;
        }
        #endregion

        private async void Login_Load(object sender, EventArgs e)
        {
            await KeyAuthApp.init();

            if (KeyAuthApp.response.message == "invalidver")
            {
                if (!string.IsNullOrEmpty(KeyAuthApp.app_data.downloadLink))
                {
                    DialogResult dialogResult = MessageBox.Show("Yes to open file in browser\nNo to download file automatically", "Auto update", MessageBoxButtons.YesNo);
                    switch (dialogResult)
                    {
                        case DialogResult.Yes:
                            Process.Start(KeyAuthApp.app_data.downloadLink);
                            Environment.Exit(0);
                            break;
                        case DialogResult.No:
                            WebClient webClient = new WebClient();
                            string destFile = Application.ExecutablePath;
                            string rand = random_string();
                            destFile = destFile.Replace(".exe", $"-{rand}.exe");
                            webClient.DownloadFile(KeyAuthApp.app_data.downloadLink, destFile);
                            Process.Start(destFile);
                            Process.Start(new ProcessStartInfo()
                            {
                                Arguments = "/C choice /C Y /N /D Y /T 3 & Del \"" + Application.ExecutablePath + "\"",
                                WindowStyle = ProcessWindowStyle.Hidden,
                                CreateNoWindow = true,
                                FileName = "cmd.exe"
                            });
                            Environment.Exit(0);
                            break;
                        default:
                            MessageBox.Show("Invalid option");
                            Environment.Exit(0);
                            break;
                    }
                }

                MessageBox.Show("Version mismatch. No update link set. Contact the developer.");
                Environment.Exit(0);
            }

            if (!KeyAuthApp.response.success)
            {
                MessageBox.Show(KeyAuthApp.response.message);
                Environment.Exit(0);
            }
        }

        private async void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            await KeyAuthApp.forgot(usernameField.Text, emailField.Text);
            MessageBox.Show("Status: " + KeyAuthApp.response.message);
        }

        private async void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            await KeyAuthApp.upgrade(usernameField.Text, keyField.Text);
            MessageBox.Show("Status: " + KeyAuthApp.response.message);
        }

        private async void loginBtn_Click_1(object sender, EventArgs e)
        {
            await KeyAuthApp.login(usernameField.Text, passwordField.Text);
            if (KeyAuthApp.response.success)
            {
                Main main = new Main();
                main.Show();
                this.Hide();
            }
            else
                MessageBox.Show("Status: " + KeyAuthApp.response.message);
        }

        private async void registerBtn_Click(object sender, EventArgs e)
        {
            string email = emailField.Text;
            if (email == "Email (leave blank if none)")
            {
                email = null;
            }

            await KeyAuthApp.register(usernameField.Text, passwordField.Text, keyField.Text, email);
            if (KeyAuthApp.response.success)
            {
                Main main = new Main();
                main.Show();
                this.Hide();
            }
            else
                MessageBox.Show("Status: " + KeyAuthApp.response.message);
        }

        private async void licenseBtn_Click(object sender, EventArgs e)
        {
            await KeyAuthApp.license(keyField.Text);
            if (KeyAuthApp.response.success)
            {
                Main main = new Main();
                main.Show();
                this.Hide();
            }
            else
                MessageBox.Show("Status: " + KeyAuthApp.response.message);
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
