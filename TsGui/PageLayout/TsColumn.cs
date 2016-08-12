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

// TsColumn.cs - class for columns in the gui window

using System.Collections.Generic;
using System.Xml.Linq;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System.ComponentModel;
using System;
//using System.Diagnostics;

namespace TsGui
{
    public class TsColumn : IGroupParent, INotifyPropertyChanged, IGroupable
    {


        private bool _enabled;
        private bool _purgeInactive;
        private Group _group;
        private bool _hidden;
        private List<IGuiOption> options = new List<IGuiOption>();
        private Grid _columngrid;
        private bool _gridlines;
        private GridLength _labelwidth;
        private GridLength _controlwidth;
        private GridLength _fullwidth;
        private MainController _controller;
        private ColumnDefinition _coldefControls;
        private ColumnDefinition _coldefLabels;

        //properties
        #region
        public bool PurgeInactive
        {
            get { return this._purgeInactive; }
            set
            {
                this._purgeInactive = value;
                foreach (IGuiOption option in this.options)
                { option.PurgeInactive = value; }
            }
        }
        public int ActiveGroupsCount { get; set; }
        public GridLength LabelWidth
        {
            get { return this._labelwidth; }
            set
            {
                this._labelwidth = value;
                this.OnPropertyChanged(this, "LabelWidth");
            }
        }
        public GridLength ControlWidth
        {
            get { return this._controlwidth; }
            set
            {
                this._controlwidth = value;
                this.OnPropertyChanged(this, "ControlWidth");
            }
        }
        public GridLength Width
        {
            get { return this._fullwidth; }
            set
            {
                this._fullwidth = value;
                this.OnPropertyChanged(this, "Width");
            }
        }
        public bool ShowGridLines
        {
            get { return this._gridlines; }
            set
            {
                this._gridlines = value;
                this.OnPropertyChanged(this, "ShowGridLines");
            }
        }
        public int Index { get; set; }
        public List<IGuiOption> Options { get { return this.options; } }
        public Panel Panel { get { return this._columngrid; } }
        public bool IsEnabled
        {
            get { return this._enabled; }
            set
            {
                this._enabled = value;
                //Debug.WriteLine("TsColumn: ParentChanged raised: IsEnabled, IsHidden: " + IsEnabled + IsHidden);
                this.ParentChanged?.Invoke(this, this.IsEnabled, this.IsHidden);
                this.OnPropertyChanged(this, "IsEnabled");
            }
        }
        public bool IsHidden
        {
            get { return this._hidden; }
            set
            {
                this._hidden = value;
                //Debug.WriteLine("TsColumn: ParentChanged raised: IsEnabled, IsHidden: " + IsEnabled + IsHidden);
                this.ParentChanged?.Invoke(this, this.IsEnabled, this.IsHidden);
                this.OnPropertyChanged(this, "IsHidden");
            }
        }
        public bool IsActive
        {
            get
            {
                if ((this.IsEnabled == true) && (this.IsHidden == false))
                { return true; }
                else { return false; }
            }
        }
        #endregion

        //constructor
        #region
        public TsColumn (XElement SourceXml,int PageIndex, MainController RootController, bool PurgeInactive)
        {

            this._controller = RootController;
            this.Index = PageIndex;
            this._purgeInactive = PurgeInactive;
            this._columngrid = new Grid();

            this._columngrid.Margin = new Thickness(0);
            this._coldefControls = new ColumnDefinition();
            this._coldefLabels = new ColumnDefinition();

            this._columngrid.DataContext = this;
            this._columngrid.SetBinding(Grid.ShowGridLinesProperty, new Binding("ShowGridLines"));

            this._coldefLabels.SetBinding(ColumnDefinition.WidthProperty, new Binding("LabelWidth"));
            this._coldefLabels.SetBinding(ColumnDefinition.WidthProperty, new Binding("LabelWidth"));
            this._coldefControls.SetBinding(ColumnDefinition.WidthProperty, new Binding("ControlWidth"));

            //Set defaults
            this._columngrid.VerticalAlignment = VerticalAlignment.Top;

            this._columngrid.ColumnDefinitions.Add(this._coldefLabels);
            this._columngrid.ColumnDefinitions.Add(this._coldefControls);

            this.ActiveGroupsCount = 0;

            this.LoadXml(SourceXml);
            this.Build();
        }
        #endregion

        //Setup events and handlers
        #region
        public event PropertyChangedEventHandler PropertyChanged;

        // OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(object sender, string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(sender, new PropertyChangedEventArgs(name));
            }
        }

        public event ParentToggleEvent ParentChanged;
        //Only subscribed if member of a group. Registers changes to parent elements. 
        public void OnParentChanged(IGroupParent p, bool IsEnabled, bool IsHidden)
        {
            //Debug.WriteLine("    TsColumn: OnParentChanged called: IsEnabled, IsHidden:" + IsEnabled + IsHidden);

            if ((IsHidden == true) || (IsEnabled == false))
            {
                this.IsEnabled = IsEnabled;
                this.IsHidden = IsHidden;
            }
            else if (this._group != null)
            {
                this.IsHidden = this._group.IsHidden;
                this.IsEnabled = this._group.IsEnabled;
            }
            else
            {
                this.IsHidden = false;
                this.IsEnabled = true;
            }
            //raise new event for child controls
            this.ParentChanged?.Invoke(this, IsEnabled, IsHidden);
        }
        public void OnGroupHide(bool Hide)
        {
            GroupingLogic.OnGroupHide(this, Hide);
        }

        public void OnGroupEnable(bool Enable)
        {
            GroupingLogic.OnGroupEnable(this, Enable);
        }

        #endregion

        private void LoadXml(XElement InputXml)
        {
            XElement x;
            string groupID = null;
            IEnumerable<XElement> optionsXml;
            IGuiOption newOption;
            XAttribute xAttrib;

            xAttrib = InputXml.Attribute("PurgeInactive");
            if (xAttrib != null)
            { this.PurgeInactive = Convert.ToBoolean(xAttrib.Value); }

            x = InputXml.Element("Group");
            if (x != null)
            {
                groupID = x.Value;
                this._group = this._controller.AddToGroup(groupID, this);
            }

            //now read in the options and add to a dictionary for later use
            optionsXml = InputXml.Elements("GuiOption");
            if (optionsXml != null)
            {
                foreach (XElement xOption in optionsXml)
                {
                    newOption = GuiFactory.CreateGuiOption(xOption,this._controller);
                    newOption.PurgeInactive = this._purgeInactive;
                    this.options.Add(newOption);
                    this._controller.AddOptionToLibary(newOption);

                    //register for events
                    //if ((this._group != null) || (this._parent.Group != null)) { this.ParentChanged += newOption.OnParentChanged; }
                    this.ParentChanged += newOption.OnParentChanged;
                }
            }

            x = InputXml.Element("LabelWidth");
            if (x != null)
            { this.LabelWidth = new GridLength(Convert.ToDouble(x.Value)); }

            x = InputXml.Element("ControlWidth");
            if (x != null)
            { this.ControlWidth = new GridLength(Convert.ToDouble(x.Value)); }

            x = InputXml.Element("Width");
            if (x != null)
            { this.Width = new GridLength(Convert.ToDouble(x.Value)); }

            x = InputXml.Element("Enabled");
            if (x != null)
            { this.IsEnabled = Convert.ToBoolean(x.Value); }

            x = InputXml.Element("Hidden");
            if (x != null)
            { this.IsHidden = Convert.ToBoolean(x.Value); }
        }


        public void Build()
        {
            int rowindex = 0;
            double width =0;
            
            
            foreach (IGuiOption option in this.options)
            {
                //option.Control.Margin = this._margin;
                //option.Label.Margin = this._margin;

                RowDefinition coldefRow = new RowDefinition();
                //coldefRow.Height = new GridLength(option.Height + option.Margin.Top + option.Margin.Bottom) ;
                this._columngrid.RowDefinitions.Add(coldefRow);

                Grid.SetColumn(option.Label, 0);
                Grid.SetColumn(option.Control, 1);
                Grid.SetRow(option.Label, rowindex);
                Grid.SetRow(option.Control, rowindex);

                this._columngrid.Children.Add(option.Label);
                this._columngrid.Children.Add(option.Control);

                //Debug.WriteLine("Control width (" + option.Label.Content + "): " + width);
                if (width < option.Control.Width)
                {                   
                    width = option.Control.Width;
                }

                rowindex++;
            }
        }

        private void HideUnhide(bool Hidden)
        {
            this._hidden = Hidden;
            if (Hidden == true)
            {
                this._columngrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                this._columngrid.Visibility = Visibility.Visible;
            }
            this.ParentChanged?.Invoke(this, this.IsEnabled, this.IsHidden);
        }

        private void EnableDisable(bool Enabled)
        {
            this._enabled = Enabled;
            this._columngrid.IsEnabled = Enabled;
            this.ParentChanged?.Invoke(this, this.IsEnabled, this.IsHidden);
        }
    }
}
