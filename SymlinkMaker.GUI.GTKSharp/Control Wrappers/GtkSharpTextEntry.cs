﻿using Gtk;
using System;

namespace SymlinkMaker.GUI.GTKSharp
{
    public class GtkSharpTextEntry : GtkSharpControl, ITextSource
    {
        public event TextChangedEventHandler TextChanged;

        protected Entry BaseWidget
        {
            get { return base.BaseWidget as Entry; }
        }

        public string Text
        {
            get { return BaseWidget.Text; }
            set { BaseWidget.Text = value; }
        }

        public GtkSharpTextEntry(Entry textEntry)
            : base(textEntry)
        { 
            BaseWidget.Changed += BaseWidget_Changed;
        }

        ~GtkSharpTextEntry()
        {
            BaseWidget.Changed -= BaseWidget_Changed;   
        }

        private void BaseWidget_Changed (object sender, EventArgs e)
        {
            if (TextChanged != null)
                TextChanged(this, new EventArgs());
        }
    }
}
