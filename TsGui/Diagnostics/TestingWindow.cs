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

// TestingWindow.cs - view model for the TestingWindowUI

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Threading;

using TsGui.View;
using TsGui.View.Layout;
using TsGui.View.Helpers;
using TsGui.Helpers;
using TsGui.Diagnostics.Logging;
using TsGui.Options;


namespace TsGui.Diagnostics
{
    public class TestingWindow: ViewModelBase
    {
        private int _pendingresize = 0;
        private IDirector _controller;
        private TestingWindowUI _testingwindowui;
        private ObservableCollection<IOption> _options;
        private int _currentscaling;
        private string _logs;
        private bool _pendinglogrefresh;

        public ObservableCollection<IOption> Options { get { return this._options; } }
        public TsMainWindow TsMainWindow { get; set; }
        public double ScreenHeight { get; set; }
        public double ScreenWidth { get; set; }
        public double WindowMaxHeight { get { return SystemParameters.PrimaryScreenHeight - 20; } }
        public double OptionsGridMaxHeight
        {
            get {
                return this.WindowMaxHeight - this._testingwindowui._positiongrid.ActualHeight - 
                    this._testingwindowui._screengrid.ActualHeight - this._testingwindowui._logtextbox.ActualHeight - 
                    this._testingwindowui._logtitle.ActualHeight - 140; }
        }
        public int CurrentScaling
        {
            get { return this._currentscaling; }
            set { this._currentscaling = value; this.OnPropertyChanged(this, "CurrentScaling"); }
        }
        public ImageSource Icon { get; set; }
        public string Logs
        {
            get { return this._logs; }
            set { this._logs = value; this.OnPropertyChanged(this, "Logs"); }
        }

        public TestingWindow(IDirector Controller)
        {
            this._controller = Controller;
            this.SubscribeToLogs();             
            this.CreateTestingWindowUI();

            this.ScreenWidth = SystemParameters.PrimaryScreenWidth;
            this.ScreenHeight = SystemParameters.PrimaryScreenHeight;
            this.Icon = IconHelper.ConvertToImageSource(SystemIcons.Information);
            this._options = this._controller.OptionLibrary.Options;
            this.TsMainWindow = this._controller.TsMainWindow;
            this._testingwindowui._logclearbtn.Click += this.OnLogClearClick;
            this._controller.ParentWindow.Loaded += this.OnParentWindowLoaded;
            this._controller.ParentWindow.Closed += this.OnParentWindowClosed;
            this._testingwindowui.Show();
        }

        public void OnWindowClosed(object o, EventArgs e)
        {
            this.UnsubscribeFromLogs();
        }

        public void OnParentWindowLoaded(object o, EventArgs e)
        {
            this.CurrentScaling = DisplayInformation.GetScaling();
        }

        public void OnParentWindowClosed(object o, EventArgs e)
        {
            this._testingwindowui.Close();
        }

        public void OnNewLogMessage(LoggingReceiverNLog sender, EventArgs e)
        {
            this._logs = this._logs + sender.LastMessage + Environment.NewLine;

            if (this._pendinglogrefresh == false)
            {
                this._pendinglogrefresh = true;
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() => this.RefreshLogView()));
            }
        }

        public void RefreshLogView()
        {
            if (this._pendinglogrefresh == true)
            {
                this.OnPropertyChanged(this, "Logs");
                this._pendinglogrefresh = false;
                this._testingwindowui._logtextbox.ScrollToEnd();
            }
            
        }

        public void OnLogClearClick(object sender, RoutedEventArgs e)
        { this.Logs = string.Empty; }

        public void OnTestingWindowRendered(object sender, EventArgs e)
        {
            this.ResizeGrids();
            this._testingwindowui._optionsgrid.EnableRowVirtualization = false;
            this._testingwindowui._valuecolumn.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            this._testingwindowui.SizeChanged += this.OnTestingWindowSizeChanged;
        }

        public void OnTestingWindowSizeChanged(object o, EventArgs e)
        {
            if (this._pendingresize < 2)
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() => this.ResizeGrids()));
                this._pendingresize++;
            } 
        }

        private void ResizeGrids()
        {
            this._testingwindowui._optionswrappergrid.Width = this._testingwindowui._parentgrid.ActualWidth;
            this._testingwindowui._optionsgrid.Width = this._testingwindowui._optionswrappergrid.ActualWidth;
            this._pendingresize--;
        }

        private void SubscribeToLogs()
        {
            foreach (ILoggingReceiver receiver in LoggingFrameworkHelpers.GetLoggingReceivers())
            { receiver.NewLogMessage += this.OnNewLogMessage; }
        }

        private void UnsubscribeFromLogs()
        {
            foreach (ILoggingReceiver receiver in LoggingFrameworkHelpers.GetLoggingReceivers())
            { receiver.NewLogMessage -= this.OnNewLogMessage; }
        }

        private void CreateTestingWindowUI()
        {
            this._testingwindowui = new TestingWindowUI();
            this._testingwindowui.DataContext = this;
            this._testingwindowui.Closed += this.OnWindowClosed;
            this._testingwindowui.ContentRendered += this.OnTestingWindowRendered;
        }
    }
}
