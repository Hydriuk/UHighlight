using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UHighlight.Models;

namespace UHighlight.Extensions
{
    public static class ZoneGroupExtensions
    {
        private static readonly HashSet<ZoneProperty.EType> _positionnalTypes = new HashSet<ZoneProperty.EType>() 
        {
            ZoneProperty.EType.PlaceStructure,
            ZoneProperty.EType.StructureDamage,
            ZoneProperty.EType.PlayerDamage,
            ZoneProperty.EType.VehicleDamage,
            ZoneProperty.EType.ZombieDamage,
            ZoneProperty.EType.AnimalDamage,
            ZoneProperty.EType.PermissionGroup
        };

        private static readonly HashSet<ZoneProperty.EType> _eventTypes = new HashSet<ZoneProperty.EType>()
        {
            ZoneProperty.EType.Chat,
            ZoneProperty.EType.ExecuteCommand,
            ZoneProperty.EType.WalkThrough
        };

        public static IEnumerable<ZoneProperty> GetPositionnalProperties(this ZoneGroup group)
        {
            return group.Properties.Where(property => _positionnalTypes.Contains(property.Type));
        }

        public static IEnumerable<ZoneProperty> GetEventProperties(this ZoneGroup group)
        {
            return group.Properties.Where(property => _eventTypes.Contains(property.Type));
        }
    }
}
