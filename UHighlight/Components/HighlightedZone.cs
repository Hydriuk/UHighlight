using SDG.Unturned;
using System;
using System.Collections.Generic;
using UHighlight.Models;
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

        public Volume Volume { get; private set; }

        public HighlightedZone()
        {
            Volume = new Volume();
        }

        internal void Init(string category, string name, Volume volume)
        {
            Category = category;
            Name = name;
            Volume = volume;
        }

        public void Dispose()
        {
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
            if (other.transform.tag == "Player")
            {
                Player player = other.GetComponent<Player>();

                Players.Add(player);

                PlayerEntered?.Invoke(this, player);
            }
            else if (other.transform.tag == "Vehicle")
            {
                InteractableVehicle vehicle = other.GetComponent<InteractableVehicle>();

                Vehicles.Add(vehicle);

                VehicleEntered?.Invoke(this, vehicle);
            }
            else if (other.transform.tag == "Agent")
            {
                if(other.TryGetComponent(out Zombie zombie))
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
    }
}
