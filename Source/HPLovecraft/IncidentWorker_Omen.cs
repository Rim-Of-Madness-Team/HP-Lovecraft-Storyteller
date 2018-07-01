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

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            //Log.Message("IncidentWorker_Omen Called");
            IncidentDef bigThreat;
            Map map = (Map) parms.target;
            var omenThreatCycle = Find.Storyteller.storytellerComps.FirstOrDefault((comp) =>
                comp.GetType() == typeof(StorytellerComp_OmenThreatCycle));
            if (omenThreatCycle == null)
            {
                Log.ErrorOnce("OmenThreatCycle Doesn't Exist", 78265);
                return false;
            }

            if (!Lovecraft.Data.NeedCosmicHorrorEvent)
            {
                if (!(from def in DefDatabase<IncidentDef>.AllDefs
                    where def.category == IncidentCategoryDefOf.ThreatBig && parms.points >= def.minThreatPoints &&
                          def.Worker.CanFireNow(parms) && def != HPLDefOf.HPLovecraft_OmenIncident
                    select def).TryRandomElementByWeight(
                    new Func<IncidentDef, float>(((StorytellerComp_OmenThreatCycle) omenThreatCycle)
                        .OmenIncidentChanceFinal), out bigThreat))
                {
                    Cthulhu.Utility.DebugReport("BigThreat Result - No Event");
                    return false;
                }
            }
            else
            {
                if (!(from def in Cthulhu.Utility.CosmicHorrorIncidents()
                    where def.category == IncidentCategoryDefOf.ThreatBig && parms.points >= def.minThreatPoints &&
                          def.Worker.CanFireNow(parms) && def != HPLDefOf.HPLovecraft_OmenIncident
                    select def).TryRandomElementByWeight(
                    new Func<IncidentDef, float>(((StorytellerComp_OmenThreatCycle) omenThreatCycle)
                        .OmenIncidentChanceFinal), out bigThreat))
                {
                    Cthulhu.Utility.DebugReport("BigThreat Result - No Event");
                    return false;
                }
            }

            var omenTracker = map.GetComponent<MapComponent_OmenIncidentTracker>();
            if (omenTracker != null)
            {
                if (omenTracker.DelayedIncidents.Count > 0)
                {
                    Cthulhu.Utility.DebugReport("Tried to do multiple omens");
                    return false;
                }
                int newDelay = Find.TickManager.TicksGame + OMENDELAY.RandomInRange;
                Cthulhu.Utility.DebugReport("New Delayed Incident :: GameTick:" + Find.TickManager.TicksGame +
                                            " DelayTick:" + (newDelay));
                omenTracker.AddDelayedIncident(new DelayedIncident(map, bigThreat, parms, newDelay));
            }

            List<IncidentDef> omens = new List<IncidentDef>
            {
                HPLDefOf.HPLovecraft_CatsIncident,
                HPLDefOf.HPLovecraft_ParanoiaIncident,
                HPLDefOf.HPLovecraft_CrowsIncident,
                HPLDefOf.HPLovecraft_MysteryIncident
            };
            omens.RandomElement().Worker.TryExecute(parms);

            Lovecraft.Data.RecordIncident(bigThreat);
            return true;
        }
    }
}