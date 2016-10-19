using System;
using Gtk;

namespace SymlinkMaker.GUI.GTKSharp
{
    public class GtkSharpButton : GtkSharpControl, IButton
    {
        public event ButtonClickEventHandler Triggered;

        protected Button BaseWidget
        {
            get { return base.BaseWidget as Button; }
        }

        public GtkSharpButton(Button button)
            : base(button)
        {
            BaseWidget.Clicked += Button_Clicked;
        }

        public override void Dispose()
        {
            BaseWidget.Clicked -= Button_Clicked;
            base.Dispose();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            if (Triggered != null)
                Triggered(this, new ButtonEventArgs());
        }

        public void Trigger()
        {
            BaseWidget.Click();
        }
    }
}

