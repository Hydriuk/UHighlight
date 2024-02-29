using Hydriuk.UnturnedModules.Extensions;
using LiteDB;
using UnityEngine;

namespace UHighlight.Models
{
    public class Volume
    {
        /// <summary>
        /// Name of the zone's group in database
        /// </summary>
        public string Group { get; set; } = string.Empty;

        /// <summary>
        /// Name of the zone in database
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Center of the Volume
        /// </summary>
        [BsonIgnore]
        public Vector3 Center { get; set; }
        /// <summary>
        /// Used for database storage only
        /// </summary>
        public float[] center { get => Center.Serialize(); set => Center = value.Deserialize(); }

        /// <summary>
        /// Size of the Volume
        /// </summary>
        [BsonIgnore]
        public Vector3 Size { get; set; }
        /// <summary>
        /// Used for database storage only
        /// </summary>
        public float[] size { get => Size.Serialize(); set => Size = value.Deserialize(); }

        /// <summary>
        /// Rotation of the volume
        /// </summary>
        [BsonIgnore]
        public Vector3 Rotation { get; set; }
        /// <summary>
        /// Used for database storage only
        /// </summary>
        public float[] rotation { get => Rotation.Serialize(); set => Rotation = value.Deserialize(); }

        /// <summary>
        /// Shape of the volume
        /// </summary>
        public EVolumeShape Shape { get; set; }
        /// <summary>
        /// Material of the volume's effect
        /// </summary>
        public EVolumeMaterial Material { get; set; } 
        /// <summary>
        /// Color of the volume's effect
        /// </summary>
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

        /// <summary>
        /// Used for initialisation only.
        /// </summary>
        internal Volume() {}
    }
}