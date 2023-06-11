using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UHighlight.Models
{
    public class Volume
    {
        public string Name { get; set; } = string.Empty;

        public Vector3 Center { get; set; }
        public Vector3 Size { get; set; }
        public Vector3 Rotation { get; set; }

        public string Shape { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Material { get; set; } = string.Empty;
    }
}
