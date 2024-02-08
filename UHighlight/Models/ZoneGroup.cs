using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace UHighlight.Models
{
    public class ZoneGroup
    {
        [BsonId]
        public ObjectId Id { get; set; } = ObjectId.Empty;

        public string Name { get; set; } = string.Empty;
        public List<ZoneProperty> Properties { get; set; } = new List<ZoneProperty>();
        public List<Volume> Zones { get; set; } = new List<Volume>();
    }
}
