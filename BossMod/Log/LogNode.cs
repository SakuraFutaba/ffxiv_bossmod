using ImGuiNET;
using BossMod.Network;
using BossMod.Network.ServerIPC;
using Dalamud.Interface.Colors;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Memory;
using ECommons.ImGuiMethods;

namespace BossMod.Log;

public static class LogColor
{
    public static Vector4 Number { get; internal set; } = new Vector4(0.929f, 0.580f, 0.753f, 1f);   // ED94C0
    public static Vector4 Property { get; internal set; } = new Vector4(0.400f, 0.765f, 0.800f, 1f); // 66C3CC
    public static Vector4 Parameter { get; internal set; } = new Vector4(0.741f, 0.741f, 0.741f, 1f);// BDBDBD
    public static Vector4 Keywords { get; internal set; } = new Vector4(0.424f, 0.584f, 0.922f, 1f); // 6C95EB
    public static Vector4 String { get; internal set; } = new Vector4(0.788f, 0.635f, 0.427f, 1f);   // C9A26D
    public static Vector4 Methods { get; internal set; } = new Vector4(0.224f, 0.800f, 0.608f, 1f);  // 39CC9B
    public static Vector4 Class { get; internal set; } = new Vector4(0.757f, 0.569f, 1f, 1f);    // C191FF
    public static Vector4 Enum { get; internal set; } = new Vector4(0.882f, 0.749f, 1f, 1f);     // E1BFFF
    public static Vector4 Comment { get; internal set; } = new Vector4(0.522f, 0.769f, 0.424f, 1f);     // 85C46C
}
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
        var type = typeof(T);
        var fields = type.GetFields();
        var count = 0;
        foreach (var field in fields)
        {
            ImGui.TextColored(LogColor.Property, $"{field.Name}: ");
            ImGui.SameLine(0, 0);
            var value = field.GetValue(Value);
            var formattedValue = value switch
            {
                ulong ulongValue => $"{ulongValue:X16} ",
                uint uintValue and >= 1 << 25 => $"{uintValue:X8} ",
                _ => $"{value} "
            };
            ImGui.TextColored(LogColor.Number, formattedValue);
            ImGui.SameLine(0, 0);

            count++;
            if (count % 20 == 0) ImGui.NewLine();
        }
        ImGui.NewLine();
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

public class ServerIPCNode(NetworkState.ServerIPC ipc) : LogNode<NetworkState.ServerIPC>(ipc)
{
    private readonly DateTimeOffset _now = DateTimeOffset.Now;
    private string _payloadStr = ipc.Payload.ToHexString();
    private readonly GameObjectInfo? _info = new();

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
            if (ImGui.MenuItem($"only log this PacketID")) LogWindow.AddToLogWhiteList(id);
            if (ImGui.MenuItem($"dont log this PacketID")) LogWindow.AddToLogBlackList(id);
            if (ImGui.MenuItem($"only show this PacketID")) LogWindow.AddToDrawWhiteList(id);
            if (ImGui.MenuItem($"dont show this PacketID")) LogWindow.AddToDrawBlackList(id);
            ImGui.EndPopup();
        }
    }
    private void DrawActorInfo()
    {
        ImGui.TextColored(ImGuiColors.HealerGreen, ObjectString(Value.SourceServerActor));
        if (ImGui.IsItemHovered())
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
            ImGui.BeginTooltip();
            ImGui.TextUnformatted($"""
                                   Name: {_info?.Name}
                                   EntityId: {_info?.EntityId:X8}
                                   DataId: {_info?.DataId:X4}
                                   ObjectKind: {_info?.ObjectKind}
                                   OwnerId: {_info?.OwnerId:X8}
                                   OwnerName: {_info?.OwnerName}
                                   """);
            ImGui.EndTooltip();
        }
    }
    private void DrawPayload(byte[] payload)
    {
        ImGuiEx.TextWrappedCopy(ImGuiColors.DalamudGrey, _payloadStr);
        if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
            ImGui.OpenPopup($"PayloadMenu##{GetHashCode()}");
        if (ImGui.BeginPopup($"PayloadMenu##{GetHashCode()}"))
        {
            if (ImGui.MenuItem($"Convert to Byte")) _payloadStr = payload.ToByteString();
            if (ImGui.MenuItem($"Convert to Ushort")) _payloadStr = payload.ToUshortString();
            if (ImGui.MenuItem($"Convert to Int")) _payloadStr = payload.ToIntString();
            if (ImGui.MenuItem($"Convert to UInt")) _payloadStr = payload.ToUIntString();
            if (ImGui.MenuItem($"Convert to Float")) _payloadStr = payload.ToFloatString();
            if (ImGui.MenuItem($"Convert to Ulong")) _payloadStr = payload.ToUlongString();
            if (ImGui.MenuItem($"Raw Data")) _payloadStr = payload.ToHexString();
            ImGui.EndPopup();
        }
    }
    public ILogNode AddChild(ILogNode child)
    {
        Children.Add(child);
        return child;
    }
    private string ObjectString(ulong id) => $"'{_info?.Name ?? "(not found)"}' <{id:X}> ";
    public override void Draw()
    {
        DrawTime();
        _info?.UpdateGameObjectInfoByEntityID(Value.SourceServerActor);
        ImGui.SameLine(0, 0);
        ImGui.TextColored(ImGuiColors.HealerGreen, "Server IPC ");
        ImGui.SameLine(0, 0);
        DrawPackedID(Value.ID);
        ImGui.SameLine(0, 0);
        DrawActorInfo();
        // ImGui.SameLine(0, 0);
        // ImGui.Text($", sent {(_now - Value.SendTimestamp).TotalMilliseconds:f3}ms ago, epoch={Value.Epoch}, data=");
        ImGui.SameLine(0, 0);
        DrawPayload(Value.Payload);
    }
}
public unsafe class CountdownNode(Countdown x) : LogNode<Countdown>(x)
{
    public override void Draw()
    {
        ImGui.Text($"Countdown: Sender={Value.SenderID}, Time={Value.Time}");
    }
}

