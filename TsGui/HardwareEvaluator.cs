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

// HardwareEvaluator.cs - class to identify hardware type
// ChassisTypes - https://technet.microsoft.com/en-us/library/ee156537.aspx

using System;
using System.Collections.Generic;
using System.Management;

using TsGui.Connectors;

namespace TsGui
{
    public class HardwareEvaluator
    {
        private bool _islaptop = false;
        private bool _isdesktop = false;
        private bool _isserver = false;
        private bool _isvm = false;
        private string _ipaddresses4 = string.Empty;
        private string _ipaddresses6 = string.Empty;
        private string _defaultgateways4 = string.Empty;
        private string _defaultgateways6 = string.Empty;
        private string _dhcpserver = string.Empty;

        public bool IsLaptop { get { return this._islaptop; } }
        public bool IsDesktop { get { return this._isdesktop; } }
        public bool IsServer { get { return this._isserver; } }
        public bool IsVirtualMachine { get { return this._isvm; } }
        public string IPv4Addresses4 { get { return this._ipaddresses4; } }
        public string IPv4Addresses6 { get { return this._ipaddresses6; } }
        public string DefaultGateways4 { get { return this._defaultgateways4; } }
        public string DefaultGateways6 { get { return this._defaultgateways6; } }
        public string DHCPServer { get { return this._dhcpserver; } }

        public HardwareEvaluator()
        {
            this.Evaluate();
        }

        public List<TsVariable> GetTsVariables()
        {
            List<TsVariable> vars = new List<TsVariable>();

            if (_islaptop) { vars.Add(new TsVariable("TsGui_IsLaptop", "TRUE")); }
            else { vars.Add(new TsVariable("TsGui_IsLaptop", "FALSE")); }

            if (_isdesktop) { vars.Add(new TsVariable("TsGui_IsDesktop", "TRUE")); }
            else { vars.Add(new TsVariable("TsGui_IsDesktop", "FALSE")); }

            if (_isserver) { vars.Add(new TsVariable("TsGui_IsServer", "TRUE")); }
            else { vars.Add(new TsVariable("TsGui_IsServer", "FALSE")); }

            if (_isvm) { vars.Add(new TsVariable("TsGui_IsVirtualMachine", "TRUE")); }
            else { vars.Add(new TsVariable("TsGui_IsVirtualMachine", "FALSE")); }

            vars.Add(new TsVariable("TsGui_IPv4", this._ipaddresses4));
            vars.Add(new TsVariable("TsGui_IPv6", this._ipaddresses6));
            vars.Add(new TsVariable("TsGui_DefaultGateway4", this._defaultgateways4));
            vars.Add(new TsVariable("TsGui_DefaultGateway6", this._defaultgateways6));
            vars.Add(new TsVariable("TsGui_DHCPServer", this._dhcpserver));

            return vars;
        }

        private void Evaluate()
        {
            //virtual machine tests
            foreach (ManagementObject m in SystemConnector.GetWmiManagementObjectCollection("select Model,Manufacturer from Win32_ComputerSystem"))
            {
                string model = (string)m["Model"];
                string maker = (string)m["Manufacturer"];
                //vmware
                if (model.Contains("VMware"))
                {
                    this._isvm = true;
                    break;
                }
                //hyper-v
                if (model == "Virtual Machine")
                {
                    this._isvm = true;
                    break;
                }
                //virtualbox
                if (model.Contains("VirtualBox"))
                {
                    this._isvm = true;
                    break;
                }
                //Xen
                if (maker.Contains("Xen"))
                {
                    this._isvm = true;
                    break;
                }
                //Parallels
                if (model.Contains("Parallels"))
                {
                    this._isvm = true;
                    break;
                }
            }

            //chassis type tests
            foreach (ManagementObject m in SystemConnector.GetWmiManagementObjectCollection("select ChassisTypes from Win32_SystemEnclosure"))
            {
                Int16[] chassistypes = (Int16[])m["ChassisTypes"];

                foreach (Int16 i in chassistypes)
                {
                    switch (i)
                    {
                        case 8: case 9: case 10: case 11:  case 12: case 14: case 18: case 21:
                            {
                                this._islaptop = true;
                                break;
                            }
                        case 3: case 4: case 5:  case 6: case 7: case 15: case 16:
                            {
                                this._isdesktop = true;
                                break;
                            }
                        case 23:
                            {
                                this._isserver = true;
                                break;
                            }
                        default:
                            { break; }
                    }
                }
            }

            //ip info gather
            foreach (ManagementObject m in SystemConnector.GetWmiManagementObjectCollection(@"Select DefaultIPGateway,IPAddress,DHCPServer from Win32_NetworkAdapterConfiguration WHERE IPEnabled = 'True'"))
            {
                string[] ipaddresses = (string[])m["IPAddress"];

                if (ipaddresses != null)
                {
                    foreach (string s in ipaddresses)
                    {
                        if (string.IsNullOrEmpty(s) == false)
                        {
                            if (s.Contains(":")) { this._ipaddresses6 = AppendToStringList(this._ipaddresses6, s); }
                            else { this._ipaddresses4 = AppendToStringList(this._ipaddresses4, s); }
                        }
                    }
                }
                    

                string[] defaultgateways = (string[])m["DefaultIPGateway"];
                if (defaultgateways != null)
                {
                    foreach (string s in defaultgateways)
                    {
                        if (s.Contains(":")) { this._defaultgateways6 = AppendToStringList(this._defaultgateways6, s); }
                        else { this._defaultgateways4 = AppendToStringList(this._defaultgateways4, s); }
                    }
                }

                string svr = (string)m["DHCPServer"];
                if (svr != null) { this._dhcpserver = svr; }
            }
        }

        private static string AppendToStringList(string basestring, string newstring)
        {
            string s;
            if (string.IsNullOrWhiteSpace(basestring)) { s = newstring; }
            else { s = basestring + ", " + newstring; }
            return s;
        }
    }
}