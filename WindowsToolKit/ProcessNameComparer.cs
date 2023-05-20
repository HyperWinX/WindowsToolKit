using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace WindowsToolKit
{
    public class ProcessNameComparer : IComparer<Process>
    {
        public int Compare(Process x, Process y)
        {
            return String.Compare(x.ProcessName, y.ProcessName, StringComparison.OrdinalIgnoreCase);
        }
    }

}
