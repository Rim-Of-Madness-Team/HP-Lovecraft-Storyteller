using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace HPLovecraft
{
    public class MapComponent_OmenIncidentTracker : MapComponent
    {
        private int bigEvents = 1;

        private List<DelayedIncident> delayedIncidents = new List<DelayedIncident>();

        public MapComponent_OmenIncidentTracker(Map map) : base(map)
        {
            this.map = map;
        }

        public List<DelayedIncident> DelayedIncidents => delayedIncidents;

        public void AddDelayedIncident(DelayedIncident delayedIncident)
        {
            ++bigEvents;
            var newParms = delayedIncident.incidentParms;
            newParms.points += newParms.points * (bigEvents * 0.05f);
            DelayedIncidents.Add(new DelayedIncident(delayedIncident.map, delayedIncident.incident, newParms,
                delayedIncident.ticksUntilIncident));
        }

        public void TriggerIncident(DelayedIncident delayedIncident)
        {
            Settings.DebugString("Trigger Incident Called");
            var omenCycle =
                Find.Storyteller.storytellerComps.FirstOrDefault(comp =>
                    comp.GetType() == typeof(StorytellerComp_OmenThreatCycle));
            if (omenCycle != null)
            {
                try
                {
                    Settings.DebugString("Fire event: " + delayedIncident.incident.label);
                    delayedIncident.incident.Worker.TryExecute(delayedIncident.incidentParms);
                    delayedIncident.incidentParms.target.StoryState.Notify_IncidentFired(
                        new FiringIncident(delayedIncident.incident, omenCycle, delayedIncident.incidentParms));
                }
                catch
                {
                    // ignored
                }
            }

            delayedIncident.didIt = true;
            Settings.DebugString("Trigger Incident Complete");
        }

        public override void MapComponentTick()
        {
            base.MapComponentTick();
            if (delayedIncidents == null || delayedIncidents.Count <= 0)
            {
                return;
            }

            var tempIncidents = new List<DelayedIncident>(delayedIncidents);
            foreach (var delayedIncident in tempIncidents)
            {
                if (!delayedIncident.didIt)
                {
                    delayedIncident.Tick();
                }
                else
                {
                    delayedIncidents.Remove(delayedIncident);
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref bigEvents, "bigEvents", 1);
            Scribe_Collections.Look(ref delayedIncidents, "delayedIncidents", LookMode.Deep);
        }
    }
}