using Hydriuk.UnturnedModules.Adapters;
using Hydriuk.UnturnedModules.PlayerKeys;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using UHighlight.API;
using UHighlight.Models;

namespace UHighlight.VolumeStrategies
{
    public abstract class BaseEditionStrategy : IEditionStrategy
    {
        private readonly Guid _actionReference;
        private readonly List<EPlayerKey> _keys = new List<EPlayerKey>();

        private readonly Player _player;

        private readonly ICoroutineAdapter _coroutineAdapter;
        private readonly IEffectBuilder _effectBuilder;

        public BaseEditionStrategy(ICoroutineAdapter coroutineAdapter, IEffectBuilder effectBuilder, Player player) 
        {
            _coroutineAdapter = coroutineAdapter;
            _effectBuilder = effectBuilder;

            _player = player;

            PlayerKeysListener.KeyStateChanged += OnKeyPressed;

            _player.gameObject.AddComponent<PlayerKeysListener>();
        }

        protected void Init()
        {
            _coroutineAdapter.RunOnFixedUpdate(_actionReference, () =>
            {
                try
                {
                    Volume? volume = UpdateVolume(_keys);

                    if (volume != null)
                        DisplayVolume(volume);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message);
                    Console.Error.WriteLine(ex.StackTrace);
                    Dispose();
                }
            });
        }

        public virtual void Dispose()
        {
            _coroutineAdapter.CancelFixedUpdate(_actionReference);

            PlayerKeysListener.KeyStateChanged -= OnKeyPressed;

            _player.gameObject
                .GetComponent<PlayerKeysListener>()
                .Dispose();
        }

        private void OnKeyPressed(Player player, EPlayerKey key, bool state)
        {
            if (player != _player)
                return;

            if (state)
                _keys.Add(key);
            else
                _keys.Remove(key);
        }

        protected void DisplayVolume(Volume volume)
        {
            _effectBuilder.DisplayEffect(volume, _player, true);
        }

        protected abstract Volume? UpdateVolume(IEnumerable<EPlayerKey> keys);
        public abstract Volume? Build();
        public abstract void Cancel();
    }
}
