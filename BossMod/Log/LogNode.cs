using ImGuiNET;
using Dalamud.Interface.Colors;
using BossMod.Network;
using BossMod.Network.ServerIPC;
using ECommons.ImGuiMethods;

namespace BossMod.Log;

public interface ILogNode
{
    List<ILogNode> Children { get; }
    bool IsLeaf => Children.Count == 0;
    ILogNode AddChild(ILogNode child)
    {
        Children.Add(child);
        return child;
    }
    void Draw();
    void Draw(LogUITree tree) => Draw();
}

public class LogNode<T>(T value) : ILogNode
{
    public T Value { get; } = value;
    public List<ILogNode> Children { get; } = [];

    public virtual void Draw()
    {
        ImGui.Text($"Test Draw LogNode {Value?.ToString() ?? "<null>"} {GetHashCode()}");
    }
}

public static class TextNodeExtensions
{
    public static ILogNode AsILogNode(this PacketDecoder.TextNode node)
    {
        return new TextNodeAdapter(node);
    }
    private class TextNodeAdapter : LogNode<string>
    {
        public TextNodeAdapter(PacketDecoder.TextNode node) : base(node.Text)
        {
            node.Children?.ForEach(child => Children.Add(child.AsILogNode()));
        }
        public override void Draw()
        {
            ImGui.Text(Value);
        }
    }
}

public unsafe class ServerIPCNode(NetworkState.ServerIPC ipc) : LogNode<NetworkState.ServerIPC>(ipc)
{
    public readonly NetworkState.ServerIPC ipc = ipc;
    private readonly DateTime _now = DateTime.UtcNow;
    private static string DecodeActor(ulong instanceID) => Utils.ObjectString(instanceID) + " ";

    private void DrawActorInfo(ulong id)
    {
        var o = Utils.GetGameObjectByEntityID(id);
        var oo = Utils.GetGameObjectByEntityID(o?.OwnerId);
        if (ImGui.IsItemHovered())
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
            ImGui.BeginTooltip();
            ImGui.TextUnformatted($"""
                                   Name: {o?.Name}
                                   EntityId: {o?.EntityId}
                                   ObjectKind: {o?.ObjectKind}
                                   OwnerId: {o?.OwnerId:X8}
                                   OwnerName: {oo?.Name}
                                   """);
            ImGui.EndTooltip();
        }
    }

    public ILogNode AddChild(ILogNode child)
    {
        Children.Add(child);
        return child;
    }

    public override void Draw()
    {
        ImGui.TextColored(ImGuiColors.HealerGreen, "Server IPC ");
        ImGui.SameLine(0, 0);
        var color = Enum.IsDefined(typeof(PacketID), ipc.ID) ? ImGuiColors.ParsedGold : ImGuiColors.DalamudRed;
        ImGui.TextColored(color, $"{ipc.ID} ");
        ImGui.SameLine(0, 0);
        ImGui.TextColored(ImGuiColors.HealerGreen, DecodeActor(ipc.SourceServerActor));
        DrawActorInfo(ipc.SourceServerActor);
        // ImGui.SameLine(0, 0);
        // ImGui.Text($", sent {(_now - ipc.SendTimestamp).TotalMilliseconds:f3}ms ago, epoch={ipc.Epoch}, data=");
        ImGui.SameLine(0, 0);
        ImGuiEx.TextWrappedCopy(ImGuiColors.DalamudGrey, string.Join(" ", ipc.Payload.Select(b => b.ToString("X2"))));
    }
}
public unsafe class CountdownNode(Countdown value) : LogNode<Countdown>(value)
{
    private readonly Countdown _value = value;

    public override void Draw()
    {
        fixed (byte* textPtr = _value.Text)
        {
            string text = new string((sbyte*)textPtr).TrimEnd('\0');
            if (ImGui.Selectable($"Countdown: Sender={Value.SenderID}, Time={Value.Time}, Text={text}"))
            {
            }
        }
    }
}
