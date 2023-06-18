using System;
using System.Collections.Generic;
using System.Text;

namespace UHighlight.EventArgs
{
    public class ZoneArgs
    {
        public string Category { get; }
        public string Name { get; }

        public ZoneArgs(string category, string name)
        {
            Category = category;
            Name = name;
        }
    }
}
