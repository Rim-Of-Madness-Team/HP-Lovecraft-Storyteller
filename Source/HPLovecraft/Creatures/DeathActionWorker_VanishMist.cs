using RimWorld;
using System;
using Verse;

namespace HPLovecraft
{
    public class DeathActionWorker_VanishMist : DeathActionWorker
    {
        public override void PawnDied(Corpse corpse)
        {
            for (int i = 0; i < 3; i++)
                MoteMaker.ThrowAirPuffUp(corpse.PositionHeld.ToVector3(), corpse.Map);
            if (!corpse.Destroyed) corpse.Destroy(DestroyMode.Vanish);
        }
    }
}
