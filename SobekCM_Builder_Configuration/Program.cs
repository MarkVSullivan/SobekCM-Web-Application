using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SobekCM.Configuration
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // Currently set this flag to false
            Control.CheckForIllegalCrossThreadCalls = false;

            bool during_install = false;
            foreach (string thisArg in args)
            {
                if (thisArg == "--installing")
                {
                    during_install = true;
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Set some colors
            // Set some defaults for round buttons
            Round_Button.Inactive_Border_Color = System.Drawing.Color.DarkGray;
            Round_Button.Inactive_Text_Color = System.Drawing.Color.White;
            Round_Button.Inactive_Fill_Color = System.Drawing.Color.DarkGray;
            Round_Button.Mouse_Down_Border_Color = System.Drawing.Color.Gray;
            Round_Button.Mouse_Down_Text_Color = System.Drawing.Color.White;
            Round_Button.Mouse_Down_Fill_Color = System.Drawing.Color.Gray;
            Round_Button.Active_Border_Color = System.Drawing.Color.FromArgb(25, 68, 141);
            Round_Button.Active_Fill_Color = System.Drawing.Color.FromArgb(25, 68, 141);
            Round_Button.Active_Text_Color = System.Drawing.Color.White;


            Application.Run(new SobekCM_Builder_Config_Edit_Form(during_install));
        }
    }
}