public class CFPreferredRoleNode(CFPreferredRole x) : LogNode<CFPreferredRole>(x)
{
    public override void Draw()
    {
        var type = typeof(CFPreferredRole);
        var fields = type.GetFields();
        foreach (var field in fields)
        {
            ImGui.TextColored(LogColor.Property,$"{field.Name}: ");
            ImGui.SameLine(0, 0);
            var color = field.GetValue(Value) switch
            {
                CFRole.Tank => ImGuiColors.TankBlue,
                CFRole.Healer => ImGuiColors.HealerGreen,
                CFRole.DPS => ImGuiColors.DPSRed,
                CFRole.DPS2 => ImGuiColors.DPSRed,
                _ => LogColor.Number,
            };
            ImGui.TextColored(color, field.GetValue(Value)?.ToString());
            ImGui.SameLine();
        }
        ImGui.NewLine();
    }
}

public class PFUpdateRecruitNumNode(PFUpdateRecruitNum x) : LogNode<PFUpdateRecruitNum>(x)
{
}

public class MountNode(Mount x) : LogNode<Mount>(x)
{
    public override void Draw()
    {
        var mountName =
            (Service.LuminaRow<Lumina.Excel.Sheets.Mount>(Value.MountID)?.Singular.ToString() ?? "<not found>") + $"({Value.MountID}) ";
        ImGui.TextColored(LogColor.Property, "Mount: ");
        ImGui.SameLine(0, 0);
        ImGui.TextColored(LogColor.String, mountName);
        if (Value.MountID == 1)
        {
            var color = Service.LuminaRow<Lumina.Excel.Sheets.Stain>(Value.StainID)?.Color ?? 0xBDBDBD;
            var colorName = Service.LuminaRow<Lumina.Excel.Sheets.Stain>(Value.StainID)?.Name.ToString();
            ImGui.SameLine(0, 0);
            ImGui.TextColored(Utils.UIntToImGuiColor(color), $"Color: {colorName}#{color:X6} ModelTop: {Value.ModelTop} ModelBody: {Value.ModelBody} ModelLegs: {Value.ModelLegs}");
        }
    }
}

public unsafe class SpawnNPCNode(SpawnNPC x) : LogNode<SpawnNPC>(x)
{
    public override void Draw()
    {
        base.Draw();
        var value = Value;
        var p = (IntPtr)value.NPCName;
        var str = MemoryHelper.ReadString(p, 74);
        ImGui.TextColored(LogColor.String, str);
    }
}

public class FirstAttackNode(FirstAttack x) : LogNode<FirstAttack>(x)
{
    private readonly GameObjectInfo _info = new();
    public override void Draw()
    {
        ImGui.TextColored(LogColor.Property, "type: ");
        ImGui.SameLine(0, 0);
        ImGui.TextColored(LogColor.Enum, Value.Type.ToString());
        switch (Value.Type)
        {
            case 0:
                return;
            case 1:
                ImGui.SameLine(0, 0);
                ImGui.TextColored(LogColor.Property, _info.ObjectAndOwnerString(Value.ID));
                break;
            case 2:
                ImGui.SameLine(0, 0);
                ImGui.TextColored(LogColor.Property, $"{Value.ID:X8} {Value.U2:X8}");
                break;
        }
    }
}
