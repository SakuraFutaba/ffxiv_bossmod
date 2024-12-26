using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;

namespace BossMod.Log;

public class GameObjectInfo
{
    public IGameObject? GameObject { get; set; }
    public IGameObject? Owner { get; set; }
    public string? Name { get; set; }
    public uint? EntityId { get; set; }
    public uint? DataId { get; set; }
    public ObjectKind? ObjectKind { get; set; }
    public uint? OwnerId { get; set; }
    public string? OwnerName { get; set; }

    public bool IsNull() => Name is null || EntityId is null || DataId is null || ObjectKind is null || OwnerId is null;

    public void UpdateGameObjectInfoByEntityID(uint id)
    {
        GameObject ??= Utils.GetGameObjectByEntityID(id);
        Owner ??= Utils.GetGameObjectByEntityID(GameObject?.OwnerId);

        Name ??= GameObject?.Name.ToString();
        EntityId ??= GameObject?.EntityId;
        DataId ??= GameObject?.DataId;
        ObjectKind ??= GameObject?.ObjectKind;
        OwnerId ??= GameObject?.OwnerId;
        OwnerName ??= Owner?.Name.ToString();
    }

    public string ObjectString(uint id)
    {
        UpdateGameObjectInfoByEntityID(id);
        return $"'{Name ?? "(not found)"}' <{id:X}> ";
    }

    public string ObjectAndOwnerString(uint id)
    {
        UpdateGameObjectInfoByEntityID(id);
        return $"Target: '{Name ?? "(not found)"}' <{id:X}> Owner: '{OwnerName ?? "(not found)"}' <{OwnerId:X}> ";
    }
}
