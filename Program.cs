using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;

namespace SystemTrayApp
{    
    static class Program
	{
        

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

            

           

            





			// Show the system tray icon.					
			using (ProcessIcon pi = new ProcessIcon())
			{
				pi.Display();

                pi.InitTimer();
                
                // Make sure the application runs!
                Application.Run();                
            }


		}

       
    }


}