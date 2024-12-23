using Dalamud.Game.ClientState.Objects.Types;
using ImGuiNET;

namespace BossMod;

public static partial class Utils
{
    public static IGameObject? GetGameObjectByEntityID(ulong id)
    {
        return (id >> 32) == 0 ? Service.ObjectTable.SearchById((uint)id) : null;
    }

    public static IGameObject? GetGameObjectByEntityID(uint id)
    {
        return Service.ObjectTable.SearchById(id);
    }

    public static IGameObject? GetGameObjectByEntityID(uint? id)
    {
        return id.HasValue ? Service.ObjectTable.SearchById((ulong)id) : null;
    }

    public static IEnumerable<string> ToStringArray(this byte[] ba, string fmt = "") => ba.Select(b => b.ToString(fmt));
    public static string ToHexString(this byte[] ba) => string.Join(" ", ba.ToStringArray("X2"));
    public static string ToByteString(this byte[] ba) => string.Join(" ", ba.ToStringArray());
    public static string ToUshortString(this byte[] ba, int offset = 0)
    {
        if (offset >= ba.Length) return ToHexString(ba);
        var chunks = ba.Skip(offset).Chunk(2).Select(chunk => chunk.Length == 2 ? BitConverter.ToUInt16(chunk, 0).ToString() : ToHexString(chunk));
        return string.Join(" ", ba.Take(offset).Select(b => b.ToString("X2")).Concat(chunks));
    }
    public static string ToIntString(this byte[] ba, int offset = 0)
    {
        if (offset >= ba.Length) return ToHexString(ba);
        var chunks = ba.Skip(offset).Chunk(4).Select(chunk => chunk.Length == 4 ? BitConverter.ToInt32(chunk, 0).ToString() : ToHexString(chunk));
        return string.Join(" ", ba.Take(offset).Select(b => b.ToString("X2")).Concat(chunks));
    }
    public static string ToUIntString(this byte[] ba, int offset = 0)
    {
        if (offset >= ba.Length) return ToHexString(ba);
        var chunks = ba.Skip(offset).Chunk(4).Select(chunk => chunk.Length == 4 ? BitConverter.ToUInt32(chunk, 0).ToString("X8") : ToHexString(chunk));
        return string.Join(" ", ba.Take(offset).Select(b => b.ToString("X2")).Concat(chunks));
    }
    public static string ToUlongString(this byte[] ba, int offset = 0)
    {
        if (offset >= ba.Length) return ToHexString(ba);
        var chunks = ba.Skip(offset).Chunk(8).Select(chunk => chunk.Length == 8 ? BitConverter.ToUInt64(chunk, 0).ToString("X16") : ToHexString(chunk));
        return string.Join(" ", ba.Take(offset).Select(b => b.ToString("X2")).Concat(chunks));
    }

    public static Vector4 UIntToImGuiColor(uint color)
    {
        color = 0xFF000000 | ((color & 0xFF0000) >> 16) | (color & 0x00FF00) | ((color & 0x0000FF) << 16);
        return ImGui.ColorConvertU32ToFloat4(color);
    }
}
