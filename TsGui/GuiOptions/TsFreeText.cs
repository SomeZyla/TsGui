﻿using System;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows;

using System.Diagnostics;



namespace TsGui
{
    public class TsFreeText: IGuiOption, IEditableGuiOption
    {
        //private TsVariable tsvar;
        private MainController _controller;
        protected string _name;
        private string _value;
        protected string _label;
        protected string _disallowedChars;
        protected bool _caseSensValidate = false;
        private bool _isvalid = true;
        private int _height = 25;
        protected int _maxlength = 0;
        protected int _minlength = 0;
        private Thickness _padding = new Thickness(3, 3, 5, 3);
        private Label _labelcontrol = new Label();
        private TextBox _control = new TextBox();

        protected TsFreeText(MainController RootController)
        {
            Debug.WriteLine("TsFreeText: protected constructor called");
            this._controller = RootController;
            this._control.TextChanged += this.onChange;
        }

        public TsFreeText (XElement SourceXml, MainController RootController)
        {
            this._controller = RootController;
            this.LoadXml(SourceXml);
            this.Build();
            this._control.TextChanged += this.onChange;
        }

        public TsVariable Variable
        {
            get
            {
                this._value = this._control.Text;
                return new TsVariable(this._name,this._value);
            }
        }

        public Label Label { get { return this._labelcontrol; } }
        public Control Control { get { return this._control; } }
        public int Height { get { return this._height; } }
        public bool IsValid
        {
            get
            {
                this.Validate();
                return this._isvalid;
            }
        }

        public void LoadXml(XElement pXml)
        {
            #region
            XElement x;
            XAttribute attrib;

            attrib = pXml.Attribute("MaxLength");
            if (attrib != null)
            {
                this._maxlength = Convert.ToInt32(attrib.Value);
                this._control.MaxLength = this._maxlength;
            }

            attrib = pXml.Attribute("MinLength");
            if (attrib != null)
            { this._minlength = Convert.ToInt32(attrib.Value); }

            x = pXml.Element("Disallowed");
            if (x != null)
            {
                x = x.Element("Characters");
                if (x != null) { this._disallowedChars = x.Value; }               
            }

            x = pXml.Element("Variable");
            if (x != null)
            { this._name = x.Value; }

            x = pXml.Element("DefaultValue");
            if (x != null)
            {
                this._value = this._controller.GetValueFromList(x);
                //if required, remove invalid characters and truncate
                if (String.IsNullOrEmpty(this._disallowedChars) != true) { this._value = Checker.RemoveInvalid(this._value, this._disallowedChars); }
                if (this._maxlength > 0) { this._value = Checker.Truncate(this._value,this._maxlength); }
            }

            x = pXml.Element("Label");
            if (x != null)
            { this._label = x.Value; }

            x = pXml.Element("Height");
            if (x != null)
            { this._height = Convert.ToInt32(x.Value); }

            x = pXml.Element("Padding");
            if (x != null)
            {
                int padInt = Convert.ToInt32(x.Value);
                this._padding = new System.Windows.Thickness(padInt, padInt, padInt, padInt);
            }
            #endregion
        }


        protected void Build()
        {
            //this._control.DataContext = this;
            this._control.MaxLines = 1;
            this._control.Height = this._height;
            this._control.Text = this._value;
            this._control.Padding = this._padding;

            this._labelcontrol.Content = this._label;
            this._labelcontrol.Height = this._height;
            this._labelcontrol.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
        }


        private void onChange(object sender, RoutedEventArgs e)
        {
            this.Validate();
        }

        public void Validate()
        {
            string s = "";
            bool valid = true;

            if (Checker.ValidCharacters(this._control.Text,this._disallowedChars, this._caseSensValidate) != true)
            {
                s = "Invalid characters: " + this._disallowedChars + Environment.NewLine;
                valid = false;
            }

            if (Checker.ValidMinLength(this._control.Text, this._minlength) == false)
            {
                string charWord;
                if (this._minlength == 1) { charWord = " character"; }
                else { charWord = " characters"; }
                s = s + "Minimum length: " + this._minlength + charWord + Environment.NewLine;
                valid = false;
            }

            if (valid == false)
            {
                if ( this._control.Text.Length == 0)
                {
                    s = "Required" + Environment.NewLine + Environment.NewLine + s;
                }
                else
                {
                    s = "\"" + this._control.Text + "\" is invalid:" + Environment.NewLine + Environment.NewLine + s;
                }
                
                this.ShowToolTip(s);
            }
            else
            {
                this.HideToolTip();
            }

            this._isvalid = valid;
        }

        private void ShowToolTip(string Message)
        {
            ToolTip tt = this._control.ToolTip as ToolTip;
            if (tt == null)
            {
                tt = new ToolTip();
                tt.Placement = PlacementMode.Right;
                tt.HorizontalOffset = 5;
                tt.PlacementTarget = this._control;
                tt.Content = new TextBlock();
                this._control.ToolTip = tt;
            }

            TextBlock tb = tt.Content as TextBlock;
            
            tb.Text = Message;
            tt.Content = tb;
            tt.StaysOpen = true;
            tt.IsOpen = true;
        }

        private void HideToolTip()
        {
            if (this._control.ToolTip != null)
            {
                ToolTip tt = this._control.ToolTip as ToolTip;
                tt.StaysOpen = false;
                tt.IsOpen = false;
            }
        }
    }
}