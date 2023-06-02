
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using WindowsToolKit.ISO9660;

namespace WindowsToolKit
{
    internal static class Tools
    {
        internal enum UninstallerModes
        {
            WinDefend
        }
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
            Console.WriteLine("Service control subsystem.");
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
            bool valSaved = false;
            while (!valSaved)
            {
                if (!bool.TryParse(Console.ReadLine(), out useJoliet))
                {
                    ClearLastLine();
                    Log.Error("Cannot parse value");
                    Thread.Sleep(2000);
                    ClearLastLine();
                    Console.Write("Use Joliet (true/false): ");
                }
                else
                {
                    valSaved = true;
                }
            }
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
            Log.Success("Build successfully finished");
            return;
        }
        internal static bool BackupMBR(int physicalDriveNumber, string filePath)
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
            byte[] mbr = new byte[512];
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
        internal static List<Action> GetActionsList(UninstallerModes mode)
        {
            List<Action> actions = new List<Action>();
            switch (mode)
            {
                case UninstallerModes.WinDefend:
                    actions.Add(() =>
                    {
                        Log.Warning("Setting MsSecCore to (dword)0x00000004");
                        Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\FVEMsSecCore", true).SetValue("Start", 4, RegistryValueKind.DWord);
                        Tools.ClearLastLine(); 
                    });
                    actions.Add(() =>
                    {
                        Log.Warning("Setting wscsvc to (dword)0x00000004");
                        Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\wscsvc", true).SetValue("Start", 4, RegistryValueKind.DWord);
                        Tools.ClearLastLine(); 
                    });
                    actions.Add(() =>
                    {
                        Log.Warning("Setting WdNisDrv to (dword)0x00000004");
                        Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\WdNisDrv", true).SetValue("Start", 4, RegistryValueKind.DWord);
                        Tools.ClearLastLine(); 
                    });
                    actions.Add(() =>
                    {
                        Log.Warning("Setting WdNisSvc to (dword)0x00000004");
                        Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\WdNisSvc", true).SetValue("Start", 4, RegistryValueKind.DWord);
                        Tools.ClearLastLine();
                    });
                    actions.Add(() =>
                    {
                        Log.Warning("Setting WdFiltrer to (dword)0x00000004");
                        Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\WdFiltrer", true).SetValue("Start", 4, RegistryValueKind.DWord);
                        Tools.ClearLastLine();
                    });
                    actions.Add(() =>
                    {
                        Log.Warning("Setting WdBoot to (dword)0x00000004");
                        Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\WdBoot", true).SetValue("Start", 4, RegistryValueKind.DWord);
                        Tools.ClearLastLine();
                    });
                    actions.Add(() =>
                    {
                        Log.Warning("Setting SecurityHealthService to (dword)0x00000004");
                        Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\SecurityHealthService", true).SetValue("Start", 4, RegistryValueKind.DWord);
                        Tools.ClearLastLine();
                    });
                    actions.Add(() =>
                    {
                        Log.Warning("Setting SrgmAgent to (dword)0x00000004");
                        Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\SrgmAgent", true).SetValue("Start", 4, RegistryValueKind.DWord);
                        Tools.ClearLastLine();
                    });
                    actions.Add(() =>
                    {
                        Log.Warning("Setting SgrmBroker to (dword)0x00000004");
                        Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\SgrmBroker", true).SetValue("Start", 4, RegistryValueKind.DWord);
                        Tools.ClearLastLine();
                    });
                    actions.Add(() =>
                    {
                        Log.Warning("Setting WinDefend to (dword)0x00000004");
                        Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\WinDefend", true).SetValue("Start", 4, RegistryValueKind.DWord);
                        Tools.ClearLastLine();
                    });
                    actions.Add(() => Log.Warning("Disabling WinDefender WMI Logger..."));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\WMI\\Autologger\\DefenderApiLogger", true).SetValue("Start", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\WMI\\Autologger\\DefenderApiLogger", true).SetValue("Status", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\WMI\\Autologger\\DefenderAuditLogger", true).SetValue("Start", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\WMI\\Autologger\\DefenderAuditLogger", true).SetValue("Status", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\WMI\\Autologger\\DefenderAuditLogger", true).SetValue("EnableSecurityProvider", 0, RegistryValueKind.DWord));
                    actions.Add(() => Tools.ClearLastLine());
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SYSTEM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true).DeleteValue("SecurityHealth"));
                    actions.Add(() => Log.Warning("Disabling Tamper Protection..."));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows Defender\\Features", true).SetValue("MbPlatformKillbitsFromEngine", new byte[8] {0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows Defender\\Features", true).SetValue("TamperProtectionSource", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows Defender\\Features", true).SetValue("MpCapability", new byte[8] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows Defender\\Features", true).SetValue("TamperProtection", 0, RegistryValueKind.DWord));
                    actions.Add(() => Console.WriteLine("Disabling Anti-Phishing system..."));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\PolicyManager\\default\\WebThreadDefense\\AuditMode", true).SetValue("value", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\PolicyManager\\default\\WebThreatDefense\\NotifyUnsafeOrReusedPassword", true).SetValue("value", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\PolicyManager\\default\\WebThreatDefense\\ServiceEnabled", true).SetValue("value", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\webthreatdefsvc", true).SetValue("Start", 4, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\webthreatdefusersvc", true).SetValue("Start", 4, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SYSTEM\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVertion\\Svchost\\WebThreadDefense", true).SetValue("Start", 4, RegistryValueKind.DWord));
                    actions.Add(() => Console.WriteLine("Disabling Security Health..."));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows Security Health\\State", true).SetValue("Disabled", 1, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows Security Health\\Platform", true).SetValue("Registered", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows Security Health\\Health Advisor", true).SetValue("UIReportingDisabled", 1, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows Security Health\\Health Advisor", true).SetValue("WarningThreshold", null));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows Security Health\\Health Advisor\\Battery", true).SetValue("UIReportingDisabled", 1, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows Security Health\\Health Advisor\\Battery", true).SetValue("WarningThreshold", null));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows Security Health\\Health Advisor\\Device Driver", true).SetValue("UIReportingDisabled", 1, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows Security Health\\Health Advisor\\Device Driver", true).SetValue("WarningThreshold", null));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows Security Health\\Health Advisor\\Reliability", true).SetValue("UIReportingDisabled", 1, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows Security Health\\Health Advisor\\Reliability", true).SetValue("WarningThreshold", null));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows Security Health\\Health Advisor\\Status Codes", true).SetValue("UIReportingDisabled", 1, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows Security Health\\Health Advisor\\Status Codes", true).SetValue("WarningThreshold", null));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows Security Health\\Health Advisor\\Storage Health", true).SetValue("UIReportingDisabled", 1, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows Security Health\\Health Advisor\\Storage Health", true).SetValue("WarningThreshold", null));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows Security Health\\Health Advisor\\Storage Health Metrics", true).SetValue("UIReportingDisabled", 1, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows Security Health\\Health Advisor\\Storage Health Metrics", true).SetValue("WarningThreshold", null));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows Security Health\\Health Advisor\\Time Service", true).SetValue("UIReportingDisabled", 1, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows Security Health\\Health Advisor\\Time Service", true).SetValue("WarningThreshold", null));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows Security Health\\Update Monitor").SetValue("DiagnosticsInterval", null));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows Security Health\\Update Monitor").SetValue("MaxDaysOnOSVersion", null));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows Security Health\\Update Monitor").SetValue("UIReportingDisabled", 1, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer").SetValue("TurnOffSPIAnimations", 1, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer").SetValue("NoWelcomeScreen", 1, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer").SetValue("EnforceShellExtensionSecurity", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\PolicyManager\\default\\Security\\AntiTheftMode").SetValue("value", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\PolicyManager\\default\\Security\\ClearTPMIfNotReady").SetValue("value", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\PolicyManager\\default\\Security\\PreventAutomaticDeviceEncryptionForAzureADJoinedDevices").SetValue("value", 1, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\PolicyManager\\default\\Security\\RecoveryEnvironmentAuthentification").SetValue("value", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\PolicyManager\\default\\Security\\RequireDeviceEncryption").SetValue("value", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\PolicyManager\\default\\Security\\RequireProvisioningPackageSignature").SetValue("value", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\PolicyManager\\default\\Security\\RequireRetrieveHealthCertificateOnBoot").SetValue("value", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows Security Health\\State").SetValue("Disabled", 1, RegistryValueKind.DWord));
                    actions.Add(() => Log.Warning("Disabling SmartScreen..."));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\AppHost").SetValue("EnableWebContentEvaluation", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\AppHost").SetValue("PreventOverride", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\PolicyManager\\default\\Browser\\AllowSmartScreen").SetValue("value", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\PolicyManager\\default\\SmartScreen\\EnableSmartScreenInShell").SetValue("value", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\PolicyManager\\default\\SmartScreen\\EnableAppInstallControl").SetValue("value", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Classes\\Local Settings\\Software\\Microsoft\\Windows\\CurrentVersion\\AppContainer\\Storage\\microsoft.microsoftedge_8wekyb3d8bbwe\\MicrosoftEdge\\PhishingFilter").SetValue("EnabledV9", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Classes\\Local Settings\\Software\\Microsoft\\Windows\\CurrentVersion\\AppContainer\\Storage\\microsoft.microsoftedge_8wekyb3d8bbwe\\MicrosoftEdge\\PhishingFilter").SetValue("PreventOverride", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\PolicyManager\\default\\SmartScreen\\PreventOverrideForFilesInShell").SetValue("value", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Edge\\SmartScreenEnabled").SetValue("(Default)", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\Windows Defender\\SmartScreen").SetValue("ConfigureAppInstallControl", "Anywhere"));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\Windows Defender\\SmartScreen").SetValue("ConfigureAppInstallControlEnabled", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer").SetValue("SmartScreenEnabled", "off"));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\Windows\\System").SetValue("EnableSmartScreen", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Edge\\SmartScreenEnabled").SetValue("@", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\Windows Defender\\SmartScreen").SetValue("ConfigureAppInstallControlEnabled", 1, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\Windows Defender\\SmartScreen").SetValue("ConfigureAppInstallControl", "Anywhere"));
                    actions.Add(() => Log.Warning("Disabling virtualization..."));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\FVE").SetValue("DisableExternalDMAUnderLock", 1, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\FVE").SetValue("OSRecovery", 1, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\FVE").SetValue("OSManageDRA", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\FVE").SetValue("OSRecoveryPassword", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\FVE").SetValue("OSRecoveryKey", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\FVE").SetValue("OSHideRecoveryPage", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\FVE").SetValue("OSActiveDirectoryBackup", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\FVE").SetValue("OSActiveDirectoryInfoToStore", 2, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\FVE").SetValue("OSRequireActiveDirectoryBackup", 1, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\FVE").SetValue("ActiveDirectoryInfoToStore", 2, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\FVE").SetValue("MorBehavior", 1, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\FVE").SetValue("RDVConfigureBDE", 1, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\FVE").SetValue("RDVAllowBDE", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\FVE").SetValue("RDVDisableBDE", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\FVE").SetValue("RDVAllowUserCert", 1, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\FVE").SetValue("RDVEnforceUserCert", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\FVE").SetValue("RDVDenyCrossOrg", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\FVE").SetValue("OSAllowSecureBootForIntegrity", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\FVE").SetValue("OSManageNKP", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\FVE").SetValue("OSHardwareEncryption", 1, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\FVE").SetValue("OSAllowSoftwareEncryptionFailover", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\FVE").SetValue("OSRestrictHardwareEncryptionAlgorithms", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\FVE").SetValue("OSAllowedHardwareEncryptionAlgorithms", new byte[94] { 0x32, 0x00, 0x2E, 0x00, 0x31, 0x00, 0x36, 0x00, 0x2E, 0x00, 0x38, 0x00, 0x34, 0x00, 0x30, 0x00, 0x2E, 0x00, 0x31, 0x00, 0x2E, 0x00, 0x31, 0x00, 0x30, 0x00, 0x31, 0x00, 0x2E, 0x00, 0x33, 0x00, 0x2E, 0x00, 0x34, 0x00, 0x2E, 0x00, 0x31, 0x00, 0x2E, 0x00, 0x32, 0x00, 0x3B, 0x00, 0x32, 0x00, 0x2E, 0x00, 0x31, 0x00, 0x36, 0x00, 0x2E, 0x00, 0x38, 0x00, 0x34, 0x00, 0x30, 0x00, 0x2E, 0x00, 0x31, 0x00, 0x2E, 0x00, 0x31, 0x00, 0x30, 0x00, 0x31, 0x00, 0x2E, 0x00, 0x33, 0x00, 0x2E, 0x00, 0x34, 0x00, 0x2E, 0x00, 0x31, 0x00, 0x2E, 0x00, 0x34, 0x00, 0x32, 0x00, 0x00, 0x00}));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\FVE").SetValue("OSEnablePrebootInputProtectorsOnSlates", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\FVE").SetValue("UseAdvancedStartup", 1, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\FVE").SetValue("EnableBDEWithNoTPM", 1, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\FVE").SetValue("UseTPM", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\FVE").SetValue("UseTPMPIN", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\FVE").SetValue("UseTPMKey", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\FVE").SetValue("UseTPMKeyPIN", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\FVE").SetValue("EnableNonTPM", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\FVE").SetValue("UsePartialEncryptionKey", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\FVE").SetValue("UsePIN", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\FVE").SetValue("TPMAutoReseal", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\FVE").SetValue("FDVDiscoveryVolumeType", "<none>"));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\FVE").SetValue("FDVNoBitLockerToGoReader", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\FVE\\OSPlatformValidation_BIOS").SetValue("Enabled", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\FVE\\OSPlatformValidation_UEFI").SetValue("Enabled", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\FVE\\PlatformValidation").SetValue("Enabled", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\Windows\\DeviceGuard").SetValue("DeployConfigCIPolicy", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\Windows\\DeviceGuard").SetValue("EnableVirtualizationBasedSecurity", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\Windows\\DeviceGuard").SetValue("HVCIMATRequired", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\Windows\\DeviceGuard").SetValue("RequirePlatformSecurityFeature", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\Windows\\DeviceGuard").SetValue("CachedDrtmAuthIndex", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\Windows\\DeviceGuard").SetValue("RequireMicrosoftSignedBootChain", 0, RegistryValueKind.DWord));
                    actions.Add(() => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\Windows\\DeviceGuard").SetValue("DeployConfigCIPolicy", 0, RegistryValueKind.DWord));

                    break;
            }
            return actions;
        }
    }
}
