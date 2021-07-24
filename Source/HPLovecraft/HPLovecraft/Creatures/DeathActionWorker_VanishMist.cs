using RimWorld;
using Verse;

namespace HPLovecraft
{
    public class DeathActionWorker_VanishMist : DeathActionWorker
    {
        public override void PawnDied(Corpse corpse)
        {
            for (var i = 0; i < 3; i++)
            {
                FleckMaker.ThrowAirPuffUp(corpse.PositionHeld.ToVector3(), corpse.Map);
            }

            if (!corpse.Destroyed)
            {
                corpse.Destroy();
            }
        }
    }
}