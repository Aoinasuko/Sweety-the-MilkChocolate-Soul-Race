using AlienRace;
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

}
