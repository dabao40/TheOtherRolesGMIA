using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace TheOtherRoles.Modules;

public class TORGameManager
{
    static private TORGameManager instance = null;
    static public TORGameManager Instance { get => instance; }
    public List<AchievementTokenBase> AllAchievementTokens = [];
    public GameStatistics GameStatistics { get; set; } = new();
    public float CurrentTime { get; private set; } = 0f;
    public RuntimeGameAsset RuntimeAsset { get; private init; }
    public List<RoleHistory> RoleHistory = [];
    public Dictionary<byte, Achievement> TitleMap = [];

    public TORGameManager()
    {
        instance = this;
        RuntimeAsset = new();
    }

    public void RecordRoleHistory(PlayerControl player)
    {
        var role = RoleInfo.getRoleInfoForPlayer(player, false, true).FirstOrDefault();
        RoleHistory.Add(new(player.PlayerId, role, Madmate.madmate.Contains(player) || CreatedMadmate.createdMadmate.Contains(player), role.color));
    }

    public void Abandon()
    {
        RuntimeAsset.Abandon();
        instance = null;
    }

    public void OnUpdate()
    {
        CurrentTime += Time.deltaTime;
    }

    public void OnGameStart()
    {
        RuntimeAsset.MinimapPrefab = ShipStatus.Instance.MapPrefab;
        RuntimeAsset.MinimapPrefab.gameObject.MarkDontUnload();
        RuntimeAsset.MapScale = ShipStatus.Instance.MapScale;
    }
}

public class RuntimeGameAsset
{
    AsyncOperationHandle<GameObject> handle = null;
    public MapBehaviour MinimapPrefab = null!;
    public float MapScale;
    public GameObject MinimapObjPrefab => MinimapPrefab.transform.GetChild(1).gameObject;
    public void SetHandle(AsyncOperationHandle<GameObject> handle) => this.handle = handle;
    public void Abandon()
    {
        if (handle?.IsValid() ?? false) handle.Release();
    }
}

public class RoleHistory
{
    public byte PlayerId;
    public RoleInfo RoleInfo;
    public float Time;
    public bool IsMadmate;
    public Color32 Color;

    public RoleHistory(byte PlayerId, RoleInfo RoleInfo, bool IsMadmate, Color32 Color)
    {
        this.PlayerId = PlayerId;
        this.RoleInfo = RoleInfo;
        Time = TORGameManager.Instance?.CurrentTime ?? 0f;
        this.IsMadmate = IsMadmate;
        this.Color = Color;
    }
}

[HarmonyPatch(typeof(AssetReference), nameof(AssetReference.InstantiateAsync), typeof(Transform), typeof(bool))]
public static class LoadShipInstancePatch
{
    static bool Prefix(AssetReference __instance, ref AsyncOperationHandle<GameObject> __result)
    {
        foreach (var reference in AmongUsClient.Instance.ShipPrefabs)
        {
            if (!reference.AssetGUID.Equals(__instance.AssetGUID)) continue;

            __result = Addressables.InstantiateAsync(__instance.RuntimeKey, null, false, false);
            TORGameManager.Instance?.RuntimeAsset.SetHandle(__result);
            __result.Acquire();
            return false;
        }
        return true;
    }
}
