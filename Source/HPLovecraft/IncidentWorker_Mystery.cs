using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace HPLovecraft
{
    public class IncidentWorker_Mystery : IncidentWorker
    {
        public override bool TryExecute(IncidentParms parms)
        {
            float rand = Rand.Value;
            if (rand < 0.5f)
            {
                BloodMoon(parms);
            }
            else
            {
                StrangeFog(parms);
            }
            return true;
        }

        public void BloodMoon(IncidentParms parms)
        {
            HPLDefOf.HPLovecraft_BloodMoon.Worker.TryExecute(parms);
        }

        public void StrangeFog(IncidentParms parms)
        {
            HPLDefOf.HPLovecraft_BloodMoon.Worker.TryExecute(parms);
        }

        public void SpiderRain(IncidentParms parms)
        {

        }
    }
}
