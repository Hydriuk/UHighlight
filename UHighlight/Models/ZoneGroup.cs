using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace UHighlight.Models
{
    public class ZoneGroup
    {
        internal ObjectId Id { get; set; } = ObjectId.Empty;

        public string Name { get; set; } = string.Empty;
        public List<EventZoneProperty> EventProperties { get; set; } = new List<EventZoneProperty>();
        public List<PositionnalZoneProperty> PositionnalProperties { get; set; } = new List<PositionnalZoneProperty>();
        public List<Volume> Zones { get; set; } = new List<Volume>();
    }
}
