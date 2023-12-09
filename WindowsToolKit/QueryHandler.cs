using System;

namespace WindowsToolKit
{
    internal static class QueryHandler
    {
        internal static Logger log = new Logger();
        internal static void Handle(string command)
        {
            if (command == "cmd")
            {
                QueryHandlerMethods.StartCommandPrompt();
            }
            else if (command == "ps")
            {
                QueryHandlerMethods.StartPowershell();
            }
            switch (command)
            {
                case "help":
                    QueryHandlerMethods.Help();
                    break;
                case "filemove":
                    QueryHandlerMethods.FileMove();
                    break;
                case "filecopy":
                    QueryHandlerMethods.FileCopy();
                    break;
                case "mkdir":
                    QueryHandlerMethods.FolderCreate();
                    break;
                case "foldermove":
                    QueryHandlerMethods.FolderMove();
                    break;
                case "foldercopy":
                    QueryHandlerMethods.FolderCopy();
                    break;
                case "clear":
                    Console.Clear();
                    break;
                case "delete":
                    QueryHandlerMethods.Delete();
                    break;
                case "hardscoptimize":
                    QueryHandlerMethods.HardSCOptimization();
                    break;
                case "sc":
                    QueryHandlerMethods.SC();
                    break;
                case "diskpart":
                    QueryHandlerMethods.DiskPart();
                    break;
                case "exit":
                    Environment.Exit(0);
                    break;
                case "wpc":
                    QueryHandlerMethods.WPC();
                    break;
                case "rc":
                    QueryHandlerMethods.RC();
                    break;
                case "listpdrives":
                    QueryHandlerMethods.ListPDrives();
                    break;
                case "backupmbr":
                    QueryHandlerMethods.BackupMBR();
                    break;
                case "hex":
                    QueryHandlerMethods.Hex();
                    break;
                case "flashmbr":
                    QueryHandlerMethods.FlashMBR();
                    break;
                case "install":
                    QueryHandlerMethods.Install();
                    break;
                case "vdc":
                    QueryHandlerMethods.VDC();
                    break;
                case "uninst":
                    QueryHandlerMethods.Uninst();
                    break;
                case "svcbackup":
                    QueryHandlerMethods.SVCBackup();
                    break;
                case "path":
                    QueryHandlerMethods.PathEditor();
                    break;
                case "fm":
                    QueryHandlerMethods.FileManager();
                    break;
                case "rand":
                    QueryHandlerMethods.Rand();
                    break;
                case "drivebackup":
                    QueryHandlerMethods.DriveBackup();
                    break;
                case "driverestore":
                    QueryHandlerMethods.DriveRestore();
                    break;
            } //Main WTK command module
        }

    }

}
