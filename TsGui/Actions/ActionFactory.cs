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
using System;
using System.Xml.Linq;
using TsGui.Diagnostics.Logging;

namespace TsGui.Actions
{
    public static class ActionFactory
    {
        public static IAction CreateAction(XElement inputxml)
        {
            XAttribute xtype = inputxml.Attribute("Type");
            if (xtype == null) { throw new ArgumentException("Missing Type attribute on Action" + Environment.NewLine); }

            LoggerFacade.Info("Creating Action, type: " + xtype.Value);

            #region
            if (xtype.Value == "Authentication")
            {
                var action = new AuthenticationAction(inputxml);
                return action;
            }

            else
            { return null; }
            #endregion
        }
    }
}
