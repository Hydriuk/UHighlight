#if OPENMOD
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UHighlight.API
{
#if OPENMOD
    [Service]
#endif
    internal interface IChatAdapter
    {
        void Send(Player player, string message);
        void Send(string message);
        void SendError(Player player, string message);
    }
}
