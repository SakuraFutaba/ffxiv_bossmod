using BossMod.Network;

namespace BossMod.Log;

public record class LogMessage(ILoggable RootNode)
{
    public readonly ILoggable RootNode = RootNode;
    public DateTimeOffset Time = DateTimeOffset.Now;

    public void Draw(UITree tree)
    {
        RootNode.Draw(tree);
    }
}
