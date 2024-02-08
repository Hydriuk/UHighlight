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

        private readonly IThreadAdapter _threadAdapter;

        public ChatAdapter(/*IConfigurationAdapter<Configuration> configurationAdapter,*/ IThreadAdapter threadAdapter)
        {
            _threadAdapter = threadAdapter;

            //Configuration configuration = configurationAdapter.Configuration;

            _iconUrl = "";
        }

        public void Send(Player player, string message)
        {
            _threadAdapter.RunOnMainThread(() =>
            {
                ChatManager.serverSendMessage(message, Color.green, toPlayer: player.GetSteamPlayer(), iconURL: _iconUrl);
            });
        }

        public void Send(string message)
        {
            _threadAdapter.RunOnMainThread(() =>
            {
                ChatManager.serverSendMessage(message, Color.green, iconURL: _iconUrl);
            });
        }

        public void SendError(Player player, string message)
        {
            _threadAdapter.RunOnMainThread(() =>
            {
                ChatManager.serverSendMessage(message, Color.red, toPlayer: player.GetSteamPlayer(), iconURL: _iconUrl);
            });
        }
    }
}
