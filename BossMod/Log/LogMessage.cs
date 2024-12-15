using BossMod.Network;

namespace BossMod.Log;

public record class LogMessage(PacketDecoder.TextNode TextNode)
{
    public readonly PacketDecoder.TextNode TextNode = TextNode;
    public DateTimeOffset Time = DateTimeOffset.Now;

    public void Draw(UITree tree)
    {
        DrawNodes(tree, TextNode);
    }

    private void DrawNodes(UITree tree, PacketDecoder.TextNode node)
    {
        DrawNodes(tree, new List<PacketDecoder.TextNode> { node });
    }

    private void DrawNodes(UITree tree, IEnumerable<PacketDecoder.TextNode>? nodes)
    {
        if (nodes == null)
            return;
        foreach (var n in tree.Nodes(nodes, n => new(n.Text, n.Children == null)))
            DrawNodes(tree, n.Children);
    }
}
