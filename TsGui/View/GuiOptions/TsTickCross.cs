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

// TsTrafficLight.cs - Displays an icon indicating whether a device is compliant with
// a specific condition

using System.Xml.Linq;
using System;
using System.Windows.Media;
using System.Windows;

using TsGui.Validation;

namespace TsGui.View.GuiOptions
{
    public class TsTickCross: ComplianceOptionBase
    {
        private TsCrossUI _crossui;
        private TsTickUI _tickui;
        private TsWarnUI _warnui;

        //constructor
        public TsTickCross(XElement InputXml, TsColumn Parent, MainController MainController): base (InputXml, Parent, MainController)
        {
            this._crossui = new TsCrossUI();
            this._tickui = new TsTickUI();
            this._warnui = new TsWarnUI();

            this._crossui.DataContext = this;
            this._tickui.DataContext = this;
            this._warnui.DataContext = this;

            this.Control = this._tickui;
            this._validationtooltiphandler.SetTarget(this.UserControl);
            this.SetDefaults();
            this.LoadXml(InputXml);
            this.ProcessQuery();
            this.Validate();
        }

        private void SetDefaults()
        {
            this.ControlFormatting.Width = 20;
            this.ControlFormatting.Height = 20;
            this.ControlFormatting.Margin = new Thickness(0);
        }

        private new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);
        }

        protected override void UpdateView()
        {
            this.SetUI(this._state);
        }

        private void SetUI(int State)
        {
            this._state = State;
            switch (State)
            {
                case ComplianceStateValues.Inactive:
                    this.FillColor.Color = Colors.LightGray;
                    this.StrokeColor.Color = Colors.Gray;
                    this.Control = this._warnui;
                    break;
                case ComplianceStateValues.OK:
                    this.FillColor.Color = Colors.Green;
                    this.StrokeColor.Color = Colors.DarkGreen;
                    this.Control = this._tickui;
                    break;
                case ComplianceStateValues.Warning:
                    this.FillColor.Color = Colors.Orange;
                    this.FillColor.Color = Colors.DarkOrange;
                    this.Control = this._warnui;
                    break;
                case ComplianceStateValues.Error:
                    this.FillColor.Color = Colors.Red;
                    this.StrokeColor.Color = Colors.OrangeRed;
                    this.Control = this._crossui;
                    break;
                case ComplianceStateValues.Invalid:
                    this.FillColor.Color = Colors.Red;
                    this.StrokeColor.Color = Colors.OrangeRed;
                    this.Control = this._crossui;
                    break;
                default:
                    throw new ArgumentException("State is not valid");
            }
            this.OnPropertyChanged(this, "Control");
        }
    }
}