using System;
using Gtk;

namespace MGME4Linux
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Application.Init();
            MainWindowMGME win = new MainWindowMGME();
            win.Show();
            Application.Run();
        }
    }
}
