using System;
using System.Linq;
using Verse;
using RimWorld;
using System.Collections.Generic;
using RimWorld.Planet;
using Verse.AI;

namespace HPLovecraft
{
    public class IncidentWorker_Omen : IncidentWorker
    {
        private static readonly IntRange OMENDELAY = new IntRange(19000, 40000);

        public override bool TryExecute(IncidentParms parms)
        {
            Log.Message("IncidentWorker_Omen Called");
            IncidentDef bigThreat;
            Map map = (Map)parms.target;
            var omenThreatCycle = Find.Storyteller.storytellerComps.FirstOrDefault((comp) => comp.GetType() == typeof(StorytellerComp_OmenThreatCycle));
            if (omenThreatCycle == null)
            {
                Log.ErrorOnce("OmenThreatCycle Doesn't Exist", 78265);
                return false;
            }

            if (!(from def in DefDatabase<IncidentDef>.AllDefs
                       where def.category == IncidentCategory.ThreatBig && parms.points >= def.minThreatPoints && def.Worker.CanFireNow(parms.target) && def != HPLDefOf.HPLovecraft_OmenIncident
                  select def).TryRandomElementByWeight(new Func<IncidentDef, float>(((StorytellerComp_OmenThreatCycle)omenThreatCycle).OmenIncidentChanceFinal), out bigThreat))
            {
                return false;
            }

            var omenTracker = map.GetComponent<MapComponent_OmenIncidentTracker>();
            if (omenTracker != null)
            {
                if (omenTracker.DelayedIncidents.Count > 0)
                {
                    Log.Message("Tried to do multiple omens");
                    return false;
                }
                int newDelay = Find.TickManager.TicksGame + OMENDELAY.RandomInRange;
                Log.Message("New Delayed Incident :: GameTick:" + Find.TickManager.TicksGame + " DelayTick:" + (newDelay));
                omenTracker.AddDelayedIncident(new DelayedIncident(map, bigThreat, parms, newDelay));
            }


            var randVal = Rand.Range(1, 4);
            switch (randVal)
            {
                case 1:
                    HPLDefOf.HPLovecraft_CatsIncident.Worker.TryExecute(parms);
                    break;
                case 2:
                    HPLDefOf.HPLovecraft_ParanoiaIncident.Worker.TryExecute(parms);
                    break;
                case 3:
                    HPLDefOf.HPLovecraft_CrowsIncident.Worker.TryExecute(parms);
                    break;
                case 4:
                    //HPLDefOf.HPLovecraft_ParanoiaIncident.Worker.TryExecute(parms);
                    HPLDefOf.HPLovecraft_MysteryIncident.Worker.TryExecute(parms);
                    break;
                default:
                    break;
            }

            return true;
        }

    }
}
