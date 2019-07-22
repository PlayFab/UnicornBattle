using System;
using System.Windows.Forms;

namespace UnicornBattleInstaller
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
            FormManager.Initialize();
            Application.Run(FormManager.Get<PlayFabLoginForm>("PlayFabLoginForm"));
        }

    }
}
