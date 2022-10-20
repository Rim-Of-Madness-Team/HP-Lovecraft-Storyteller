// RimWorld.StorytellerCompProperties_OnOffCycle

using System.Collections.Generic;
using RimWorld;
using Verse;

namespace HPLovecraft
{
    public class StorytellerCompProperties_OmenOnOffCycle : StorytellerCompProperties
    {
#pragma warning disable CS0649
        private readonly IncidentCategoryDef category;
#pragma warning restore CS0649

        public SimpleCurve acceptFractionByDaysPassedCurve;

        public SimpleCurve acceptPercentFactorPerThreatPointsCurve;

        public int cosmicHorrorRaidPercentage;

        public float forceCosmicHorrorRaidBeforeDaysPassed;

        public float forceRaidEnemyBeforeDaysPassed;

        public IncidentDef incident;

        public float minSpacingDays;

        public FloatRange numIncidentsRange = FloatRange.Zero;

        public float offDays;
        public float onDays;

        public StorytellerCompProperties_OmenOnOffCycle()
        {
            compClass = typeof(StorytellerComp_OmenOnOffCycle);
        }

        public IncidentCategoryDef IncidentCategory
        {
            get
            {
                if (incident != null)
                {
                    return incident.category;
                }

                return category;
            }
        }

        public override IEnumerable<string> ConfigErrors(StorytellerDef parentDef)
        {
            if (incident != null && category != null)
            {
                yield return "incident and category should not both be defined";
            }

            if (onDays <= 0f)
            {
                yield return "onDays must be above zero";
            }

            if (numIncidentsRange.TrueMax <= 0f)
            {
                yield return "numIncidentRange not configured";
            }

            if (minSpacingDays * numIncidentsRange.TrueMax > onDays * 0.9f)
            {
                yield return "minSpacingDays too high compared to max number of incidents.";
            }
        }
    }
}