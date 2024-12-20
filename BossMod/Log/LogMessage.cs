using ImGuiNET;

namespace BossMod.Log;

public record LogMessage(ILogNode RootNode)
{
    public readonly ILogNode RootNode = RootNode;
    public DateTimeOffset Time = DateTimeOffset.UtcNow;

    public static void DrawLogMessage(LogMessage logMessage) => DrawTree(logMessage.RootNode);

    private static void DrawTree(ILogNode node)
    {
        if (node is ServerIPCNode ipcNode && LogWindow.IsNotNeedToDraw(ipcNode)) return;
        var flags = ImGuiTreeNodeFlags.OpenOnArrow;
        if (node.IsLeaf) flags |= ImGuiTreeNodeFlags.Leaf;
        var opened = ImGui.TreeNodeEx($"##LogNode {node.GetHashCode()}", flags);
        ImGui.SameLine();
        node.Draw();

        if (!opened) return;

        node.Children.ForEach(DrawTree);
        ImGui.TreePop();
    }
}
