using Hydriuk.UnturnedModules.Adapters;
using Hydriuk.UnturnedModules.PlayerKeys;
using SDG.Unturned;
using System.Collections.Generic;
using System.Linq;
using UHighlight.API;
using UHighlight.Models;
using UHighlight.VolumeStrategies;
using UnityEngine;

namespace UHighlight.VolumeEditors
{
    public class CubeStrategy : BaseEditionStrategy
    {
        private readonly Cube _cube;

        private readonly Player _player;
        private readonly string _material;
        private readonly string _color;

        public CubeStrategy(
            ICoroutineAdapter coroutineAdapter, 
            IEffectBuilder effectBuilder, 
            Player player, 
            string material, 
            string color) : base(coroutineAdapter, effectBuilder, player)
        {
            _player = player;
            _material = material;
            _color = color;

            _cube = new Cube();

            PlayerEquipment.OnPunch_Global += OnPunched;

            base.Init();
        }

        public override void Dispose()
        {
            PlayerEquipment.OnPunch_Global -= OnPunched;

            base.Dispose();
        }

        private void OnPunched(PlayerEquipment equipment, EPlayerPunch punch)
        {
            if (equipment.player != _player)
                return;

            Vector3 position;
            if (Physics.Raycast(_player.look.aim.position, _player.look.aim.forward, out RaycastHit hit, 1.75f, RayMasks.DAMAGE_SERVER))
            {
                position = hit.point;
            }
            else
            {
                position = _player.look.aim.position + _player.look.aim.forward * 1.75f;
            }

            switch (punch)
            {
                case EPlayerPunch.LEFT:
                    _cube.Corner1 = position;
                    break;
                case EPlayerPunch.RIGHT:
                    _cube.Corner2 = position;
                    break;
            }

            _cube.Rotation = _player.transform.rotation;

            Volume? volume = BuildVolume();

            if (volume == null)
                return;

            DisplayVolume(volume);
        }

        protected override Volume? UpdateVolume(IEnumerable<EPlayerKey> keys)
        {
            // Remove positionnal keys
            keys = keys.Where(key => key != EPlayerKey.Crouch && key != EPlayerKey.Prone);

            if (keys.Count() == 0)
                return null;

            foreach (EPlayerKey key in keys)
            {
                switch (key)
                {
                    case EPlayerKey.Hotkey1:
                        _cube.Rotation = _player.transform.rotation;
                        break;

                    default:
                        return null;
                }
            }

            Volume? volume = BuildVolume();

            if (volume == null)
                return null;

            return volume;
        }

        public override Volume? Build()
        {
            Volume? volume = BuildVolume();

            if(volume == null) 
                return null;

            Dispose();
            return volume;
        }

        public override void Cancel()
        {
            Dispose();
        }

        private Volume? BuildVolume()
        {
            return _cube.BuildVolume(_material, _color);
        }

        private class Cube
        {
            public Vector3 Corner1 { get; set; }
            public Vector3 Corner2 { get; set; }
            public Quaternion Rotation { get; set; }

            public Volume? BuildVolume(string material, string color)
            {
                if (Corner1 == default || Corner2 == default || Rotation == default)
                    return null;

                return new Volume()
                {
                    Center = GetCenter(),
                    Shape = "Cube",
                    Material = material,
                    Color = color,
                    Size = GetSize(),
                    Rotation = GetDirection()
                };
            }

            private Vector3 GetCenter()
            {
                Vector3 shapeDiagonal = Corner2 - Corner1;
                return Corner1 + shapeDiagonal / 2;
            }

            private Vector3 GetSize()
            {
                Vector3 shapeDiagonal = Corner2 - Corner1;

                return new Vector3
                (
                    Mathf.Abs(Vector3.Project(shapeDiagonal, Rotation * Vector3.right).magnitude),
                    Mathf.Abs(shapeDiagonal.y),
                    Mathf.Abs(Vector3.Project(shapeDiagonal, Rotation * Vector3.forward).magnitude)
                );
            }

            private Vector3 GetDirection()
            {
                return Rotation * Vector3.forward;
            }
        }
    }
}
