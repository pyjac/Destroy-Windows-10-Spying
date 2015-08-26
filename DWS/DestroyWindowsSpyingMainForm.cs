using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using DWS_Lite.Lib;

namespace DWS_Lite
{


    public partial class DestroyWindowsSpyingMainForm : Form
    {

        internal ResourceManager rm;
        private string path = Path.GetPathRoot(Environment.SystemDirectory);
        private string ShellCmdLocation = null;
        private string system32location = null;
        private string logfilename = "DWS.log";
        private Logger logger = null;
        private DestroyWindowsSpying destroyWindowsSpying = null;
        public DestroyWindowsSpyingMainForm(string[] args)
        {
            InitializeComponent();

            logger = new Logger(this);
            destroyWindowsSpying = new DestroyWindowsSpying(logger);

            // Re create log file
            FileUtil.RecreateLogFile(logfilename);
            // Check windows version
            CheckWindowsVersion();
            //Check SYSNATIVE (x64)
            setShellSys32Paths();

            CheckEnableOrDisableUAC();

            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            this.Text += Properties.Resources.build_number;
            labelBuildDataTime.Text = "Build number:" + Properties.Resources.build_number + "  |  Build Time:" +
                                      Properties.Resources.build_datatime;

            SetLanguage(getLang(args));
            ChangeLanguage();

          

        }

