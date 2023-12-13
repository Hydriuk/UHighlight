using SDG.Unturned;
using UHighlight.API;
using UHighlight.Models;
using UnityEngine;

namespace UHighlight.EditionStrategies
{
    internal class CubeStrategy : IEditionStrategy
    {
        private readonly Cube _cube;

        private readonly Player _player;
        private readonly string _material;
        private readonly string _color;

        private readonly IEffectBuilder _effectBuilder;

        public CubeStrategy(
            IEffectBuilder effectBuilder,
            Player player,
            string material,
            string color)
        {
            _effectBuilder = effectBuilder;

            _player = player;
            _material = material;
            _color = color;

            _cube = new Cube();

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
                    _cube.Corner1 = position;
                    break;

                case EPlayerPunch.RIGHT:
                    _cube.Corner2 = position;
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
                    _cube.Rotation = _player.transform.rotation;
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
            throw new System.NotImplementedException();
        }

        public void Cancel()
        {
            Dispose();
        }

        private Volume? BuildVolume() => _cube.BuildVolume(_material, _color);

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
                    Shape = EVolumeShape.Cube,
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