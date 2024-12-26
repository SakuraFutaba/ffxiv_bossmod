using BossMod.Log;
using BossMod.Network.ServerIPC;
using System.Runtime.CompilerServices;

namespace BossMod.Network;

public abstract unsafe partial class PacketDecoder
{
    public ServerIPCNode DecodeServerIPCNode(NetworkState.ServerIPC ipc)
    {
        var node = new ServerIPCNode(ipc);
        var ptr = (byte*)Unsafe.AsPointer(ref ipc.Payload[0]);
        var child = ipc.ID switch
        {
            PacketID.CFPreferredRole when (CFPreferredRole*)ptr is var p => new CFPreferredRoleNode(*p),
            PacketID.PFUpdateRecruitNum when (PFUpdateRecruitNum*)ptr is var p => new PFUpdateRecruitNumNode(*p),
            PacketID.Mount when (Mount*)ptr is var p => new MountNode(*p),
            PacketID.SpawnNPC when (SpawnNPC*)ptr is var p => new SpawnNPCNode(*p),
            PacketID.FirstAttack when (FirstAttack*)ptr is var p => new FirstAttackNode(*p),
            _ => DecodePacket(ipc.ID, ptr)?.AsILogNode(),
        };
        if (child != null)
            node.AddChild(child);
        return node;
    }
}
