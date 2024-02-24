using SDG.Unturned;
using UHighlight.API;
using UHighlight.Models;
using UnityEngine;

namespace UHighlight.EditionStrategies
{
    internal class SphereStrategy : IEditionStrategy
    {
        private readonly Sphere _sphere;

        private readonly Player _player;
        private readonly EVolumeMaterial _material;
        private readonly EVolumeColor _color;

        private readonly IEffectBuilder _effectBuilder;

        public SphereStrategy(
            IEffectBuilder effectBuilder,
            Player player,
            EVolumeMaterial material,
            EVolumeColor color)
        {
            _effectBuilder = effectBuilder;

            _player = player;
            _material = material;
            _color = color;

            _sphere = new Sphere();

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
                    _sphere.Center = position;
                    break;

                case EPlayerPunch.RIGHT:
                    _sphere.Radius = (_sphere.Center - position).magnitude;
                    break;
            }

            Volume? volume = BuildVolume();

            if (volume == null)
                return;

            _effectBuilder.DisplayEffect(volume, _player, true);
        }

        protected void OnPluginKeyTick(Player player, uint simulation, byte key, bool state)
        {
            if (!state || player != _player)
                return;

            _sphere.Radius = key switch
            {
                0 => _sphere.Radius + 1f,
                1 => _sphere.Radius - 1f,
                2 => _sphere.Radius * 1.05f,
                3 => _sphere.Radius / 1.05f,
                _ => _sphere.Radius
            };

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
            _sphere.Radius = size;

            Volume? volume = BuildVolume();

            if (volume == null)
                return;

            _effectBuilder.DisplayEffect(volume, _player, true);
        }

        public void Cancel()
        {
            Dispose();
        }

        private Volume? BuildVolume() => _sphere.BuildVolume(_material, _color);

        private class Sphere
        {
            public Vector3 Center { get; set; }
            public float Radius { get; set; }

            public Volume? BuildVolume(EVolumeMaterial material, EVolumeColor color)
            {
                if (Center == default || Radius == default)
                    return null;

                return new Volume
                (
                    Center,
                    GetSize(),
                    Vector3.forward,
                    EVolumeShape.Sphere,
                    material,
                    color
                );
            }

            private Vector3 GetSize()
            {
                return Vector3.one * Radius * 2;
            }
        }
    }
}