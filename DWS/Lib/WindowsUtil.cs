using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Windows.Forms;

namespace DWS_Lite.Lib
{
    class WindowsUtil
    {
        public static object readSubKeyValue(string subKey, string keyName)
        {
            object result;
            RegistryKey rkSubKey = Registry.LocalMachine.OpenSubKey(subKey);
            if (rkSubKey == null)
            {
                MessageBox.Show("Error while reading registry key: " + subKey + "\\" + keyName + " does not exist!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            try
            {
                result = rkSubKey.GetValue(keyName);
                rkSubKey.Close();
                return result;

            }
            catch (Exception ex)   //This exception is thrown
            {
                MessageBox.Show("Error while reading registry key: " + subKey + "\\" + keyName + ". ErrorMessage: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        public static int getWindowsBuildNumber()
        {
            return Convert.ToInt32(readSubKeyValue(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\", "CurrentBuildNumber"));
        }
        public static bool isLUAEnabled()
        {
            return !Convert.ToBoolean(readSubKeyValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System\", "EnableLUA"));
        }

        public static int isSystemRestoreEnabled()
        {
            return Convert.ToInt32(readSubKeyValue(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore\", "RPSessionInterval"));
        }

        public static string getSystemProductName()
        {
            return Convert.ToString(readSubKeyValue(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\", "ProductName"));
        }

        public static string getSystemBuild()
        {
            return Convert.ToString(readSubKeyValue(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\", "BuildLabEx"));
        }
        private string getwindowsbuildorversion()
        {
           
                // в value массив из байт
                string value = "Product Name: " + getSystemProductName() + "\n";
                value += "  Build: " + getSystemBuild();
                return value;
        }


        public static void CreateRestorePoint(string description)
        {
            ManagementScope oScope = new ManagementScope("\\\\localhost\\root\\default");
            ManagementPath oPath = new ManagementPath("SystemRestore");
            ObjectGetOptions oGetOp = new ObjectGetOptions();
            ManagementClass oProcess = new ManagementClass(oScope, oPath, oGetOp);

            ManagementBaseObject oInParams =
                oProcess.GetMethodParameters("CreateRestorePoint");
            oInParams["Description"] = description;
            oInParams["RestorePointType"] = 12; // MODIFY_SETTINGS
            oInParams["EventType"] = 100;

            ManagementBaseObject oOutParams =
                oProcess.InvokeMethod("CreateRestorePoint", oInParams, null);
        }

    }
}
