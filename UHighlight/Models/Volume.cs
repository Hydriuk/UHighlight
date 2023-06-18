using Hydriuk.UnturnedModules.Extensions;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UHighlight.Models
{
    public class Volume
    {
        public string Category { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        [BsonIgnore]
        public Vector3 Center { get; set; }
        public float[] center { get => Center.Serialize(); set => Center = value.Deserialize(); }

        [BsonIgnore]
        public Vector3 Size { get; set; }
        public float[] size { get => Size.Serialize(); set => Size = value.Deserialize(); }

        [BsonIgnore]
        public Vector3 Rotation { get; set; }
        public float[] rotation { get => Rotation.Serialize(); set => Rotation = value.Deserialize(); }

        public string Shape { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Material { get; set; } = string.Empty;
    }
}
