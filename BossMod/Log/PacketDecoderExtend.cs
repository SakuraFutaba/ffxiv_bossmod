using BossMod.Log;
using System.Runtime.CompilerServices;

namespace BossMod.Network;

public abstract unsafe partial class PacketDecoder
{
    public ServerIPCNode DecodeServerIPCNode(NetworkState.ServerIPC ipc)
    {
        var node = new ServerIPCNode(ipc);
        var child = DecodePacket(ipc.ID, (byte*)Unsafe.AsPointer(ref ipc.Payload[0]));
        if (child != null)
            node.AddChild(child.AsILogNode());
        return node;
    }
}
