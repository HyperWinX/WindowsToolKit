
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using WindowsToolKit.ISO9660;

namespace WindowsToolKit
{
    internal static class Tools
    {
        internal static long KB = 1024;
        internal static long MB = 1048576;
        internal static long GB = 1073741824;
        internal static long TB = GB * 1024;
        internal enum VHDSubSystemModes
        {
            None,
            ISO,
            VHD,
            VHDX,
            WIM,
            FLP
        }
        #region
        [Flags]
        public enum ThreadAccess : int
        {
            TERMINATE = (0x0001),
            SUSPEND_RESUME = (0x0002),
            GET_CONTEXT = (0x0008),
            SET_CONTEXT = (0x0010),
            SET_INFORMATION = (0x0020),
            QUERY_INFORMATION = (0x0040),
            SET_THREAD_TOKEN = (0x0080),
            IMPERSONATE = (0x0100),
            DIRECT_IMPERSONATION = (0x0200)
        }
        [DllImport("ntdll.dll", SetLastError = true)]
        public static extern int NtSetInformationProcess(IntPtr hProcess, int processInformationClass, ref int processInformation, int processInformationLength);
        [DllImport("Kernel32.dll")]
        public static extern bool QueryFullProcessImageName([In] IntPtr hProcess, [In] uint dwFlags, [Out] StringBuilder lpExeName, [In, Out] ref uint lpdwSize);
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        [DllImport("kernel32.dll")]
        public static extern uint SuspendThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        public static extern int ResumeThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr handle);
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, uint dwProcessId);
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern SafeFileHandle CreateFile(
            string fileName,
            FileAccess access,
            FileShare share,
            IntPtr securityAttributes,
            FileMode mode,
            FileAttributes flagsAndAttributes,
            IntPtr template);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetFilePointerEx(
            SafeFileHandle hFile,
            long liDistanceToMove,
            out long lpNewFilePointer, SeekOrigin dwMoveMethod);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadFile(
            SafeFileHandle hFile,
            byte[] lpBuffer,
            int nNumberOfBytesToRead,
            out int lpNumberOfBytesRead,
            IntPtr lpOverlapped);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteFile(
            SafeFileHandle hFile,
            byte[] lpBuffer,
            int nNumberOfBytesToWrite,
            out int lpNumberOfBytesWritten,
            IntPtr lpOverlapped);
        [DllImport("kernel32.dll")]
        public static extern bool DeleteVolumeMountPoint(string lpszVolumeMountPoint);
        #endregion
        internal static void RecursiveDelete(DirectoryInfo baseDir)
        {
            if (!baseDir.Exists)
                return;

            foreach (DirectoryInfo dir in baseDir.EnumerateDirectories())
            {
                RecursiveDelete(dir);
            }
            FileInfo[] files = baseDir.GetFiles();
            foreach (FileInfo file in files)
            {
                SystemSetFileOwnership(file.FullName);

                Console.WriteLine(file.Name);
                file.Delete();
                ClearLastLine();
            }
            SystemSetFileOwnership(baseDir.FullName);
            baseDir.Delete();
        }
        internal static void CopyDirectory(string sourceDir, string destinationDir)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDir);
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");
            DirectoryInfo[] dirs = dir.GetDirectories();
            Directory.CreateDirectory(destinationDir);
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }
            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                CopyDirectory(subDir.FullName, newDestinationDir);
            }
        }
        internal static bool TryToRemoveFile(string path)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("takeown");
            string args = "/f " + path;
            startInfo.Arguments = args;
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            Process.Start(startInfo);
            try
            {
                System.IO.File.Delete(path);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex);
            }
            if (System.IO.File.Exists(path))
                return false;
            return true;
        }
        internal static bool TryToRemoveDirectory(string path)
        {
            if (!Directory.Exists(path))
                return false;
            try
            {
                RecursiveDelete(new DirectoryInfo(path));
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex);
            }
            if (Directory.Exists(path))
                return false;
            return true;
        }
        internal static void SystemSetFileOwnership(string path)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("takeown");
            string args = "/f " + path;
            startInfo.Arguments = args;
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            Process proc = Process.Start(startInfo);
            proc.WaitForExit();
        }
        internal static void ClearLastLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write(new string(' ', Console.BufferWidth));
            Console.SetCursorPosition(0, Console.CursorTop - 1);
        }
        internal static void ClearLastTwoLines()
        {
            ClearLastLine();
            ClearLastLine();
        }
        internal static void HardSCOptimization()
        {
            Console.Clear();
            Console.WriteLine("Preparing service list...");
            ServiceController[] services = ServiceController.GetServices();
            List<ServiceController> servicesToOptimize = new List<ServiceController>();
            List<string> log = new List<string>();
            foreach (ServiceController service in services)
            {
                switch (service.ServiceName)
                {
                    case "AXInstSV":
                        servicesToOptimize.Add(service);
                        break;
                    case "AJRouter":
                        servicesToOptimize.Add(service);
                        break;
                    case "ALG":
                        servicesToOptimize.Add(service);
                        break;
                    case "AppMgmt":
                        servicesToOptimize.Add(service);
                        break;
                    case "aspnet_state":
                        servicesToOptimize.Add(service);
                        break;
                    case "AssignedAccessManagerSvc":
                        servicesToOptimize.Add(service);
                        break;
                    case "BITS":
                        servicesToOptimize.Add(service);
                        break;
                    case "BDESVC":
                        servicesToOptimize.Add(service);
                        break;
                    case "PeerDistSvc":
                        servicesToOptimize.Add(service);
                        break;
                    case "autotimesvc":
                        servicesToOptimize.Add(service);
                        break;
                    case "CertPropSvc":
                        servicesToOptimize.Add(service);
                        break;
                    case "COMSysApp":
                        servicesToOptimize.Add(service);
                        break;
                    case "CDPSvc":
                        servicesToOptimize.Add(service);
                        break;
                    case "VaultSvc":
                        servicesToOptimize.Add(service);
                        break;
                    case "DsSvc":
                        servicesToOptimize.Add(service);
                        break;
                    case "DusmSvc":
                        servicesToOptimize.Add(service);
                        break;
                    case "DeviceInstall":
                        servicesToOptimize.Add(service);
                        break;
                    case "DmEnrollmentSvc":
                        servicesToOptimize.Add(service);
                        break;
                    case "dmwappushservice":
                        servicesToOptimize.Add(service);
                        break;
                    case "DsmSvc":
                        servicesToOptimize.Add(service);
                        break;
                    case "DevQueryBroker":
                        servicesToOptimize.Add(service);
                        break;
                    case "diagsvc":
                        servicesToOptimize.Add(service);
                        break;
                    case "DialogBlockingService":
                        servicesToOptimize.Add(service);
                        break;
                    case "MSDTC":
                        servicesToOptimize.Add(service);
                        break;
                    case "EFS":
                        servicesToOptimize.Add(service);
                        break;
                    case "Eaphost":
                        servicesToOptimize.Add(service);
                        break;
                    case "fhsvc":
                        servicesToOptimize.Add(service);
                        break;
                    case "fdPHost":
                        servicesToOptimize.Add(service);
                        break;
                    case "FDResPub":
                        servicesToOptimize.Add(service);
                        break;
                    case "lfsvc":
                        servicesToOptimize.Add(service);
                        break;
                    case "GoogleChromeElevationService":
                        servicesToOptimize.Add(service);
                        break;
                    case "GraphicsPerfSvc":
                        servicesToOptimize.Add(service);
                        break;
                    case "HgClientService":
                        servicesToOptimize.Add(service);
                        break;
                    case "IKEEXT":
                        servicesToOptimize.Add(service);
                        break;
                    case "IpxlatCfgSvc":
                        servicesToOptimize.Add(service);
                        break;
                    case "PolicyAgent":
                        servicesToOptimize.Add(service);
                        break;
                    case "KtmRm":
                        servicesToOptimize.Add(service);
                        break;
                    case "LxpSvc":
                        servicesToOptimize.Add(service);
                        break;
                    case "lltdsvc":
                        servicesToOptimize.Add(service);
                        break;
                    case "diagnosticshub.standardcollector.service":
                        servicesToOptimize.Add(service);
                        break;
                    case "AppVClient":
                        servicesToOptimize.Add(service);
                        break;
                    case "MicrosoftEdgeElevationService":
                        servicesToOptimize.Add(service);
                        break;
                    case "edgeupdate":
                        servicesToOptimize.Add(service);
                        break;
                    case "edgeupdatem":
                        servicesToOptimize.Add(service);
                        break;
                    case "MSiSCSI":
                        servicesToOptimize.Add(service);
                        break;
                    case "MsKeyboardFilter":
                        servicesToOptimize.Add(service);
                        break;
                    case "swprv":
                        servicesToOptimize.Add(service);
                        break;
                    case "InstallService":
                        servicesToOptimize.Add(service);
                        break;
                    case "uhssvc":
                        servicesToOptimize.Add(service);
                        break;
                    case "SmsRouter":
                        servicesToOptimize.Add(service);
                        break;
                    case "NaturalAuthentification":
                        servicesToOptimize.Add(service);
                        break;
                    case "Netlogon":
                        servicesToOptimize.Add(service);
                        break;
                    case "NcdAutoSetup":
                        servicesToOptimize.Add(service);
                        break;
                    case "Netman":
                        servicesToOptimize.Add(service);
                        break;
                    case "NcaSvc":
                        servicesToOptimize.Add(service);
                        break;
                    case "CscService":
                        servicesToOptimize.Add(service);
                        break;
                    case "ssh-agent":
                        servicesToOptimize.Add(service);
                        break;
                    case "defragsvc":
                        servicesToOptimize.Add(service);
                        break;
                    case "SEMgrSvc":
                        servicesToOptimize.Add(service);
                        break;
                    case "PNRPsvc":
                        servicesToOptimize.Add(service);
                        break;
                    case "p2psvc":
                        servicesToOptimize.Add(service);
                        break;
                    case "p2pimsvc":
                        servicesToOptimize.Add(service);
                        break;
                    case "pla":
                        servicesToOptimize.Add(service);
                        break;
                    case "PhoneSvc":
                        servicesToOptimize.Add(service);
                        break;
                    case "PNRPAutoReg":
                        servicesToOptimize.Add(service);
                        break;
                    case "Spooler":
                        servicesToOptimize.Add(service);
                        break;
                    case "PrintNotify":
                        servicesToOptimize.Add(service);
                        break;
                    case "QWAVE":
                        servicesToOptimize.Add(service);
                        break;
                    case "RmSvc":
                        servicesToOptimize.Add(service);
                        break;
                    case "RasAuto":
                        servicesToOptimize.Add(service);
                        break;
                    case "SessionEnv":
                        servicesToOptimize.Add(service);
                        break;
                    case "RpcLocator":
                        servicesToOptimize.Add(service);
                        break;
                    case "RemoteRegistry":
                        servicesToOptimize.Add(service);
                        break;
                    case "RetailDemo":
                        servicesToOptimize.Add(service);
                        break;
                    case "RemoteAccess":
                        servicesToOptimize.Add(service);
                        break;
                    case "seclogon":
                        servicesToOptimize.Add(service);
                        break;
                    case "SensorDataService":
                        servicesToOptimize.Add(service);
                        break;
                    case "SensrSvc":
                        servicesToOptimize.Add(service);
                        break;
                    case "SensorService":
                        servicesToOptimize.Add(service);
                        break;
                    case "LanmanServer":
                        servicesToOptimize.Add(service);
                        break;
                    case "shpamsvc":
                        servicesToOptimize.Add(service);
                        break;
                    case "SCardSvr":
                        servicesToOptimize.Add(service);
                        break;
                    case "ScDeviceEnum":
                        servicesToOptimize.Add(service);
                        break;
                    case "SCPolicySvc":
                        servicesToOptimize.Add(service);
                        break;
                    case "smphost":
                        servicesToOptimize.Add(service);
                        break;
                    case "SharedRealitySvc":
                        servicesToOptimize.Add(service);
                        break;
                    case "SSDPSRV":
                        servicesToOptimize.Add(service);
                        break;
                    case "SysMain":
                        servicesToOptimize.Add(service);
                        break;
                    case "TabletInputService":
                        servicesToOptimize.Add(service);
                        break;
                    case "UsoSvc":
                        servicesToOptimize.Add(service);
                        break;
                    case "UevAgentService":
                        servicesToOptimize.Add(service);
                        break;
                    case "SDRSVC":
                        servicesToOptimize.Add(service);
                        break;
                    case "FrameServer":
                        servicesToOptimize.Add(service);
                        break;
                    case "stisvc":
                        servicesToOptimize.Add(service);
                        break;
                    case "LicenseManager":
                        servicesToOptimize.Add(service);
                        break;
                    case "MixedRealityOpenXRSvc":
                        servicesToOptimize.Add(service);
                        break;
                    case "icssvc":
                        servicesToOptimize.Add(service);
                        break;
                    case "spectrum":
                        servicesToOptimize.Add(service);
                        break;
                    case "perceptionsimulation":
                        servicesToOptimize.Add(service);
                        break;
                    case "PushToInstall":
                        servicesToOptimize.Add(service);
                        break;
                    case "WSearch":
                        servicesToOptimize.Add(service);
                        break;
                    case "dot3svc":
                        servicesToOptimize.Add(service);
                        break;
                    case "LanmanWorkstation":
                        servicesToOptimize.Add(service);
                        break;
                    case "tzautoupdate":
                        servicesToOptimize.Add(service);
                        break;
                    case "AppReadiness":
                        servicesToOptimize.Add(service);
                        break;
                    case "SNMPTRAP":
                        servicesToOptimize.Add(service);
                        break;
                    case "Fax":
                        servicesToOptimize.Add(service);
                        break;
                }
            }
            foreach (ServiceController service in servicesToOptimize)
            {
                try
                {
                    if (service.CanStop)
                        service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped);
                    if (service.Status == ServiceControllerStatus.Stopped)
                    {
                        Log.Success($"Service {service.ServiceName} successfully stopped.");
                        log.Add($"Service {service.ServiceName} successfully stopped.");
                    }
                    else
                    {
                        Log.Error($"Service {service.ServiceName} cannot be stopped.");
                        log.Add($"Service {service.ServiceName} cannot be stopped.");
                    }
                    RegistryKey serviceKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\" + service.ServiceName, true);
                    serviceKey.SetValue("Start", 4, RegistryValueKind.DWord);
                    if (Convert.ToInt32(serviceKey.GetValue("Start")) == 4)
                    {
                        Log.Success($"Start type of service {service.ServiceName} successfully set to DISABLED.");
                        log.Add($"Start type of service {service.ServiceName} successfully set to DISABLED.");
                    }
                    else
                    {
                        Log.Error($"Start type of service {service.ServiceName} cannot be set to DISABLED.");
                        log.Add($"Start type of service {service.ServiceName} cannot be set to DISABLED.");
                    }
                    serviceKey.Close();
                    ClearLastTwoLines();
                }
                catch (Exception ex)
                {
                    ErrorHandler.HandleError(ex);
                }
            }
            Console.WriteLine("Writing logs...");
            System.IO.File.WriteAllLines("log.log", log);
            Log.Success($"Hard service optimization completed. Reboot recomended.");
        }
        internal static List<ServiceController> GetServiceList()
        {
            return ServiceController.GetServices().ToList();
        }
        internal static ServiceController GetServiceObject(string serviceName)
        {
            ServiceController[] serviceArray = ServiceController.GetServices();
            string[] serviceNames = new string[serviceArray.Length];
            for (int i = 0; i < serviceArray.Length; i++)
                serviceNames[i] = serviceArray[i].ServiceName;
            int index = Array.IndexOf(serviceNames, serviceName);
            if (index == -1)
                return null;
            return serviceArray[index];
        }
        internal static Process CreateProcessObject(string fileName, string args, bool CreateNoWindow, bool errRedirect, bool inRedirect, bool outRedirect, bool UseShellExecute)
        {
            Process proc = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = args,
                CreateNoWindow = CreateNoWindow,
                RedirectStandardError = errRedirect,
                RedirectStandardInput = inRedirect,
                RedirectStandardOutput = outRedirect,
                UseShellExecute = UseShellExecute
            };
            proc.StartInfo = startInfo;
            return proc;
        }
        internal static string GetMainModuleFileName(this Process process, int buffer = 1024)
        {
            StringBuilder fileNameBuilder = new StringBuilder(buffer);
            uint bufferLength = (uint)fileNameBuilder.Capacity + 1;
            return QueryFullProcessImageName(process.Handle, 0, fileNameBuilder, ref bufferLength) ?
                fileNameBuilder.ToString() :
                null;
        }
        internal static void SuspendProcess(int pid)
        {
            Process process = Process.GetProcessById(pid);
            foreach (ProcessThread pT in process.Threads)
            {
                IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);
                if (pOpenThread == IntPtr.Zero)
                    continue;
                SuspendThread(pOpenThread);
                CloseHandle(pOpenThread);
            }
        }
        internal static void ResumeProcess(int pid)
        {
            Process process = Process.GetProcessById(pid);
            if (process.ProcessName == string.Empty)
                return;
            foreach (ProcessThread pT in process.Threads)
            {
                IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);
                if (pOpenThread == IntPtr.Zero)
                    continue;
                int suspendCount = 0;
                do
                {
                    suspendCount = ResumeThread(pOpenThread);
                } while (suspendCount > 0);
                CloseHandle(pOpenThread);
            }
        }
        internal static String ToBinary(string value)
        {
            byte[] data = Encoding.ASCII.GetBytes(value);
            return string.Join(" ", data.Select(byt => Convert.ToString(byt, 2).PadLeft(8, '0')));
        }
        internal static bool GetRegistryValueKindFromString(string valueKind, out RegistryValueKind kind)
        {
            switch (valueKind.ToLower())
            {
                case "string":
                    kind = RegistryValueKind.String;
                    return true;
                case "binary":
                    kind = RegistryValueKind.Binary;
                    return true;
                case "dword":
                    kind = RegistryValueKind.DWord;
                    return true;
                case "qword":
                    kind = RegistryValueKind.QWord;
                    return true;
                case "multistring":
                    kind = RegistryValueKind.MultiString;
                    return true;
                case "expandstring":
                    kind = RegistryValueKind.ExpandString;
                    return true;
            }
            kind = RegistryValueKind.None;
            return false;
        }
        internal static object GetValueUsingKind(string value, RegistryValueKind valueKind)
        {
            switch (valueKind)
            {
                case RegistryValueKind.String:
                    return value;
                case RegistryValueKind.Binary:
                    return ToBinary(value);
                case RegistryValueKind.DWord:
                    uint rData;
                    if (UInt32.TryParse(value, out rData))
                        return rData;
                    else
                        return null;
                case RegistryValueKind.QWord:
                    return UInt64.Parse(value);
                case RegistryValueKind.MultiString:
                    return value.Split(' ');
                case RegistryValueKind.ExpandString:
                    return value;
            }
            return null;
        }
        internal static void ServiceControl()
        {
            Console.Clear();
            List<ServiceController> services = GetServiceList();
            Console.WriteLine("Service control subsystem.");
            bool work = true;
            while (work)
            {
                Console.Write("(SC_CONTROL) >>> ");
                string query = Console.ReadLine();
                string[] queryArray = query.Split(' ');
                switch (queryArray[0])
                {
                    case "help":
                        Console.WriteLine("1. list - list of services");
                        Console.WriteLine("2. stop <service_name> - stops specified service");
                        Console.WriteLine("3. info <service_name> - displays full info about service");
                        Console.WriteLine("4. set <service_name> <boot_type> - set service startup type");
                        Console.WriteLine("5. start <service_name> - starts specified service");
                        Console.WriteLine("6. delete <service_name> - uninstalls specified service");
                        Console.WriteLine("7. create <service_name> <service_path> <start_type> - creates service with specified parameters");
                        break;
                    case "list":
                        Program.currentTask = "sc_list";
                        int errors = 0;
                        Console.WriteLine("[NAME]                                [StartType]     [Status]        ");
                        foreach (ServiceController serv in services)
                        {
                            int servNameLength;
                            int startTypeLength;
                            try
                            {
                                servNameLength = serv.ServiceName.Length;
                                startTypeLength = serv.StartType.ToString().Length;
                                StringBuilder builder = new StringBuilder();
                                if (servNameLength > 38)
                                {
                                    builder.Append(serv.ServiceName.Remove(37));
                                    builder.Append(' ');
                                }
                                else
                                {
                                    builder.Append(serv.ServiceName);
                                    for (int i = servNameLength; i < 38; i++)
                                        builder.Append(' ');
                                }
                                if (startTypeLength > 15)
                                {
                                    builder.Append(serv.StartType.ToString().Remove(15));
                                    builder.Append(' ');
                                }
                                else
                                {
                                    builder.Append(serv.StartType.ToString());
                                }
                                for (int i = builder.Length; i < 54; i++)
                                    builder.Append(' ');
                                builder.Append(serv.Status.ToString());
                                Console.WriteLine(builder.ToString());
                            }
                            catch (Exception ex)
                            {
                                ErrorHandler.HandleError(ex);
                                errors += 1;
                            }
                        }
                        Console.WriteLine();
                        if (errors > 0)
                            Console.WriteLine("Command executed with one or more errors.");
                        else
                            Console.WriteLine("Command executed without errors.");
                        break;
                    case "stop":
                        if (queryArray.Length < 2)
                        {
                            Log.Warning("Not enough arguments given.");
                            break;
                        }
                        ServiceController service = GetServiceObject(queryArray[1]);
                        if (service == null)
                        {
                            Log.Error("Cannot find service with specified name: " + queryArray[1]);
                            break;
                        }
                        try
                        {
                            if (service.CanStop && service.Status != ServiceControllerStatus.Stopped)
                            {
                                service.Stop();
                                service.WaitForStatus(ServiceControllerStatus.Stopped);
                                if (service.Status == ServiceControllerStatus.Stopped)
                                {
                                    Log.Success($"Service {service.ServiceName} successfully stopped.");
                                }
                                else
                                {
                                    Log.Error($"Service {service.ServiceName} cannot be stopped.");
                                }
                            }
                            else
                            {
                                Log.Error("Service cannot be stopped");
                            }
                        }
                        catch (Exception ex)
                        {
                            ErrorHandler.HandleError(ex);
                        }
                        break;
                    case "info":
                        if (queryArray.Length < 2)
                        {
                            Log.Error("Not enough arguments.");
                            break;
                        }
                        ServiceController service1 = GetServiceObject(queryArray[1]);
                        if (service1 == null)
                        {
                            Log.Error("Cannot find service with specified name: " + queryArray[1]);
                            break;
                        }
                        ServiceController[] DependentServices = service1.DependentServices;
                        ServiceController[] ServicesDependedOn = service1.ServicesDependedOn;
                        Console.WriteLine();
                        Console.WriteLine("[NAME]");
                        Console.WriteLine($"DisplayName: {service1.DisplayName}");
                        Console.WriteLine($"MachineName: {service1.MachineName}");
                        Console.WriteLine($"ServiceName: {service1.ServiceName}");
                        Console.WriteLine();
                        Console.WriteLine("[WORK INFO]");
                        Console.WriteLine($"StartType: {service1.StartType}");
                        Console.WriteLine($"Status: {service1.Status}");
                        Console.WriteLine();
                        Console.WriteLine("[PAUSE INFO]");
                        Console.WriteLine($"CanPauseAndContinue: {service1.CanPauseAndContinue}");
                        Console.WriteLine($"CanShutdown: {service1.CanShutdown}");
                        Console.WriteLine($"CanStop: {service1.CanStop}");
                        Console.WriteLine();
                        Console.WriteLine("[DEPENDENCIES]");
                        Console.Write($"DependentServices: ");
                        if (DependentServices.Length == 0)
                            Console.WriteLine("null");
                        else
                        {
                            for (int i = 0; i < DependentServices.Length; i++)
                            {
                                if (i == 0)
                                    Console.WriteLine($"{DependentServices[i].ServiceName}");
                                else
                                    Console.WriteLine($"                   {DependentServices[i].ServiceName}");
                            }
                        }
                        Console.Write($"ServicesDependedOn: ");
                        if (ServicesDependedOn.Length == 0)
                            Console.WriteLine("null");
                        else
                        {
                            for (int i = 0; i < ServicesDependedOn.Length; i++)
                            {
                                if (i == 0)
                                    Console.WriteLine($"{ServicesDependedOn[i].ServiceName}");
                                else
                                    Console.WriteLine($"                   {ServicesDependedOn[i].ServiceName}");
                            }
                        }
                        Console.WriteLine();
                        Console.WriteLine("[MORE INFO]");
                        Console.WriteLine($"ServiceHandle: {service1.ServiceHandle}");
                        Console.WriteLine($"ServiceType: {service1.ServiceType}");
                        Console.WriteLine($"Site: {service1.Site}");
                        Console.WriteLine($"Container: {service1.Container}");
                        break;
                    case "set":
                        if (queryArray.Length < 3)
                        {
                            Console.WriteLine("Available start types: ");
                            Console.WriteLine("Boot: Starting service with system startup.");
                            Console.WriteLine("System: Starting service in system boot.");
                            Console.WriteLine("Automatic: Starting service when OS startup is completed.");
                            Console.WriteLine("Manual: User starts service manually.");
                            Console.WriteLine("Disabled: Service cannot be started.");
                            break;
                        }
                        queryArray[2] = queryArray[2].ToLower();
                        ServiceController service2 = GetServiceObject(queryArray[1]);
                        if (service2 == null)
                        {
                            Log.Error("Cannot find service with specified name: " + queryArray[1]);
                            break;
                        }
                        try
                        {
                            using (RegistryKey serviceKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\" + service2.ServiceName, true))
                            {
                                switch (queryArray[2])
                                {
                                    case "boot":
                                        serviceKey.SetValue("Start", 0, RegistryValueKind.DWord);
                                        break;
                                    case "system":
                                        serviceKey.SetValue("Start", 1, RegistryValueKind.DWord);
                                        break;
                                    case "automatic":
                                        serviceKey.SetValue("Start", 2, RegistryValueKind.DWord);
                                        break;
                                    case "manual":
                                        serviceKey.SetValue("Start", 3, RegistryValueKind.DWord);
                                        break;
                                    case "disabled":
                                        serviceKey.SetValue("Start", 4, RegistryValueKind.DWord);
                                        break;
                                }
                            }
                            if (service2.StartType.ToString().ToLower() == queryArray[2])
                                Log.Success("Service start type updated successfully");
                            else
                                Log.Error("Cannot update service start type");
                        }
                        catch (Exception ex)
                        {
                            ErrorHandler.HandleError(ex);
                        }
                        break;
                    case "start":
                        if (queryArray.Length < 2)
                        {
                            Log.Warning("Not enough arguments given.");
                            break;
                        }
                        ServiceController service3 = GetServiceObject(queryArray[1]);
                        if (service3 == null)
                        {
                            Log.Error("Cannot find service with specified name: " + queryArray[1]);
                            break;
                        }
                        try
                        {
                            if (service3.Status != ServiceControllerStatus.Running)
                            {
                                service3.Start();
                                service3.WaitForStatus(ServiceControllerStatus.Running);
                                if (service3.Status == ServiceControllerStatus.Running)
                                {
                                    Log.Success($"Service {service3.ServiceName} successfully started.");
                                }
                                else
                                {
                                    Log.Error($"Service {service3.ServiceName} cannot be started.");
                                }
                            }
                            else
                            {
                                Log.Error("Service cannot be started");
                            }
                        }
                        catch (Exception ex)
                        {
                            ErrorHandler.HandleError(ex);
                        }
                        break;
                    case "exit":
                        return;
                    case "delete":
                        if (queryArray.Length < 2)
                        {
                            Log.Warning("Not enough arguments given.");
                            break;
                        }
                        ServiceController service5 = GetServiceObject(queryArray[1]);
                        if (service5 == null)
                        {
                            Log.Error("Cannot find service with specified name: " + queryArray[1]);
                            break;
                        }
                        string serviceName = service5.ServiceName;
                        Process serviceDelete = new Process();
                        ProcessStartInfo startInfo = new ProcessStartInfo
                        {
                            FileName = "cmd.exe",
                            Arguments = $"/c sc delete {serviceName}",
                            CreateNoWindow = true
                        };
                        serviceDelete.StartInfo = startInfo;
                        serviceDelete.Start();
                        serviceDelete.WaitForExit();
                        if (serviceDelete.ExitCode != 0 && Array.IndexOf(GetServiceList().ToArray(), GetServiceObject(queryArray[1])) == -1)
                            Log.Error("Cannot remove service.");
                        else
                            Log.Success("Service successfully deleted.");
                        break;
                    case "create":
                        if (queryArray.Length < 4)
                        {
                            Log.Warning("Not enough arguments given.");
                            break;
                        }
                        string CreationServiceName = queryArray[1];
                        string servicePath = queryArray[2];
                        string startType = queryArray[3];
                        Process proc = CreateProcessObject("cmd.exe", $"/c sc create {CreationServiceName} binPath={servicePath} start={startType}", true, false, false, false, true);
                        proc.Start();
                        proc.WaitForExit();
                        if (proc.ExitCode != 0)
                            Log.Error("Cannot create service.");
                        else
                            Log.Success("Service successfully created.");
                        break;
                }
            }
        }
        internal static void ProcessControl()
        {
            Console.WriteLine("Process control subsystem.");
            bool work = true;
            while (work)
            {
                Console.Write("(P_CONTROL) >>> ");
                string query = Console.ReadLine();
                string[] queryArray = query.Split(' ');
                switch (queryArray[0])
                {
                    case "help":
                        Console.WriteLine("1. list - list of running processes");
                        Console.WriteLine("2. idkill <proc_id> - kill process by id");
                        Console.WriteLine("3. namekill <proc_name> - kill processes by name");
                        Console.WriteLine("4. cr <proc_id> - make process critical");
                        Console.WriteLine("5. info <proc_id> - get full info about process");
                        Console.WriteLine("6. pause <proc_id> - pause process");
                        Console.WriteLine("7. resume <proc_id> - resume process");
                        break;
                    case "list":
                        Console.WriteLine("Preparing process list...");
                        Process[] processes = Process.GetProcesses();
                        Array.Sort(processes, new ProcessNameComparer());
                        List<string> strings = new List<string>();
                        try
                        {
                            foreach (Process proc in processes)
                            {
                                StringBuilder builder = new StringBuilder();
                                builder.Append(proc.Id);
                                if (proc.Id.ToString().Length == 3)
                                    builder.Append(' ');
                                builder.Append(' ');
                                builder.Append(' ');
                                builder.Append(' ');
                                builder.Append(proc.ProcessName);
                                strings.Add(builder.ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            ErrorHandler.HandleError(ex);
                        }
                        ClearLastLine();
                        Console.WriteLine("[PID]  [NAME]");
                        foreach (string str in strings)
                            Console.WriteLine(str);
                        break;
                    case "idkill":
                        if (queryArray.Length < 2)
                        {
                            Log.Warning("Not enough arguments given.");
                            break;
                        }
                        int pid = 0;
                        try
                        {
                            pid = Convert.ToInt32(queryArray[1]);
                        }
                        catch
                        {
                            Log.Error("Cannot find process by id");
                            break;
                        }
                        Process proc1 = Process.GetProcessById(pid);
                        proc1.Kill();
                        try
                        {
                            proc1 = Process.GetProcessById(Convert.ToInt32(queryArray[1]));
                            Log.Error($"Cannot kill process with pid {queryArray[1]}");
                        }
                        catch
                        {
                            Log.Success($"Process with pid {queryArray[1]} successfully killed.");
                        }
                        break;
                    case "namekill":
                        if (queryArray.Length < 2)
                        {
                            Log.Warning("Not enough arguments given.");
                            break;
                        }
                        Process[] processesNames = Process.GetProcessesByName(queryArray[1]);
                        if (processesNames.Length == 0)
                        {
                            Log.Error("Cannot find process by name");
                            break;
                        }
                        foreach (Process proc2 in processesNames)
                        {
                            try
                            {
                                proc2.Kill();
                                Log.Success($"Successfully killed process {proc2.ProcessName} with pid {proc2.Id}");
                            }
                            catch
                            {
                                Log.Error($"Cannot kill process {proc2.ProcessName} with pid {proc2.Id}");
                            }
                        }
                        break;
                    case "cr":
                        if (queryArray.Length < 2)
                        {
                            Log.Warning("Not enough arguments given.");
                            break;
                        }
                        int IsCritical = 1;
                        int BreakOnTermination = 0x1D;
                        Process.EnterDebugMode();
                        int pid6 = 0;
                        try
                        {
                            pid6 = Convert.ToInt32(queryArray[1]);
                        }
                        catch
                        {
                            Log.Error("Cannot find process by pid");
                            break;
                        }
                        try
                        {
                            NtSetInformationProcess(Process.GetProcessById(Convert.ToInt32(queryArray[1])).Handle, BreakOnTermination, ref IsCritical, sizeof(int));
                        }
                        catch (Exception ex)
                        {
                            ErrorHandler.HandleError(ex);
                        }
                        Process.LeaveDebugMode();
                        break;
                    case "info":
                        if (queryArray.Length < 2)
                        {
                            Log.Warning("Not enough arguments given.");
                            break;
                        }
                        int pid1 = 0;
                        try
                        {
                            pid1 = Convert.ToInt32(queryArray[1]);
                        }
                        catch
                        {
                            Log.Error("Cannot find process by pid");
                            break;
                        }
                        try
                        {
                            Process procInfo = Process.GetProcessById(pid1);
                            List<Action> t = new List<Action>();
                            t.Add(() => Console.WriteLine($"BasePriority: {procInfo.BasePriority}"));
                            t.Add(() => Console.WriteLine($"EnableRaisingEvents: {procInfo.EnableRaisingEvents}"));
                            t.Add(() => Console.WriteLine($"HandleCount: {procInfo.HandleCount}"));
                            t.Add(() => Console.WriteLine($"PID: {procInfo.Id}"));
                            t.Add(() => Console.WriteLine($"MachineName: {procInfo.MachineName}"));
                            t.Add(() => Console.WriteLine($"NonpagedSystemMemorySize64: {procInfo.NonpagedSystemMemorySize64}"));
                            t.Add(() => Console.WriteLine($"PagedMemorySize64: {procInfo.PagedMemorySize64}"));
                            t.Add(() => Console.WriteLine($"PagedSystemMemorySize64: {procInfo.PagedSystemMemorySize64}"));
                            t.Add(() => Console.WriteLine($"PeakPagedMemorySize64: {procInfo.PeakPagedMemorySize64}"));
                            t.Add(() => Console.WriteLine($"PeakVirtualMemorySize64: {procInfo.PeakVirtualMemorySize64}"));
                            t.Add(() => Console.WriteLine($"PeakWorkingSet64: {procInfo.PeakWorkingSet64}"));
                            t.Add(() => Console.WriteLine($"PriorityBoostEnabled: {procInfo.PriorityBoostEnabled}"));
                            t.Add(() => Console.WriteLine($"PrivateMemorySize64: {procInfo.PrivateMemorySize64}"));
                            t.Add(() => Console.WriteLine($"PrivilegedProcessorTime: {procInfo.PrivilegedProcessorTime}"));
                            t.Add(() => Console.WriteLine($"ProcessName: {procInfo.ProcessName}"));
                            t.Add(() => Console.WriteLine($"ProcessPath: {procInfo.GetMainModuleFileName()}"));
                            t.Add(() => Console.WriteLine($"Responding: {procInfo.Responding}"));
                            t.Add(() => Console.WriteLine($"SessionId: {procInfo.SessionId}"));
                            t.Add(() => Console.WriteLine($"TotalProcessorTime: {procInfo.TotalProcessorTime}"));
                            t.Add(() => Console.WriteLine($"UserProcessorTime: {procInfo.UserProcessorTime}"));
                            t.Add(() => Console.WriteLine($"VirtualMemorySize64: {procInfo.VirtualMemorySize64}"));
                            t.Add(() => Console.WriteLine($"WorkingSet64: {procInfo.WorkingSet64}"));
                            foreach (Action tt in t)
                            {
                                try
                                {
                                    tt();
                                }
                                catch
                                {
                                    Console.WriteLine("Cannot get property: no access");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ErrorHandler.HandleError(ex);
                        }
                        break;
                    case "exit":
                        work = false;
                        break;
                    case "pause":
                        if (queryArray.Length < 2)
                        {
                            Log.Warning("Not enough arguments given.");
                            break;
                        }
                        int pid2 = 0;
                        try
                        {
                            pid2 = Convert.ToInt32(queryArray[1]);
                        }
                        catch
                        {
                            Log.Error("Cannot find process by pid");
                            break;
                        };
                        try
                        {
                            SuspendProcess(pid2);
                        }
                        catch (Exception ex)
                        {
                            ErrorHandler.HandleError(ex);
                        }
                        break;
                    case "resume":
                        if (queryArray.Length < 2)
                        {
                            Log.Warning("Not enough arguments given.");
                            break;
                        }
                        int pid3 = 0;
                        try
                        {
                            pid3 = Convert.ToInt32(queryArray[1]);
                        }
                        catch
                        {
                            Log.Error("Cannot find process by pid");
                            break;
                        }
                        try
                        {
                            ResumeProcess(pid3);
                        }
                        catch (Exception ex)
                        {
                            ErrorHandler.HandleError(ex);
                        }
                        break;
                }
            }
        }
        internal static void RegistryControl()
        {
            Console.Clear();
            bool work = true;
            string currentRegPath = null;
            RegistryKey currentTree = null;
            while (work)
            {
                Console.Write("(REG_CONTROL) >>> ");
                string query = Console.ReadLine();
                string[] splittedQuery = query.Split(' ');
                switch (splittedQuery[0])
                {
                    case "help":
                        Console.WriteLine("1. cd <key_name> - enter the section");
                        Console.WriteLine("2. dir - list of sections located in the current section");
                        Console.WriteLine("3. path - outputs current path");
                        Console.WriteLine("4. read - outputs values located in current key");
                        break;
                    case "dir":
                        if (currentTree == null)
                        {
                            Console.WriteLine();
                            Console.WriteLine("HKEY_CLASSES_ROOT   <tree>");
                            Console.WriteLine("HKEY_CURRENT_USER   <tree>");
                            Console.WriteLine("HKEY_LOCAL_MACHINE  <tree>");
                            Console.WriteLine("HKEY_USERS          <tree>");
                            Console.WriteLine("HKEY_CURRENT_CONFIG <tree>");
                            Console.WriteLine();
                        }
                        else
                        {
                            if (currentTree != null && currentRegPath == null)
                            {
                                string[] keys1 = currentTree.GetSubKeyNames();
                                int i1 = 1;
                                Console.WriteLine();
                                foreach (string key in keys1)
                                {
                                    Console.WriteLine($"[{i1}]: {key}");
                                    i1 += 1;
                                }
                                Console.WriteLine();
                                break;
                            }
                            RegistryKey reg = null;
                            try
                            {
                                reg = currentTree.OpenSubKey(currentRegPath, true);
                            }
                            catch (Exception ex)
                            {
                                ErrorHandler.HandleError(ex);
                                break;
                            }
                            if (reg == null)
                            {
                                Log.Error("Cannot open key");
                                break;
                            }
                            else
                            {
                                RegistryKey reg1 = currentTree.OpenSubKey(currentRegPath, true);
                                string[] keys2 = reg1.GetSubKeyNames();
                                int i2 = 1;
                                Console.WriteLine();
                                foreach (string key in keys2)
                                {
                                    Console.WriteLine($"[{i2}]: {key}");
                                    i2 += 1;
                                }
                                Console.WriteLine();
                            }
                        }
                        break;
                    case "cd":
                        if (splittedQuery.Length < 2)
                        {
                            break;
                        }
                        if (splittedQuery[1] == "..")
                        {
                            string[] path = currentRegPath.Split('\\');
                            string[] path1 = new string[path.Length - 1];
                            for (int i = 0; i < path1.Length; i++)
                                path1[i] = path[i];
                            currentRegPath = string.Join("\\", path1);
                            break;
                        }
                        if (currentTree == null)
                        {
                            string targetTree = splittedQuery[1].ToLower();
                            switch (targetTree)
                            {
                                case "hkey_classes_root":
                                    currentTree = Registry.ClassesRoot;
                                    Log.Success("Successfully opened key");
                                    break;
                                case "hkey_current_user":
                                    currentTree = Registry.CurrentUser;
                                    Log.Success("Successfully opened key");
                                    break;
                                case "hkey_local_machine":
                                    currentTree = Registry.LocalMachine;
                                    Log.Success("Successfully opened key");
                                    break;
                                case "hkey_users":
                                    currentTree = Registry.CurrentUser;
                                    Log.Success("Successfully opened key");
                                    break;
                                case "hkey_current_config":
                                    currentTree = Registry.CurrentConfig;
                                    Log.Success("Successfully opened key");
                                    break;
                            }
                            if (currentTree == null)
                            {
                                Log.Error("Target key does not exist!");
                                break;
                            }
                            break;
                        }
                        try
                        {
                            if (currentRegPath == null)
                            {
                                string[] keys2 = currentTree.GetSubKeyNames();
                                if (Array.IndexOf(keys2, splittedQuery[1]) != -1)
                                {
                                    currentRegPath = currentRegPath + splittedQuery[1];
                                }
                                else
                                {
                                    Log.Error("Target key not found");
                                }
                                break;
                            }
                            RegistryKey reg = currentTree.OpenSubKey(currentRegPath, true);
                            if (reg == null)
                            {
                                Log.Error("Cannot open key");
                                return;
                            }
                            string[] keys = reg.GetSubKeyNames();
                            if (Array.IndexOf(keys, splittedQuery[1]) == -1)
                            {
                                Log.Error("Target key does not exist!");
                                return;
                            }
                            currentRegPath = currentRegPath + "\\" + splittedQuery[1];
                            Log.Success("Successfully opened key.");
                        }
                        catch (Exception ex)
                        {
                            ErrorHandler.HandleError(ex);
                        }
                        break;
                    case "path":
                        Console.WriteLine($"Current tree: {currentTree}");
                        Console.WriteLine($"Current path: {currentRegPath}");
                        break;
                    case "read":
                        try
                        {
                            Console.WriteLine(currentTree.ToString());
                            RegistryKey reg = currentTree.OpenSubKey(currentRegPath, true);
                            string[] values = reg.GetValueNames();
                            Console.WriteLine();
                            Console.WriteLine($"Value list of key {reg.Name}");
                            Array.Sort(values);
                            int i = 1;
                            foreach (string value in values)
                            {
                                string data = null;
                                if (reg.GetValue(value).GetType() == typeof(string[]))
                                {
                                    foreach (string data1 in (string[])reg.GetValue(value))
                                        data = $"{data} {data1}";
                                    Console.WriteLine($"[{i}]: {value}, {reg.GetValueKind(value)}, {data}");
                                }
                                else
                                    Console.WriteLine($"[{i}]: {value}, {reg.GetValueKind(value)}, {reg.GetValue(value)}");
                                i += 1;
                            }
                            Console.WriteLine();
                        }
                        catch (Exception ex)
                        {
                            ErrorHandler.HandleError(ex);
                        }
                        break;
                    case "set":
                        if (splittedQuery.Length < 4)
                        {
                            Log.Error("Not enough arguments given");
                            break;
                        }
                        if (currentTree == null || currentRegPath == null)
                        {
                            Log.Error("Cannot set value");
                            break;
                        }
                        try
                        {
                            string name = splittedQuery[1];
                            if (string.IsNullOrEmpty(splittedQuery[2]))
                                return;
                            RegistryValueKind valueType;
                            if (!GetRegistryValueKindFromString(splittedQuery[2], out valueType))
                            {
                                Log.Error("Cannot convert value");
                                return;
                            }
                            object data2 = null;
                            switch (valueType)
                            {
                                case RegistryValueKind.String:
                                    data2 = (string)GetValueUsingKind(splittedQuery[3], valueType);
                                    break;
                                case RegistryValueKind.ExpandString:
                                    data2 = (string)GetValueUsingKind(splittedQuery[3], valueType);
                                    break;
                                case RegistryValueKind.Binary:
                                    data2 = (string)GetValueUsingKind(splittedQuery[3], valueType);
                                    break;
                                case RegistryValueKind.DWord:
                                    data2 = (UInt32)GetValueUsingKind(splittedQuery[3], valueType);
                                    break;
                                case RegistryValueKind.MultiString:
                                    data2 = (string[])GetValueUsingKind(splittedQuery[3], valueType);
                                    break;
                                case RegistryValueKind.QWord:
                                    data2 = (UInt64)GetValueUsingKind(splittedQuery[3], valueType);
                                    break;
                            }
                            if (data2 == null)
                            {
                                Log.Error("Cannot define RegistryValueKind");
                                return;
                            }
                            RegistryKey key2;
                            try
                            {
                                key2 = currentTree.OpenSubKey(currentRegPath, true);
                            }
                            catch (Exception ex)
                            {
                                ErrorHandler.HandleError(ex);
                                break;
                            }
                            key2.SetValue(splittedQuery[1], data2);
                            if (key2.GetValue(splittedQuery[1]) == data2)
                                Log.Success("Successfully set value");
                            else
                                Log.Error("Cannot set value");
                        }
                        catch (Exception ex)
                        {
                            ErrorHandler.HandleError(ex);
                        }
                        break;
                    case "exit":
                        work = false;
                        return;
                }
            }
        }
        internal static void VirtualDiskControl()
        {
            Console.Clear();
            bool work = true;
            VHDSubSystemModes curMode = VHDSubSystemModes.None;
            while (work)
            {
                Console.Write("(VHD_CONTROL) >>> ");
                string query = Console.ReadLine();
                string[] splittedQuery = query.Split(' ');
                switch (splittedQuery[0])
                {
                    case "help":
                        Console.WriteLine("mode <mode_name> - select virtual disk type to work with");
                        break;
                    case "mode":
                        if (splittedQuery.Length < 2)
                        {
                            Console.WriteLine("Current mode is " + curMode.ToString());
                            break;
                        }
                        if (splittedQuery[1].ToLower() == "help")
                        {
                            Console.WriteLine("wim - work with wim images");
                            Console.WriteLine("iso - work with iso images");
                            break;
                        }
                        switch (splittedQuery[1].ToLower())
                        {
                            case "iso":
                                curMode = VHDSubSystemModes.ISO;
                                break;
                            case "wim":
                                curMode = VHDSubSystemModes.WIM;
                                break;
                        }
                        break;
                    case "test":
                        VHDBuilder(curMode);
                        break;
                    case "read":
                        CDReader reader = new CDReader(System.IO.File.Open("C:\\test.iso", FileMode.Open), true);
                        foreach (var file in reader.GetFiles("test"))
                            Console.WriteLine(file);
                        break;
                }
            }
        }
        internal static void FileManager()
        {
            Console.Clear();
            Console.WriteLine("WTK File Manager\n");
            while (true)
            {
                Console.Write(Program.currentPath + ">");
                string query = Console.ReadLine();
                string[] splittedQuery = query.Split(' ');
                switch (splittedQuery[0])
                {
                    case "cd":
                        if (splittedQuery.Length < 2)
                        {
                            Log.Error("Not enough arguments!");
                            break;
                        }
                        if (splittedQuery[1] == "..")
                        {
                            Program.currentPath = new DirectoryInfo(Program.currentPath).Parent.FullName;
                            Console.Write("\n");
                            break;
                        }
                        if (query.Contains("\""))
                        {
                            string clonedquery = query;
                            int startindex = clonedquery.IndexOf('"');
                            clonedquery = clonedquery.Remove(startindex, 1);
                            int endindex = clonedquery.IndexOf('"');
                            clonedquery = clonedquery.Remove(endindex, 1);
                            if (clonedquery.Contains('"'))
                            {
                                Log.Error("Invalid path!");
                                break;
                            }
                            string path = clonedquery.Substring(startindex, endindex - startindex);
                            if (System.IO.File.Exists(path) || System.IO.Directory.Exists(path))
                            {
                                if (Path.IsPathRooted(path))
                                {
                                    Program.currentPath = path;
                                    break;
                                }
                            }
                            string[] subpaths = null;
                            if (path.Contains("\\"))
                            {
                                subpaths = path.Split('\\');
                                if (subpaths.Length < 1)
                                {
                                    Log.Error("Invalid path!");
                                    break;
                                }
                                foreach (string subpath in subpaths)
                                {
                                    if (Directory.Exists(Program.currentPath + "\\" + subpath))
                                        Program.currentPath = Program.currentPath + "\\" + subpath;
                                    else
                                    {
                                        Log.Error("Directory " + subpath + "does not exist");
                                        break;
                                    }
                                }
                                Console.Write("\n");
                            }
                            else
                            {
                                if (!Directory.Exists(Program.currentPath + "\\" + path))
                                {
                                    Log.Error("Invalid path!");
                                    break;
                                }
                                Program.currentPath = Program.currentPath + "\\" + path;
                            }
                        }
                        else
                        {
                            if (!Directory.Exists(Program.currentPath + "\\" + splittedQuery[1]))
                            {
                                Log.Error("Invalid path!");
                                break;
                            }
                            Program.currentPath = Program.currentPath + "\\" + splittedQuery[1];
                            Console.Write("\n");
                        }
                        break;
                    case "info":
                        if (splittedQuery.Length < 2)
                        {
                            Log.Error("Got less than two arguments.");
                            return;
                        }
                        string cquery = splittedQuery[1];
                        if (splittedQuery[1].Contains("\""))
                        {
                            string ccquery = query;
                            int findex = ccquery.IndexOf('"');
                            ccquery = ccquery.Remove(findex, 1);
                            int lindex = ccquery.IndexOf('"');
                            ccquery = ccquery.Remove(lindex, 1);
                            cquery = ccquery.Substring(findex, lindex - findex);
                            while (cquery.Contains("\""))
                                cquery = cquery.Remove(cquery.IndexOf('"'), 1);
                            string testpath = "";
                            if (!Path.IsPathRooted(cquery))
                            {
                                testpath = Program.currentPath + "\\" + cquery;
                                if (!System.IO.File.Exists(testpath) && !Directory.Exists(testpath))
                                {
                                    Log.Error("Cannot find file!");
                                    return;
                                }
                                else
                                    cquery = testpath;
                            }
                        }
                        else
                            if (!Path.IsPathRooted(cquery))
                                cquery = Program.currentPath + "\\" + cquery;
                        if (System.IO.File.Exists(cquery) && !Directory.Exists(cquery))
                        {
                            FileInfo fi = new FileInfo(cquery);
                            Console.WriteLine("Info about file " + fi.Name + "\n");
                            Console.WriteLine("Attributes: " + fi.Attributes.ToString());
                            Console.WriteLine("Creation time: " + fi.CreationTime.ToString());
                            Console.WriteLine("Extension: " + fi.Extension);
                            Console.WriteLine("Full path: " + fi.FullName);
                            Console.WriteLine("Is ReadOnly: " + fi.IsReadOnly.ToString());
                            Console.WriteLine("Last access time: " + fi.LastAccessTime.ToString());
                            Console.WriteLine("Last write time: " + fi.LastWriteTime.ToString());
                            if (fi.Length < Tools.KB)
                                Console.WriteLine("File size: " + fi.Length + " B\n");
                            else if (fi.Length < Tools.MB)
                                Console.WriteLine("File size: " + (int)(fi.Length) + " KB\n");
                            else if (fi.Length < Tools.GB)
                                Console.WriteLine("File size: " + (fi.Length / Tools.MB) + "." + (fi.Length / Tools.KB).ToString().Substring((int)(fi.Length / Tools.MB).ToString().Length, 1) + " MB\n");
                            else if (fi.Length < Tools.TB)
                                Console.WriteLine("File size: " + (fi.Length / Tools.GB) + "." + (fi.Length / Tools.MB).ToString().Substring((int)(fi.Length / Tools.GB).ToString().Length, 1) + " GB\n");
                        }
                        else if (!System.IO.File.Exists(cquery) && Directory.Exists(cquery))
                        {
                            DirectoryInfo di = new DirectoryInfo(cquery);
                            Console.WriteLine("Info about " + cquery + "\n");
                            Console.WriteLine("Attributes: " + di.Attributes.ToString());
                            Console.WriteLine("Creation time: " + di.CreationTime.ToString());
                            Console.WriteLine("Full path: " + di.FullName);
                            Console.WriteLine("Last access time: " + di.LastAccessTime.ToString());
                            Console.WriteLine("Last write time: " + di.LastWriteTime.ToString() + "\n");
                        }
                        else
                        {
                            Log.Error("Cannot find file or folder");
                        }
                        break;
                    case "dir":
                        long totalBytes = 0;
                        int totalFiles = 0;
                        int totalFolders = 0;
                        List<string> strings = new List<string>();
                        foreach (var file in Directory.GetFiles(Program.currentPath))
                        {
                            FileInfo fi = new FileInfo(file);
                            totalBytes += fi.Length;
                            totalFiles++;
                            strings.Add($"{fi.CreationTime.Date.ToString().Substring(0, 10)}  {fi.CreationTime.TimeOfDay.ToString().Substring(0, 8)}                   {fi.Name}");
                        }
                        foreach (var directory in Directory.GetDirectories(Program.currentPath))
                        {
                            DirectoryInfo di = new DirectoryInfo(directory);
                            totalFolders++;
                            strings.Add($"{di.CreationTime.Date.ToString().Substring(0, 10)}  {di.CreationTime.TimeOfDay.ToString().Substring(0, 8)}    <DIR>          {di.Name}");
                        }
                        Console.Write("\n");
                        foreach (var line in strings)
                        {
                            Console.WriteLine(line);
                        }
                        Console.WriteLine(new string(' ', 16 - totalFiles.ToString().Length) + totalFiles + " files, " + totalBytes + " bytes");
                        Console.WriteLine(new string(' ', 16 - totalFolders.ToString().Length) + totalFolders + " folders\n");
                        break;
                    case "delete":
                        if (Program.request.Count < 2)
                        {
                            Log.Warning("Not enough arguments given.");
                            return;
                        }
                        for (int i = 0; i < Program.request.Count; i++)
                        {
                            Program.request[i] = Program.request[i].Trim('"');
                        }
                        if (System.IO.File.Exists(Program.request[1]))
                        {
                            if (!Tools.TryToRemoveFile(Program.request[1]))
                                Log.Error("Cannot remove file.");
                            else
                                Log.Success("Remove successful.");
                        }
                        else
                        {
                            bool needToRemove;
                            if (!Directory.Exists(Program.request[1]))
                            {
                                Log.Error("Object does not exist");
                                needToRemove = false;
                                return;
                            }
                            else { needToRemove = true; }
                            if (needToRemove)
                            {
                                if (!Tools.TryToRemoveDirectory(Program.request[1]))
                                    Log.Error("Cannot remove directory.");
                                else
                                if (!Directory.Exists(Program.request[1]))
                                    Log.Success("Remove successful.");
                            }
                        }
                        break;
                }
            }
        }
        internal static void VHDBuilder(VHDSubSystemModes mode)
        {
            Console.Clear();
            CDBuilder builder = new CDBuilder();
            Console.Write("Enter path for ISO saving: ");
            string path = Console.ReadLine();
            Console.Write("Use Joliet (true/false): ");
            bool useJoliet = false;
            while (!bool.TryParse(Console.ReadLine(), out useJoliet))
                Console.Write("Use Joliet (true/false): ");
            builder.UseJoliet = useJoliet;
            Console.Write("Select drive label: ");
            string driveLabel;
            driveLabel = Console.ReadLine();
            builder.VolumeIdentifier = driveLabel;
            string obj = "";
            Console.WriteLine("Now you can add files and folders to ISO image. Enter \"continue\" to build the ISO.");
            while (obj != "continue")
            {
                Console.Write(">>> ");
                obj = Console.ReadLine();
                if (Directory.Exists(obj))
                    builder.AddDirectory(obj);
                else if (System.IO.File.Exists(obj))
                    builder.AddFile(obj, System.IO.File.ReadAllBytes(obj));
                else
                    Log.Error("Cannot define object type");
            }
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
            Stream stream = System.IO.File.Open(path, FileMode.OpenOrCreate);
            Console.WriteLine("Building...");
            builder.Build(stream);
            stream.Close();
            Log.Success("Build successfully finished");
            return;
        }
        internal static void WriteManifestResource(string manifestResourceName, string targetFilePath)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            var stream = asm.GetManifestResourceStream("WindowsToolKit.AdditionalFiles." + manifestResourceName);
            var targetStream = System.IO.File.Open(targetFilePath, FileMode.Create);
            var br = new BinaryReader(stream);
            var bw = new BinaryWriter(targetStream);
            long offset = 0;
            while (stream.Length - offset > 1024)
            {
                bw.Write(br.ReadBytes(1024));
                offset += 1024;
            }
            bw.Write(br.ReadBytes((int)(stream.Length - offset)));
            bw.Dispose(); bw.Close();
            br.Dispose(); br.Close();
        }
        internal static bool BackupMBR(int physicalDriveNumber, string filePath, int byteCount)
        {
            SafeFileHandle handle = CreateFile(
                $@"\\.\PhysicalDrive{physicalDriveNumber}",
                FileAccess.ReadWrite,
                FileShare.ReadWrite,
                IntPtr.Zero,
                FileMode.Open,
                FileAttributes.Normal,
                IntPtr.Zero);
            if (handle.IsInvalid)
                return false;
            long byteOffset = 0;
            SetFilePointerEx(handle, byteOffset, out _, SeekOrigin.Begin);
            byte[] mbr = new byte[byteCount];
            int bytesRead = 0;
            if (!ReadFile(handle, mbr, mbr.Length, out bytesRead, IntPtr.Zero))
            {
                handle.Close();
                return false;
            }
            else
            {
                handle.Close();
                System.IO.File.Create(filePath).Close();
                System.IO.File.WriteAllBytes(filePath, mbr);
                return true;
            }
        }
        internal static bool FlashMBR(int physicalDriveNumber, string filePath)
        {
            SafeFileHandle handle = CreateFile(
                $@"\\.\PhysicalDrive{physicalDriveNumber}",
                FileAccess.ReadWrite,
                FileShare.ReadWrite,
                IntPtr.Zero,
                FileMode.Open,
                FileAttributes.Normal,
                IntPtr.Zero);
            if (handle.IsInvalid)
                return false;
            long byteOffset = 0;
            SetFilePointerEx(handle, byteOffset, out _, SeekOrigin.Begin);
            byte[] mbr = System.IO.File.ReadAllBytes(filePath);
            if (!WriteFile(handle, mbr, mbr.Length, out _, IntPtr.Zero))
            {
                handle.Close();
                return false;
            }
            else
            {
                handle.Close();
                return true;
            }
        }
        internal static void SVCBackup(string targetFilename)
        {
            ServiceController[] services = ServiceController.GetServices();
            StreamWriter writer;
            try
            {
                writer = new StreamWriter(System.IO.File.Open(targetFilename, FileMode.OpenOrCreate));
            }
            catch
            {
                Log.Error("Cannot open file");
                return;
            }
            foreach (ServiceController service in services)
            {
                writer.WriteLine(service.DisplayName);
                writer.WriteLine($"{service.StartType.ToString()} {service.Status.ToString()} {service.ServiceName}");
            }
            if (System.IO.File.Exists(targetFilename))
                Log.Success("Backup successfully completed");
            writer.Dispose();
            services = null;
        }
        internal static void SVCRestore(string targetFilename)
        {

        }
        internal static ManagementObject[] GetPartitions(string driveLetter)
        {
            ObjectQuery query = new ObjectQuery("ASSOCIATORS OF {Win32_LogicalDisk.DeviceID='" + driveLetter + "'} WHERE AssocClass = Win32_LogicalDiskToPartition");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            return searcher.Get().OfType<ManagementObject>().ToArray();
        }
        internal static void EjectPartition(ManagementObject partition)
        {
            ObjectQuery query = new ObjectQuery("ASSOCIATORS OF {Win32_DiskPartition.DeviceID='" + partition["DeviceID"] + "'} WHERE AssocClass = Win32_DiskDriveToDiskPartition");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObject[] DiskDrives = searcher.Get().OfType<ManagementObject>().ToArray();
            if (DiskDrives.Length > 0)
            {
                ManagementObject drive = DiskDrives.First();
                string driveLetter = (string)drive["DeviceID"];
                Process process = new Process();
                process.StartInfo.FileName = "mountvol";
                process.StartInfo.Arguments = driveLetter + " /D";
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                process.WaitForExit();
            }
        }
        internal static void PathEditor()
        {
            Console.Clear();
            Console.WriteLine("Path Editor");
            Console.WriteLine("Type \"help\" to see commands list");
            List<string> strs = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User).Split(';').ToList();
            while (true)
            {
                Console.Write(">>> ");
                string query = Console.ReadLine();
                string[] splittedQuery = query.Split(' ');
                switch (splittedQuery[0])
                {
                    case "help":
                        Console.WriteLine("\nread - reads current value of PATH variable");
                        Console.WriteLine("remove <index> - removes specified string from variable");
                        Console.WriteLine("add - adds new string to variable");
                        break;
                    case "read":
                        Console.Write("\n");
                        for (int i = 1; i < strs.Count; i++)
                            Console.WriteLine($"{i}: {strs[i - 1]}");
                        Console.Write("\n");
                        break;
                    case "remove":
                        if (splittedQuery.Length < 2)
                        {
                            Log.Error("Got less than one argument.");
                            break;
                        }
                        int index = 0;
                        if (!int.TryParse(splittedQuery[1], out index))
                        {
                            Log.Error("Not a number!");
                            return;
                        }
                        if (index < 0 || index > strs.Count - 1)
                        {
                            Log.Error("Invalid index!");
                            return;
                        }
                        strs.RemoveAt(index - 1);
                        Console.WriteLine("\nNew value: \n");
                        for (int i = 1; i < strs.Count; i++)
                            Console.WriteLine($"{i}: {strs[i - 1]}");
                        Console.Write("\n");
                        break;
                    case "save":
                        try
                        {
                            Environment.SetEnvironmentVariable("PATH", null, EnvironmentVariableTarget.User);
                            Environment.SetEnvironmentVariable("PATH", string.Join(";", strs), EnvironmentVariableTarget.User);
                        } catch (Exception ex)
                        {
                            ErrorHandler.HandleError(ex);
                        }
                        break;
                }
            }
        }
    }
}
