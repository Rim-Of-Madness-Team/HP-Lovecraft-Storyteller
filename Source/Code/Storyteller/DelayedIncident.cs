using RimWorld;
using Verse;

namespace HPLovecraft
{
    public class DelayedIncident : IExposable
    {
        public bool didIt;
        public IncidentDef incident;
        public IncidentParms incidentParms;
        public Map map;
        public int ticksUntilIncident = -1;

        public DelayedIncident()
        {
        }

        public DelayedIncident(DelayedIncident copy)
        {
            ticksUntilIncident = copy.ticksUntilIncident;
            incident = copy.incident;
            incidentParms = copy.incidentParms;
            map = copy.map;
        }

        public DelayedIncident(Map newMap, IncidentDef newIncident, IncidentParms newParms, int newTicks)
        {
            map = newMap;
            incident = newIncident;
            incidentParms = newParms;
            ticksUntilIncident = newTicks;
        }

        public void ExposeData()
        {
            Scribe_References.Look(ref map, "map");
            Scribe_Defs.Look(ref incident, "incident");
            Scribe_Deep.Look(ref incidentParms, "incidentParms");
            Scribe_Values.Look(ref ticksUntilIncident, "ticksUntilIncident", -1);
            Scribe_Values.Look(ref didIt, "didIt");
        }

        public void Tick()
        {
            if (!didIt && Find.TickManager.TicksGame % 1000 == 0)
            {
                Log.Message(Find.TickManager.TicksGame + " " + ticksUntilIncident);
            }

            if (Find.TickManager.TicksGame % 100 == 0 &&
                Find.TickManager.TicksGame > ticksUntilIncident &&
                map.GetComponent<MapComponent_OmenIncidentTracker>() is { } tracker)
            {
                //Log.Message("IncidentTriggered");
                tracker.TriggerIncident(this);
            }
        }
    }
}