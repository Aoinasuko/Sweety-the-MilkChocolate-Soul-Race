using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Sweety_Race
{
	public class CompProperties_Sweety : CompProperties
	{
		public CompProperties_Sweety()
		{
			compClass = typeof(Comp_Sweety);
		}
	}

	public class Comp_Sweety : ThingComp
	{
		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			Pawn pawn = (Pawn)this.parent;
			if (pawn.Faction?.IsPlayer ?? true)
			{
				pawn.health.AddHediff(HediffDef.Named("Sweety_HeatResist"));
			}
		}

		public override void CompTick()
		{
			Pawn pawn = (Pawn)this.parent;
			// 溶け状態と回復の調整
			if (this.parent.IsHashIntervalTick(300))
            {
				if (pawn.health.hediffSet.HasHediff(HediffDefOf.Heatstroke))
                {
					if (!pawn.health.hediffSet.HasHediff(HediffDef.Named("Sweety_HeatResistMod")) && !pawn.health.hediffSet.HasHediff(HediffDef.Named("Sweety_HeatResist")))
                    {
						// 熱中症の場合、溶ける
						BodyPartRecord bodyPartRecord = (from p in pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Outside)
														 where p.def.defName.ToStringSafe() != "Waist"
														 select p).RandomElement();
						float severity = 3.0f;
						Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.Burn, pawn, bodyPartRecord);
						hediff.Severity = severity;
						pawn.health.AddHediff(hediff, bodyPartRecord);
					}
				} else if (pawn.HasAttachment(ThingDefOf.Fire))
                {
					if (!pawn.health.hediffSet.HasHediff(HediffDef.Named("Sweety_HeatResistMod")))
					{
						// 炎上中の場合、溶ける
						BodyPartRecord bodyPartRecord = (from p in pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Outside)
														 where p.def.defName.ToStringSafe() != "Waist"
														 select p).RandomElement();
						float severity = 5.0f;
						Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.Burn, pawn, bodyPartRecord);
						hediff.Severity = severity;
						pawn.health.AddHediff(hediff, bodyPartRecord);
					}
				} else
                {
					// そうでない場合は回復
					Util.HealInjury(pawn, 5);
					Util.HealFirstMissingBodyPart(pawn);
				}
			} else
            {
				if (this.parent.IsHashIntervalTick(30))
                {
					if (!pawn.health.hediffSet.HasHediff(HediffDef.Named("Sweety_HeatResistMod")))
					{
						if (pawn.HasAttachment(ThingDefOf.Fire))
						{
							// 炎上中の場合、溶ける
							BodyPartRecord bodyPartRecord = (from p in pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Outside)
															 where p.def.defName.ToStringSafe() != "Waist"
															 select p).RandomElement();
							float severity = 5.0f;
							Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.Burn, pawn, bodyPartRecord);
							hediff.Severity = severity;
							pawn.health.AddHediff(hediff, bodyPartRecord);
						}
					}
                }
            }
		}
	}

    public static class Util
    {
        // 傷と傷跡を治癒
        public static void HealInjury(Pawn pawn, int amount)
        {
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
            for (int i = hediffs.Count - 1; i >= 0; i--)
            {
                Hediff_Injury hediff_Injury = hediffs[i] as Hediff_Injury;
                if (hediff_Injury != null && hediff_Injury.Visible && hediff_Injury.def.everCurableByItem)
                {
                    hediff_Injury.Heal(amount);
                }
            }
            return;
        }

        public static void HealFirstMissingBodyPart(Pawn pawn)
        {
            List<Hediff_MissingPart> hediffs = pawn.health.hediffSet.GetMissingPartsCommonAncestors();
            for (int i = hediffs.Count - 1; i >= 0; i--)
            {
                pawn.health.RemoveHediff(hediffs[i]);
            }
            return;
        }
    }

}
