using AlienRace;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
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

    [HarmonyPatch(typeof(FactionGiftUtility), "GetBaseGoodwillChange")]
    [HarmonyPatch(new Type[]
    {
        typeof(Thing),
        typeof(int),
        typeof(float),
        typeof(Faction),
    })]
    static class GetBaseGoodwillChange_Patch
    {
        [HarmonyPrefix]
        static bool Prefix(ref float __result, Thing anyThing, int count, float singlePrice, Faction theirFaction)
        {
            if (count <= 0)
            {
                return true;
            }
            if (anyThing.def.defName == "Sweety_GorgeousChocolateParfait")
            {
                __result = count * 200.0f;
                return false;
            }
            return true;
        }
    }

}
