using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Text;
using System.Threading;
using WindowsToolKit.ISO9660;
using WindowsToolKit.Streams;
using static System.Net.WebRequestMethods;

namespace WindowsToolKit
{
    internal static class QueryHandlerMethods
    {
        internal static void StartCommandPrompt()
        {
            try
            {
                Program.log.LogInfo("Initializing command prompt");
                var cmd = new ProcessStartInfo("cmd.exe");
                cmd.CreateNoWindow = false;
                cmd.RedirectStandardError = true;
                cmd.RedirectStandardInput = true;
                cmd.RedirectStandardOutput = true;
                cmd.UseShellExecute = false;
                var process = new Process { StartInfo = cmd };
                process.OutputDataReceived += OutputDataReceived;
                process.ErrorDataReceived += ErrorDataReceived;
                process.Start();
                Program.log.LogInfo("Initialized command prompt");
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                Program.log.LogInfo("Started redirection");
                bool work = true;
                while (work)
                {
                    string query = Console.ReadLine();
                    Program.log.LogInfo($"Catched query {query}");
                    if (query == "exit")
                    {
                        process.OutputDataReceived -= OutputDataReceived;
                        process.ErrorDataReceived -= ErrorDataReceived;
                        process.StandardInput.WriteLine("exit");
                        Program.log.LogInfo("Exiting from command prompt...");
                        work = false;
                    }
                    else if (query.ToLower() == "wtk")
                    {
                        Program.currentMode = "WTK";
                        work = !work;
                        process.OutputDataReceived -= OutputDataReceived;
                        process.ErrorDataReceived -= ErrorDataReceived;
                        process.StandardInput.WriteLine("exit");
                        Program.log.LogInfo("Exiting from command prompt...");
                        break;
                    }
                    process.StandardInput.WriteLine(query);
                } //Main command perform cycle
                process.Close();
                return;
            }
            catch (Exception ex)
            {
                Program.log.LogError("Catched error: " + ex.Message);
                ErrorHandler.HandleError(ex);
            } //Command prompt command redirect module
        }
        internal static void StartPowershell()
        {
            try
            {
                string[] temp = Program.requestStr.Split();
                List<string> temp2 = new List<string>(temp.Length - 1);
                for (int i = 1; i < temp.Length; i++)
                    temp2[i - 1] = temp[i];
                string args = string.Join(" ", temp2.ToArray());
                Program.log.LogInfo("Initializing powershell");
                var PS = new ProcessStartInfo("powershell");
                PS.CreateNoWindow = false;
                PS.RedirectStandardError = true;
                PS.RedirectStandardInput = true;
                PS.RedirectStandardOutput = true;
                PS.UseShellExecute = false;
                var process = new Process { StartInfo = PS };
                process.OutputDataReceived += OutputDataReceived;
                process.ErrorDataReceived += ErrorDataReceived;
                process.Start();
                Program.log.LogInfo("Initialized powershell");
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                Program.log.LogInfo("Initialized redirect");
                bool work = true;
                while (work)
                {
                    string query = Console.ReadLine();
                    Program.log.LogInfo("Catched query " + query);
                    if (query == "exit")
                    {
                        process.OutputDataReceived -= OutputDataReceived;
                        process.ErrorDataReceived -= ErrorDataReceived;
                        process.StandardInput.WriteLine("exit");
                        Program.log.LogInfo("Exiting from powershell");
                        work = false;
                    }
                    else if (query.ToLower() == "wtk")
                    {
                        Program.currentMode = "WTK";
                        work = !work;
                        process.OutputDataReceived -= OutputDataReceived;
                        process.ErrorDataReceived -= ErrorDataReceived;
                        process.StandardInput.WriteLine("exit");
                        Program.log.LogInfo("Exiting from powershell");
                        break;
                    }
                    process.StandardInput.WriteLine(query);
                }
                process.Close();
                return;
            }
            catch (Exception ex)
            {
                Program.log.LogError("Catched error: " + ex.Message);
                ErrorHandler.HandleError(ex);
            } //Powershell command redirect module
        }
        internal static void Help()
        {
            Console.WriteLine("\n[HELP]");
            Console.WriteLine("1. help - displays current screen");
            Console.WriteLine("2. filemove <source_file> <destination_file> - moves file to the destination path");
            Console.WriteLine("3. filecopy <source_file> <destination_file> - copies file to the destination path");
            Console.WriteLine("4. mkdir <target_folder> - creates folder");
            Console.WriteLine("5. foldermove <source_folder> <destination_folder> - moves folder to the destination path with name");
            Console.WriteLine("6. foldercopy <source_folder> <destination_folder> - copies folder to the destination path with name");
            Console.WriteLine("7. clear - clears the screen");
            Console.WriteLine("8. delete <file_or_folder> - deletes object");
            Console.WriteLine("9. listpdrives - list of physical drives");
            Console.WriteLine("10. hex <file> - view file in hex");
            Console.WriteLine("11. backupmbr <physical_drive_number> <file> <bytecount> - backups mbr from selected drive to specified file");
            Console.WriteLine("12. flashmbr <physical_drive_number> <file> - flashes mbr to selected drive from specified file");
            Console.WriteLine("13. hardscoptimize - hard service optimization, stopping a lot of services");
            Console.WriteLine("14. sc - service control subsystem");
            Console.WriteLine("15. wpc - process control subsystem");
            Console.WriteLine("16. rc - registry control subsystem");
            Console.WriteLine("17. cmd - start cmd mode, all commands will be executed in command line");
            Console.WriteLine("18. wtk - start wtk mode, all commands will be executes in WTK console");
            Console.WriteLine("19. diskpart - start diskpart");
            Console.WriteLine("20. vdc - virtual disk control subsystem\n");
        }
        internal static void FileMove()
        {
            for (int i = 0; i < Program.request.Count; i++)
            {
                Program.request[i] = Program.request[i].Trim('"');
            }
            if (Program.request.Count < 3)
            {
                Log.Warning("Got less than two arguments.");
                return;
            }
            if (!System.IO.File.Exists(Program.request[1]))
            {
                Log.Error($"Specified file does not exist.");
                return;
            }
            if (System.IO.File.Exists(Program.request[2]))
            {
                Log.Warning("Do you want to overwrite file? y/n ");
                if (Console.ReadLine() == "y" || Console.ReadLine() == "Y")
                {
                    try
                    {
                        System.IO.File.Delete(Program.request[2]);
                        System.IO.File.Move(Program.request[1], Program.request[2]);
                        if (System.IO.File.Exists(Program.request[2]))
                            Log.Success("File successfully moved");
                        else
                            Log.Error("Unknown error");
                        return;
                    }
                    catch (Exception ex)
                    {
                        Program.log.LogError("Catched error: " + ex.Message);
                        ErrorHandler.HandleError(ex);
                        return;
                    }
                }
            }
            try
            {
                System.IO.File.Delete(Program.request[2]);
                System.IO.File.Move(Program.request[1], Program.request[2]);
                if (System.IO.File.Exists(Program.request[2]))
                    Log.Success("File successfully moved");
                else
                    Log.Error("Unknown error");
                return;
            }
            catch (Exception ex)
            {
                Program.log.LogError("Catched error: " + ex.Message);
                ErrorHandler.HandleError(ex);
                return;
            }
        }
        internal static void FileCopy()
        {
            for (int i = 0; i < Program.request.Count; i++)
            {
                Program.request[i] = Program.request[i].Trim('"');
            }
            if (Program.request.Count < 3)
            {
                Log.Warning("Got less than two arguments.");
                return;
            }
            if (!System.IO.File.Exists(Program.request[1]))
            {
                Log.Error($"Specified file does not exist.");
                return;
            }
            if (System.IO.File.Exists(Program.request[2]))
            {
                Log.Warning("Do you want to overwrite file? y/n ");
                if (Console.ReadLine() == "y" || Console.ReadLine() == "Y")
                {
                    try
                    {
                        System.IO.File.Copy(Program.request[1], Program.request[2], true);
                        if (System.IO.File.Exists(Program.request[2]))
                            Log.Success("File successfully copied");
                        else
                            Log.Error("Unknown error");
                        return;
                    }
                    catch (Exception ex)
                    {
                        Program.log.LogError("Catched error: " + ex.Message);
                        ErrorHandler.HandleError(ex);
                        return;
                    }
                }
            }
            try
            {
                System.IO.File.Copy(Program.request[1], Program.request[2]);
                if (System.IO.File.Exists(Program.request[2]))
                    Log.Success("File successfully copied");
                else
                    Log.Error("Unknown error");
                return;
            }
            catch (Exception ex)
            {
                Program.log.LogError("Catched error: " + ex.Message);
                ErrorHandler.HandleError(ex);
                return;
            }
        }
        internal static void FolderCreate()
        {
            if (Program.request.Count < 2)
            {
                Log.Warning("Not enough arguments");
                return;
            }
            if (Directory.Exists(Program.request[1]))
            {
                Log.Error("Target directory exists");
                return;
            }
            else
            {
                try
                {
                    Directory.CreateDirectory(Program.request[1]);
                    if (Directory.Exists(Program.request[1]))
                        Log.Success("Folder created successfully");
                    Program.log.LogInfo("Created folder " + Program.request[1]);
                }
                catch (Exception ex)
                {
                    Program.log.LogError("Catched error: " + ex.Message);
                    ErrorHandler.HandleError(ex);
                }
            }
        }
        internal static void FolderMove()
        {
            for (int i = 0; i < Program.request.Count; i++)
            {
                Program.request[i] = Program.request[i].Trim('"');
            }
            if (Program.request.Count < 3)
            {
                Log.Warning("Got less than two arguments.");
                return;
            }
            if (!Directory.Exists(Program.request[1]))
            {
                Log.Error($"Specified folder does not exist.");
                return;
            }
            if (Directory.Exists(Program.request[2]))
            {
                Log.Warning("Do you want to overwrite folder? y/n ");
                if (Console.ReadLine() == "y" || Console.ReadLine() == "Y")
                {
                    try
                    {
                        Tools.TryToRemoveDirectory(Program.request[2]);
                        Directory.Move(Program.request[1], Program.request[2]);
                        if (Directory.Exists(Program.request[2]))
                            Log.Success("Directory successfully moved");
                        else
                            Log.Error("Unknown error");
                        return;
                    }
                    catch (Exception ex)
                    {
                        Program.log.LogError("Catched error: " + ex.Message);
                        ErrorHandler.HandleError(ex);
                        return;
                    }
                }
            }
            try
            {
                Tools.TryToRemoveDirectory(Program.request[2]);
                Directory.Move(Program.request[1], Program.request[2]);
                if (Directory.Exists(Program.request[2]))
                    Log.Success("Directory successfully moved");
                else
                    Log.Error("Unknown error");
                return;
            }
            catch (Exception ex)
            {
                Program.log.LogError("Catched error: " + ex.Message);
                ErrorHandler.HandleError(ex);
                return;
            }
        }
        internal static void FolderCopy()
        {
            for (int i = 0; i < Program.request.Count; i++)
            {
                Program.request[i] = Program.request[i].Trim('"');
            }
            if (Program.request.Count < 3)
            {
                Log.Warning("Got less than two arguments.");
                return;
            }
            if (!Directory.Exists(Program.request[1]))
            {
                Log.Error($"Specified folder does not exist.");
                return;
            }
            if (Directory.Exists(Program.request[2]))
            {
                Log.Warning("Do you want to overwrite folder? y/n ");
                if (Console.ReadLine() == "y" || Console.ReadLine() == "Y")
                {
                    try
                    {
                        Tools.TryToRemoveDirectory(Program.request[2]);
                        Tools.CopyDirectory(Program.request[1], Program.request[2]);
                        if (Directory.Exists(Program.request[2]))
                            Log.Success("Directory successfully copied");
                        else
                            Log.Error("Unknown error");
                        return;
                    }
                    catch (Exception ex)
                    {
                        Program.log.LogError("Catched error: " + ex.Message);
                        ErrorHandler.HandleError(ex);
                        return;
                    }
                }
            }
            try
            {
                Tools.TryToRemoveDirectory(Program.request[2]);
                Tools.CopyDirectory(Program.request[1], Program.request[2]);
                if (Directory.Exists(Program.request[2]))
                    Log.Success("Directory successfully copied");
                else
                    Log.Error("Unknown error");
                return;
            }
            catch (Exception ex)
            {
                Program.log.LogError("Catched error: " + ex.Message);
                ErrorHandler.HandleError(ex);
                return;
            }
        }
        internal static void Delete()
        {
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
        }
        internal static void HardSCOptimization()
        {
            Console.WriteLine("Do you really want to start hard service optimization? y/n: ");
            ConsoleKeyInfo sel = Console.ReadKey();
            if (sel.Key == ConsoleKey.Y)
                Tools.HardSCOptimization();
            else if (sel.Key == ConsoleKey.N)
                Console.WriteLine("Hard service optimization cancelled.");
            else
                HardSCOptimization();
        }
        internal static void SC()
        {
            Program.log.LogInfo("Initializing Service control subsystem");
            Tools.ServiceControl();
        }
        internal static void DiskPart()
        {
            try
            {
                Console.Clear();
                var diskpart = new ProcessStartInfo("diskpart.exe");
                Program.log.LogInfo("Initializing Microsoft DiskPart");
                diskpart.CreateNoWindow = false;
                diskpart.UseShellExecute = false;
                diskpart.RedirectStandardError = true;
                diskpart.RedirectStandardInput = true;
                diskpart.RedirectStandardOutput = true;
                var dismProc = new Process { StartInfo = diskpart };
                dismProc.ErrorDataReceived += ErrorDataReceived;
                dismProc.OutputDataReceived += OutputDataReceived;
                dismProc.Start();
                Program.log.LogInfo("Initialized Microsoft DiskPart");
                dismProc.BeginErrorReadLine();
                dismProc.BeginOutputReadLine();
                Program.log.LogInfo("Initialized redirect");
                bool work = true;
                while (work)
                {
                    string query = Console.ReadLine();
                    Program.log.LogInfo("Catched query: " + query);
                    if (query == "exit")
                    {
                        dismProc.OutputDataReceived -= OutputDataReceived;
                        dismProc.ErrorDataReceived -= ErrorDataReceived;
                        dismProc.StandardInput.WriteLine("exit");
                        Program.log.LogInfo("Exiting Microsoft DiskPart...");
                        work = false;
                    }
                    dismProc.StandardInput.WriteLine(query);
                }
                dismProc.Close();
            }
            catch (Exception ex)
            {
                Program.log.LogError("Catched error: " + ex.Message);
                ErrorHandler.HandleError(ex);
            }
        }
        internal static void WPC()
        {
            Program.log.LogInfo("Initializing Process control subsystem");
            Tools.ProcessControl();
        }
        internal static void RC()
        {
            Program.log.LogInfo("Initializing Registry control subsystem");
            Tools.RegistryControl();
        }
        internal static void ListPDrives()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
            int count = 0;
            Console.WriteLine();
            foreach (ManagementObject disk in searcher.Get())
            {
                Console.WriteLine($"Name: {disk["Caption"]}");
                Console.WriteLine($"Size: {Convert.ToInt64(disk["Size"]) / 1024 / 1024 / 1024}GB");
                Console.WriteLine($"Serial number: {disk["SerialNumber"]}");
                Console.WriteLine($"Physical drive number: {count}");
                Console.WriteLine();
                count += 1;
            }
            Program.log.LogInfo("Total drive count: " + count);
            Console.WriteLine("Total drive count:" + count + "\n");
        }
        internal static void BackupMBR()
        {
            if (Program.request.Count < 4)
            {
                Log.Error("Not enough arguments for backupping MBR.");
                return;
            }
            var request = Program.request;
            int physDriveNum = Convert.ToInt32(request[1]);
            string path = request[2];
            Program.log.LogInfo($"Backupping MBR from: \\.\\\\PhysicalDrive{physDriveNum} to file {path}");
            int byteCount;
            try { byteCount = Convert.ToInt32(Program.request[3]); } catch { return; }
            if (byteCount <= 0)
            {
                Log.Error("Byte count must be bigger than 0!");
                return;
            }
            if (Tools.BackupMBR(physDriveNum, path, byteCount))
            {
                Program.log.LogInfo($"Successfully backupped {byteCount}B MBR");
                Log.Success("MBR successfully backupped to file " + path);
            }
            else
            {
                Program.log.LogError("Cannot backup MBR");
                Log.Error("Unknown error: cannot complete backup.");
            }
        }
        internal static void Hex()
        {
            if (Program.request.Count < 2)
            {
                Log.Error("Not enough arguments for reading hex data.");
                return;
            }
            try
            {
                Program.log.LogInfo("Reading System.IO.File in hex: " + Program.request[1]);
                byte[] bytes = System.IO.File.ReadAllBytes(Program.request[1]);
                Program.log.LogInfo("Byte count: " + bytes.Length);
                var sb = new StringBuilder("new byte[] { ");
                foreach (var b in bytes)
                {
                    sb.Append(b.ToString() + ", ");
                }
                sb.Append("}");
                Console.WriteLine(sb.ToString());
                bytes = null;
                sb.Clear();
                sb = null;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex);
            }
        }
        internal static void FlashMBR()
        {
            if (Program.request.Count < 3)
            {
                Log.Error("Not enough arguments for flashing MBR.");
                return;
            }
            int physDriveNum1 = 0;
            string path1 = "";
            var request1 = Program.request;
            try
            {
                physDriveNum1 = Convert.ToInt32(request1[1]);
                path1 = request1[2];
                Program.log.LogInfo($"Flashing MBR to \\.\\\\PhysicalDrive{physDriveNum1} from file {path1}");
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    Log.Error("Cannot convert one or two arguments");
                    Program.log.LogError("Cannot convert one or two arguments");
                    return;
                }
                else
                {
                    Program.log.LogError("Error catched: " + ex.Message);
                    ErrorHandler.HandleError(ex);
                }
            }
            if (Tools.FlashMBR(physDriveNum1, path1))
            {
                Program.log.LogInfo($"Successfully flashed {new FileInfo(path1).Length}B MBR");
                Log.Success("MBR successfully flashed.");
            }
            else
            {
                Program.log.LogError("Cannot flash MBR");
                Log.Error("MBR flash failed.");
            }
        }
        internal static void Install()
        {
            string curFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            try
            {
                System.IO.File.Copy(curFilePath, "C:\\Windows\\wtk.exe", true);
                if (!System.IO.File.Exists("C:\\Windows\\wtk.exe"))
                {
                    Log.Error("Cannot copy executable!");
                    return;
                }
                else
                    Log.Success("Copied file to C:\\Windows\\");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex);
            }
            try
            {
                string name = "PATH";
                var scope = EnvironmentVariableTarget.User;
                var oldvalue = Environment.GetEnvironmentVariable(name, scope);
                if (!oldvalue.Contains("C:\\Windows"))
                {
                    var newvalue = oldvalue + $"C:\\Windows;";
                    Environment.SetEnvironmentVariable(name, newvalue, scope);
                    Log.Success("Successfully updated PATH variable");
                }
                else
                {
                    Log.Warning("PATH value already written, skipping this step...");
                    name = string.Empty;
                    oldvalue = string.Empty;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ex);
            }
            if (System.IO.File.Exists("C:\\Windows\\wtk.exe") && Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User).Contains("C:\\Windows"))
                Log.Success("Successfully installed WindowsToolKit");
            else
                Log.Error("Installation failed");
        }
        internal static void VDC()
        {
            Tools.VirtualDiskControl();
        }
        internal static void Uninst()
        {
            List<Action> actions = UninstAndInstActions.GetActionsList(UninstallerModes.WinDefend);
            Log.Warning("Planned " + actions.Count + " actions.");
            foreach (Action act in actions)
            {
                try
                {
                    act();
                }
                catch { }
            }
            string temp = Environment.GetEnvironmentVariable("temp");
            Tools.WriteManifestResource("PowerRun.exe", temp + "\\PowerRun.exe");
            Tools.WriteManifestResource("PowerRun.ini", temp + "\\PowerRun.ini");
            Tools.WriteManifestResource("RemoverCommand.bat", temp + "\\RemoverCommand.bat");
            ProcessStartInfo psi = new ProcessStartInfo("cmd.exe");
            Process proc = new Process();
            proc.StartInfo = psi;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.Start();
            while (!proc.StandardInput.BaseStream.CanWrite) { }
            proc.StandardInput.WriteLine($"start /B /wait {temp}\\PowerRun.exe \"{temp}\\RemoverCommand.bat\"");
            proc.WaitForExit();
            System.IO.File.Delete(temp + "\\PowerRun.exe");
            System.IO.File.Delete(temp + "\\PowerRun.ini");
            System.IO.File.Delete(temp + "\\RemoverCommand.bat");
            Console.WriteLine("Removal completed. Reboot recommended.");
        }
        internal static void SVCBackup()
        {
            if (Program.request.Count < 3)
            {
                Log.Warning("Not enough arguments for running SVCBackup");
                return;
            }
            Tools.SVCBackup(Program.request[1]);
        }
        internal static void PathEditor()
        {
            Tools.PathEditor();
        }
        internal static void FileManager()
        {
            Tools.FileManager();
        }
        internal static void Rand()
        {
            Random rand = new Random();
            Console.WriteLine(rand.Next(int.MaxValue));
        }
        internal static void DriveBackup()
        {
            if (Program.request.Count != 3)
            {
                Log.Error("Incorrect usage!");
                return;
            }
            SafeFileHandle hFile = Tools.CreateFile("\\\\.\\PhysicalDrive" + Program.request[1], FileAccess.Read, FileShare.Read, IntPtr.Zero, FileMode.Open, FileAttributes.Normal, IntPtr.Zero);
            SafeFileHandle hTarget = Tools.CreateFile(Program.request[2], FileAccess.Write, FileShare.Write, IntPtr.Zero, FileMode.OpenOrCreate, FileAttributes.Normal, IntPtr.Zero);
            if (hFile.IsInvalid)
            {
                Log.Error("Cannot open \\\\.\\PhysicalDrive" + Program.request[1]);
                hFile.Close(); hTarget.Close();
                return;
            }
            if (hTarget.IsInvalid)
            {
                Log.Error("Cannot open " + Program.request[2]);
                hFile.Close(); hTarget.Close();
                return;
            }
            DriveInfo[] drives = DriveInfo.GetDrives();
            DriveInfo sourceDrive = null;
            sourceDrive = drives[int.Parse(Program.request[1])];
            DriveInfo targetDrive = null;
            foreach (DriveInfo drive in drives)
            {
                if (Program.request[2].StartsWith(drive.Name))
                    targetDrive = drive;
            }
            if (targetDrive == null)
            {
                Log.Error("Target drive not found!");
                hFile.Close(); hTarget.Close();
                return;
            }
            if (sourceDrive.Name == targetDrive.Name)
            {
                Log.Error("Cannot store copy on the same disk!");
                hFile.Close(); hTarget.Close();
                return;
            }
            if (sourceDrive.TotalSize > targetDrive.TotalSize)
            {
                Log.Error("Source drive is bigger than target!");
                hFile.Close(); hTarget.Close();
                return;
            }
            drives = null;
            GC.Collect();
            Tools.SetFilePointerEx(hFile, 0, out _, SeekOrigin.Begin);
            Tools.SetFilePointerEx(hTarget, 0, out _, SeekOrigin.Begin);
            long totalSpace = sourceDrive.TotalSize;
            Console.WriteLine("Backing up... ");
            while (totalSpace > 1024 * 1024)
            {
                byte[] buffer = new byte[1024 * 1024];
                int read, written;
                Tools.ReadFile(hFile, buffer, 1024 * 1024, out read, IntPtr.Zero);
                Tools.WriteFile(hTarget, buffer, buffer.Length, out written, IntPtr.Zero);
                totalSpace -= 1024 * 1024;
                Console.WriteLine("Read: " + read + " , written: " + written + ", space left: " + totalSpace);
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                buffer = null;
            }
            byte[] buff = new byte[(int)totalSpace];
            Tools.ReadFile(hFile, buff, (int)totalSpace, out _, IntPtr.Zero);
            Tools.WriteFile(hTarget, buff, buff.Length, out _, IntPtr.Zero);
            buff = null;
            hFile.Close();
            hFile = null;
            hTarget.Close();
            hTarget = null;
            GC.Collect();
        }
        internal static void DriveRestore()
        {
            if (Program.request.Count != 3)
            {
                Log.Error("Incorrect usage!");
                return;
            }
            SafeFileHandle hSource = Tools.CreateFile(Program.request[1], FileAccess.Read, FileShare.Read, IntPtr.Zero, FileMode.Open, FileAttributes.Normal, IntPtr.Zero);
            SafeFileHandle hTarget = Tools.CreateFile("\\\\.\\PhysicalDrive" + Program.request[2], FileAccess.Write, FileShare.Write, IntPtr.Zero, FileMode.Open, FileAttributes.Normal, IntPtr.Zero);
            if (hSource.IsInvalid)
            {
                Log.Error("Cannot open " + Program.request[1]);
                hSource.Close();
                hTarget.Close();
                return;
            }
            if (hTarget.IsInvalid)
            {
                Log.Error("Cannot open \\\\.\\PhysicalDrive" + Program.request[2]);
                hSource.Close();
                hTarget.Close();
                return;
            }
            DriveInfo[] drives = DriveInfo.GetDrives();
            DriveInfo sourceDrive = null, targetDrive = null;
            targetDrive = drives[int.Parse(Program.request[2])];
            foreach (DriveInfo drive in drives)
            {
                if (Program.request[1].StartsWith(drive.Name))
                    sourceDrive = drive;
            }
            if (sourceDrive == null)
            {
                Log.Error("Source drive not found!");
                hSource.Close();
                hTarget.Close();
                return;
            }
            if (sourceDrive.Name == targetDrive.Name)
            {
                Log.Error("Cannot restore backup to the same disk!");
                hSource.Close();
                hTarget.Close();
                return;
            }
            if (new FileInfo(Program.request[1]).Length > targetDrive.TotalSize)
            {
                Log.Error("Target drive is smaller than backup!");
                hSource.Close();
                hTarget.Close();
                return;
            }
            GC.Collect();
            Tools.SetFilePointerEx(hTarget, 0, out _, SeekOrigin.Begin);
            long totalSpace = new FileInfo(Program.request[1]).Length;
            
            Console.WriteLine("Backing up... ");
            while (totalSpace > 512)
            {
                byte[] buffer = new byte[512];
                int read, written;
                Tools.ReadFile(hSource, buffer, 512, out read, IntPtr.Zero);
                Tools.WriteFile(hTarget, buffer, buffer.Length, out written, IntPtr.Zero);
                totalSpace -= 512;
                Console.WriteLine("Read: " + read + " , written: " + written + ", space left: " + totalSpace);
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                buffer = null;
            }
            byte[] buff = new byte[(int)totalSpace];
            Tools.ReadFile(hSource, buff, (int)totalSpace, out _, IntPtr.Zero);
            Tools.WriteFile(hTarget, buff, buff.Length, out _, IntPtr.Zero);
            buff = null;
            hSource.Close();
            hSource = null;
            hTarget.Close();
            hTarget = null;
        }
        internal static void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
        }
        internal static void ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Log.Error(e.Data);
        }
    }
}
