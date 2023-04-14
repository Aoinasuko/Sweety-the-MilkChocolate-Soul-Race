﻿using AlienRace;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Sweety_Race
{

    [StaticConstructorOnStartup]
    static class Sweety_Harmony
    {
        static Sweety_Harmony()
        {
            var harmony = new Harmony("BEP.Sweety");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    // 肉を人間扱いにしなくなる
    [HarmonyPatch(typeof(FoodUtility), "IsHumanlikeCorpseOrHumanlikeMeat")]
    static class Sweety_FixFoodUtility
    {
        [HarmonyPrefix]
        public static bool Fix_IsHumanlike(ref bool __result, ref Thing source, ref ThingDef foodDef)
        {
            Corpse corpse = source as Corpse;
            if (corpse != null && (corpse.InnerPawn.def.race.FleshType == FleshType_Sweety.Sweety))
            {
                __result = false;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(FoodUtility), "GetMeatSourceCategoryFromCorpse")]
    [HarmonyPatch(new Type[]
    {
        typeof(Thing)
    })]
    static class Sweety_FixFoodUtility_GetMeatSourceCategoryCorpse
    {
        [HarmonyPrefix]
        public static bool Fix_IsHumanlike(ref MeatSourceCategory __result, Thing thing)
        {
            Corpse corpse = thing as Corpse;
            if (corpse != null && corpse.InnerPawn.def.race.FleshType == FleshType_Sweety.Sweety)
            {
                __result = MeatSourceCategory.Undefined;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(FoodUtility), "GetMeatSourceCategory")]
    [HarmonyPatch(new Type[]
    {
        typeof(ThingDef)
    })]
    static class Sweety_FixFoodUtility_GetMeatSourceCategory
    {
        [HarmonyPrefix]
        public static bool Fix_IsHumanlike(ref MeatSourceCategory __result, ThingDef source)
        {
            if (source.ingestible == null)
            {
                return true;
            }
            if ((source.ingestible.foodType & FoodTypeFlags.Meat) == FoodTypeFlags.Meat)
            {
                if (source.ingestible.sourceDef != null)
                {
                    if (source.ingestible.sourceDef.race.FleshType == FleshType_Sweety.Sweety)
                    {
                        __result = MeatSourceCategory.Undefined;
                        return false;
                    }
                }
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Corpse), "ButcherProducts")]
    [HarmonyPatch(new Type[] { typeof(Pawn), typeof(float) })]
    static class Sweety_Corpse_Patch
    {

        [HarmonyBefore(new string[] { "rimworld.erdelf.alien_race.main" })]
        [HarmonyPrefix]
        static bool Prefix(Pawn butcher, float efficiency, ref IEnumerable<Thing> __result, Corpse __instance)
        {

            if (__instance.InnerPawn.def.race.FleshType == FleshType_Sweety.Sweety)
            {
                Pawn corpse = __instance.InnerPawn;
                IEnumerable<Thing> enumerable = corpse.ButcherProducts(butcher, efficiency);
                if (corpse.RaceProps.BloodDef != null)
                {
                    FilthMaker.TryMakeFilth(butcher.Position, butcher.Map, corpse.RaceProps.BloodDef, corpse.LabelIndefinite());
                }
                __result = enumerable;
                return false;
            }
            return true;
        }

    }

    /// <summary>
    /// 脱走しない
    /// </summary>
    [HarmonyPatch(typeof(PrisonBreakUtility), "InitiatePrisonBreakMtbDays")]
    [HarmonyPatch(new Type[]
    {
        typeof(Pawn),
        typeof(StringBuilder),
        typeof(bool)
    })]
    static class FixPrisonBreak
    {
        [HarmonyPrefix]
        static bool Prefix(ref float __result, Pawn pawn, StringBuilder sb, bool ignoreAsleep)
        {
            if (pawn.def.defName == "Sweety_Pawn")
            {
                __result = -1f;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(SlaveRebellionUtility), "InitiateSlaveRebellionMtbDays")]
    [HarmonyPatch(new Type[]
    {
        typeof(Pawn),
    })]
    static class FixSlaveRebellion
    {
        [HarmonyPrefix]
        static bool Prefix(ref float __result, Pawn pawn)
        {
            if (pawn.def.defName == "Sweety_Pawn")
            {
                __result = -1f;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(SlaveRebellionUtility), "InitiateSlaveRebellionMtbDaysHelper")]
    [HarmonyPatch(new Type[]
    {
        typeof(Pawn),
    })]
    static class FixSlaveRebellion_Fix2
    {
        [HarmonyPrefix]
        static bool Prefix(ref float __result, Pawn pawn)
        {
            if (pawn.def.defName == "Sweety_Pawn")
            {
                __result = -1f;
                return false;
            }
            return true;
        }
    }

    // モフィーの派閥追加時、服装規定を無効化する
    [HarmonyPatch(typeof(Precept_Role), "GenerateNewApparelRequirements")]
    [HarmonyPatch(new Type[]
    {
        typeof(FactionDef),
    })]
    internal static class ApparelRequirement_Patch
    {

        [HarmonyPrefix]
        static bool Prefix(ref Precept_Role __instance, ref List<PreceptApparelRequirement> __result, FactionDef generatingFor)
        {
            if (generatingFor != null)
            {
                if (generatingFor.defName == "Sweety_WildSweety")
                {
                    List<PreceptApparelRequirement> ApparelRequirement = new List<PreceptApparelRequirement>();
                    // 役割のdefName取得
                    String role = __instance.def.defName;
                    // 役割によって装備を指定
                    if (!ApparelRequirement.NullOrEmpty())
                    {
                        __result = ApparelRequirement;
                    }
                    else
                    {
                        __result = null;
                    }
                    return false;
                }
            }
            return true;
        }
    }

    /// <summary>
    /// 奴隷時に作業速度ボーナス
    /// <summary>
    [StaticConstructorOnStartup]
    [HarmonyPatch(typeof(StatPart_Slave))]
    internal static class TransformValue_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(StatPart_Slave.TransformValue),
            new Type[] { typeof(StatRequest), typeof(float) },
            new ArgumentType[] { ArgumentType.Normal, ArgumentType.Ref })]
        static bool Prefix(StatRequest req, ref float val)
        {
            if (req.HasThing)
            {
                if (req.Thing as Pawn != null)
                {
                    Pawn pawn = (Pawn)req.Thing;
                    if (pawn.def.defName == "Sweety_Pawn")
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }

    /// <summary>
    /// 奴隷時のボーナス説明
    /// <summary>
    [HarmonyPatch(typeof(StatPart_Slave), "ExplanationPart")]
    [HarmonyPatch(new Type[]
    {
        typeof(StatRequest),
    })]
    internal static class ExplanationPart_Patch
    {
        [HarmonyPrefix]
        static bool Prefix(ref StatPart_Slave __instance, ref String __result, StatRequest req)
        {
            if (req.HasThing)
            {
                if (req.Thing as Pawn != null)
                {
                    Pawn pawn = (Pawn)req.Thing;
                    if (pawn.def.defName == "Sweety_Pawn")
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }

    /// <summary>
    /// 年齢補正無効化
    /// <summary>
    [HarmonyPatch(typeof(StatPart_Age), "AgeMultiplier")]
    [HarmonyPatch(new Type[]
    {
        typeof(Pawn),
    })]
    internal static class AgeMultiplier_Patch
    {
        [HarmonyPrefix]
        static bool Prefix(ref StatPart_Age __instance, ref float __result, Pawn pawn)
        {
            if (pawn.def.defName == "Sweety_Pawn")
            {
                __result = 1.0f;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(StatPart_AgeOffset), "AgeOffset")]
    [HarmonyPatch(new Type[]
    {
        typeof(Pawn),
    })]
    internal static class AgeMultiplier_Patch_Offset
    {
        [HarmonyPrefix]
        static bool Prefix(ref StatPart_AgeOffset __instance, ref float __result, Pawn pawn)
        {
            if (pawn.def.defName == "Sweety_Pawn")
            {
                __result = 0f;
                return false;
            }
            return true;
        }
    }

}
