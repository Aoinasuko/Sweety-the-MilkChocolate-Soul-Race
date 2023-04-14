using RimWorld;
using Verse;

namespace Sweety_Race
{
    public class Sweety_MeltBullet : Bullet
    {
		protected override void Impact(Thing hitThing, bool blockedByShield = false)
		{
			base.Impact(hitThing, blockedByShield);
			Pawn pawn = hitThing as Pawn;
			if (pawn != null)
            {
				if (!pawn.Dead)
                {
					Hediff Hediff = HediffMaker.MakeHediff(HediffDef.Named("Sweety_Melt"), pawn);
					Hediff.Severity = 0.05f;
					pawn.health.AddHediff(Hediff);
					if (pawn.Dead)
                    {
						if (!pawn.Corpse.DestroyedOrNull() && pawn.Corpse.Map != null)
                        {
							for (int i = 0; i < 5; i++)
							{
								FilthMaker.TryMakeFilth(pawn.Corpse.Position, pawn.Corpse.Map, ThingDef.Named("Filth_Sweety_Blood"));
							}
							pawn.Corpse.Destroy();							
						}
                    }
				}
            }
		}
	}
}
