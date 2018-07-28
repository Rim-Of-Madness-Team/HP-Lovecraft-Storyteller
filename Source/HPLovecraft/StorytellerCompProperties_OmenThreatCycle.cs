using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace HPLovecraft
{
    public class StorytellerCompProperties_OmenThreatCycle : StorytellerCompProperties
    {
        
        public float onDays = 0f;

        public float offDays = 0f;

        public float minSpacingDays = 0f;

        public FloatRange numIncidentsRange = FloatRange.Zero;

        public bool applyRaidBeaconThreatMtbFactor = false;


        private IncidentCategoryDef category;

        public float forceRaidEnemyBeforeDaysPassed = 0f;

        public StorytellerCompProperties_OmenThreatCycle()
        {
            this.compClass = typeof(StorytellerComp_OmenThreatCycle);
        }

        public IncidentCategoryDef Category => this.category;

        public override IEnumerable<string> ConfigErrors(StorytellerDef parentDef)
        {
            if (this.onDays <= 0f)
            {
                yield return "onDays must be above zero";
            }
            if (this.numIncidentsRange.TrueMax <= 0f)
            {
                yield return "numIncidentRange not configured";
            }
            if (this.minSpacingDays * this.numIncidentsRange.TrueMax > this.onDays * 0.9f)
            {
                yield return "minSpacingDays too high compared to max number of incidents.";
            }
            yield break;
        }
        
    }
}
