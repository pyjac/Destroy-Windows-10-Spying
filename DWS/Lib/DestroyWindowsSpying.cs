using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DWS_Lite.Lib
{
    class DestroyWindowsSpying
    {
        private Logger logger;

        public DestroyWindowsSpying(Logger logger)
        {
            this.logger = logger;
        }

        internal void DeleteOneDrive()
        {
            logger.output(
                       ProcessUtil.RunCmd("/c taskkill /f /im OneDrive.exe > NUL 2>&1"));
            logger.output(
                ProcessUtil.RunCmd("/c ping 127.0.0.1 -n 5 > NUL 2>&1"));

            if (File.Exists(Paths.SysDir + @"Windows\System32\OneDriveSetup.exe"))
            {

                logger.output(
                    ProcessUtil.StartProcess(Paths.SysDir + @"Windows\System32\OneDriveSetup.exe", "/uninstall"));
            }
            if (File.Exists(Paths.SysDir + @"Windows\SysWOW64\OneDriveSetup.exe"))
            {
                logger.output(
                    ProcessUtil.StartProcess(Paths.SysDir + @"Windows\SysWOW64\OneDriveSetup.exe", "/uninstall"));
            }

            logger.output(
                ProcessUtil.RunCmd("/c ping 127.0.0.1 -n 5 > NUL 2>&1"));
            logger.output(
                ProcessUtil.RunCmd("/c rd \"%USERPROFILE%\\OneDrive\" /Q /S > NUL 2>&1"));
            logger.output(
                ProcessUtil.RunCmd("/c rd \"C:\\OneDriveTemp\" /Q /S > NUL 2>&1"));
            logger.output(
                ProcessUtil.RunCmd("/c rd \"%LOCALAPPDATA%\\Microsoft\\OneDrive\" /Q /S > NUL 2>&1"));
            logger.output(
                ProcessUtil.RunCmd("/c rd \"%PROGRAMDATA%\\Microsoft OneDrive\" /Q /S > NUL 2>&1"));
            logger.output(
                 ProcessUtil.RunCmd(
                    "/c REG DELETE \"HKEY_CLASSES_ROOT\\CLSID\\{018D5C66-4533-4307-9B53-224DE2ED1FE6}\" /f > NUL 2>&1"));
            logger.output(
                 ProcessUtil.RunCmd(
                    "/c REG DELETE \"HKEY_CLASSES_ROOT\\Wow6432Node\\CLSID\\{018D5C66-4533-4307-9B53-224DE2ED1FE6}\" /f > NUL 2>&1"));


        }


        internal void disableSpyTasks()
        {

            string[] disabletaskslist =
                {
                    @"Microsoft\Office\Office ClickToRun Service Monitor",
                    @"Microsoft\Office\OfficeTelemetryAgentFallBack2016",
                    @"Microsoft\Office\OfficeTelemetryAgentLogOn2016",
                    @"Microsoft\Windows\Customer Experience Improvement Program\KernelCeipTask",
                    @"Microsoft\Windows\Customer Experience Improvement Program\UsbCeip",
                    @"Microsoft\Windows\Power Efficiency Diagnostics\AnalyzeSystem",
                    @"Microsoft\Windows\Shell\FamilySafetyMonitor",
                    @"Microsoft\Windows\Shell\FamilySafetyRefresh",
                    @"Microsoft\Windows\Application Experience\AitAgent",
                    @"Microsoft\Windows\Application Experience\ProgramDataUpdater",
                    @"Microsoft\Windows\Application Experience\StartupAppTask",
                    @"Microsoft\Windows\Autochk\Proxy",
                    @"Microsoft\Windows\Customer Experience Improvement Program\BthSQM",
                    @"Microsoft\Windows\Customer Experience Improvement Program\Consolidator",
                    @"Microsoft\Office\OfficeTelemetry\AgentFallBack2016",
                    @"Microsoft\Office\OfficeTelemetry\OfficeTelemetryAgentLogOn2016",
                    @"Microsoft\Windows\Application Experience\Microsoft Compatibility Appraiser",
                    @"Microsoft\Windows\DiskDiagnostic\Microsoft-Windows-DiskDiagnosticDataCollector",
                    @"Microsoft\Windows\Maintenance\WinSAT",
                    @"Microsoft\Windows\Media Center\ActivateWindowsSearch",
                    @"Microsoft\Windows\Media Center\ConfigureInternetTimeService",
                    @"Microsoft\Windows\Media Center\DispatchRecoveryTasks",
                    @"Microsoft\Windows\Media Center\ehDRMInit",
                    @"Microsoft\Windows\Media Center\InstallPlayReady",
                    @"Microsoft\Windows\Media Center\mcupdate",
                    @"Microsoft\Windows\Media Center\MediaCenterRecoveryTask",
                    @"Microsoft\Windows\Media Center\ObjectStoreRecoveryTask",
                    @"Microsoft\Windows\Media Center\OCURActivate",
                    @"Microsoft\Windows\Media Center\OCURDiscovery",
                    @"Microsoft\Windows\Media Center\PBDADiscovery",
                    @"Microsoft\Windows\Media Center\PBDADiscoveryW1",
                    @"Microsoft\Windows\Media Center\PBDADiscoveryW2",
                    @"Microsoft\Windows\Media Center\PvrRecoveryTask",
                    @"Microsoft\Windows\Media Center\PvrScheduleTask",
                    @"Microsoft\Windows\Media Center\RegisterSearch",
                    @"Microsoft\Windows\Media Center\ReindexSearchRoot",
                    @"Microsoft\Windows\Media Center\SqlLiteRecoveryTask",
                    @"Microsoft\Windows\Media Center\UpdateRecordPath"
                };
            for (int i = 0; i < disabletaskslist.Length; i++)
            {
                ProcessUtil.StartProcess("SCHTASKS", "/Change /TN \"" + disabletaskslist[i] + "\" /disable");
                logger.output("Disabled task: " + disabletaskslist[i]);
            }
        }

        internal void DeleteWindows10MetroApp(string appname)
        {
            ProcessUtil.RunPowerShell("-command \"Get-AppxPackage *" + appname + "* | Remove-AppxPackage\"");
        }
        internal void disablehostsandaddfirewall()
        {
            try
            {
                string hostslocation = Paths.system32location + @"drivers\etc\hosts";
                string hosts = null;
                if (File.Exists(hostslocation))
                {
                    hosts = File.ReadAllText(hostslocation);
                    File.SetAttributes(hostslocation, FileAttributes.Normal);
                    FileUtil.DeleteFile(hostslocation);
                }
                File.Create(hostslocation).Close();
                File.WriteAllText(hostslocation, hosts + "\r\n");
                for (int i = 0; i < HostsDomains.hostsdomains.Length; i++)
                {
                    if (hosts.IndexOf(HostsDomains.hostsdomains[i]) == -1)
                    {
                        ProcessUtil.RunCmd(
                            "/c echo " + "0.0.0.0 " + HostsDomains.hostsdomains[i] + " >> \"" + hostslocation +
                            "\"");
                        logger.output("Add to hosts - " + HostsDomains.hostsdomains[i]);
                    }
                }
            }
            catch (Exception)
            {
                // fatalerrors++;
                logger.output("Error add HOSTS");
            }
            ProcessUtil.RunCmd("/c ipconfig /flushdns");

            logger.output("Add hosts MS complete.");
            ProcessUtil.RunCmd("/c netsh advfirewall firewall delete rule name=\"MS Spynet block\"");
            ProcessUtil.RunCmd("/c netsh advfirewall firewall add rule name=\"MS Spynet block\" dir=out interface=any action=block remoteip=23.96.0.0/13");
            logger.output("Add Windows Firewall rule: \"MS Spynet block\"");
            ProcessUtil.RunCmd("/c route -p add 23.218.212.69 MASK 255.255.255.255 0.0.0.0");
            ProcessUtil.RunCmd("/c route -p add 65.55.108.23 MASK 255.255.255.255 0.0.0.0");
            ProcessUtil.RunCmd("/c route -p add 65.39.117.230 MASK 255.255.255.255 0.0.0.0");
            ProcessUtil.RunCmd("/c route -p add 134.170.30.202 MASK 255.255.255.255 0.0.0.0");
            ProcessUtil.RunCmd("/c route -p add 137.116.81.24 MASK 255.255.255.255 0.0.0.0");
            ProcessUtil.RunCmd("/c route -p add 204.79.197.200 MASK 255.255.255.255 0.0.0.0");
            ProcessUtil.RunCmd("/c route -p add 23.218.212.69 MASK 255.255.255.255 0.0.0.0");

        }

        internal void disableTelemetryAndKeylogger()
        {
            // DISABLE TELEMETRY
            logger.output("Disable telemetry...");
            ProcessUtil.RunCmd("/c net stop DiagTrack ");
            ProcessUtil.RunCmd("/c net stop diagnosticshub.standardcollector.service ");
            ProcessUtil.RunCmd("/c net stop dmwappushservice ");
            ProcessUtil.RunCmd("/c net stop WMPNetworkSvc ");
            ProcessUtil.RunCmd("/c sc config DiagTrack start=disabled ");
            ProcessUtil.RunCmd("/c sc config diagnosticshub.standardcollector.service start=disabled ");
            ProcessUtil.RunCmd("/c sc config dmwappushservice start=disabled ");
            ProcessUtil.RunCmd("/c sc config WMPNetworkSvc start=disabled ");
            ProcessUtil.RunCmd("/c REG ADD HKLM\\SYSTEM\\ControlSet001\\Control\\WMI\\AutoLogger\\AutoLogger-Diagtrack-Listener /v Start /t REG_DWORD /d 0 /f");
            ProcessUtil.RunCmd("/c net stop dmwappushservice");
            ProcessUtil.RunCmd("/c net stop diagtrack");
            ProcessUtil.RunCmd("/c sc delete dmwappushsvc");
            ProcessUtil.RunCmd("/c sc delete \"Diagnostics Tracking Service\"");
            ProcessUtil.RunCmd("/c sc delete diagtrack");
            ProcessUtil.RunCmd("/c reg add \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Device Metadata\" /v \"PreventDeviceMetadataFromNetwork\" /t REG_DWORD /d 1 /f ");
            ProcessUtil.RunCmd("/c reg add \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection\" /v \"AllowTelemetry\" /t REG_DWORD /d 0 /f ");
            ProcessUtil.RunCmd("/c reg add \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Policies\\Microsoft\\MRT\" /v \"DontOfferThroughWUAU\" /t REG_DWORD /d 1 /f ");
            ProcessUtil.RunCmd("/c reg add \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Policies\\Microsoft\\SQMClient\\Windows\" /v \"CEIPEnable\" /t REG_DWORD /d 0 /f ");
            ProcessUtil.RunCmd("/c reg add \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Policies\\Microsoft\\Windows\\AppCompat\" /v \"AITEnable\" /t REG_DWORD /d 0 /f ");
            ProcessUtil.RunCmd("/c reg add \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Policies\\Microsoft\\Windows\\AppCompat\" /v \"DisableUAR\" /t REG_DWORD /d 1 /f ");
            ProcessUtil.RunCmd("/c reg add \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Policies\\Microsoft\\Windows\\DataCollection\" /v \"AllowTelemetry\" /t REG_DWORD /d 0 /f ");
            ProcessUtil.RunCmd("/c reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\WMI\\AutoLogger\\AutoLogger-Diagtrack-Listener\" /v \"Start\" /t REG_DWORD /d 0 /f ");
            ProcessUtil.RunCmd("/c reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\WMI\\AutoLogger\\SQMLogger\" /v \"Start\" /t REG_DWORD /d 0 /f ");
            ProcessUtil.RunCmd("/c reg add \"HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Siuf\\Rules\" /v \"NumberOfSIUFInPeriod\" /t REG_DWORD /d 0 /f ");
            ProcessUtil.RunCmd("/c reg add \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Policies\\Microsoft\\Windows\\AppCompat\" /v \"DisableUAR\" /t REG_DWORD /d 1 /f ");
            ProcessUtil.RunCmd("/c reg add \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Policies\\Microsoft\\SQMClient\\Windows\" /v \"CEIPEnable\" /t REG_DWORD /d 0 /f ");
            ProcessUtil.RunCmd("/c reg delete \"HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Siuf\\Rules\" /v \"PeriodInNanoSeconds\" /f ");
            // DELETE KEYLOGGER
            logger.output("Delete keylogger...");
        }

        //TODO: Refactor, Move to a More be-fitting Class 
        internal void SetRegValueHKCU(string regkeyfolder, string paramname, string paramvalue,
            Microsoft.Win32.RegistryValueKind keytype)
        {
            try
            {
                RegistryKey myKey = Registry.CurrentUser.OpenSubKey(regkeyfolder, true);
                if (myKey != null)
                {
                    myKey.SetValue(paramname, paramvalue, keytype);
                    myKey.Close();
                }
            }
            catch (Exception ex)
            {
                //fatalerrors++;
                logger.output(logger.GetTranslateText("Error") + ": " + ex.Message);
            }
        }
        //TODO: Refactor, Move to a More be-fitting Class 
        internal void SetRegValueHKLM(string regkeyfolder, string paramname, string paramvalue,
            Microsoft.Win32.RegistryValueKind keytype)
        {
            try
            {
                RegistryKey myKey = Registry.LocalMachine.OpenSubKey(regkeyfolder, true);
                if (myKey != null)
                {
                    myKey.SetValue(paramname, paramvalue, keytype);
                    myKey.Close();
                }
            }
            catch (Exception ex)
            {
               // fatalerrors++;
                logger.output(logger.GetTranslateText("Error") + ": " + ex.Message);
            }
        }
    }
}
