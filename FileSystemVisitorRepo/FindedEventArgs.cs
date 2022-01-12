using System;

namespace Mod4Task
{
    public class FindedEventArgs : EventArgs
    {
        public Type EntityType { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
    }
}
