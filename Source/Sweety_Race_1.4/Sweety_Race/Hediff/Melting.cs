using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Sweety_Race
{
    public class HediffCompProperties_Melting : HediffCompProperties
    {
		public HediffCompProperties_Melting()
		{
			compClass = typeof(HediffComp_Melting);
		}
	}

	public class HediffComp_Melting : HediffComp
	{
		public override void CompPostTick(ref float severityAdjustment)
		{
			if (Pawn.IsHashIntervalTick(300))
            {
				// 溶ける数
				int count = parent.CurStageIndex;
				if (count > 0)
                {
					// 現在のステージの数だけ溶ける
					for (int i=0; i < count; i++)
                    {
						BodyPartRecord bodyPartRecord = (from p in Pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Outside)
														 where p.def.defName.ToStringSafe() != "Waist"
														 select p).RandomElement();
						if (bodyPartRecord != null)
                        {
							Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.MissingBodyPart, Pawn, bodyPartRecord);
							hediff.Severity = 1.0f;
							Pawn.health.AddHediff(hediff, bodyPartRecord);
							if (Pawn.Dead)
                            {
								if (!Pawn.Corpse.DestroyedOrNull() && Pawn.Corpse.Map != null)
								{
									for (int j = 0; j < 5; j++)
									{
										FilthMaker.TryMakeFilth(Pawn.Corpse.Position, Pawn.Corpse.Map, ThingDef.Named("Filth_Sweety_Blood"));
									}
									Pawn.Corpse.Destroy();
								}
								break;
                            }
						}
					}
					
				}
            }
		}
	}

}
