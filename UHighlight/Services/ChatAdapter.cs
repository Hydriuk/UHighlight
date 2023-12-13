#if OPENMOD
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
#endif
using Hydriuk.UnturnedModules.Adapters;
using Hydriuk.UnturnedModules.Extensions;
using SDG.Unturned;
using UHighlight.API;
using UnityEngine;

namespace UHighlight.Services
{
#if OPENMOD
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
#endif
    internal class ChatAdapter : IChatAdapter
    {
        private readonly string _iconUrl;

        public ChatAdapter(IConfigurationAdapter<Configuration> config, IThreadAdapter threadAdapter)
        {
            _iconUrl = config.Configuration.IconURL;
        }

        public void Send(Player player, string message)
        {
            ChatManager.serverSendMessage(message, Color.green, toPlayer: player.GetSteamPlayer(), iconURL: _iconUrl);
        }

        public void Send(string message)
        {
            ChatManager.serverSendMessage(message, Color.green, iconURL: _iconUrl);
        }

        public void SendError(Player player, string message)
        {
            ChatManager.serverSendMessage(message, Color.red, toPlayer: player.GetSteamPlayer(), iconURL: _iconUrl);
        }
    }
}
