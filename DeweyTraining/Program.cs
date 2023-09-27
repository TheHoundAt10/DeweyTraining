using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeweyTraining
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Create an instance of the MainPage form
            MainPage mainPage = new MainPage();

            // Run the application with the MainPage form
            Application.Run(mainPage);
        }
    }
}
