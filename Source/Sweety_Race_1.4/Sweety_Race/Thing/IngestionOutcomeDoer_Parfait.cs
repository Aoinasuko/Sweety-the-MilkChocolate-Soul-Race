using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Sweety_Race
{
	/// <summary>
	/// スウィーティのパフェ共通効果
	/// </summary>
	public class IngestionOutcomeDoer_Parfait : IngestionOutcomeDoer
	{
		protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested)
		{
            // 対高温バフ
            pawn.health.AddHediff(HediffDef.Named("Sweety_CoolBody"));
            // 満腹感バフ
            pawn.health.AddHediff(HediffDef.Named("Sweety_AteCake"));
        }
	}
}
