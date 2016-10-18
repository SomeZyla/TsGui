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

// IGuiOption_2.cs - controls to apply to the gui. 

using System.Windows.Controls;
using System.Windows;

namespace TsGui.View.GuiOptions
{
    public interface IGuiOption_2: IGroupChild
    {
        TsVariable Variable { get; }
        string VariableName { get; }
        string InactiveValue { get; }
        UserControl Control { get; }
        bool PurgeInactive { get; set; }
    }
}