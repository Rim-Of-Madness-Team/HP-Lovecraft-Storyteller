using System;
using RimWorld;

namespace HPLovecraft
{
    public class StorytellerCompProperties_OmenThreatCycle : StorytellerCompProperties
    {
        public float mtbDaysThreatSmall;

        public float mtbDaysThreatBig;

        public float threatOffDays;

        public float threatOnDays;

        public float minDaysBetweenThreatBigs;

        public float ThreatCycleTotalDays
        {
            get
            {
                return this.threatOffDays + this.threatOnDays;
            }
        }

        public StorytellerCompProperties_OmenThreatCycle()
        {
            this.compClass = typeof(StorytellerComp_OmenThreatCycle);
        }
    }
}
