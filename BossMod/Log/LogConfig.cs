using ECommons.Configuration;
using BossMod.Network.ServerIPC;

namespace BossMod.Log;

public class LogConfig : IEzConfig
{
    public bool Autoscroll = false;
    public readonly SortedSet<PacketID> LogWhiteList = [];
    public readonly SortedSet<PacketID> LogBlackList = [];
    public readonly SortedSet<PacketID> DrawWhiteList = [];

    public readonly SortedSet<PacketID> DrawBlackList =
    [
        PacketID.Ping,
        PacketID.ActorControl,
        PacketID.ActorControlSelf,
        PacketID.ActorControlTarget,

        PacketID.SpawnPlayer,
        PacketID.DespawnCharacter,

        PacketID.StatusEffectList,
        PacketID.ActorMove,
        PacketID.UpdateHpMpTp,
        PacketID.ItemInfo,
        PacketID.ContainerInfo,
        PacketID.CurrencyCrystalInfo,
        PacketID.RetainerInformation,
        PacketID.InventoryTransaction,
        PacketID.UpdateInventorySlot
    ];
}
