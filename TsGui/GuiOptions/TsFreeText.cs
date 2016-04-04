﻿using System;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Data;
using System.Diagnostics;


namespace TsGui
{
    public class TsFreeText: TsBaseOption, IGuiOption, IEditableGuiOption
    {
        //fields
        #region
        private bool _caseSensValidate;
        private bool _isvalid;
        private int _maxlength;
        private int _minlength;
        new private TextBox _control;
        private ToolTip _validToolTip;
        #endregion

        //properties
        #region
        
        public string DisallowedCharacters { get; set; }       
        public bool CaseSensitive
        {
            get { return this._caseSensValidate; }
            set { this._caseSensValidate = value; }
        }
        protected int MinLength
        {
            get { return this._minlength; }
            set
            {
                if (value < 0) { this._minlength = 0; }
                else { this._minlength = value; }
            }
        }
        public TsVariable Variable
        {
            get
            {
                return new TsVariable(this.VariableName, this.Value);
            }
        }
        public int MaxLength
        {
            get { return this._maxlength; }
            set {
                this._maxlength = value;
                this.OnPropertyChanged(this,"MaxLength"); 
            }
        }                
        public bool IsValid
        {
            get
            {
                this.Validate();
                return this._isvalid;
            }
        }
        #endregion


        //constructors
        #region
        protected TsFreeText(MainController RootController): base()
        {
            Debug.WriteLine("TsFreeText: protected constructor called");
            this.Startup(RootController);
        }

        public TsFreeText (XElement SourceXml, MainController RootController): base()
        {
            this.Startup(RootController);
            this.LoadXml(SourceXml);
        }
        #endregion


        //Generic startup function to share between constructors
        private void Startup(MainController RootController)
        {
            this._caseSensValidate = false;
            this._isvalid = true;
            this._height = 25;
            this._maxlength = 0;
            this._minlength = 0;
            this._padding = new Thickness(3, 3, 5, 3);
            
            this._control = new TextBox();
            base._control = this._control;
            this._controller = RootController;

            //Subscribe to events           
            this._control.TextChanged += this.onChange;

            //setup the bindings
            this._control.DataContext = this;
            this._labelcontrol.DataContext = this;
            this._labelcontrol.SetBinding(Label.ContentProperty, new Binding("LabelText"));
            this._labelcontrol.SetBinding(Label.HeightProperty, new Binding("Height"));

            this._control.SetBinding(TextBox.MaxLengthProperty, new Binding("MaxLength"));
            this._control.SetBinding(TextBox.HeightProperty, new Binding("Height"));
            this._control.SetBinding(TextBox.TextProperty, new Binding("Value"));
            this._control.SetBinding(TextBox.PaddingProperty, new Binding("Padding"));

            this._control.MaxLines = 1;         
        }


        public void LoadXml(XElement InputXml)
        {
            #region
            XElement x;
            XAttribute attrib;

            //load the xml for the base class stuff
            this.LoadBaseXml(InputXml);

            attrib = InputXml.Attribute("MaxLength");
            if (attrib != null)
            { this.MaxLength = Convert.ToInt32(attrib.Value); }

            attrib = InputXml.Attribute("MinLength");
            if (attrib != null)
            { this._minlength = Convert.ToInt32(attrib.Value); }

            x = InputXml.Element("Disallowed");
            if (x != null)
            {
                x = x.Element("Characters");
                if (x != null) { this.DisallowedCharacters = x.Value; }               
            }

            x = InputXml.Element("DefaultValue");
            if (x != null)
            {
                this.Value = this._controller.GetValueFromList(x);
                //if required, remove invalid characters and truncate
                if (String.IsNullOrEmpty(this.DisallowedCharacters) != true) { this.Value = Checker.RemoveInvalid(this.Value, this.DisallowedCharacters); }
                if (this._maxlength > 0) { this.Value = Checker.Truncate(this.Value, this._maxlength); }
            }
            #endregion
        }

        private void onChange(object sender, RoutedEventArgs e)
        {
            this.Validate();
        }

        public void Validate()
        {
            //Debug.WriteLine("Validate started. Text: " + this._control.Text);
            string s = "";
            bool valid = true;

            if (Checker.ValidCharacters(this._control.Text,this.DisallowedCharacters, this.CaseSensitive) != true)
            {
                s = "Invalid characters: " + this.DisallowedCharacters + Environment.NewLine;
                valid = false;
            }

            if (Checker.ValidMinLength(this._control.Text, this.MinLength) == false)
            {
                string charWord;
                if (this.MinLength == 1) { charWord = " character"; }
                else { charWord = " characters"; }

                s = s + "Minimum length: " + this.MinLength + charWord + Environment.NewLine;
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

                this._validToolTip = TsWindowAlerts.ShowUnboundToolTip(this._validToolTip,this._control,s);
                this._validToolTip.Placement = PlacementMode.Right;
            }
            else
            {
                TsWindowAlerts.HideToolTip(this._validToolTip);
            }

            this._isvalid = valid;
        }
    }
}
