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

// NoUIContainer.cs - container for BlindOptions

using System.Collections.Generic;
using System.Xml.Linq;
using TsGui.Grouping;

namespace TsGui.Blind
{
    public class NoUIContainer: GroupableBlindBase
    {
        private List<IOption> _options = new List<IOption>();
        private List<NoUIContainer> _containers = new List<NoUIContainer>();
        
        //constructors
        public NoUIContainer(MainController MainController,XElement InputXml) : base(MainController)
        {
            this.LoadXml(InputXml);
        }
        public NoUIContainer(NoUIContainer Parent, MainController MainController, XElement InputXml) : base(Parent, MainController)
        {
            this.LoadXml(InputXml);
        }

        //methods
        private new void LoadXml(XElement InputXml)
        {
            base.LoadXml(InputXml);
            foreach (XElement opx in InputXml.Elements())
            {
                if (opx.Name == "BlindOption")
                {
                    this.AddOption(new BlindOption(this, this._controller, opx));
                }

                else if (opx.Name == "Container")
                {
                    this._containers.Add(new NoUIContainer(this,this._controller, opx));
                }
            }
        }

        private void AddOption (IOption Option)
        {
            this._options.Add(Option);
            this._controller.AddOptionToLibary(Option);
        }
    }
}
