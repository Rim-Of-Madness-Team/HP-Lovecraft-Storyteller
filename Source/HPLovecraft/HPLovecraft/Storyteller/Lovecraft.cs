using System.Collections.Generic;
using RimWorld;

namespace HPLovecraft
{
    public static class Lovecraft
    {
        private static StorytellerData data;

        public static List<IncidentDef> Omens = new List<IncidentDef>
        {
            HPLDefOf.HPLovecraft_CatsIncident,
            HPLDefOf.HPLovecraft_ParanoiaIncident,
            HPLDefOf.HPLovecraft_CrowsIncident,
            HPLDefOf.HPLovecraft_MysteryIncident
        };

        public static StorytellerData Data
        {
            get
            {
                if (data == null)
                {
                    data = new StorytellerData();
                }

                return data;
            }
        }
    }
}