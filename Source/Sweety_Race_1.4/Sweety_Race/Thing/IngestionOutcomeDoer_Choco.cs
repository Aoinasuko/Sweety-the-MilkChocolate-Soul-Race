using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Sweety_Race
{
	public class IngestionOutcomeDoer_Choco : IngestionOutcomeDoer
	{
		protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested)
		{
			if (pawn.RaceProps.intelligence == Intelligence.Humanlike)
			{
				PawnGenerationRequest request = new PawnGenerationRequest(PawnKindDef.Named("Sweety_Visitor"), Find.FactionManager.FirstFactionOfDef(Faction_Sweety.Sweety_WildSweety), PawnGenerationContext.NonPlayer, -1);
				Pawn pawn_sweety = PawnGenerator.GeneratePawn(request);
				if (PawnUtility.TrySpawnHatchedOrBornPawn(pawn_sweety, pawn))
				{
					Find.LetterStack.ReceiveLetter("Sweety.Food.Erosion".Translate(), "Sweety.Food.Erosion_Desc".Translate(pawn), LetterDefOf.NegativeEvent, pawn_sweety);
					pawn.Destroy();
					for (int i = 0; i < 5; i++)
					{
						FilthMaker.TryMakeFilth(pawn_sweety.PositionHeld, pawn_sweety.MapHeld, pawn_sweety.RaceProps.BloodDef, pawn_sweety.LabelIndefinite());
					}
					pawn_sweety.health.AddHediff(HediffDef.Named("Anesthetic"));
					pawn_sweety.guest.Recruitable = false;
                }
			}
			else
			{
				PawnGenerationRequest request = new PawnGenerationRequest(PawnKindDef.Named("Sweety_Bitterdragon"), Find.FactionManager.FirstFactionOfDef(Faction_Sweety.Sweety_WildSweety), PawnGenerationContext.NonPlayer, -1);
				Pawn pawn_sweety = PawnGenerator.GeneratePawn(request);
				if (PawnUtility.TrySpawnHatchedOrBornPawn(pawn_sweety, pawn))
				{
					Find.LetterStack.ReceiveLetter("Sweety.Food.Erosion".Translate(), "Sweety.Food.Erosion_Desc".Translate(pawn), LetterDefOf.NegativeEvent, pawn_sweety);
					pawn.Destroy();
					for (int i = 0; i < 5; i++)
					{
						FilthMaker.TryMakeFilth(pawn_sweety.PositionHeld, pawn_sweety.MapHeld, pawn_sweety.RaceProps.BloodDef, pawn_sweety.LabelIndefinite());
					}
					pawn_sweety.health.AddHediff(HediffDef.Named("Anesthetic"));
				}
			}
		}
	}

	public class IngestionOutcomeDoer_ChocoCake : IngestionOutcomeDoer
	{
		protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested)
		{
			pawn.health.AddHediff(HediffDef.Named("Sweety_AteCake"));
		}
	}
}
