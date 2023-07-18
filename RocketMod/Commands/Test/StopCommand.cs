using Rocket.Unturned.Player;

namespace UHighlight.RocketMod.Commands.Test
{
    public static class StopCommand
    {
        public static void Execute(UnturnedPlayer uPlayer, string[] command)
        {
            Plugin.Instance.VolumeTester.StopTest(uPlayer.Player);
        }
    }
}
