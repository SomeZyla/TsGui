﻿//    Copyright (C) 2016 Mike Pohatu

//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; version 2 of the License.

//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.

//    You should have received a copy of the GNU General Public License along
//    with this program; if not, write to the Free Software Foundation, Inc.,
//    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.

// TsFreeText.cs - TextBox control for entering text. Can be configured to 
// check for the validity of the entered text

using TsGui.Validation;

using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace TsGui.View.GuiOptions
{
    public class TsFreeText: GuiOptionBase, IGuiOption_2, IEditableGuiOption
    {
        protected TsFreeTextUI _textbox;
        protected ToolTip _validationtooltip;
        protected string _controltext;
        protected StringValidation _stringvalidation;
        protected Color _bordercolor;
        protected Color _mouseoverbordercolor;
        protected Color _focusbordercolor;

        //Properties
        #region
        //Custom stuff for control
        public string ControlText
        {
            get { return this._controltext; }
            set
            {
                this._controltext = value;
                this.OnPropertyChanged(this, "ControlText");
                this.Validate();
            }
        }
        public bool IsValid { get { return this.Validate(); } }
        public int MaxLength
        {
            get { return this._stringvalidation.MaxLength; }
            set { this._stringvalidation.MaxLength = value; this.OnPropertyChanged(this, "MaxLength"); }
        }
        public TsVariable Variable
        {
            get
            {
                if ((this.IsActive == false) && (this.PurgeInactive == true))
                { return null; }
                else
                { return new TsVariable(this.VariableName, this.ControlText); }
            }
        }
        #endregion

        //Constructor
        public TsFreeText(XElement InputXml, TsColumn Parent, MainController MainController): base (Parent, MainController)
        {
            this.Init(MainController);
            this.LoadXml(InputXml);
        }

        protected TsFreeText(TsColumn Parent, MainController MainController): base(Parent, MainController)
        {
            this.Init(MainController);
        }

        private void Init(MainController MainController)
        {
            this._controller = MainController;
            this._stringvalidation = new StringValidation();
            this._textbox = new TsFreeTextUI();
            this.Control = this._textbox;
            this.Label = new TsLabelUI();
            this.UserControl.DataContext = this;
            this._textbox.Control.LostFocus += this.onLoseFocus;
            this.SetDefaults();
        }

        private void SetDefaults()
        {
            this._stringvalidation.MaxLength = 32760;
            this._stringvalidation.MinLength = 0;
            this.ControlFormatting.HorizontalAlignment = HorizontalAlignment.Stretch;

            //record the default colors
            this._bordercolor = this.ControlFormatting.BorderBrush.Color;
            this._mouseoverbordercolor = this.ControlFormatting.MouseOverBorderBrush.Color;
            this._focusbordercolor = this.ControlFormatting.FocusedBorderBrush.Color;
        }

        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);
            XElement x;

            //load legacy options
            x = InputXml.Element("Disallowed");
            if (x != null) { this._stringvalidation.LoadLegacyXml(x); }
            this._stringvalidation.MinLength = XmlHandler.GetIntFromXAttribute(InputXml, "MinLength", this._stringvalidation.MinLength);
            this._stringvalidation.MaxLength = XmlHandler.GetIntFromXAttribute(InputXml, "MaxLength", this._stringvalidation.MaxLength);

            //this.LabelText = XmlHandler.GetStringFromXElement(InputXml, "Label", this.LabelText);

            x = InputXml.Element("Validation");
            if (x != null) { this._stringvalidation.LoadXml(x); }

            x = InputXml.Element("DefaultValue");
            if (x != null)
            {
                XAttribute xusecurrent = x.Attribute("UseCurrent");
                if (xusecurrent != null)
                {
                    //default behaviour is to check if the ts variable is already set. If it is, set that as the default i.e. add a query for 
                    //an environment variable to the start of the query list. 
                    if (!string.Equals(xusecurrent.Value, "false", StringComparison.OrdinalIgnoreCase))
                    {
                        XElement xcurrentquery = new XElement("Query", new XElement("Variable", this.VariableName));
                        xcurrentquery.Add(new XAttribute("Type", "EnvironmentVariable"));
                        x.AddFirst(xcurrentquery);
                    }
                }

                this._controltext = this._controller.GetValueFromList(x);
                if (this._controltext == null) { this._controltext = string.Empty; }

                //if required, remove invalid characters and truncate
                //if (!string.IsNullOrEmpty(this.DisallowedCharacters)) { this.ControlText = ResultValidator.RemoveInvalid(this.ControlText, this.DisallowedCharacters); }
                if (this.MaxLength > 0) { this._controltext = ResultValidator.Truncate(this.ControlText, this.MaxLength); }
            }

            this.Validate();
        }

        //Handle UI events
        #region

        public void onLoseFocus(object sender, RoutedEventArgs e)
        { this.Validate(); }
        #endregion


        private bool Validate()
        {
            bool valid = this._stringvalidation.IsValid(this.ControlText);
            string s = this._stringvalidation.ValidationMessage;
            
            if (valid == false)
            {
                if (string.IsNullOrEmpty(s)) { s = "\"" + this.ControlText + "\" is invalid" + Environment.NewLine + Environment.NewLine + _stringvalidation.FailedValidationMessage; }
                //{ s = "\"" + this.ControlText + "\" is invalid" + Environment.NewLine; }
                this.ShowInvalidToolTip(s);               
            }
            else { this.ClearToolTips(); }

            return valid;
        }


        public void ClearToolTips()
        {
            TsWindowAlerts.HideToolTip(this._validationtooltip);
            this.ControlFormatting.BorderBrush.Color = this._bordercolor;
            this.ControlFormatting.MouseOverBorderBrush.Color = this._mouseoverbordercolor;
            this.ControlFormatting.FocusedBorderBrush.Color = this._focusbordercolor;
        }


        public void ShowInvalidToolTip(string Message)
        {
            this._validationtooltip = TsWindowAlerts.ShowUnboundToolTip(this._validationtooltip, this._textbox.Control, Message);
            this._validationtooltip.Placement = PlacementMode.Right;
            //this._validationtooltip.hor = TextAlignment

            //update the colors to red. 
            this.ControlFormatting.BorderBrush.Color = Colors.Red;
            this.ControlFormatting.MouseOverBorderBrush.Color = Colors.Red;
            this.ControlFormatting.FocusedBorderBrush.Color = Colors.Red;
        }
    }
}
