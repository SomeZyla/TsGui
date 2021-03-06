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

// TsPageHeader.cs - view model class for the header on a page

using System.Xml.Linq;
using System.Windows.Media;
using System.Windows;

using TsGui.Events;
using TsGui.Images;
using TsGui.Validation;

namespace TsGui.View.Layout
{
    public class TsPageHeader : BaseLayoutElement, IComplianceRoot
    {
        public event ComplianceRetryEventHandler ComplianceRetry;

        private string _title;
        private string _text;
        private SolidColorBrush _bgColor;
        private SolidColorBrush _fontColor;
        private double _titlefontsize;
        private double _textfontsize;

        //Properties
        public Image Image { get; set; }
        public TsTable Table { get; set; }
        public TsPageHeaderUI UI { get; set; }
        public SolidColorBrush BgColor
        {
            get { return this._bgColor; }
            set
            {
                this._bgColor = value;
                this.OnPropertyChanged(this, "BgColor");
            }
        }
        public string Title
        {
            get { return this._title; }
            set
            {
                this._title = value;
                this.OnPropertyChanged(this, "HeadingTitle");
            }
        }
        public string Text
        {
            get { return this._text; }
            set
            {
                this._text = value;
                this.OnPropertyChanged(this, "HeadingText");
            }
        }

        public SolidColorBrush FontColor
        {
            get { return this._fontColor; }
            set
            {
                this._fontColor = value;
                this.OnPropertyChanged(this, "HeadingFontColor");
            }
        }

        public double TitleFontSize
        {
            get { return this._titlefontsize; }
            set  { this._titlefontsize = value; this.OnPropertyChanged(this, "TitleFontSize"); }
        }

        public double TextFontSize
        {
            get { return this._textfontsize; }
            set { this._textfontsize = value; this.OnPropertyChanged(this, "TextFontSize"); }
        }

        //Constructors
        public TsPageHeader(BaseLayoutElement Parent, TsPageHeader Template, XElement SourceXml):base(Parent)
        {
            this.Height = Template.Height;
            this.Title = Template.Title;
            this.Text = Template.Text;
            this.FontColor = Template.FontColor;
            this.BgColor = Template.BgColor;
            this.Image = Template.Image;
            this.TitleFontSize = Template.TitleFontSize;
            this.Margin = Template.Margin;

            this.Init(SourceXml);
        }

        public TsPageHeader(BaseLayoutElement Parent, XElement SourceXml): base (Parent)
        {
            this.ShowGridLines = Director.Instance.ShowGridLines;
            this.SetDefaults();

            this.Init(SourceXml);
        }

        public TsPageHeader() : base()
        {
            this.SetDefaults();
        }

        private void Init(XElement SourceXml)
        {
            this.UI = new TsPageHeaderUI();
            this.UI.DataContext = this;

            this.LoadXml(SourceXml);

            if (string.IsNullOrEmpty(this.Title)) { this.UI.HeaderTitle.Visibility = Visibility.Collapsed; }
            if (string.IsNullOrEmpty(this.Text)) { this.UI.HeaderText.Visibility = Visibility.Collapsed; }
            if (this.Image == null) { this.UI.ImageElement.Visibility = Visibility.Collapsed; }
        }

        public bool OptionsValid()
        {
            if (this.Table != null) { return ResultValidator.OptionsValid(this.Table.ValidationOptions); }
            else { return true; }
        }

        private void SetDefaults()
        {
            if (Director.Instance.UseTouchDefaults)
            {
                this.Margin = new Thickness(10, 10, 10, 10);
                this.Height = 65;
                this.TitleFontSize = 14;
                this.TextFontSize = 12;
            }
            else
            {
                this.Margin = new Thickness(10, 5, 10, 5);
                this.Height = 50;
                this.TitleFontSize = 13;
                this.TextFontSize = 12;
            }
            
            this.FontColor = new SolidColorBrush(Colors.White);
            this.BgColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF006AD4"));
        }

        //Methods
        public new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);

            XElement x;

            if (InputXml != null)
            {
                base.LoadXml(InputXml);

                this.FontColor = XmlHandler.GetSolidColorBrushFromXElement(InputXml, "Font-Color", this.FontColor);
                this.BgColor = XmlHandler.GetSolidColorBrushFromXElement(InputXml, "Bg-Color", this.BgColor);
                this.Title = XmlHandler.GetStringFromXElement(InputXml, "Title", this.Title);
                this.Text = XmlHandler.GetStringFromXElement(InputXml, "Text", this.Text);
                this.FontColor = XmlHandler.GetSolidColorBrushFromXElement(InputXml, "TextColor", this.FontColor);
                this.Height = XmlHandler.GetDoubleFromXElement(InputXml, "Height", this.Height);

                x = InputXml.Element("Image");
                if (x != null) { this.Image = new Image(x); }

                x = InputXml.Element("Row");
                if (x != null)
                { this.Table = new TsTable(InputXml, this); }
            }
        }

        public void RaiseComplianceRetryEvent()
        {
            this.ComplianceRetry?.Invoke(this, new RoutedEventArgs());
        }
    }
}