        private string getLang(string[] args)
        {
            string langname = null;
            // check args lang
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].IndexOf("/lang=") > -1)
                {
                    langname = args[i].Replace("/lang=", null);
                }
            }
            return langname;
        }

        private void setShellSys32Paths()
        {
            if (File.Exists(path + @"Windows\Sysnative\cmd.exe"))
            {
                ShellCmdLocation = path + @"Windows\Sysnative\cmd.exe";
                system32location = path + @"Windows\System32\";
            }
            else
            {
                ShellCmdLocation = path + @"Windows\System32\cmd.exe";
                system32location = path + @"Windows\System32\";
            }
        }
        private void CheckWindowsVersion()
        {
            int WindowsBuildNumber = WindowsUtil.getWindowsBuildNumber();


            if (WindowsBuildNumber < 7600)
            {
                MessageBox.Show("Minimum windows version - 7", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Process.GetCurrentProcess().Kill();
            }

            // check Win 7 or 8.1
            if (WindowsBuildNumber < 10000)
            {
                tabPageUtilites.Enabled = false;
                tabPageSettings.Enabled = false;
                btnDestroyWindowsSpying.Visible = false;
                btnDestroyWindows78Spy.Visible = true;
            }
            //------------------------------------------

        }
        void SetLanguage(string currentlang = null)
        {
            if (currentlang == null)
            {
                currentlang = CultureInfo.CurrentUICulture.Name.ToLower();
            }
            if (currentlang.IndexOf("ru") > -1)
            {
                rm = lang.ru_RU.ResourceManager;
                comboBoxLanguageSelect.Text = "ru-RU | Русский";
            }
            else if (currentlang.IndexOf("fr") > -1)
            {
                rm = lang.fr_FR.ResourceManager;
                comboBoxLanguageSelect.Text = "fr-FR | French";
            }
            else if (currentlang.IndexOf("es") > -1)
            {
                rm = lang.es_ES.ResourceManager;
                comboBoxLanguageSelect.Text = "es-ES | Spanish";
            }
            else
            {
                rm = lang.en_US.ResourceManager;
                comboBoxLanguageSelect.Text = "en-US | English";
            }
        }

        void ChangeLanguage()
        {
            tabPageMain.Text = GetTranslateText("tabPageMain");
            tabPageAbout.Text = GetTranslateText("tabPageAbout");
            tabPageReadMe.Text = GetTranslateText("tabPageReadMe");
            tabPageSettings.Text = GetTranslateText("tabPageSettings");
            tabPageUtilites.Text = GetTranslateText("tabPageUtilites");
            btnDeleteAllWindows10Apps.Text = GetTranslateText("btnDeleteAllWindows10Apps");
            btnDeleteOneDrive.Text = GetTranslateText("btnDeleteOneDrive");
            btnOpenAndEditHosts.Text = GetTranslateText("btnOpenAndEditHosts");
            btnProfessionalMode.Text = GetTranslateText("btnProfessionalMode");
            btnRestoreSystem.Text = GetTranslateText("btnRestoreSystem");
            checkBoxAddToHosts.Text = GetTranslateText("checkBoxAddToHosts");
            checkBoxCreateSystemRestorePoint.Text = GetTranslateText("checkBoxCreateSystemRestorePoint");
            checkBoxDeleteWindows10Apps.Text = GetTranslateText("checkBoxDeleteWindows10Apps");
            checkBoxDisablePrivateSettings.Text = GetTranslateText("checkBoxDisablePrivateSettings");
            checkBoxDisableWindowsDefender.Text = GetTranslateText("checkBoxDisableWindowsDefender");
            checkBoxKeyLoggerAndTelemetry.Text = GetTranslateText("checkBoxKeyLoggerAndTelemetry");
            checkBoxSetDefaultPhoto.Text = GetTranslateText("checkBoxSetDefaultPhoto");
            checkBoxSPYTasks.Text = GetTranslateText("checkBoxSPYTasks");
            labelInfoDeleteMetroApps.Text = GetTranslateText("labelInfoDeleteMetroApps");
            btnEnableUac.Text = GetTranslateText("Enable") + " UAC";
            btnDisableUac.Text = GetTranslateText("Disable") + " UAC";
            btnDisableWindowsUpdate.Text = GetTranslateText("Disable") + " Windows Update";
            btnEnableWindowsUpdate.Text = GetTranslateText("Enable") + " Windows Update";
            checkBoxDeleteApp3d.Text = GetTranslateText("Delete") + " Builder 3D";
            checkBoxDeleteAppCamera.Text = GetTranslateText("Delete") + " Camera";
            checkBoxDeleteMailCalendarMaps.Text = GetTranslateText("Delete") + " Mail, Calendar, Maps";
            checkBoxDeleteAppBing.Text = GetTranslateText("Delete") + " Money, Sports, News, Weather";
            checkBoxDeleteAppZune.Text = GetTranslateText("Delete") + " Groove Music, Film TV";
            checkBoxDeleteAppPeopleOneNote.Text = GetTranslateText("Delete") + " People, OneNote";
            checkBoxDeleteAppPhone.Text = GetTranslateText("Delete") + " Phone Companion";
            checkBoxDeleteAppPhotos.Text = GetTranslateText("Delete") + " Photos";
            checkBoxDeleteAppSolit.Text = GetTranslateText("Delete") + " Solitaire Collection";
            checkBoxDeleteAppVoice.Text = GetTranslateText("Delete") + " Voice Recorder";
            checkBoxDeleteAppXBOX.Text = GetTranslateText("Delete") + " XBOX";
            btnRemoveOldFirewallRules.Text = GetTranslateText("RemoveAllOldFirewallRules");
            btnReportABug.Text = GetTranslateText("ReportABug");
            groupBoxLinks.Text = GetTranslateText("Links");
        }

        string GetTranslateText(string name)
        {
            try
            {
                return rm.GetString(name);
            }
            catch (Exception )
            {
                return null;
            }
        }

        private void CheckEnableOrDisableUAC()
        {
            //destroyWindowsSpying.SetRegValueHKLM(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "EnableLUA", "1", RegistryValueKind.DWord);

            if (WindowsUtil.isLUAEnabled())
            {
                btnEnableUac.Enabled = true;
                btnDisableUac.Enabled = false;
            }
            else
            {
                btnEnableUac.Enabled = false;
                btnDisableUac.Enabled = true;
            }

            if (WindowsUtil.isSystemRestoreEnabled() == 0)
            {
                checkBoxCreateSystemRestorePoint.Checked = false;
                checkBoxCreateSystemRestorePoint.Enabled = false;
            }

        }

        private void btnDestroyWindowsSpying_Click(object sender, EventArgs e)
        {
            StartDestroyWindowsSpying();
        }

        private void progressbaradd(int numberadd)
        {
            try
            {
                Invoke(new MethodInvoker(delegate
                {
                    try
                    {
                        ProgressBarStatus.Value += numberadd;
                    }
                    catch (Exception)
                    {

                    }
                }));
            }
            catch (Exception)
            {
                try
                {
                    ProgressBarStatus.Value += numberadd;
                }
                catch (Exception)
                {

                }
            }
        }

        public void appendLogOutputTextBox(string str)
        {
            LogOutputTextBox.Text += str + "\n";
        }

        private void EnableOrDisableTab(bool enableordisable)
        {
            try
            {
                Invoke(new MethodInvoker(delegate
                {
                    btnDestroyWindowsSpying.Enabled = enableordisable;
                    tabPageSettings.Enabled = enableordisable;
                    tabPageUtilites.Enabled = enableordisable;
                }));
            }
            catch (Exception)
            {
                tabPageMain.Enabled = enableordisable;
                tabPageSettings.Enabled = enableordisable;
                tabPageUtilites.Enabled = enableordisable;
            }
        }

        private string ProcStartargs(string name, string args)
        {
            try
            {
                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = name,
                        Arguments = args,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                        StandardOutputEncoding = Encoding.GetEncoding(866)
                    }
                };
                proc.Start();
                string line = null;
                while (!proc.StandardOutput.EndOfStream)
                {
                    line += "\n" + proc.StandardOutput.ReadLine();
                }
                return line;
            }
            catch (Exception ex)
            {
                fatalerrors++;
                return ex.ToString();
            }
        }


        private string getwindowsbuildorversion()
        {

            // в value массив из байт
            string value = "Product Name: " + WindowsUtil.getSystemProductName() + "\n";
            value += "  Build: " + WindowsUtil.getSystemBuild();
            return value;

        }
        
       
        

        private void LogOutputTextBox_TextChanged(object sender, EventArgs e)
        {
            LogOutputTextBox.SelectionStart = LogOutputTextBox.Text.Length;
            LogOutputTextBox.ScrollToCaret();
        }

        // ----- fatalerrors -----
        private int fatalerrors = 0;
        // ------ end ------

        private void StartDestroyWindowsSpying()
        {
            EnableOrDisableTab(false);
            setcompletetext(true);
            logger.output("Starting: " + DateTime.Now.ToString() + ".");
            logger.output(getwindowsbuildorversion());
            logger.output("=====================================");
            fatalerrors = 0;
            ProgressBarStatus.Value = 0;
            new Thread(() =>
            {
                DWSThread();
            }).Start();
        }

        private void DWSThread()
        {
            if (checkBoxCreateSystemRestorePoint.Checked)
            {
                try
                {
                    string restorepoint_name = "DestroyWindowsSpying " + DateTime.Now.ToString();
                    logger.output("Creating restore point " + restorepoint_name + "...");
                    WindowsUtil.CreateRestorePoint(restorepoint_name);
                    logger.output("Restore point " + restorepoint_name + " created.");
                }
                catch (Exception ex)
                {
                    logger.output("Error creating restore point. Reason: " + ex.Message);
                }
            }
            progressbaradd(10);
            if (checkBoxKeyLoggerAndTelemetry.Checked)
            {
                destroyWindowsSpying.disableTelemetryAndKeylogger(); 
            }
            progressbaradd(15); //25
            if (checkBoxAddToHosts.Checked)
            {
                destroyWindowsSpying.disablehostsandaddfirewall();
            }
            progressbaradd(20); //45
            if (checkBoxDisablePrivateSettings.Checked)
            {

                destroyWindowsSpying.SetRegValueHKCU(
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\DeviceAccess\Global\{21157C1F-2651-4CC1-90CA-1F28B02263F6}",
                    "Value", "Deny", RegistryValueKind.String);
                destroyWindowsSpying.SetRegValueHKCU(
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\DeviceAccess\Global\{2EEF81BE-33FA-4800-9670-1CD474972C3F}",
                    "Value", "Deny", RegistryValueKind.String);
                destroyWindowsSpying.SetRegValueHKCU(
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\DeviceAccess\Global\{7D7E8402-7C54-4821-A34E-AEEFD62DED93}",
                    "Value", "Deny", RegistryValueKind.String);
                destroyWindowsSpying.SetRegValueHKCU(
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\DeviceAccess\Global\{992AFA70-6F47-4148-B3E9-3003349C1548}",
                    "Value", "Deny", RegistryValueKind.String);
                destroyWindowsSpying.SetRegValueHKCU(
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\DeviceAccess\Global\{9D9E0118-1807-4F2E-96E4-2CE57142E196}",
                    "Value", "Deny", RegistryValueKind.String);
                destroyWindowsSpying.SetRegValueHKCU(
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\DeviceAccess\Global\{A8804298-2D5F-42E3-9531-9C8C39EB29CE}",
                    "Value", "Deny", RegistryValueKind.String);
                destroyWindowsSpying.SetRegValueHKCU(
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\DeviceAccess\Global\{B19F89AF-E3EB-444B-8DEA-202575A71599}",
                    "Value", "Deny", RegistryValueKind.String);
                destroyWindowsSpying.SetRegValueHKCU(
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\DeviceAccess\Global\{BFA794E4-F964-4FDB-90F6-51056BFE4B44}",
                    "Value", "Deny", RegistryValueKind.String);
                destroyWindowsSpying.SetRegValueHKCU(
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\DeviceAccess\Global\{C1D23ACC-752B-43E5-8448-8D0E519CD6D6}",
                    "Value", "Deny", RegistryValueKind.String);
                destroyWindowsSpying.SetRegValueHKCU(
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\DeviceAccess\Global\{D89823BA-7180-4B81-B50C-7E471E6121A3}",
                    "Value", "Deny", RegistryValueKind.String);
                destroyWindowsSpying.SetRegValueHKCU(
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\DeviceAccess\Global\{E5323777-F976-4f5b-9B55-B94699C46E44}",
                    "Value", "Deny", RegistryValueKind.String);
                destroyWindowsSpying.SetRegValueHKCU(
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\DeviceAccess\Global\{E6AD100E-5F4E-44CD-BE0F-2265D88D14F5}",
                    "Value", "Deny", RegistryValueKind.String);
                destroyWindowsSpying.SetRegValueHKCU(
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\DeviceAccess\Global\{E83AF229-8640-4D18-A213-E22675EBB2C3}",
                    "Value", "Deny", RegistryValueKind.String);
                destroyWindowsSpying.SetRegValueHKCU(@"SOFTWARE\Microsoft\Windows\CurrentVersion\DeviceAccess\Global\LooselyCoupled",
                    "Value", "Deny", RegistryValueKind.String);
                logger.output("Disable private settings");
                destroyWindowsSpying.SetRegValueHKCU(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Search", "CortanaEnabled", "0",
                    RegistryValueKind.DWord);
                destroyWindowsSpying.SetRegValueHKCU(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Search", "BingSearchEnabled", "0",
                    RegistryValueKind.DWord);
            }
            progressbaradd(10); //55
            if (checkBoxDisableWindowsDefender.Checked)
            {
                destroyWindowsSpying.SetRegValueHKLM(@"SOFTWARE\Policies\Microsoft\Windows Defender", "DisableAntiSpyware", "1",
                    RegistryValueKind.DWord);
                logger.output("Disable Windows Defender.");
            }
            progressbaradd(5); //60
            if (checkBoxSetDefaultPhoto.Checked)
            {
                destroyWindowsSpying.SetRegValueHKCU(@"Software\Classes\.ico", null, "PhotoViewer.FileAssoc.Tiff", RegistryValueKind.String);
                destroyWindowsSpying.SetRegValueHKCU(@"Software\Classes\.tiff", null, "PhotoViewer.FileAssoc.Tiff", RegistryValueKind.String);
                destroyWindowsSpying.SetRegValueHKCU(@"Software\Classes\.bmp", null, "PhotoViewer.FileAssoc.Tiff", RegistryValueKind.String);
                destroyWindowsSpying.SetRegValueHKCU(@"Software\Classes\.png", null, "PhotoViewer.FileAssoc.Tiff", RegistryValueKind.String);
                destroyWindowsSpying.SetRegValueHKCU(@"Software\Classes\.gif", null, "PhotoViewer.FileAssoc.Tiff", RegistryValueKind.String);
                destroyWindowsSpying.SetRegValueHKCU(@"Software\Classes\.jpeg", null, "PhotoViewer.FileAssoc.Tiff", RegistryValueKind.String);
                destroyWindowsSpying.SetRegValueHKCU(@"Software\Classes\.jpg", null, "PhotoViewer.FileAssoc.Tiff", RegistryValueKind.String);
                logger.output("Set Default PhotoViewer");
            }
            progressbaradd(10); //70
            if (checkBoxSPYTasks.Checked)
            {
                destroyWindowsSpying.disableSpyTasks();
            }
            progressbaradd(10); //80
            if (checkBoxDeleteWindows10Apps.Checked)
            {
                RemoveWindows10Apps();
            }
            progressbaradd(20); //100
            EnableOrDisableTab(true);
            try
            {
                Invoke(new MethodInvoker(delegate
                {
                    setcompletetext();
                }));
            }
            catch (Exception)
            {
                try
                {
                    setcompletetext();
                }
                catch (Exception)
                {

                }
            }
        }

        private void setcompletetext(bool start = false)
        {
            if (start)
            {
                StatusCommandsLable.Text = "Destroy Windows 10 Spying";
                StatusCommandsLable.ForeColor = Color.Black;
            }
            else
            {
                if (fatalerrors == 0)
                {
                    StatusCommandsLable.Text = "Destroy Windows 10 Spying - " + GetTranslateText("Complete") + "!";
                    StatusCommandsLable.ForeColor = Color.DarkGreen;
                    if (
                        MessageBox.Show(GetTranslateText("CompleteMSG"), GetTranslateText("Info"),
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                        DialogResult.Yes)
                    {
                        Process.Start("shutdown.exe", "-r -t 0");
                    }
                }
                else
                {
                    StatusCommandsLable.Text = "Destroy Windows 10 Spying - errors: " + fatalerrors.ToString();
                    StatusCommandsLable.ForeColor = Color.Red;
                    if (
                        MessageBox.Show(string.Format(GetTranslateText("ErrorMSG"), fatalerrors), GetTranslateText("Info"),
                            MessageBoxButtons.YesNo, MessageBoxIcon.Warning) ==
                        DialogResult.Yes)
                    {
                        Process.Start("shutdown.exe", "-r -t 0");
                    }
                }
            }
        }

        private void RemoveWindows10Apps()
        {
           
            if (checkBoxDeleteApp3d.Checked)
            {
                destroyWindowsSpying.DeleteWindows10MetroApp("3d");
                logger.output("Delete builder 3D");
            }
            if (checkBoxDeleteAppCamera.Checked)
            {
                destroyWindowsSpying.DeleteWindows10MetroApp("camera");
                logger.output("Delete Camera");
            }
            if (checkBoxDeleteMailCalendarMaps.Checked)
            {
                destroyWindowsSpying.DeleteWindows10MetroApp("communi");
                destroyWindowsSpying.DeleteWindows10MetroApp("maps");
                logger.output("Delete Mail, Calendar, Maps");
            }
            if (checkBoxDeleteAppBing.Checked)
            {
                destroyWindowsSpying.DeleteWindows10MetroApp("bing");
                logger.output("Delete Money, Sports, News and Weather");
            }
            if (checkBoxDeleteAppZune.Checked)
            {
                destroyWindowsSpying.DeleteWindows10MetroApp("zune");
                logger.output("Delete Groove Music and Film TV");
            }
            if (checkBoxDeleteAppPeopleOneNote.Checked)
            {
                destroyWindowsSpying.DeleteWindows10MetroApp("people");
                destroyWindowsSpying.DeleteWindows10MetroApp("note");
                logger.output("Delete People and OneNote");
            }
            if (checkBoxDeleteAppPhone.Checked)
            {
                destroyWindowsSpying.DeleteWindows10MetroApp("phone");
                logger.output("Delete Phone Companion");
            }
            if (checkBoxDeleteAppPhotos.Checked)
            {
                destroyWindowsSpying.DeleteWindows10MetroApp("photo");
                logger.output("Delete Photos");
            }
            if (checkBoxDeleteAppSolit.Checked)
            {
                destroyWindowsSpying.DeleteWindows10MetroApp("solit");
                logger.output("Delete Solitaire Collection");
            }
            if (checkBoxDeleteAppVoice.Checked)
            {
                destroyWindowsSpying.DeleteWindows10MetroApp("soundrec");
                logger.output("Delete Voice Recorder");
            }
            if (checkBoxDeleteAppXBOX.Checked)
            {
                destroyWindowsSpying.DeleteWindows10MetroApp("xbox");
                logger.output("Delete XBOX");
            }

        }


        private void checkBoxDeleteWindows10Apps_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxDeleteApp3d.Enabled = checkBoxDeleteWindows10Apps.Checked;
            checkBoxDeleteAppBing.Enabled = checkBoxDeleteWindows10Apps.Checked;
            checkBoxDeleteAppCamera.Enabled = checkBoxDeleteWindows10Apps.Checked;
            checkBoxDeleteAppPeopleOneNote.Enabled = checkBoxDeleteWindows10Apps.Checked;
            checkBoxDeleteAppPhone.Enabled = checkBoxDeleteWindows10Apps.Checked;
            checkBoxDeleteAppPhotos.Enabled = checkBoxDeleteWindows10Apps.Checked;
            checkBoxDeleteAppSolit.Enabled = checkBoxDeleteWindows10Apps.Checked;
            checkBoxDeleteAppVoice.Enabled = checkBoxDeleteWindows10Apps.Checked;
            checkBoxDeleteAppXBOX.Enabled = checkBoxDeleteWindows10Apps.Checked;
            checkBoxDeleteAppZune.Enabled = checkBoxDeleteWindows10Apps.Checked;
            checkBoxDeleteMailCalendarMaps.Enabled = checkBoxDeleteWindows10Apps.Checked;
        }

        private void btnDeleteAllWindows10Apps_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(GetTranslateText("Really"), GetTranslateText("Question"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                DialogResult.Yes)
            {
                EnableOrDisableTab(false);
                MessageBox.Show(GetTranslateText("PressOkAndWait15"));
                new Thread(() =>
                {
                    destroyWindowsSpying.DeleteWindows10MetroApp(null);
                    Invoke(new MethodInvoker(delegate
                    {
                        EnableOrDisableTab(true);
                        MessageBox.Show(GetTranslateText("Complete"), GetTranslateText("Info"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }));
                }).Start();
            }
            else
            {
                MessageBox.Show("=(", "%(");
            }
        }

        private void btnRestoreSystem_Click(object sender, EventArgs e)
        {
            Process.Start(system32location + "rstrui.exe");
        }

        private void btnEnableUac_Click(object sender, EventArgs e)
        {
            destroyWindowsSpying.SetRegValueHKLM(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System\", "EnableLUA", "1",
                RegistryValueKind.DWord);
            logger.output("Enable UAC");
            CheckEnableOrDisableUAC();
            if (
                MessageBox.Show(GetTranslateText("CompleteMSG"), GetTranslateText("Info"), MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Process.Start("shutdown.exe", "-r -t 0");
            }
        }

        private void btnDisableUac_Click(object sender, EventArgs e)
        {
            destroyWindowsSpying.SetRegValueHKLM(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System\", "EnableLUA", "0",
                RegistryValueKind.DWord);
            logger.output("Disable UAC");
            CheckEnableOrDisableUAC();
            if (
                MessageBox.Show(GetTranslateText("CompleteMSG"), GetTranslateText("Info"), MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Process.Start("shutdown.exe", "-r -t 0");
            }
        }

        private void btnEnableWindowsUpdate_Click(object sender, EventArgs e)
        {
            ProcessUtil.RunPowerShell("-command \"Set-Service -Name wuauserv -StartupType Automatic\"");
            ProcessUtil.RunCmd("/c net start wuauserv");
            logger.output("Windows Update enabled");
        }

        private void btnDisableWindowsUpdate_Click(object sender, EventArgs e)
        {
            ProcessUtil.RunCmd("/c net stop wuauserv");
            ProcessUtil.RunPowerShell("-command \"Set-Service -Name wuauserv -StartupType Disabled\"");
            logger.output("Windows Update disabled");
        }

        private void btnOpenAndEditHosts_Click(object sender, EventArgs e)
        {
            logger.output(ProcessUtil.StartProcess("notepad", system32location + @"drivers\etc\hosts"));
        }

        private void DestroyWindowsSpyingMainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        private void DestroyWindowsSpyingMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://goo.gl/EpFSzj");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://goo.gl/fxEkcl");
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://goo.gl/CDaZye");
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://goo.gl/uzXaHM");
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://goo.gl/sZIfQD");
        }

        private bool professionalmode = false;

        private void btnProfessionalMode_Click(object sender, EventArgs e)
        {
            professionalmode = !professionalmode;
            ProfessionalModeSet(professionalmode);
        }

        private void ProfessionalModeSet(bool enableordisable)
        {
            checkBoxCreateSystemRestorePoint.Visible = enableordisable;
            checkBoxKeyLoggerAndTelemetry.Visible = enableordisable;
            checkBoxAddToHosts.Visible = enableordisable;
            checkBoxDisablePrivateSettings.Visible = enableordisable;
            checkBoxDisableWindowsDefender.Visible = enableordisable;
            checkBoxSetDefaultPhoto.Visible = enableordisable;
            checkBoxSPYTasks.Visible = enableordisable;
            btnDeleteAllWindows10Apps.Visible = enableordisable;
            groupBoxUACEdit.Visible = enableordisable;
            btnDeleteMetroAppsInfo.Visible = enableordisable;
            btnDeleteOneDrive.Visible = enableordisable;
            if (enableordisable)
            {
                this.Text += "  !Professional mode!";
                btnProfessionalMode.Text = btnProfessionalMode.Text.Replace("Enable", "Disable");
            }
            else
            {
                this.Text = this.Text.Replace("  !Professional mode!", null);
                btnProfessionalMode.Text = btnProfessionalMode.Text.Replace("Disable", "Enable");
            }
        }

        private void linkLabelOtherThanks_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Чёрная поганка, Архангел", "Thanks");
        }

        private void btnDeleteMetroAppsInfo_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Delete apps: Calculator, Windows Store, Windows Feedback, and other METRO apps.", "Info",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnDeleteOneDrive_Click(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                EnableOrDisableTab(false);
                try
                {
                    destroyWindowsSpying.DeleteOneDrive();
                }
                catch (Exception ex)
                {
                    Invoke(new MethodInvoker(delegate
                    {
                        MessageBox.Show(ex.Message, GetTranslateText("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }));
                }
                Invoke(new MethodInvoker(delegate
                {
                    MessageBox.Show(GetTranslateText("Complete"), GetTranslateText("Info"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }));
                EnableOrDisableTab(true);
            }).Start();
        }

        private void linkLabelSourceCode_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/Nummer/Destroy-Windows-10-Spying");
        }

        private void btnRemoveOldFirewallRules_Click(object sender, EventArgs e)
        {
            ProcessUtil.RunCmd("/c netsh advfirewall firewall delete rule name=\"MS Spynet block 1\"");
            ProcessUtil.RunCmd("/c netsh advfirewall firewall delete rule name=\"MS Spynet block 2\"");
            ProcessUtil.RunCmd("/c netsh advfirewall firewall delete rule name=\"MS telemetry block 1\"");
            ProcessUtil.RunCmd("/c netsh advfirewall firewall delete rule name=\"MS telemetry block 2\"");
            MessageBox.Show(GetTranslateText("Complete"), GetTranslateText("Info"));
        }

        private void btnReportABug_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/Nummer/Destroy-Windows-10-Spying/issues/new");
        }

        private void comboBoxLanguageSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxLanguageSelect.Text.Split('|')[0].Replace(" ", "") == "ru-RU")
            {
                rm = lang.ru_RU.ResourceManager;
                ChangeLanguage();
            }
            else if (comboBoxLanguageSelect.Text.Split('|')[0].Replace(" ", "") == "fr-FR")
            {
                rm = lang.fr_FR.ResourceManager;
                ChangeLanguage();
            }
            else if (comboBoxLanguageSelect.Text.Split('|')[0].Replace(" ", "") == "es-ES")
            {
                rm = lang.es_ES.ResourceManager;
                ChangeLanguage();
            }
            else
            {
                rm = lang.en_US.ResourceManager;
                ChangeLanguage();
            }
        }

        private void btnDestroyWindows78Spy_Click(object sender, EventArgs e)
        {
            btnDestroyWindows78Spy.Enabled = false;
            fatalerrors = 0;
            new Thread(() =>
            {
                destroyWindowsSpying.disablehostsandaddfirewall();
                destroyWindowsSpying.disableSpyTasks();
                Invoke(new MethodInvoker(delegate
                {
                    btnDestroyWindows78Spy.Enabled = true;
                    MessageBox.Show(GetTranslateText("Complete"), GetTranslateText("Info"), MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }));
            }).Start();
        }
    }
}
