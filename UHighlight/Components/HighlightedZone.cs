using SDG.Unturned;
using System;
using System.Collections.Generic;
using UHighlight.Extensions;
using UHighlight.Models;
using UHighlight.Patches;
using UnityEngine;

namespace UHighlight.Components
{
    public class HighlightedZone : MonoBehaviour, IDisposable
    {
        public string Category { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public List<Player> Players { get; } = new List<Player>();
        public event EventHandler<Player>? PlayerEntered;
        public event EventHandler<Player>? PlayerExited;

        public List<InteractableVehicle> Vehicles { get; } = new List<InteractableVehicle>();
        public event EventHandler<InteractableVehicle>? VehicleEntered;
        public event EventHandler<InteractableVehicle>? VehicleExited;

        public List<Zombie> Zombies { get; } = new List<Zombie>();
        public event EventHandler<Zombie>? ZombieEntered;
        public event EventHandler<Zombie>? ZombieExited;

        public List<Animal> Animals { get; } = new List<Animal>();
        public event EventHandler<Animal>? AnimalEntered;
        public event EventHandler<Animal>? AnimalExited;

        public List<BarricadeDrop> Barricades { get; } = new List<BarricadeDrop>();
        public event EventHandler<BarricadeDrop>? BarricadeEntered;
        public event EventHandler<BarricadeDrop>? BarricadeExited;

        public List<StructureDrop> Structures { get; } = new List<StructureDrop>();
        public event EventHandler<StructureDrop>? StructureEntered;
        public event EventHandler<StructureDrop>? StructureExited;

        public Volume Volume { get; private set; }

        internal Collider Collider;

        internal HighlightedZone()
        {
            Volume = new Volume();
            Collider = new Collider();
        }

        internal void Init(string category, string name, Volume volume)
        {
            Category = category;
            Name = name;
            Volume = volume;

            if (!TryGetComponent<Collider>(out Collider))
                throw new Exception("You must set a collider on the HighlightedZone before calling Init()");

            Barricades.AddRange(this.GetBarricades());
            Structures.AddRange(this.GetStructures());

            BarricadeManager.onBarricadeSpawned += OnBarricadeSpawned;
            StructureManager.onStructureSpawned += OnStructureSpawned;

            BarricadeDestroyedPatch.BarricadeDestroyed += OnBarricadeDestroyed;
            StructureDestroyedPatch.StructureDestroyed += OnStructureDestroyed;
        }

        private void Start()
        {
            // Call first time entered events on start to let some time for the plugins to subscribe the events
            Barricades.ForEach(barricade => BarricadeEntered?.Invoke(this, barricade));
            Structures.ForEach(structure => StructureEntered?.Invoke(this, structure));
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
    }
}