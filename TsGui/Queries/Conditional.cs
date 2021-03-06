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

// Conditional.cs - IF -> then processing

using System.Xml.Linq;
using TsGui.Linking;
using TsGui.Validation.StringMatching;

namespace TsGui.Queries
{
    public class Conditional
    {
        private QueryPriorityList _sourcequerylist;
        private QueryPriorityList _resultquerylist;
        private RuleSet _ruleset ;
        private ILinkTarget _linktargetoption;

        public Conditional(XElement inputxml, ILinkTarget targetoption)
        {
            this._linktargetoption = targetoption;
            this._ruleset = new RuleSet(targetoption);
            this._sourcequerylist = new QueryPriorityList(this._linktargetoption);
            this._resultquerylist = new QueryPriorityList(this._linktargetoption);
            this.LoadXml(inputxml);
        }

        #region
        //<IF>
        //  <Source>
        //      <Query/>
        //  </Source>
        //  <Ruleset>
        //      <Rule Type="StartsWith">test</Rule>
        //  </Ruleset>
        //  <Result>
        //      <Query/>
        //  </Result>
        //</IF>
        #endregion
        public void LoadXml(XElement InputXml)
        {
            XElement x;
            x = InputXml.Element("Source");
            if (x != null) { this._sourcequerylist.LoadXml(x); }

            x = InputXml.Element("Ruleset");
            if (x != null) { this._ruleset.LoadXml(x); }

            x = InputXml.Element("Result");
            if (x != null) { this._resultquerylist.LoadXml(x); }
        }

        public ResultWrangler GetResultWrangler()
        {
            return this.ProcessQuery();
        }

        public ResultWrangler ProcessQuery()
        {
            string sourcevalue = this._sourcequerylist.GetResultWrangler()?.GetString();

            if (this._ruleset.DoesMatch(sourcevalue) == true)
            { return this._resultquerylist.GetResultWrangler(); }
            else { return null; }
        }
    }
}
