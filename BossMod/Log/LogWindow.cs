using ImGuiNET;
using BossMod.Network;
using ECommons.ImGuiMethods;
using ECommons.CircularBuffers;
using ECommons.Configuration;

namespace BossMod.Log;

public class LogWindow() : UIWindow("Boss mod log UI", false, new(1000, 300))
{
    private static readonly CircularBuffer<LogMessage> LogMessageBuffer = new(1000);
    private const string ConfigPath = "LogConfig.json";
    public static readonly LogConfig C = EzConfig.Init<LogConfig>();

    public override void Draw()
    {
        ImGui.Checkbox("##Autoscroll", ref C.Autoscroll);
        ImGuiEx.Tooltip("Autoscroll");
        ImGui.SameLine();
        if (ImGuiEx.IconButton(FontAwesome.Trash))
        {
            LogMessageBuffer.Clear();
        }
        ImGuiEx.Tooltip("Clear All");
        ImGui.SameLine();
        ImGui.Text(string.Join(" ", C.DrawBlackList));

        ImGui.BeginChild($"Boss mod Log");

        // DrawSampleNode();

        // LogMessageBuffer.ToList().ForEach(logMessage => LogMessage.DrawTree(logMessage.RootNode));
        LogMessageBuffer.ToList().ForEach(LogMessage.DrawLogMessage);

        if(C.Autoscroll)
        {
            ImGui.SetScrollHereY();
        }
        ImGui.EndChild();
    }

    public static void Log(ILogNode rootNode)
    {
        LogMessageBuffer.PushBack(new LogMessage(rootNode));
    }
    public static void Log(PacketDecoder.TextNode textNode)
    {
        LogMessageBuffer.PushBack(new LogMessage(textNode.AsILogNode()));
    }

    private static bool IsInDrawBlackList(ServerIPCNode node) => C.DrawBlackList.Contains(node.Value.ID);
    private static bool IsNotInDrawWhiteList(ServerIPCNode node) => C.DrawWhiteList.Count > 0 && !C.DrawWhiteList.Contains(node.Value.ID);

    public static bool IsNotNeedToDraw(ServerIPCNode node) => IsInDrawBlackList(node) || IsNotInDrawWhiteList(node);

    public override void OnClose()
    {
        C.SaveConfiguration(ConfigPath);
        base.OnClose();
    }
}
