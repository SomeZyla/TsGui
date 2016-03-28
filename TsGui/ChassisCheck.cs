﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Management;

namespace TsGui.GuiOptions
{
    internal class ChassisCheck
    {
        private bool _islaptop = false;
        private bool _isdesktop = false;
        private bool _isserver = false;

        public ChassisCheck()
        {

        }

        private void Evaluate()
        { 
            foreach (ManagementObject m in SystemConnector.GetWmiManagementObjects("select ChassisTypes from Win32_SystemEnclosure"))
            {
                foreach (PropertyData propdata in m.Properties)
                {
                    string s = (string)propdata.Value;


                    switch (s)
                    {
                        case "8":
                        case "9":
                        case "10":
                        case "11":
                        case "12":
                        case "14":
                        case "18":
                        case "21":
                            {
                                this._islaptop = true;
                                break;
                            }
                        default:
                            {
                                break;
                                //nothing
                            }
                    }
                }
            }
        }

  //      Set objResults = objWMI.InstancesOf("Win32_SystemEnclosure")

  //      bIsLaptop = false
		//bIsDesktop = false
		//bIsServer = false
		//For each objInstance in objResults

  //          If objInstance.ChassisTypes(0) = 12 or objInstance.ChassisTypes(0) = 21 then
		//		' Ignore docking stations
		//	Else

  //              If not IsNull(objInstance.SMBIOSAssetTag) then
  //                  sAssetTag = Trim(objInstance.SMBIOSAssetTag)

  //              End if
		//		Select Case objInstance.ChassisTypes(0)
		//		Case "8", "9", "10", "11", "12", "14", "18", "21"
		//			bIsLaptop = true
		//		Case "3", "4", "5", "6", "7", "15", "16"
		//			bIsDesktop = true
		//		Case "23"
		//			bIsServer = true
		//		Case Else
		//			' Do nothing

  //              End Select


  //          End if

		//Next
    }
}
