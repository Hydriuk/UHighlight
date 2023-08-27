using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UHighlight.API
{
    public interface IHighlightAdapter
    {
        Task ExecuteCreate(Player player, string shape, string material, string color);
        Task ExecuteCancel(Player player);
        Task ExecuteValidate(Player player, string group, string zone);
        Task ExecuteDelete(Player player, string group, string zone);
        Task ExecuteShow(Player player, string group, string zone);
        Task ExecuteGroups(Player player);
        Task ExecuteVolumes(Player player, string group);
    }
}
