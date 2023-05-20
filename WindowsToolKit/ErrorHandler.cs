using System;
using System.IO;

namespace WindowsToolKit
{
    internal static class ErrorHandler
    {
        internal static void HandleError(Exception ex)
        {
            if (ex is IOException)
                Log.Error("IOException: " + ex.Message);
            else if (ex is ArgumentNullException)
                Log.Error("ArgumentNullException: " + ex.Message);
            else if (ex is ArgumentException)
                Log.Error("ArgumentException: " + ex.Message);
            else if (ex is UnauthorizedAccessException)
                Log.Error("UnauthorizedAccessException: " + ex.Message);
            else if (ex is PathTooLongException)
                Log.Error("PathTooLongException: " + ex.Message);
            else if (ex is DirectoryNotFoundException)
                Log.Error("DirectoryNotFoundException: " + ex.Message);
            else if (ex is NotSupportedException)
                Log.Error("NotSupportedException: " + ex.Message);
            else if (ex is FileNotFoundException)
                Log.Error("FileNotFoundException: " + ex.Message);
            else if (ex is ObjectDisposedException)
                Log.Error("ObjectDisposedException: " + ex.Message);
            else if (ex is InvalidOperationException)
                Log.Error("InvalidOperationException: " + ex.Message);
            else if (ex is System.ComponentModel.Win32Exception)
                Log.Error("Win32Exception: " + ex.Message);
            else if (ex is SystemException)
                Log.Error("SystemException: " + ex.Message);
            else if (ex is IOException)
                Log.Error("IOException: " + ex.Message);
            else if (ex is ArgumentOutOfRangeException)
                Log.Error("ArgumentOutOfRangeException: " + ex.Message);
        }
    }

}
