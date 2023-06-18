using SDG.Unturned;

namespace UHighlight.EventArgs
{
    public class VehicleArgs : ZoneArgs
    {
        public InteractableVehicle Vehicle { get; }

        public VehicleArgs(string category, string name, InteractableVehicle vehicle) : base(category, name)
        {
            Vehicle = vehicle;
        }
    }
}