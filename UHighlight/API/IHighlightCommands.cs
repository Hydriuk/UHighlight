#if OPENMOD
using OpenMod.API.Ioc;
#endif
using SDG.Unturned;
using System.Threading.Tasks;

namespace UHighlight.API
{
#if OPENMOD
    [Service]
#endif
    public interface IHighlightCommands
    {
        Task ExecuteCreate(Player player, string shape, string material, string color);
        Task ExecuteCancel(Player player);
        Task ExecuteValidate(Player player, string group, string zone);
        Task ExecuteDelete(Player player, string group, string zone);
        Task ExecuteShow(Player player, string group, string zone, float customSize = -1);
        Task ExecuteGroups(Player player);
        Task ExecuteVolumes(Player player, string group);
    }
}