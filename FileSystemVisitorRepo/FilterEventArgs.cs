using System;
using System.Collections.Generic;

namespace Mod4Task
{
    public class FilterEventArgs : EventArgs
    {
        public string Name { get; set; }
        public bool Cancel { get; set; }
        public List<string> ExcludeNames { get; set; } = new List<string>();
    }
}
