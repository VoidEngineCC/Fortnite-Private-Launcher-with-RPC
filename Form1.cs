using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using DiscordRPC; // Ensure you have the right using directive for Discord RPC
using WindowsFormsApp1.files;
using WindowsFormsApp1.Properties;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private DiscordRpcClient client; // Use DiscordRpcClient instead of DiscordClient
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;
        private Timer fadeTimer; // Timer for fade effect
        private float opacityStep = 0.05f; // Step for opacity change

        public Form1()
        {
            InitializeComponent();
            InitializeDiscordRichPresence(); // Initialize Discord Rich Presence
            InitializeSettings();
            InitializeUI();
            InitializeFadeTimer();
        }

        private void InitializeDiscordRichPresence()
        {
            client = new DiscordRpcClient("842898656318849054"); // Use your Application ID

            client.Initialize();
            UpdateRichPresence("Playing Battle Royale", "testing...", "large_image_key", "Large Image Text");
        }

        private void UpdateRichPresence(string state, string details, string largeImageKey, string largeImageText)
        {
            client.SetPresence(new RichPresence()
            {
                State = state,
                Details = details,
                Assets = new Assets()
                {
                    LargeImageKey = largeImageKey,
                    LargeImageText = largeImageText
                },
                Buttons = new DiscordRPC.Button[] // Specify the namespace for Button
                {
            new DiscordRPC.Button // Fully qualified name for DiscordRPC Button
            {
                Label = "Join us!", // The label displayed on the button
                Url = "https://discord.gg/ne7menaqPDn36WBX8" // Optional: The URL to open when the button is clicked
            }
                }
            });
        }



        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            client.Dispose(); // Dispose the client on form closing
        }

        private void InitializeSettings()
        {
            string iniFilePath = "config.ini";

            if (File.Exists(iniFilePath))
            {
                IniFile ini = new IniFile(iniFilePath);
                string gamePath = ini.Read("Settings", "GamePath", "");

                if (!string.IsNullOrEmpty(gamePath))
                {
                    this.PathBox.Text = gamePath;
                }
                else
                {
                    MessageBox.Show("GamePath not found in INI file, using default path.");
                    this.PathBox.Text = "DefaultPath"; // Fall back to default if empty
                }
            }
            else
            {
                MessageBox.Show("INI file not found, using default path.");
                this.PathBox.Text = "DefaultPath"; // Fall back to default if file doesn't exist
            }
        }

        private void InitializeUI()
        {
            this.BackColor = Color.FromArgb(30, 30, 30); // Dark grey
            this.ForeColor = Color.White; // Default text color to white

            this.PathBox.BackColor = Color.FromArgb(45, 45, 45); // Slightly lighter dark grey for input
            this.PathBox.ForeColor = Color.White;

            ConfigureButtonStyle(this.button2);
            ConfigureButtonStyle(this.button1);
            this.closeButton.BackColor = Color.FromArgb(200, 50, 50); // Close button color
            this.closeButton.ForeColor = Color.White; // Bright text color for close button
        }

        private void ConfigureButtonStyle(System.Windows.Forms.Button button) // Fully qualified Button type
        {
            button.BackColor = Color.FromArgb(60, 60, 60); // Dark button
            button.ForeColor = Color.White; // Bright text color for button
            button.FlatStyle = FlatStyle.Flat; // Flat style for a cleaner look
            button.Font = new Font("Segoe UI", 10F, FontStyle.Bold); // Bold font

            button.MouseEnter += Button_MouseEnter;
            button.MouseLeave += Button_MouseLeave;
        }

        private void Button_MouseEnter(object sender, EventArgs e)
        {
            if (sender is System.Windows.Forms.Button button) // Fully qualified Button type
            {
                button.BackColor = Color.FromArgb(80, 80, 80); // On hover color change
            }
        }

        private void Button_MouseLeave(object sender, EventArgs e)
        {
            if (sender is System.Windows.Forms.Button button) // Fully qualified Button type
            {
                button.BackColor = Color.FromArgb(60, 60, 60); // Reset to original color
            }
        }

        private void InitializeFadeTimer()
        {
            fadeTimer = new Timer();
            fadeTimer.Interval = 50; // Adjust the interval for speed of fade
            fadeTimer.Tick += new EventHandler(fadeTimer_Tick);
        }

        private void fadeTimer_Tick(object sender, EventArgs e)
        {
            this.Opacity -= opacityStep;
            if (this.Opacity <= 0)
            {
                fadeTimer.Stop();
                LaunchGame(); // Call the method to launch the game
            }
        }

        private void LaunchGame()
        {
            var clientPath = Path.Combine(PathBox.Text, $"FortniteGame\\Binaries\\Win64\\{stringvalues.ClientExecutable}");

            if (!File.Exists(clientPath))
            {
                MessageBox.Show($"\"{stringvalues.ClientExecutable}\" was not found, please make sure it exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var nativePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), stringvalues.dll);

            if (!File.Exists(nativePath))
            {
                MessageBox.Show($"\"{stringvalues.dll}\" was not found, please make sure it exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Process process = StartProcess(Path.Combine(PathBox.Text, "FortniteGame\\Binaries\\Win64\\FortniteLauncher.exe"), true, "-NOSSLPINNING");
            Process process2 = StartProcess(Path.Combine(PathBox.Text, "FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping_BE.exe"), true, "");
            Process process3 = StartProcess(Path.Combine(PathBox.Text, "FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping.exe"), false, "-AUTH_TYPE=epic");
            process3.WaitForInputIdle();
            base.Hide();
            inject.InjectDll(process3.Id, Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), stringvalues.dll)); // Ensure inject is defined properly
            process3.WaitForExit();
            base.Show();
            MessageBox.Show("Thanks for playing Heavenly Hope!");
            this.Opacity = 1; // Reset opacity
        }

        private void button2_Click(object sender, EventArgs e)
        {
            fadeTimer.Start(); // Start the fade effect
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Application.Exit(); // Exit the application
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.discord.com/butterfly"); // Open the Discord URL
        }

        private void titleBar_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }

        private void titleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(dif));
            }
        }

        private void titleBar_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        public static Process StartProcess(string path, bool shouldFreeze, string extraArgs = "")
        {
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = path,
                    Arguments = $"-epicapp=Fortnite -epicenv=Prod -epiclocale=en-us -epicportal -noeac -fromfl=be -fltoken=5dh74c635862g575778132fb -skippatchcheck {extraArgs}"
                }
            };
            process.Start();

            if (shouldFreeze)
            {
                foreach (ProcessThread thread in process.Threads)
                {
                    Win32.SuspendThread(Win32.OpenThread(2, false, thread.Id));
                }
            }
            return process;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.SelectedPath = "C:\\";

                DialogResult result = folderBrowserDialog.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
                {
                    PathBox.Text = folderBrowserDialog.SelectedPath;

                    // Update the config.ini file with the new path
                    string iniFilePath = "config.ini"; // Path to the INI file
                    IniFile ini = new IniFile(iniFilePath);
                    ini.Write("Settings", "GamePath", folderBrowserDialog.SelectedPath);

                    MessageBox.Show("Game path updated in config.ini", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}