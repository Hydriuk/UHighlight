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
            PermissionGroup,
            Chat,
            WalkThrough,
            ExecuteCommand
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

    //public class ZoneProperty<T> where T : struct
    //{

    //    public readonly static ReadOnlyDictionary<EType, Type> TypeData = new ReadOnlyDictionary<EType, Type>(new Dictionary<EType, Type>() {
    //        { EType.PlaceStructure, typeof(bool) },
    //        { EType.StructureDamage, typeof(float) },
    //        { EType.PlayerDamage, typeof(float) },
    //        { EType.VehicleDamage, typeof(float) },
    //        { EType.ZombieDamage, typeof(float) },
    //        { EType.AnimalDamage, typeof(float) },
    //        { EType.Chat, typeof(string) },
    //        { EType.WalkThrough, typeof(bool) },
    //        { EType.ExecuteCommand, typeof(string) },
    //        { EType.PermissionGroup, typeof(string) },
    //    });
    //}


}
