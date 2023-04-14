using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Sweety_Race
{
	public class Recipe_HeatMod : Recipe_Surgery
	{
		public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
		{
			pawn.health.AddHediff(HediffDef.Named("Sweety_HeatResistMod"));
		}
	}
}
