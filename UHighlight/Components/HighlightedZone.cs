using SDG.Unturned;
using System;
using UHighlight.EventArgs;
using UHighlight.Models;
using UnityEngine;

namespace UHighlight.Components
{
    public class HighlightedZone : MonoBehaviour, IDisposable
    {
        private string _category = string.Empty;
        private string _name = string.Empty;

        public event EventHandler<PlayerArgs>? PlayerEntered;
        public event EventHandler<PlayerArgs>? PlayerExited;

        public event EventHandler<VehicleArgs>? VehicleEntered;
        public event EventHandler<VehicleArgs>? VehicleExited;

        //public event EventHandler<ZombieArgs>? ZombieEntered;
        //public event EventHandler<ZombieArgs>? ZombieExited;

        public Volume Volume { get; private set; }

        public HighlightedZone()
        {
            Volume = new Volume();
        }

        internal void Init(string category, string name, Volume volume)
        {
            _category = category;
            _name = name;
            Volume = volume;
        }

        public void Dispose()
        {
            PlayerEntered = null;
            PlayerExited = null;
            VehicleEntered = null;
            VehicleExited = null;
            //ZombieEntered = null;
            //ZombieExited = null;

            Destroy(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.tag == "Player")
            {
                Player player = other.GetComponent<Player>();

                PlayerEntered?.Invoke(this, new PlayerArgs(_category, _name, player));
            }
            else if (other.transform.tag == "Vehicle")
            {
                InteractableVehicle vehicle = other.GetComponent<InteractableVehicle>();

                VehicleEntered?.Invoke(this, new VehicleArgs(_category, _name, vehicle));
            }
            //else if (other.transform.tag == "Zombie")
            //{
            //    Zombie zombie = other.GetComponent<Zombie>();

            //    ZombieEntered?.Invoke(this, new ZombieArgs(_category, _name, zombie));
            //}
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.transform.tag == "Player")
            {
                Player player = other.GetComponent<Player>();

                PlayerExited?.Invoke(this, new PlayerArgs(_category, _name, player));
            }
            else if (other.transform.tag == "Vehicle")
            {
                InteractableVehicle vehicle = other.GetComponent<InteractableVehicle>();

                VehicleExited?.Invoke(this, new VehicleArgs(_category, _name, vehicle));
            }
            //else if (other.transform.tag == "Zombie")
            //{
            //    Zombie zombie = other.GetComponent<Zombie>();

            //    ZombieExited?.Invoke(this, new ZombieArgs(_category, _name, zombie));
            //}
        }
    }
}
