using RimWorld;
using Verse;

namespace HPLovecraft
{
    public class IncidentWorker_Mystery : IncidentWorker
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Settings.DebugString("== Enter IncidentWorker_Mystery ==");
            var rand = Rand.Value;
            //Log.Message(rand.ToString());
            if (rand < 0.3f)
            {
                BloodMoon(parms);
            }
            else if (rand < 0.6f)
            {
                StrangeFog(parms);
            }
            else
            {
                LoneSurvivor(parms);
            }

            return true;
        }

        public void BloodMoon(IncidentParms parms)
        {
            Settings.DebugString("Blood Moon");
            HPLDefOf.HPLovecraft_BloodMoon.Worker.TryExecute(parms);
        }

        public void StrangeFog(IncidentParms parms)
        {
            Settings.DebugString("Strange Fog");
            HPLDefOf.HPLovecraft_TheMist.Worker.TryExecute(parms);
        }

        public void LoneSurvivor(IncidentParms parms)
        {
            Settings.DebugString("Lone Survivor");
            HPLDefOf.HPLovecraft_LoneSurvivor.Worker.TryExecute(parms);
        }

        public void TheStranger(IncidentParms parms)
        {
            //HPLDefOf.HPLovecraft_TheStranger.Worker.TryExecute(parms);
        }

        public void SpiderRain(IncidentParms parms)
        {
        }
    }
}