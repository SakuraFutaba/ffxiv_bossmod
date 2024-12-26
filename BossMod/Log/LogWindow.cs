using ImGuiNET;
using BossMod.Network;
using BossMod.Network.ServerIPC;
using ECommons.ImGuiMethods;
using ECommons.CircularBuffers;
using ECommons.Configuration;

namespace BossMod.Log;

public class LogWindow() : UIWindow("Boss mod log UI", false, new(1000, 300))
{
    private static readonly CircularBuffer<LogMessage> LogMessageBuffer = new(10000);
    private const string ConfigPath = "LogConfig.json";
    public static readonly LogConfig C = EzConfig.Init<LogConfig>();
    public static SortedSet<PacketID> LogWhiteList = [.. C.LogWhiteList];
    public static SortedSet<PacketID> LogBlackList = [.. C.LogBlackList];
    public static SortedSet<PacketID> DrawWhiteList = [.. C.DrawWhiteList];
    public static SortedSet<PacketID> DrawBlackList = [.. C.DrawBlackList];

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
        if (ImGui.Button("Reset Log White")) ResetLogWhiteList();
        ImGui.SameLine();
        if (ImGui.Button("Reset Log Black")) ResetLogBlackList();
        ImGui.SameLine();
        if (ImGui.Button("Reset Draw White")) ResetDrawWhiteList();
        ImGui.SameLine();
        if (ImGui.Button("Reset Draw Black")) ResetDrawBlackList();
        ImGui.SameLine();
        if (ImGui.Button("Clear Log Black")) ClearLogBlackList();
        ImGui.SameLine();
        ImGui.Text(string.Join(" ", DrawBlackList));

        ImGui.BeginChild($"Boss mod Log");
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

    public static bool IsInLogBlackList(PacketID id) => LogBlackList.Contains(id);
    public static bool IsInLogWhiteList(PacketID id) => LogWhiteList.Contains(id);
    public static bool IsNotInLogWhiteList(PacketID id) => LogWhiteList.Count > 0 && !LogWhiteList.Contains(id);
    public static bool IsNotNeedToLog(PacketID id) => IsInLogBlackList(id) || IsNotInLogWhiteList(id);
    public static bool IsInDrawBlackList(PacketID id) => DrawBlackList.Contains(id);
    public static bool IsInDrawWhiteList(PacketID id) => DrawWhiteList.Contains(id);
    public static bool IsNotInDrawWhiteList(PacketID id) => DrawWhiteList.Count > 0 && !DrawWhiteList.Contains(id);
    public static bool IsNotNeedToDraw(PacketID id) => IsInDrawBlackList(id) || IsNotInDrawWhiteList(id);
    public static bool IsInDrawBlackList(ServerIPCNode node) => DrawBlackList.Contains(node.Value.ID);
    public static bool IsNotInDrawWhiteList(ServerIPCNode node) => DrawWhiteList.Count > 0 && !DrawWhiteList.Contains(node.Value.ID);
    public static bool IsNotNeedToDraw(ServerIPCNode node) => IsInDrawBlackList(node) || IsNotInDrawWhiteList(node);

    public static bool AddToLogWhiteList(PacketID id) => LogWhiteList.Add(id);
    public static bool AddToLogBlackList(PacketID id) => LogBlackList.Add(id);
    public static bool AddToDrawWhiteList(PacketID id) => DrawWhiteList.Add(id);
    public static bool AddToDrawBlackList(PacketID id) => DrawBlackList.Add(id);
    public static void ClearLogWhiteList() => LogWhiteList.Clear();
    public static void ClearLogBlackList() => LogBlackList.Clear();
    public static void ClearDrawWhiteList() => DrawWhiteList.Clear();
    public static void ClearDrawBlackList() => DrawBlackList.Clear();
    public static void ResetLogWhiteList() => ResetList(LogWhiteList, C.LogWhiteList);
    public static void ResetLogBlackList() => ResetList(LogBlackList, C.LogBlackList);
    public static void ResetDrawWhiteList() => ResetList(DrawWhiteList, C.DrawWhiteList);
    public static void ResetDrawBlackList() => ResetList(DrawBlackList, C.DrawBlackList);
    public static void ResetList(SortedSet<PacketID> listA, SortedSet<PacketID> listB)
    {
        listA.Clear();
        listA.UnionWith(listB);
    }


    public override void OnClose()
    {
        C.SaveConfiguration(ConfigPath);
        base.OnClose();
    }
}
