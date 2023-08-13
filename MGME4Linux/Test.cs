using System;
namespace MGME4Linux
{
    public partial class Test : Gtk.Window
    {
        public Test() :
                base(Gtk.WindowType.Toplevel)
        {
            this.Build();
        }
    }
}
