using System;
using System.Diagnostics;
using System.Windows.Forms;
using SystemTrayApp.Properties;
using System.Security;
using System.Security.Principal;
using System.Reflection;

namespace SystemTrayApp
{	
	class ProcessIcon : IDisposable
	{
		/// <summary>
		/// The NotifyIcon object.
		/// </summary>
		NotifyIcon ni;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessIcon"/> class.
		/// </summary>
		public ProcessIcon()
		{
			// Instantiate the NotifyIcon object.
			ni = new NotifyIcon();

            if (!IsRunAsAdministrator())
            {
                var processInfo = new ProcessStartInfo(Assembly.GetExecutingAssembly().CodeBase);

                // The following properties run the new process as administrator
                processInfo.UseShellExecute = true;
                processInfo.Verb = "runas";

                // Start the new process
                try
                {
                    Process.Start(processInfo);
                }
                catch (Exception)
                {
                    // The user did not allow the application to run as administrator
                    MessageBox.Show("Sorry, this application must be run as Administrator.");
                }

                // Shut down the current process
                //Application.Exit();
                //System.Windows.Forms.Application.Exit();
                System.Environment.Exit(1);
            }

        }

		/// <summary>
		/// Displays the icon in the system tray.
		/// </summary>
		public void Display()
		{
			// Put the icon in the system tray and allow it react to mouse clicks.			
			ni.MouseClick += new MouseEventHandler(ni_MouseClick);
			ni.Icon = Resources.greenheart;
			ni.Text = Resources.notifytext;
			ni.Visible = true;

			// Attach a context menu.
			ni.ContextMenuStrip = new ContextMenus().Create();
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		public void Dispose()
		{
			// When the application closes, this will remove the icon from the system tray immediately.
			ni.Dispose();
		}


        private Timer timer1;
        public void InitTimer()
        {
            timer1 = new Timer();
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = 5000; // in miliseconds
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            updateIcon();
        }

        //The function for getting the sleep status (0 = ok, 1 = one category blocking, 2- = two or more blocking)
        private int sleepStatus()
        {
            Debug.WriteLine("Send to debug output.");

            //Process.Start("powercfg", "-requests");

            var proc = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "powercfg.exe",
                    Arguments = "requests",
                    //Verb = "runas",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true

                }
            };

            proc.Start();
            string output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            Debug.WriteLine(output);
            var count1 = (output.Length - output.Replace(":", "").Length) / ":".Length;
            var count2 = (output.Length - output.Replace("None", "").Length) / "None".Length;
            Debug.WriteLine(count1 + " " + count2);

            return (count1 - count2);
        }

        void updateIcon()
        {
            if (sleepStatus() > 1) this.ni.Icon = Properties.Resources.redheart;
            else if (sleepStatus() == 1) this.ni.Icon = Properties.Resources.yellowheart;
            else this.ni.Icon = Properties.Resources.greenheart;
        }
        /// <summary>
        /// Handles the MouseClick event of the ni control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        void ni_MouseClick(object sender, MouseEventArgs e)
		{
			// Handle mouse button clicks.
			if (e.Button == MouseButtons.Left)
			{
                //Update Tray Icon.
                updateIcon();
            }
		}

        private bool IsRunAsAdministrator()
        {
            var wi = WindowsIdentity.GetCurrent();
            var wp = new WindowsPrincipal(wi);

            return wp.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}