using Hydriuk.UnturnedModules.Extensions;
using LiteDB;
using UnityEngine;

namespace UHighlight.Models
{
    public class Volume
    {
        public string Group { get; set; } = string.Empty;
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

        public EVolumeShape Shape { get; set; }
        public EVolumeMaterial Material { get; set; } 
        public EVolumeColor Color { get; set; }

        public Volume(string group, string name, Vector3 center, Vector3 size, Vector3 rotation, EVolumeShape shape, EVolumeMaterial material, EVolumeColor color)
        {
            Group = group;
            Name = name;
            Center = center;
            Size = size;
            Rotation = rotation;
            Shape = shape;
            Material = material;
            Color = color;
        }

        public Volume(Vector3 center, Vector3 size, Vector3 rotation, EVolumeShape shape, EVolumeMaterial material, EVolumeColor color)
        {
            Center = center;
            Size = size;
            Rotation = rotation;
            Shape = shape;
            Material = material;
            Color = color;
        }

        internal Volume() {}
    }
}