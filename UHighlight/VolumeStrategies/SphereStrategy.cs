using SDG.Framework.IO.FormattedFiles.KeyValueTables.TypeReaders.CoreTypes;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Text;
using UHighlight.API;
using UHighlight.Models;
using UnityEngine;

namespace UHighlight.VolumeStrategies
{
    public class SphereStrategy : IEditionStrategy
    {
        private readonly Sphere _sphere;

        private readonly Player _player;
        private readonly string _material;
        private readonly string _color;

        private readonly IEffectBuilder _effectBuilder;

        private float _originalRadius = -1f;
        private bool _keyPressed;

        public SphereStrategy(
            IEffectBuilder effectBuilder,
            Player player,
            string material,
            string color)
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
                3 => _sphere.Radius * 0.95f,
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

        public void Cancel()
        {
            Dispose();
        }


        private Volume? BuildVolume() => _sphere.BuildVolume(_material, _color);

        private class Sphere
        {
            public Vector3 Center { get; set; }
            public float Radius { get; set; }

            public Volume? BuildVolume(string material, string color)
            {
                if (Center == default || Radius == default)
                    return null;

                return new Volume()
                {
                    Center = Center,
                    Shape = "Sphere",
                    Material = material,
                    Color = color,
                    Size = GetSize(),
                    Rotation = Vector3.zero
                };
            }

            private Vector3 GetSize()
            {
                return Vector3.one * Radius * 2;
            }
        }
    }
}
