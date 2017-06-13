using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace HPLovecraft
{


    public class MapComponent_OmenIncidentTracker : MapComponent
    {
        private int bigEvents = 1;

        List<DelayedIncident> delayedIncidents = new List<DelayedIncident>();
        public List<DelayedIncident> DelayedIncidents
        {
            get
            {
                return delayedIncidents;
            }
        }

        public void AddDelayedIncident(DelayedIncident delayedIncident)
        {
            ++bigEvents;
            IncidentParms newParms = delayedIncident.incidentParms;
            newParms.points += newParms.points * (bigEvents * 0.05f);
            DelayedIncidents.Add(new DelayedIncident(delayedIncident.map, delayedIncident.incident, newParms, delayedIncident.ticksUntilIncident));
        }

        public MapComponent_OmenIncidentTracker(Map map) : base(map)
        {
            this.map = map;
        }

        public void TriggerIncident(DelayedIncident delayedIncident)
        {
            Log.Message("Trigger Incident Called");
            var omenCycle = Find.Storyteller.storytellerComps.FirstOrDefault((comp) => comp.GetType() == typeof(StorytellerComp_OmenThreatCycle));
            if (omenCycle != null)
            {
                Log.Message("Fire event: " + delayedIncident.incident.label);
                delayedIncident.incident.Worker.TryExecute(delayedIncident.incidentParms);
                delayedIncident.incidentParms.target.StoryState.Notify_IncidentFired(new FiringIncident(delayedIncident.incident, omenCycle, delayedIncident.incidentParms));
            }
            delayedIncident.didIt = true;
            Log.Message("Trigger Incident Complete");
        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();
            if (delayedIncidents != null && delayedIncidents.Count > 0)
            {
                List<DelayedIncident> tempIncidents = new List<DelayedIncident>(delayedIncidents);
                foreach (DelayedIncident delayedIncident in tempIncidents)
                {
                    if (!delayedIncident.didIt)
                        delayedIncident.Tick();
                    else
                        delayedIncidents.Remove(delayedIncident);
                }
            }
        }
        
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.bigEvents, "bigEvents", 1);
            Scribe_Collections.Look<DelayedIncident>(ref this.delayedIncidents, "delayedIncidents", LookMode.Deep, new object[0]);

        }
    }
}
