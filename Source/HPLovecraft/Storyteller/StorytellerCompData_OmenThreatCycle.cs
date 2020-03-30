using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace HPLovecraft
{
    public static class Lovecraft
    {
        private static StorytellerData data;
        public static StorytellerData Data
        {
            get
            {
                if (data == null) data = new StorytellerData();
                return data;
            }
        }

        public static List<IncidentDef> Omens = new List<IncidentDef>
            {
                HPLDefOf.HPLovecraft_CatsIncident,
                HPLDefOf.HPLovecraft_ParanoiaIncident,
                HPLDefOf.HPLovecraft_CrowsIncident,
                HPLDefOf.HPLovecraft_MysteryIncident
            };

    }

    public class StorytellerData : IExposable
    {
        public int standardEvents = 0;
        public int cosmicHorrorEvents = 0;
        public bool lastEventWasOmen;

        public void RecordIncident(IncidentDef def)
        {
            if (Cthulhu.Utility.CosmicHorrorIncidents().Contains(def))
            {
                cosmicHorrorEvents++;
                return;
            }
            standardEvents++;
        }

        public bool NeedCosmicHorrorEvent => Cthulhu.Utility.IsCosmicHorrorsLoaded() && (standardEvents != 0 && cosmicHorrorEvents != 0) && standardEvents > 2 && ((int)(standardEvents / 3) > cosmicHorrorEvents);

        public StorytellerData()
        {
            Settings.DebugString("Lovecraft :: Storyteller Data Written");
        }

        public void ExposeData()
        {
            Scribe_Values.Look<int>(ref this.standardEvents, "standardEvents", 0);
            Scribe_Values.Look<int>(ref this.cosmicHorrorEvents, "cosmicHorrorEvents", 0);
            Scribe_Values.Look<bool>(ref this.lastEventWasOmen, "lastEventWasOmen");
        }
    }
}
