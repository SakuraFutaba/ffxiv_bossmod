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
        PacketID.ActorControlSelf,
        PacketID.ContainerInfo,
        PacketID.ItemInfo,
        PacketID.UpdateHpMpTp,
        PacketID.ActorMove,
        PacketID.CurrencyCrystalInfo
    ];
}
