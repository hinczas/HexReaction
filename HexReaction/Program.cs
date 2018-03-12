using System;
using System.Windows.Forms;

namespace HexReaction
{
    class Program
    {
        /// <summary>
     /// The mainLogic entry point for the application.
     /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
