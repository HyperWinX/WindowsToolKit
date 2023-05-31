using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsToolKit
{
    public static class Uninstaller
    {
        private const string path = "SYSTEM\\CurrentControlSet\\Services\\";
        public static void UninstDefender()
        {

            Log.Warning("Setting MsSecCore to (dword)0x00000004");
            Registry.LocalMachine.OpenSubKey(path + "MsSecCore").SetValue("Start", 4, RegistryValueKind.DWord);
            Tools.ClearLastLine();
            Log.Warning("Setting wscsvc to (dword)0x00000004");
            Registry.LocalMachine.OpenSubKey(path + "wscsvc").SetValue("Start", 4, RegistryValueKind.DWord);
            Tools.ClearLastLine();
            Log.Warning("Setting WdNisDrv to (dword)0x00000004");
            Registry.LocalMachine.OpenSubKey(path + "WdNisDrv").SetValue("Start", 4, RegistryValueKind.DWord);
            Tools.ClearLastLine();
            Log.Warning("Setting WdNisSvc to (dword)0x00000004");
            Registry.LocalMachine.OpenSubKey(path + "WdNisSvc").SetValue("Start", 4, RegistryValueKind.DWord);
            Tools.ClearLastLine();
            Log.Warning("Setting WdFiltrer to (dword)0x00000004");
            Registry.LocalMachine.OpenSubKey(path + "WdFiltrer").SetValue("Start", 4, RegistryValueKind.DWord);
            Tools.ClearLastLine();
            Log.Warning("Setting WdBoot to (dword)0x00000004");
            Registry.LocalMachine.OpenSubKey(path + "WdBoot").SetValue("Start", 4, RegistryValueKind.DWord);
            Tools.ClearLastLine();
            Log.Warning("Setting SecurityHealthService to (dword)0x00000004");
            Registry.LocalMachine.OpenSubKey(path + "SecurityHealthService").SetValue("Start", 4, RegistryValueKind.DWord);
            Tools.ClearLastLine();
            Log.Warning("Setting SrgmAgent to (dword)0x00000004");
            Registry.LocalMachine.OpenSubKey(path + "SrgmAgent").SetValue("Start", 4, RegistryValueKind.DWord);
            Tools.ClearLastLine();
            Log.Warning("Setting SgrmBroker to (dword)0x00000004");
            Registry.LocalMachine.OpenSubKey(path + "SgrmBroker").SetValue("Start", 4, RegistryValueKind.DWord);
            Tools.ClearLastLine();
            Log.Warning("Setting WinDefend to (dword)0x00000004");
            Registry.LocalMachine.OpenSubKey(path + "WinDefend").SetValue("Start", 4, RegistryValueKind.DWord);
            Tools.ClearLastLine();
            Log.Warning("Disabling WinDefender WMI Logger...");
            Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\WMI\\Autologger\\DefenderApiLogger").SetValue("Start", 0, RegistryValueKind.DWord);
            Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\WMI\\Autologger\\DefenderApiLogger").SetValue("Status", 0, RegistryValueKind.DWord);
            Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\WMI\\Autologger\\DefenderAuditLogger").SetValue("Start", 0, RegistryValueKind.DWord);
            Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\WMI\\Autologger\\DefenderAuditLogger").SetValue("Status", 0, RegistryValueKind.DWord);
            Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\WMI\\Autologger\\DefenderAuditLogger").SetValue("EnableSecurityProvider", 0, RegistryValueKind.DWord);
        }
    }
}
