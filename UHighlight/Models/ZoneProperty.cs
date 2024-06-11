using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace UHighlight.Models
{
    public class ZoneProperty
    {
        public EType Type { get; set; }
        public EEvent Event { get; set; }
        public string Data { get; set; } = string.Empty;

        public enum EType
        {
            PlaceStructure,
            StructureDamage,
            PlayerDamage,
            VehicleDamage,
            ZombieDamage,
            AnimalDamage,
            GivePermissionGroup,
            RemovePermissionGroup,
            Chat,
            Repulse,
            ExecuteCommand,
            NoEquip
        }

        public enum EEvent
        {
            Enter,
            Exit,

            OnEnter = Enter,
            OnExit = Exit,

            In = Enter,
            Out = Exit,

            Inside = Enter,
            Outside = Exit
        }
    }
}
