using ImGuiNET;

namespace BossMod.Log;

public class LogUITree
{
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
