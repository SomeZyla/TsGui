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

// Delegates.cs - Delegates for control parts of the app

using System;
using System.Windows;
using TsGui.View.Layout;

namespace TsGui.Events
{
    public delegate void TsGuiWindowEventHandler(object sender, RoutedEventArgs e);
    public delegate void TsGuiWindowMovingEventHandler(object sender, EventArgs e);
    public delegate void ConfigLoadFinishedEventHandler(object sender, EventArgs e);
    public delegate void ComplianceRetryEventHandler(IComplianceRoot sender, EventArgs e);
}
