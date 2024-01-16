using HarmonyLib;
using System.Linq.Expressions;
using System;
using UnityEngine;

namespace EtGModMenu
{
    [HarmonyPatch]
    internal class Patches
    {
        public static void ApplyPatch(Type type, string methodName, Expression<Action> action, bool status, bool prefix)
        {
            if (status)
            {
                var original = AccessTools.Method(type, methodName);
                var mMethod = SymbolExtensions.GetMethodInfo(action);
                if (prefix)
                {
                    Loader.s_harmony.Patch(original, new HarmonyMethod(mMethod));
                }
                else
                {
                    Loader.s_harmony.Patch(original, postfix: new HarmonyMethod(mMethod));
                }
            }
            else
            {
                var original = AccessTools.Method(type, methodName);
                var patchType = prefix ? HarmonyPatchType.Prefix : HarmonyPatchType.Postfix;
                Loader.s_harmony.Unpatch(original, patchType);
            }
        }

        public static void ApplyPatchGetter(Type type, string methodName, Expression<Action> action, bool status, bool prefix)
        {
            if (status)
            {
                var original = AccessTools.PropertyGetter(type, methodName);
                var mMethod = SymbolExtensions.GetMethodInfo(action);
                Loader.s_harmony.Patch(original, new HarmonyMethod(mMethod));
            }
            else
            {
                var original = AccessTools.PropertyGetter(type, methodName);
                var patchType = prefix ? HarmonyPatchType.Prefix : HarmonyPatchType.Postfix;
                Loader.s_harmony.Unpatch(original, patchType);
            }
        }


        //[HarmonyPatch(typeof(HealthHaver), "ApplyDamageDirectional")]
        //[HarmonyPrefix]
        public static bool ApplyDamageDirectional(HealthHaver __instance)
        {
            PlayerController localPlayer = GameManager.Instance?.PrimaryPlayer;
            if (localPlayer != null)
            {
                var m_player = (PlayerController)Traverse.Create(__instance).Field("m_player").GetValue();
                if (m_player == localPlayer)
                {
                    return false;
                }
            }
            return true;
        }

        //[HarmonyPatch(typeof(HealthHaver), "ApplyDamage")]
        //[HarmonyPrefix]
        public static bool ApplyDamage(HealthHaver __instance, ref float damage, Vector2 direction, string sourceName,
            ref CoreDamageTypes damageTypes, ref DamageCategory damageCategory, ref bool ignoreInvulnerabilityFrames)
        {
            var m_player = (PlayerController)Traverse.Create(__instance).Field("m_player").GetValue();
            if (!m_player)
            {
                __instance.NextDamageIgnoresArmor = true;
                damage = __instance.GetMaxHealth();
                damageTypes = CoreDamageTypes.SpecialBossDamage;
                damageCategory = DamageCategory.Unstoppable;
                ignoreInvulnerabilityFrames = true;
            }
            return true;
        }

        //[HarmonyPatch(typeof(Gun), "DecrementAmmoCost")]
        //[HarmonyPrefix]
        public static bool DecrementAmmoCost(Gun __instance)
        {
            PlayerController localPlayer = GameManager.Instance?.PrimaryPlayer;
            return !(localPlayer != null && __instance == localPlayer.CurrentGun);
        }

        //[HarmonyPatch(typeof(Gun), "IncrementModuleFireCountAndMarkReload")]
        //[HarmonyPrefix]
        public static bool IncrementModuleFireCountAndMarkReload(Gun __instance)
        {
            PlayerController localPlayer = GameManager.Instance?.PrimaryPlayer;
            return !(localPlayer != null && __instance == localPlayer.CurrentGun);
        }

        //[HarmonyPatch(typeof(ProjectileModule.ChargeProjectile), "GetChargeProjectile")]
        //[HarmonyPrefix]
        public static bool GetChargeProjectile(ref float chargeTime)
        {
            chargeTime = 99f;
            return true;
        }

        //[HarmonyPatch(typeof(RoomHandler), "SealRoom")]
        //[HarmonyPrefix]
        public static bool SealRoom()
        {
            return false;
        }

        //[HarmonyPatch(typeof(PlayerController), "DoConsumableBlank")]
        //[HarmonyPrefix]
        public static bool DoConsumableBlank(PlayerController __instance)
        {
            __instance.Blanks++;
            return true;
        }

        //[HarmonyPatch(typeof(PlayerItem), "UseConsumableStack")]
        //[HarmonyPrefix]
        public static bool UseConsumableStack()
        {
            return false;
        }

        //[HarmonyPatch(typeof(PlayerItem), "ApplyCooldown")]
        //[HarmonyPrefix]
        public static bool ApplyCooldown(PlayerItem __instance)
        {
            Traverse.Create(__instance).Field("m_activeElapsed").SetValue(0f);
            __instance.ClearCooldowns();
            return false;
        }

        //[HarmonyPatch(typeof(FoyerCharacterSelectFlag), "PrerequisitesFulfilled")]
        //[HarmonyPrefix]
        public static bool PrerequisitesFulfilled(ref bool __result)
        {
            __result = true;
            return false;
        }

        //[HarmonyPatch(typeof(AIActor), "get_TargetRigidbody")]
        //[HarmonyPrefix]
        public static bool TargetRigidbody(ref SpeculativeRigidbody __result)
        {
            __result = null;
            return false;
        }
    }
}
