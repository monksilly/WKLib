using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using WKLib.API.Events;
using WKLib.Core.Attributes;

namespace WKLib.Core.Patches
{
    [PatchOnEntry]
    [HarmonyPatch]
    internal static class GameEntityPatch
    {
        [HarmonyPatch(typeof(GameEntity), nameof(GameEntity.Damage)), HarmonyPostfix]
        private static void GameEntity_Damage(GameEntity __instance, ref Damageable.DamageInfo info)
        {
            if (info.sourceEntity == ENT_Player.GetPlayer())
            {
                GameEvents.Raise(HookId.PlayerDealtDamage);
            }
        }
    }
}