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

// TsImage.cs - Used for displaying images

using System.Xml.Linq;
using System.Windows;

using TsGui.Images;

namespace TsGui.View.GuiOptions
{
    public class TsImage : GuiOptionBase, IGuiOption
    {
        public Image Image { get; set; }
        public TsVariable Variable { get { return null; } }
        public override string CurrentValue { get { return null; } }

        //Constructor
        public TsImage(XElement InputXml, TsColumn Parent, MainController MainController) : base(Parent,MainController)
        {
            this.Control = new TsImageUI();
            this.Label = new TsLabelUI();
            this.UserControl.DataContext = this;
            this.SetDefaults();
            this.LoadXml(InputXml);          
        }


        //Methods
        private new void LoadXml(XElement InputXml)
        {
            //load the xml for the base class stuff
            base.LoadXml(InputXml);

            XElement x;
            x = InputXml.Element("Image");
            if (x != null) { this.Image = new Image(x,this._controller ); }
        }

            

        //fire an intial event to make sure things are set correctly. This is
        //called by the controller once everything is loaded

        private void SetDefaults()
        {
            this.RightCellWidth = this.LeftCellWidth + this.RightCellWidth;
            this.LeftCellWidth = 0;
            this.ControlFormatting.Padding = new Thickness(0);
            this.ControlFormatting.Margin = new Thickness(0);
            this.ControlFormatting.VerticalAlignment = VerticalAlignment.Center;
            this.ControlFormatting.HorizontalAlignment = HorizontalAlignment.Center;
        }
    }
}
