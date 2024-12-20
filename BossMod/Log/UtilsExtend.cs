using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game.Object;

namespace BossMod;

public static partial class Utils
{
    public static IGameObject? GetGameObjectByEntityID(ulong id)
    {
        return (id >> 32) == 0 ? Service.ObjectTable.SearchById((uint)id) : null;
    }

    public static IGameObject? GetGameObjectByEntityID(uint id)
    {
        return Service.ObjectTable.SearchById(id);
    }

    public static IGameObject? GetGameObjectByEntityID(uint? id)
    {
        return id.HasValue ? Service.ObjectTable.SearchById((ulong)id) : null;
    }
}
