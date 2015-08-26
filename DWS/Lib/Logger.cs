using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Windows.Forms;

namespace DWS_Lite.Lib
{
    class Logger
    {
        private DestroyWindowsSpyingMainForm destroyWindowsSpyingMainForm;
        private ResourceManager rm;
        private string logfilename = "DWS.log";

        public Logger(DestroyWindowsSpyingMainForm destroyWindowsSpyingMainForm)
        {
            this.destroyWindowsSpyingMainForm = destroyWindowsSpyingMainForm;
            this.rm = destroyWindowsSpyingMainForm.rm;
        }

        public void output(string str, bool split = false)
        {
            try
            {
                this.destroyWindowsSpyingMainForm.Invoke(new MethodInvoker(delegate
                {
                    outputnoinvoke(str, split);
                }));
            }
            catch (Exception)
            {
                try
                {
                    outputnoinvoke(str, split);
                }
                catch (Exception)
                {
                   // fatalerrors++;
                }
            }
        }

        private void outputnoinvoke(string str, bool split = false)
        {
            DateTime temp = DateTime.Now;
            str = "[" + temp.Hour.ToString() + ":" + temp.Minute.ToString() + ":" + temp.Second.ToString() + "] " + str;
            File.WriteAllText(logfilename, File.ReadAllText(logfilename) + str + "\n");
            Console.WriteLine(str);
            this.destroyWindowsSpyingMainForm.appendLogOutputTextBox( str );
            if (split)
            {

                File.WriteAllText(logfilename, File.ReadAllText(logfilename) + "==========================\n");
                Console.WriteLine("==========================");
                this.destroyWindowsSpyingMainForm.appendLogOutputTextBox("==========================\n");
            }
        }

       
        //TODO: refactor
        public string GetTranslateText(string name)
        {
            try
            {
                return rm.GetString(name);
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
