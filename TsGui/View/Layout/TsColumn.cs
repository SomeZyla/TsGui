﻿#region license
// Copyright (c) 2020 Mike Pohatu
//
// This file is part of TsGui.
//
// TsGui is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, version 3 of the License.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
#endregion

// TsColumn.cs - class for columns in the gui window

using System.Collections.Generic;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;

using TsGui.View.Layout;
using TsGui.View.GuiOptions;

namespace TsGui
{
    public class TsColumn : BaseLayoutElement
    {
        private List<IGuiOption> _options = new List<IGuiOption>();
        private Grid _columnpanel;

        //properties
        #region
        public int Index { get; set; }
        public List<IGuiOption> Options { get { return this._options; } }
        public Grid Panel { get { return this._columnpanel; } }
        
        #endregion

        //constructor
        #region
        public TsColumn (XElement SourceXml,int PageIndex, BaseLayoutElement Parent) :base (Parent)
        {
            this.Index = PageIndex;
            this._columnpanel = new Grid();
            this._columnpanel.Name = "_columnpanel";
            this._columnpanel.DataContext = this;
            this._columnpanel.SetBinding(Grid.IsEnabledProperty, new Binding("IsEnabled"));
            this._columnpanel.SetBinding(Grid.VisibilityProperty, new Binding("Visibility"));
            this._columnpanel.SetBinding(Grid.ShowGridLinesProperty, new Binding("ShowGridLines"));
            this._columnpanel.SetBinding(Grid.WidthProperty, new Binding("Width"));

            this.LoadXml(SourceXml);
        }
        #endregion


        private new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);

            IEnumerable<XElement> xlist;
            IGuiOption newOption;
            int index = 0;

            this.Width = XmlHandler.GetDoubleFromXElement(InputXml, "Width", double.NaN);
            //now read in the options and add to a dictionary for later use
            //do this last so the event subscriptions don't get setup too early (no toggles fired 
            //until everything is loaded.
            xlist = InputXml.Elements("GuiOption");
            if (xlist != null)
            {
                foreach (XElement xOption in xlist)
                {
                    newOption = GuiFactory.CreateGuiOption(xOption,this);
                    if (newOption ==null) { continue; }
                    this._options.Add(newOption);

                    RowDefinition rowdef = new RowDefinition();
                    rowdef.Height = GridLength.Auto;
                    this._columnpanel.RowDefinitions.Add(rowdef);
                    Grid.SetRow(newOption.UserControl, index);

                    this._columnpanel.Children.Add(newOption.UserControl);
                    
                    index++;
                }
            }

        }
    }
}
