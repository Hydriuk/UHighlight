using SDG.Unturned;
using System;
using UHighlight.API;
using UHighlight.Models;
using UnityEngine;

namespace UHighlight.EditionStrategies
{
    internal class CylinderStrategy : IEditionStrategy
    {
        private Cylinder _cylinder;

        private readonly Player _player;
        private readonly EVolumeMaterial _material;
        private readonly EVolumeColor _color;

        private readonly IEffectBuilder _effectBuilder;

        public CylinderStrategy(
            IEffectBuilder effectBuilder,
            Player player,
            EVolumeMaterial material,
            EVolumeColor color)
        {
            _effectBuilder = effectBuilder;

            _player = player;
            _material = material;
            _color = color;

            _cylinder = new Cylinder();

            PlayerEquipment.OnPunch_Global += OnPunched;

            PlayerInput.onPluginKeyTick += OnPluginKeyTick;
        }

        public void Dispose()
        {
            PlayerEquipment.OnPunch_Global -= OnPunched;

            PlayerInput.onPluginKeyTick -= OnPluginKeyTick;

            Volume? volume = BuildVolume();

            if (volume == null)
                return;

            _effectBuilder.KillEffect(volume, _player);
        }

        private void OnPunched(PlayerEquipment equipment, EPlayerPunch punch)
        {
            if (equipment.player != _player)
                return;

            Vector3 position;
            if (Physics.Raycast(_player.look.aim.position, _player.look.aim.forward, out RaycastHit hit, 2f, RayMasks.DAMAGE_SERVER))
            {
                position = hit.point;
            }
            else
            {
                position = _player.look.aim.position + _player.look.aim.forward * 2f;
            }

            switch (punch)
            {
                case EPlayerPunch.LEFT:
                    _cylinder.Center = position;
                    break;

                case EPlayerPunch.RIGHT:
                    var t = _cylinder.Center - position;
                    _cylinder.Size = new Vector3(t.magnitude, t.magnitude, t.magnitude);
                    break;
            }

            Volume? volume = BuildVolume();

            if (volume == null)
                return;

            _effectBuilder.DisplayEffect(volume, _player, true);
        }

        protected void OnPluginKeyTick(Player player, uint simulation, byte key, bool state)
        {
            if (!state)
                return;

            switch (key)
            {
                case 0:
                    _cylinder.Rotation = Quaternion.Euler((_cylinder.Rotation.eulerAngles.x + 2) % 90, _cylinder.Rotation.eulerAngles.y, _cylinder.Rotation.eulerAngles.z);
                    break;

                case 1:
                    _cylinder.Rotation = Quaternion.Euler(_cylinder.Rotation.eulerAngles.x, (_cylinder.Rotation.eulerAngles.y + 2) % 360, _cylinder.Rotation.eulerAngles.z);
                    break;

                case 2:
                    break;

                case 3:
                    _cylinder.Size *= 1.05f;
                    break;

                case 4:
                    _cylinder.Size /= 1.05f;
                    break;

                default:
                    return;
            }

            Volume? volume = BuildVolume();

            if (volume == null)
                return;

            _effectBuilder.DisplayEffect(volume, _player, true);
        }

        public Volume? Build()
        {
            Volume? volume = BuildVolume();

            if (volume == null)
                return null;

            Dispose();

            return volume;
        }

        public void SetSize(float size)
        {
            throw new NotImplementedException();
        }

        public void Cancel()
        {
            Dispose();
        }

        private Volume? BuildVolume() => _cylinder.BuildVolume(_material, _color);

        private class Cylinder
        {
            public Vector3 Center { get; set; }
            public Vector3 Size { get; set; } = Vector3.one;
            public Quaternion Rotation { get; set; } = Quaternion.Euler(0, 0, 0);

            public Volume? BuildVolume(EVolumeMaterial material, EVolumeColor color)
            {
                if (Center == default || Size == default)
                    return null;

                Console.WriteLine($"{Rotation.eulerAngles.x} {Rotation.eulerAngles.y} {Rotation.eulerAngles.z}");

                return new Volume
                (
                    Center,
                    Size,
                    GetDirection(),
                    EVolumeShape.Cylinder,
                    material,
                    color
                );
            }

            private Vector3 GetDirection()
            {
                return Rotation * Vector3.forward;
            }
        }
    }
}