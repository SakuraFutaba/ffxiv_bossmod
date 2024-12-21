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
    private readonly DateTime _now = DateTime.UtcNow;

    private void DrawTime()
    {
        ImGui.TextColored(ImGuiColors.DalamudGrey, $"[{_now:HH:mm:ss.fff}] ");
    }
    private void DrawPackedID(PacketID id)
    {
        var color = Enum.IsDefined(typeof(PacketID), id) ? ImGuiColors.ParsedGold : ImGuiColors.DalamudRed;
        ImGui.TextColored(color, $"{id} ");
        if (ImGui.IsItemHovered()) ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
        if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
            ImGui.OpenPopup($"PacketIDMenu##{GetHashCode()}");
        if (ImGui.BeginPopup($"PacketIDMenu##{GetHashCode()}"))
        {
            if (!LogWindow.IsInLogWhiteList(id) && ImGui.MenuItem($"only log this PacketID")) LogWindow.AddToLogWhiteList(id);
            if (!LogWindow.IsInLogBlackList(id) && ImGui.MenuItem($"dont log this PacketID")) LogWindow.AddToLogBlackList(id);
            if (!LogWindow.IsInDrawWhiteList(id) && ImGui.MenuItem($"only show this PacketID")) LogWindow.AddToDrawWhiteList(id);
            if (!LogWindow.IsInDrawBlackList(id) && ImGui.MenuItem($"dont show this PacketID")) LogWindow.AddToDrawBlackList(id);
            ImGui.EndPopup();
        }
    }
    private void DrawActorInfo(ulong id)
    {
        ImGui.TextColored(ImGuiColors.HealerGreen, DecodeActor(Value.SourceServerActor));
        var o = Utils.GetGameObjectByEntityID(id);
        var owner = Utils.GetGameObjectByEntityID(o?.OwnerId);
        if (ImGui.IsItemHovered())
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
            ImGui.BeginTooltip();
            ImGui.TextUnformatted($"""
                                   Name: {o?.Name}
                                   EntityId: {o?.EntityId}
                                   DataId: {o?.DataId}
                                   ObjectKind: {o?.ObjectKind}
                                   OwnerId: {o?.OwnerId:X8}
                                   OwnerName: {owner?.Name}
                                   """);
            ImGui.EndTooltip();
        }
    }
    private void DrawPayload(byte[] payload)
    {
        ImGuiEx.TextWrappedCopy(ImGuiColors.DalamudGrey, string.Join(" ", payload.Select(b => b.ToString("X2"))));
    }
    public ILogNode AddChild(ILogNode child)
    {
        Children.Add(child);
        return child;
    }
    private static string DecodeActor(ulong instanceID) => Utils.ObjectString(instanceID) + " ";
    public override void Draw()
    {
        DrawTime();
        ImGui.SameLine(0, 0);
        ImGui.TextColored(ImGuiColors.HealerGreen, "Server IPC ");
        ImGui.SameLine(0, 0);
        DrawPackedID(Value.ID);
        ImGui.SameLine(0, 0);
        DrawActorInfo(Value.SourceServerActor);
        // ImGui.SameLine(0, 0);
        // ImGui.Text($", sent {(_now - Value.SendTimestamp).TotalMilliseconds:f3}ms ago, epoch={Value.Epoch}, data=");
        ImGui.SameLine(0, 0);
        DrawPayload(Value.Payload);
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

public class CFPreferredRoleNode(CFPreferredRole value) : LogNode<CFPreferredRole>(value)
{
    private readonly CFPreferredRole _value = value;

    public override void Draw()
    {
        var type = typeof(CFPreferredRole);
        var fields = type.GetFields();
        foreach (var field in fields)
        {
            ImGui.Text($"{field.Name}: ");
            ImGui.SameLine(0, 0);
            var color = field.GetValue(Value) switch
            {
                CFRole.Tank => ImGuiColors.TankBlue,
                CFRole.Healer => ImGuiColors.HealerGreen,
                CFRole.DPS => ImGuiColors.DPSRed,
                CFRole.DPS2 => ImGuiColors.DPSRed,
                _ => ImGuiColors.DalamudWhite,
            };
            ImGui.TextColored(color, field.GetValue(Value)?.ToString());
            ImGui.SameLine();
        }
        ImGui.NewLine();
    }
}
