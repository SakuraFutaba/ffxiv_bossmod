using ImGuiNET;
using BossMod.Network;

namespace BossMod.Log;

public interface INode<out T>
{
    T Value { get; }
    IEnumerable<INode<T>> Children { get; }
}

public interface ILoggable
{
    void Draw(UITree tree);
}

public interface ILogNode<T> : INode<T>, ILoggable
{
    void DrawNodes(UITree tree, IEnumerable<INode<T>>? nodes);
}

public class StructNode<T>(T value) : ILogNode<T>
{
    public T Value { get; } = value;
    public List<StructNode<T>> Children { get; } = [];
    IEnumerable<INode<T>> INode<T>.Children => Children;

    public StructNode<T> AddChild(StructNode<T> child)
    {
        Children.Add(child);
        return child;
    }

    public StructNode<T> AddChild(T value) => AddChild(new StructNode<T>(value));

    public void Draw(UITree tree)
    {
        ImGui.Text($"Test Draw Node {this.GetHashCode()}");
        DrawNodes(tree, new List<INode<T>> { this });
    }

    public void DrawNodes(UITree tree, IEnumerable<INode<T>>? nodes)
    {
        if (nodes == null)
            return;
        foreach (var n in tree.Nodes(nodes, n => new(n.Value?.ToString() ?? "", !n.Children.Any())))
            DrawNodes(tree, n.Children);
    }
}

public static class TextNodeExtensions
{
    public static INode<string> AsINode(this PacketDecoder.TextNode node)
    {
        return new TextNodeAdapter(node);
    }
    public static ILoggable AsILoggable(this PacketDecoder.TextNode node)
    {
        return new TextNodeAdapter(node);
    }

    private class TextNodeAdapter(PacketDecoder.TextNode node) : INode<string>, ILoggable
    {
        public string Value => node.Text;
        public IEnumerable<INode<string>> Children => node.Children?.Select(child => child.AsINode()) ?? [];
        public void Draw() => ImGui.Text(Value);
        public void Draw(UITree tree)
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
}
