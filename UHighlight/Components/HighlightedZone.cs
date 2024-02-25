using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using UHighlight.API;
using UHighlight.Extensions;
using UHighlight.Models;
using UHighlight.Patches;
using UnityEngine;

namespace UHighlight.Components
{
    public class HighlightedZone : MonoBehaviour, IDisposable
    {
        public string Group { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        #region Entites
        public HashSet<Player> Players { get; } = new HashSet<Player>();
        public event EventHandler<Player>? PlayerEntered;
        public event EventHandler<Player>? PlayerExited;

        public HashSet<InteractableVehicle> Vehicles { get; } = new HashSet<InteractableVehicle>();
        public event EventHandler<InteractableVehicle>? VehicleEntered;
        public event EventHandler<InteractableVehicle>? VehicleExited;

        public HashSet<Zombie> Zombies { get; } = new HashSet<Zombie>();
        public event EventHandler<Zombie>? ZombieEntered;
        public event EventHandler<Zombie>? ZombieExited;

        public HashSet<Animal> Animals { get; } = new HashSet<Animal>();
        public event EventHandler<Animal>? AnimalEntered;
        public event EventHandler<Animal>? AnimalExited;

        public HashSet<BarricadeDrop> Barricades { get; } = new HashSet<BarricadeDrop>();
        public event EventHandler<BarricadeDrop>? BarricadeEntered;
        public event EventHandler<BarricadeDrop>? BarricadeExited;

        public HashSet<StructureDrop> Structures { get; } = new HashSet<StructureDrop>();
        public event EventHandler<StructureDrop>? StructureEntered;
        public event EventHandler<StructureDrop>? StructureExited;
        #endregion

        #region Display events
        private Dictionary<object, Provider.ServerConnected> _onServerConnectedActions = new Dictionary<object, Provider.ServerConnected>();
        #endregion

        public Volume Volume { get; private set; }

        internal Collider Collider;

        private IEffectBuilder? _effectBuilder;

        internal HighlightedZone()
        {
            Volume = new Volume();
            Collider = new Collider();
        }

        internal void Init(IEffectBuilder effectBuilder, string group, string name, Volume volume)
        {
            _effectBuilder = effectBuilder;
            Group = group;
            Name = name;
            Volume = volume;

            if (!TryGetComponent<Collider>(out Collider))
                throw new Exception("You must set a collider on the HighlightedZone before calling Init()");

            foreach (var barricade in this.GetBarricades())
                Barricades.Add(barricade);
            foreach (var structure in this.GetStructures())
                Structures.Add(structure);

            BarricadeManager.onBarricadeSpawned += OnBarricadeSpawned;
            StructureManager.onStructureSpawned += OnStructureSpawned;

            BarricadeDestroyedPatch.BarricadeDestroyed += OnBarricadeDestroyed;
            StructureDestroyedPatch.StructureDestroyed += OnStructureDestroyed;
        }

        private void Start()
        {
            // Call first time entered events on start to let some time for the plugins to subscribe the events
            foreach (var barricade in Barricades)
                BarricadeEntered?.Invoke(this, barricade);
            foreach (var structure in Structures)
                StructureEntered?.Invoke(this, structure);
        }

        public void Dispose()
        {
            BarricadeManager.onBarricadeSpawned -= OnBarricadeSpawned;
            StructureManager.onStructureSpawned -= OnStructureSpawned;

            BarricadeDestroyedPatch.BarricadeDestroyed -= OnBarricadeDestroyed;
            StructureDestroyedPatch.StructureDestroyed -= OnStructureDestroyed;

            PlayerEntered = null;
            PlayerExited = null;
            VehicleEntered = null;
            VehicleExited = null;
            ZombieEntered = null;
            ZombieExited = null;
            AnimalEntered = null;
            AnimalExited = null;

            Destroy(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag("Player"))
            {
                Player player = other.GetComponent<Player>();

                Players.Add(player);

                PlayerEntered?.Invoke(this, player);
            }
            else if (other.transform.CompareTag("Vehicle"))
            {
                InteractableVehicle vehicle = other.GetComponent<InteractableVehicle>();

                Vehicles.Add(vehicle);

                VehicleEntered?.Invoke(this, vehicle);
            }
            else if (other.transform.CompareTag("Agent"))
            {
                if (other.TryGetComponent(out Zombie zombie))
                {
                    Zombies.Add(zombie);

                    ZombieEntered?.Invoke(this, zombie);
                }
                else if (other.TryGetComponent(out Animal animal))
                {
                    Animals.Add(animal);

                    AnimalEntered?.Invoke(this, animal);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.transform.tag == "Player")
            {
                Player player = other.GetComponent<Player>();

                Players.Remove(player);

                PlayerExited?.Invoke(this, player);
            }
            else if (other.transform.tag == "Vehicle")
            {
                InteractableVehicle vehicle = other.GetComponent<InteractableVehicle>();

                Vehicles.Remove(vehicle);

                VehicleExited?.Invoke(this, vehicle);
            }
            else if (other.transform.tag == "Agent")
            {
                if (other.TryGetComponent(out Zombie zombie))
                {
                    Zombies.Remove(zombie);

                    ZombieExited?.Invoke(this, zombie);
                }
                else if (other.TryGetComponent(out Animal animal))
                {
                    Animals.Remove(animal);

                    AnimalExited?.Invoke(this, animal);
                }
            }
        }

        private void OnBarricadeSpawned(BarricadeRegion region, BarricadeDrop drop) => OnBarricadeSpawned(drop);
        private void OnBarricadeSpawned(BarricadeDrop drop)
        {
            if (this.Collides(drop))
            {
                Barricades.Add(drop);
                BarricadeEntered?.Invoke(this, drop);
            }
        }

        private void OnStructureSpawned(StructureRegion region, StructureDrop drop) => OnStructureSpawned(drop);
        private void OnStructureSpawned(StructureDrop drop)
        {
            if (this.Collides(drop))
            {
                Structures.Add(drop);
                StructureEntered?.Invoke(this, drop);
            }
        }

        private void OnBarricadeDestroyed(BarricadeDrop drop)
        {
            if (Barricades.Contains(drop))
            {
                Barricades.Remove(drop);
                BarricadeExited?.Invoke(this, drop);
            }
        }

        private void OnStructureDestroyed(StructureDrop drop)
        {
            if (Structures.Contains(drop))
            {
                Structures.Remove(drop);
                StructureExited?.Invoke(this, drop);
            }
        }

        /// <summary>
        /// Displays the effect to all players
        /// </summary>
        /// <param name="unique">If true, effects with same shape and color will be cleared</param>
        public void Show(bool unique = false)
        {
            _effectBuilder?.DisplayEffect(Volume, unique);

            if (!_onServerConnectedActions.ContainsKey(true))
            {
                Provider.ServerConnected showAction = (CSteamID playerId) =>
                {
                    _effectBuilder?.DisplayEffect(Volume, PlayerTool.getPlayer(playerId), unique);
                };

                _onServerConnectedActions.Add(true, showAction);

                Provider.onServerConnected += showAction;
            }
        }

        /// <summary>
        /// Displays the effect to a player
        /// </summary>
        /// <param name="player">Player to display the effect to</param>
        /// <param name="unique">If true, effects with same shape and color will be cleared</param>
        public void Show(Player player, bool unique = false)
        {
            _effectBuilder?.DisplayEffect(Volume, player);

            if (!_onServerConnectedActions.ContainsKey(player))
            {
                Provider.ServerConnected showAction = (CSteamID playerId) =>
                {
                    Player connectedPlayer = PlayerTool.getPlayer(playerId);

                    if (connectedPlayer == player)
                    {
                        _effectBuilder?.DisplayEffect(Volume, connectedPlayer, unique);
                    }
                };

                _onServerConnectedActions.Add(player, showAction);

                Provider.onServerConnected += showAction;
            }
        }

        /// <summary>
        /// Displays the effect to some players
        /// </summary>
        /// <param name="player">Players to display the effect to</param>
        /// <param name="unique">If true, effects with same shape and color will be cleared</param>
        public void Show(IEnumerable<Player> players, bool unique = false)
        {
            _effectBuilder?.DisplayEffect(Volume, players);

            if (!_onServerConnectedActions.ContainsKey(players))
            {
                Provider.ServerConnected showAction = (CSteamID playerId) =>
                {
                    Player connectedPlayer = PlayerTool.getPlayer(playerId);

                    if (players.Contains(connectedPlayer))
                    {
                        _effectBuilder?.DisplayEffect(Volume, connectedPlayer, unique);
                    }
                };

                _onServerConnectedActions.Add(players, showAction);

                Provider.onServerConnected += showAction;
            }
        }

        /// <summary>
        /// Kill the effect
        /// </summary>
        public void Hide()
        {
            _effectBuilder?.KillEffect(Volume);

            if(_onServerConnectedActions.TryGetValue(true, out var action))
            {
                Provider.onServerConnected -= action;

                _onServerConnectedActions.Remove(true);
            }
        }

        /// <summary>
        /// Kill the effect for a player
        /// </summary>
        /// <param name="player">Player for whom the kill the effect</param>
        public void Hide(Player player)
        {
            _effectBuilder?.KillEffect(Volume, player);

            if (_onServerConnectedActions.TryGetValue(player, out var action))
            {
                Provider.onServerConnected -= action;

                _onServerConnectedActions.Remove(player);
            }
        }

        /// <summary>
        /// Kill the effect for some players
        /// </summary>
        /// <param name="players">Players for whom to kill the effect</param>
        public void Hide(IEnumerable<Player> players)
        {
            _effectBuilder?.KillEffect(Volume, players);

            if (_onServerConnectedActions.TryGetValue(players, out var action))
            {
                Provider.onServerConnected -= action;

                _onServerConnectedActions.Remove(players);
            }
        }
    }
}