﻿using System.Windows;
using System.Windows.Controls;
using System;
using System.Windows.Threading;

namespace TsGui.View.Layout
{
    /// <summary>
    /// Interaction logic for PageLayout.xaml
    /// </summary>
    public partial class TsPageUI : Page
    {
        private TsPage _page;

        public TsPageUI(TsPage Page)
        {
            InitializeComponent();
            this._page = Page;
        }

        public void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this._page.Cancel();
        }

        public void buttonPrev_Click(object sender, RoutedEventArgs e)
        {
            this._page.MovePrevious();
        }

        public void buttonNext_Click(object sender, RoutedEventArgs e)
        {
            this._page.MoveNext();
        }

        public void buttonFinish_Click(object sender, RoutedEventArgs e)
        {
            this._page.Finish();
        }
    }
}