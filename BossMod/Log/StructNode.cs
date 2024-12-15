using ImGuiNET;

namespace BossMod.Log;

public class StructNode<T>(T value)
{
    public T Value { get; }
    public List<StructNode<T>> Children { get; } = new();

    public StructNode<T> AddChild(StructNode<T> child)
    {
        Children.Add(child);
        return child;
    }

    public StructNode<T> AddChild(T value) => AddChild(new StructNode<T>(value));

    public void DrawStructNode()
    {
        ImGui.Text($"Test DrawStructNode {this.GetHashCode()}");
    }
}
