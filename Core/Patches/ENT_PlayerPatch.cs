using HarmonyLib;
using WKLib.API.Events;
using WKLib.Core.Attributes;

namespace WKLib.Core.Patches;

[PatchOnEntry]
[HarmonyPatch]
internal static class ENT_PlayerPatch
{
    [HarmonyPatch(typeof(ENT_Player), nameof(ENT_Player.AddPerk)), HarmonyPostfix]
    private static void ENT_Player_AddPerk(ENT_Player __instance)
    {
        GameEvents.Raise(HookId.PlayerPerkAdded);
    }

    [HarmonyPatch(typeof(ENT_Player), nameof(ENT_Player.Damage)), HarmonyPostfix]
    private static void ENT_Player_Damage(ENT_Player __instance)
    {
        GameEvents.Raise(HookId.PlayerTookDamage);
    }

    [HarmonyPatch(typeof(ENT_Player), nameof(ENT_Player.DamageGripStrength)), HarmonyPostfix]
    private static void ENT_Player_DamageGripStrength(ENT_Player __instance)
    {
        GameEvents.Raise(HookId.PlayerTookGripStrengthDamage);
    }
}