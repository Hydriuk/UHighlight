using SDG.Unturned;

namespace UHighlight.EventArgs
{
    public class PlayerArgs : ZoneArgs
    {
        public Player Player { get; }

        public PlayerArgs(string category, string name, Player player) : base(category, name)
        {
            Player = player;
        }
    }
}