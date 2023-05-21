using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsToolKit
{
    internal static class QueryHandler
    {
        internal static Logger log = new Logger();
        internal static void Handle(string command)
        {
            if (command == "cmd")
            {
                try
                {
                    log.LogInfo("Initializing command prompt");
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
                    log.LogInfo("Initialized command prompt");
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    log.LogInfo("Started redirection");
                    bool work = true;
                    while (work)
                    {
                        string query = Console.ReadLine();
                        log.LogInfo($"Catched query {query}");
                        if (query == "exit")
                        {
                            process.OutputDataReceived -= OutputDataReceived;
                            process.ErrorDataReceived -= ErrorDataReceived;
                            process.StandardInput.WriteLine("exit");
                            log.LogInfo("Exiting from command prompt...");
                            work = false;
                        }
                        else if (query.ToLower() == "wtk")
                        {
                            Program.currentMode = "WTK";
                            work = !work;
                            process.OutputDataReceived -= OutputDataReceived;
                            process.ErrorDataReceived -= ErrorDataReceived;
                            process.StandardInput.WriteLine("exit");
                            log.LogInfo("Exiting from command prompt...");
                            break;
                        }
                        process.StandardInput.WriteLine(query);
                    } //Main command perform cycle
                    process.Close();
                    return;
                }
                catch (Exception ex)
                {
                    log.LogError("Catched error: " + ex.Message);
                    ErrorHandler.HandleError(ex);
                } //Command prompt command redirect module
            }
            else if (command == "ps")
            {
                try
                {
                    string[] temp = Program.requestStr.Split();
                    List<string> temp2 = new List<string>(temp.Length - 1);
                    for (int i = 1; i < temp.Length; i++)
                        temp2[i - 1] = temp[i];
                    string args = string.Join(" ", temp2.ToArray());
                    log.LogInfo("Initializing powershell");
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
                    log.LogInfo("Initialized powershell");
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    log.LogInfo("Initialized redirect");
                    bool work = true;
                    while (work)
                    {
                        string query = Console.ReadLine();
                        log.LogInfo("Catched query " + query);
                        if (query == "exit")
                        {
                            process.OutputDataReceived -= OutputDataReceived;
                            process.ErrorDataReceived -= ErrorDataReceived;
                            process.StandardInput.WriteLine("exit");
                            log.LogInfo("Exiting from powershell");
                            work = false;
                        }
                        else if (query.ToLower() == "wtk")
                        {
                            Program.currentMode = "WTK";
                            work = !work;
                            process.OutputDataReceived -= OutputDataReceived;
                            process.ErrorDataReceived -= ErrorDataReceived;
                            process.StandardInput.WriteLine("exit");
                            log.LogInfo("Exiting from powershell");
                            break;
                        }
                        process.StandardInput.WriteLine(query);
                    }
                    process.Close();
                    return;
                }
                catch (Exception ex)
                {
                    log.LogError("Catched error: " + ex.Message);
                    ErrorHandler.HandleError(ex);
                } //Powershell command redirect module
            }
            switch (command)
            {
                case "help":
                    Console.Clear();
                    Console.WriteLine("[HELP]");
                    Console.WriteLine("1. help - displays current screen");
                    Console.WriteLine("2. filemove <source_file> <destination_file> - moves file to the destination path");
                    Console.WriteLine("3. filecopy <source_file> <destination_file> - copies file to the destination path");
                    Console.WriteLine("4. foldercreate <target_folder> - creates folder");
                    Console.WriteLine("5. foldermove <source_folder> <destination_folder> - moves folder to the destination path with name");
                    Console.WriteLine("6. foldercopy <source_folder> <destination_folder> - copies folder to the destination path with name");
                    Console.WriteLine("7. clear - clears the screen");
                    Console.WriteLine("8. delete <file_or_folder> - deletes object");
                    Console.WriteLine("9. listpdrives - list of physical drives");
                    Console.WriteLine("10. hex <file> - view file in hex");
                    Console.WriteLine("11. backupmbr <physical_drive_number> <file> - backups mbr from selected drive to specified file");
                    Console.WriteLine("12. flashmbr <physical_drive_number> <file> - flashes mbr to selected drive from specified file");
                    Console.WriteLine("13. hardscoptimize - hard service optimization, stopping a lot of services");
                    Console.WriteLine("14. sc - Service control subsystem");
                    Console.WriteLine("15. wpc - Process control subsystem");
                    Console.WriteLine("16. rc - Registry control subsystem");
                    Console.WriteLine("17. cmd - start cmd mode, all commands will be executed in command line");
                    Console.WriteLine("18. wtk - start wtk mode, all commands will be executes in WTK console");
                    Console.WriteLine("19. diskpart - start diskpart");
                    Console.Write("Press any key to exit...");
                    Console.Read();
                    Console.Clear();
                    Thread.Sleep(500);
                    Program.helpExited = false;
                    break;
                case "filemove":
                    for (int i = 0; i < Program.request.Count; i++)
                    {
                        Program.request[i] = Program.request[i].Trim('"');
                    }
                    if (Program.request.Count < 3)
                    {
                        Log.Warning("Got less than two arguments.");
                        break;
                    }
                    if (!File.Exists(Program.request[1]))
                    {
                        Log.Error($"Specified file does not exist.");
                        break;
                    }
                    if (File.Exists(Program.request[2]))
                    {
                        Log.Warning("Do you want to overwrite file? y/n ");
                        if (Console.ReadLine() == "y" || Console.ReadLine() == "Y")
                        {
                            try
                            {
                                File.Delete(Program.request[2]);
                                File.Move(Program.request[1], Program.request[2]);
                                if (File.Exists(Program.request[2]))
                                    Log.Success("File successfully moved");
                                else
                                    Log.Error("Unknown error");
                                break;
                            }
                            catch (Exception ex)
                            {
                                log.LogError("Catched error: " + ex.Message);
                                ErrorHandler.HandleError(ex);
                                break;
                            }
                        }
                    }
                    try
                    {
                        File.Delete(Program.request[2]);
                        File.Move(Program.request[1], Program.request[2]);
                        if (File.Exists(Program.request[2]))
                            Log.Success("File successfully moved");
                        else
                            Log.Error("Unknown error");
                        break;
                    }
                    catch (Exception ex)
                    {
                        log.LogError("Catched error: " + ex.Message);
                        ErrorHandler.HandleError(ex);
                        break;
                    }
                case "filecopy":
                    for (int i = 0; i < Program.request.Count; i++)
                    {
                        Program.request[i] = Program.request[i].Trim('"');
                    }
                    if (Program.request.Count < 3)
                    {
                        Log.Warning("Got less than two arguments.");
                        break;
                    }
                    if (!File.Exists(Program.request[1]))
                    {
                        Log.Error($"Specified file does not exist.");
                        break;
                    }
                    if (File.Exists(Program.request[2]))
                    {
                        Log.Warning("Do you want to overwrite file? y/n ");
                        if (Console.ReadLine() == "y" || Console.ReadLine() == "Y")
                        {
                            try
                            {
                                File.Copy(Program.request[1], Program.request[2], true);
                                if (File.Exists(Program.request[2]))
                                    Log.Success("File successfully copied");
                                else
                                    Log.Error("Unknown error");
                                break;
                            }
                            catch (Exception ex)
                            {
                                log.LogError("Catched error: " + ex.Message);
                                ErrorHandler.HandleError(ex);
                                break;
                            }
                        }
                    }
                    try
                    {
                        File.Copy(Program.request[1], Program.request[2]);
                        if (File.Exists(Program.request[2]))
                            Log.Success("File successfully copied");
                        else
                            Log.Error("Unknown error");
                        break;
                    }
                    catch (Exception ex)
                    {
                        log.LogError("Catched error: " + ex.Message);
                        ErrorHandler.HandleError(ex);
                        break;
                    }
                case "foldercreate":
                    if (Program.request.Count < 2)
                    {
                        Log.Warning("Not enough arguments");
                        break;
                    }
                    if (Directory.Exists(Program.request[1]))
                    {
                        Log.Error("Target directory exists");
                        break;
                    }
                    else
                    {
                        try
                        {
                            Directory.CreateDirectory(Program.request[1]);
                            if (Directory.Exists(Program.request[1]))
                                Log.Success("Folder created successfully");
                            log.LogInfo("Created folder " + Program.request[1]);
                        } catch (Exception ex)
                        {
                            log.LogError("Catched error: " + ex.Message);
                            ErrorHandler.HandleError(ex);
                        }
                    }
                    break;
                case "foldermove":
                    for (int i = 0; i < Program.request.Count; i++)
                    {
                        Program.request[i] = Program.request[i].Trim('"');
                    }
                    if (Program.request.Count < 3)
                    {
                        Log.Warning("Got less than two arguments.");
                        break;
                    }
                    if (!Directory.Exists(Program.request[1]))
                    {
                        Log.Error($"Specified folder does not exist.");
                        break;
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
                                break;
                            }
                            catch (Exception ex)
                            {
                                log.LogError("Catched error: " + ex.Message);
                                ErrorHandler.HandleError(ex);
                                break;
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
                        break;
                    }
                    catch (Exception ex)
                    {
                        log.LogError("Catched error: " + ex.Message);
                        ErrorHandler.HandleError(ex);
                        break;
                    }
                case "foldercopy":
                    for (int i = 0; i < Program.request.Count; i++)
                    {
                        Program.request[i] = Program.request[i].Trim('"');
                    }
                    if (Program.request.Count < 3)
                    {
                        Log.Warning("Got less than two arguments.");
                        break;
                    }
                    if (!Directory.Exists(Program.request[1]))
                    {
                        Log.Error($"Specified folder does not exist.");
                        break;
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
                                break;
                            }
                            catch (Exception ex)
                            {
                                log.LogError("Catched error: " + ex.Message);
                                ErrorHandler.HandleError(ex);
                                break;
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
                        break;
                    }
                    catch (Exception ex)
                    {
                        log.LogError("Catched error: " + ex.Message);
                        ErrorHandler.HandleError(ex);
                        break;
                    }
                case "clear":
                    Console.Clear();
                    break;
                case "delete":
                    if (Program.request.Count < 2)
                    {
                        Log.Warning("Not enough arguments given.");
                        break;
                    }
                    for (int i = 0; i < Program.request.Count; i++)
                    {
                        Program.request[i] = Program.request[i].Trim('"');
                    }
                    if (File.Exists(Program.request[1]))
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
                            break;
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
                case "hardscoptimize":
                    Console.WriteLine("Do you really want to start hard service optimization? y/n: ");
                    ConsoleKeyInfo sel = Console.ReadKey();
                    if (sel.Key == ConsoleKey.Y)
                        Tools.HardSCOptimization();
                    else if (sel.Key == ConsoleKey.N)
                        Console.WriteLine("Hard service optimization cancelled.");
                    break;
                case "sc":
                    log.LogInfo("Initializing Service control subsystem");
                    Tools.ServiceControl();
                    break;
                case "diskpart":
                    try
                    {
                        Console.Clear();
                        var diskpart = new ProcessStartInfo("diskpart.exe");
                        log.LogInfo("Initializing Microsoft DiskPart");
                        diskpart.CreateNoWindow = false;
                        diskpart.UseShellExecute = false;
                        diskpart.RedirectStandardError = true;
                        diskpart.RedirectStandardInput = true;
                        diskpart.RedirectStandardOutput = true;
                        var dismProc = new Process { StartInfo = diskpart };
                        dismProc.ErrorDataReceived += ErrorDataReceived;
                        dismProc.OutputDataReceived += OutputDataReceived;
                        dismProc.Start();
                        log.LogInfo("Initialized Microsoft DiskPart");
                        dismProc.BeginErrorReadLine();
                        dismProc.BeginOutputReadLine();
                        log.LogInfo("Initialized redirect");
                        bool work = true;
                        while (work)
                        {
                            string query = Console.ReadLine();
                            log.LogInfo("Catched query: " + query);
                            if (query == "exit")
                            {
                                dismProc.OutputDataReceived -= OutputDataReceived;
                                dismProc.ErrorDataReceived -= ErrorDataReceived;
                                dismProc.StandardInput.WriteLine("exit");
                                log.LogInfo("Exiting Microsoft DiskPart...");
                                work = false;
                            }
                            dismProc.StandardInput.WriteLine(query);
                        }
                        dismProc.Close();
                    }
                    catch (Exception ex)
                    {
                        log.LogError("Catched error: " + ex.Message);
                        ErrorHandler.HandleError(ex);
                    }
                    break;
                case "exit":
                    Environment.Exit(0);
                    break;
                case "wpc":
                    log.LogInfo("Initializing Process control subsystem");
                    Tools.ProcessControl();
                    break;
                case "rc":
                    log.LogInfo("Initializing Registry control subsystem");
                    Tools.RegistryControl();
                    break;
                case "listpdrives":
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
                    log.LogInfo("Total drive count: " + count);
                    Console.WriteLine("Total drive count:" + count);
                    Console.WriteLine();
                    break;
                case "backupmbr":
                    if (Program.request.Count < 3)
                    {
                        Log.Error("Not enough arguments for backupping MBR.");
                        return;
                    }
                    var request = Program.request;
                    int physDriveNum = Convert.ToInt32(request[1]);
                    string path = request[2];
                    log.LogInfo($"Backupping MBR from: \\.\\\\PhysicalDrive{physDriveNum} to file {path}");
                    if (Tools.BackupMBR(physDriveNum, path))
                    {
                        log.LogInfo("Successfully backupped 512B MBR");
                        Log.Success("MBR successfully backupped to file " + path);
                    }
                    else
                    {
                        log.LogError("Cannot backup MBR");
                        Log.Error("Unknown error: cannot complete backup.");
                    }
                    break;
                case "hex":
                    if (Program.request.Count < 2)
                    {
                        Log.Error("Not enough arguments for reading hex data.");
                        return;
                    }
                    log.LogInfo("Reading file in hex: " + Program.request[1]);
                    byte[] bytes = File.ReadAllBytes(Program.request[1]);
                    log.LogInfo("Byte count: " + bytes.Length);
                    var sb = new StringBuilder("new byte[] { ");
                    foreach (var b in bytes)
                    {
                        sb.Append(b.ToString() + ", ");
                    }
                    sb.Append("}");
                    Console.WriteLine(sb.ToString());
                    break;
                case "flashmbr":
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
                        log.LogInfo($"Flashing MBR to \\.\\\\PhysicalDrive{physDriveNum1} from file {path1}");
                    }
                    catch (Exception ex)
                    {
                        if (ex is FormatException)
                        {
                            Log.Error("Cannot convert one or two arguments");
                            log.LogError("Cannot convert one or two arguments");
                            return;
                        }
                        else
                        {
                            log.LogError("Error catched: " + ex.Message);
                            ErrorHandler.HandleError(ex);
                        }
                    }
                    if (Tools.FlashMBR(physDriveNum1, path1))
                    {
                        log.LogInfo("Successfully flashed 512B MBR");
                        Log.Success("MBR successfully flashed.");
                    }
                    else
                    {
                        log.LogError("Cannot flash MBR");
                        Log.Error("MBR flash failed.");
                    }
                    break;
            } //Main WTK command module
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
