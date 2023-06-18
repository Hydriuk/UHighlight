using SDG.Unturned;

namespace UHighlight.EventArgs
{
    public class ZombieArgs : ZoneArgs
    {
        public Zombie Zombie { get; }

        public ZombieArgs(string category, string name, Zombie zombie) : base(category, name)
        {
            Zombie = zombie;
        }
    }
}