using ECommons.Configuration;
using BossMod.Network.ServerIPC;

namespace BossMod.Log;

public class LogConfig : IEzConfig
{
    public bool Autoscroll = false;
    public readonly SortedSet<PacketID> LogWhiteList = [];
    public readonly SortedSet<PacketID> LogBlackList =
    [
        PacketID.Ping,
        PacketID.ActorControl,
        PacketID.ActorControlSelf,
        PacketID.ActorControlTarget,

        PacketID.SpawnPlayer,
        PacketID.DespawnCharacter,

        PacketID.ActionEffect1,
        PacketID.ActionEffect8,
        PacketID.ActionEffect16,
        PacketID.ActionEffect24,
        PacketID.ActionEffect32,
        PacketID.EffectResult1,
        PacketID.EffectResult8,
        PacketID.EffectResult16,
        PacketID.EffectResultBasic1,
        PacketID.EffectResultBasic4,
        PacketID.EffectResultBasic8,
        PacketID.EffectResultBasic16,
        PacketID.EffectResultBasic32,
        PacketID.EffectResultBasic64,

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
    public readonly SortedSet<PacketID> DrawWhiteList = [];

    public readonly SortedSet<PacketID> DrawBlackList =
    [

    ];
}
