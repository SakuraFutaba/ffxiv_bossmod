using BossMod.Network;
using ImGuiNET;

namespace BossMod.Log;

public record class LogMessage(ILogNode RootNode)
{
    public readonly ILogNode RootNode = RootNode;
    public DateTimeOffset Time = DateTimeOffset.UtcNow;

    public void Draw(LogUITree tree)
    {
        tree.DrawTree(RootNode);
    }
    public void DrawTree(ILogNode node)
    {
        var flags = ImGuiTreeNodeFlags.SpanAvailWidth;
        if (node.IsLeaf)
        {
            flags |= ImGuiTreeNodeFlags.Leaf;
        }
        var opened = ImGui.TreeNodeEx($"##LogNode {node.GetHashCode()}", flags);
        ImGui.SameLine();
        node.Draw();

        if (opened)
        {
            node.Children.ForEach(DrawTree);
            ImGui.TreePop();
        }
    }
}
