using ImGuiNET;
using BossMod.Network;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Style;
using ECommons.ImGuiMethods;
using ECommons.CircularBuffers;

namespace BossMod.Log;

public class LogWindow() : UIWindow("Boss mod log UI", false, new(1000, 300))
{
    private static readonly CircularBuffer<LogMessage> LogMessageBuffer = new(1000);
    private static bool _autoscroll = true;
    private readonly UITree _tree = new();

    public override void Draw()
    {
        ImGui.Checkbox("##Autoscroll", ref _autoscroll);
        ImGuiEx.Tooltip("Autoscroll");
        ImGui.SameLine();
        if (ImGuiEx.IconButton(FontAwesome.Trash))
        {
            LogMessageBuffer.Clear();
        }
        ImGuiEx.Tooltip("Clear All");
        ImGui.SameLine();
        if (ImGuiEx.IconButton(FontAwesome.Plus))
        {
            LogMessageBuffer.PushBack(CreateTestMsg());
        }

        ImGui.BeginChild($"Boss mod Log");

        DrawSampleNode();

        LogMessageBuffer.ToList().ForEach(logMessage => logMessage.Draw(_tree));

        if(_autoscroll)
        {
            ImGui.SetScrollHereY();
        }
        ImGui.EndChild();
    }

    public static void Log(PacketDecoder.TextNode textNode)
    {
        LogMessageBuffer.PushBack(new LogMessage(textNode));
    }

    private LogMessage CreateTestMsg()
    {
        PacketDecoder.TextNode node1 = new($"{DateTime.Now:[HH:mm:ss.fff]} 1");
        PacketDecoder.TextNode node2 = new("2");
        PacketDecoder.TextNode node3 = new("3");
        PacketDecoder.TextNode node4 = new("4");
        PacketDecoder.TextNode node5 = new("5");
        PacketDecoder.TextNode node6 = new("6");

        node2.AddChild(node3);
        node4.AddChild(node5).AddChild(node6);
        node1.AddChild(node2);
        node1.AddChild(node4);

        return new LogMessage(node1);
    }

    private void DrawSampleNode()
    {
        bool isopen = ImGui.TreeNodeEx("##CustomNode1", ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.AllowItemOverlap | ImGuiTreeNodeFlags.SpanAvailWidth);
        ImGui.SameLine();
        ImGui.Text("Custom ");
        ImGui.SameLine(0, 0);
        ImGui.TextColored(ImGuiColors.DalamudYellow, "Node 1");
        if (isopen)
        {
            bool isopen2 = ImGui.TreeNodeEx("##CustomNode2", ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.SpanAvailWidth);
            ImGui.SameLine();
            ImGui.Text("This is ");
            ImGui.SameLine(0, 0);
            ImGui.TextColored(ImGuiColors.DalamudYellow, "Custom ");
            ImGuiEx.Tooltip("Custom ");
            // ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudYellow);
            // ImGui.Selectable("Custom ", false, ImGuiSelectableFlags.AllowItemOverlap);
            // ImGui.PopStyleColor();
            ImGui.SameLine(0, 0);
            ImGui.TextColored(ImGuiColors.DalamudRed, "Node 2");
            if (isopen2)
            {
                ImGui.TreePop();
            }
            ImGui.TreePop();
        }

        if (ImGui.TreeNodeEx("##CustomNode3", ImGuiTreeNodeFlags.DefaultOpen))
        {
            ImGui.SameLine();
            var textStart = ImGui.GetCursorScreenPos();
            // var cursorPos = ImGui.GetCursorPos();
            var drawList = ImGui.GetWindowDrawList();
            drawList.AddText(textStart, ImGui.ColorConvertFloat4ToU32(ImGuiColors.DalamudWhite), "Custom");
            var part1Size = ImGui.CalcTextSize("Custom");
            textStart.X += part1Size.X;
            drawList.AddText(textStart, ImGui.ColorConvertFloat4ToU32(ImGuiColors.DalamudYellow), " Node 3");
            var fullTextSize = ImGui.CalcTextSize("Custom Node 3");
            ImGui.Dummy(new System.Numerics.Vector2(fullTextSize.X, fullTextSize.Y));

            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("This is Custom Node 3!");
            }

            // 子节点
            // if (ImGui.TreeNodeEx("##CustomNode4", ImGuiTreeNodeFlags.Leaf))
            // {
            //     ImGui.SameLine();
            //     var cursorPos4 = ImGui.GetCursorPos();
            //     ImGui.Text("Custom");
            //     ImGui.SameLine(0, 0);
            //     ImGui.TextColored(ImGuiColors.DalamudYellow, " Node 4");
            //
            //     ImGui.TreePop();
            // }
            ImGui.TreePop();
        }
    }
}
