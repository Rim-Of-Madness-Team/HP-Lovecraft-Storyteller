// RimWorld.StorytellerComp_OnOffCycle

using System.Collections.Generic;
using Cthulhu;
using RimWorld;
using Verse;

namespace HPLovecraft
{
    public class StorytellerComp_OmenOnOffCycle : StorytellerComp
    {
        protected StorytellerCompProperties_OmenOnOffCycle Props => (StorytellerCompProperties_OmenOnOffCycle) props;

        public override IEnumerable<FiringIncident> MakeIntervalIncidents(IIncidentTarget target)
        {
            //Settings.DebugString("== Enter StorytellerComp_OmenOnOffCycle.MakeIntervalIncidents ==");
            var num = 1f;
            if (Props.acceptFractionByDaysPassedCurve != null)
            {
                num *= Props.acceptFractionByDaysPassedCurve.Evaluate(GenDate.DaysPassedFloat);
            }

            if (Props.acceptPercentFactorPerThreatPointsCurve != null)
            {
                num *= Props.acceptPercentFactorPerThreatPointsCurve.Evaluate(
                    StorytellerUtility.DefaultThreatPointsNow(target));
            }

            var incCount = IncidentCycleUtility.IncidentCountThisInterval(target,
                Find.Storyteller.storytellerComps.IndexOf(this), Props.minDaysPassed, Props.onDays, Props.offDays,
                Props.minSpacingDays, Props.numIncidentsRange.min, Props.numIncidentsRange.max, num);

            //Settings.DebugString($"Accept Fraction: {num} + Incident Count: {incCount}");
            for (var i = 0; i < incCount; i++)
            {
                var firingIncident = GenerateIncident(target);
                if (firingIncident == null)
                {
                    continue;
                }

                Settings.DebugString($"Make Incident: {firingIncident.def.defName}");
                yield return firingIncident;
            }

            //Settings.DebugString("== Exit StorytellerComp_OmenOnOffCycle.MakeIntervalIncidents ==");
        }

        private FiringIncident GenerateIncident(IIncidentTarget target)
        {
            var parms = GenerateParms(Props.IncidentCategory, target);
            IncidentDef result;

            //OTHER INCIDENTS

            //If no cosmic horrors, keep throwing raids...
            if (GenDate.DaysPassed < (Utility.IsCosmicHorrorsLoaded()
                ? Props.forceRaidEnemyBeforeDaysPassed
                : Props.forceRaidEnemyBeforeDaysPassed * 2f))
            {
                if (!IncidentDefOf.RaidEnemy.Worker.CanFireNow(parms))
                {
                    return null;
                }

                result = IncidentDefOf.RaidEnemy;
            }
            //Cosmic horrors? Well, after the first week, they will come...
            else if (Utility.IsCosmicHorrorsLoaded() &&
                     GenDate.DaysPassed < Props.forceCosmicHorrorRaidBeforeDaysPassed)
            {
                if (!IncidentDef.Named("ROM_RaidCosmicHorrors").Worker.CanFireNow(parms))
                {
                    if (!IncidentDefOf.RaidEnemy.Worker.CanFireNow(parms))
                    {
                        return null;
                    }

                    result = IncidentDefOf.RaidEnemy;
                }
                else
                {
                    result = IncidentDef.Named("ROM_RaidCosmicHorrors");
                }
            }
            else if (Props.incident != null)
            {
                if (!Props.incident.Worker.CanFireNow(parms))
                {
                    return null;
                }

                result = Props.incident;
            }
            else if (!UsableIncidentsInCategory(Props.IncidentCategory, parms)
                .TryRandomElementByWeight(IncidentChanceFinal, out result))
            {
                return null;
            }

            // OMENS

            if (!Lovecraft.Data.lastEventWasOmen)
            {
                Lovecraft.Data.lastEventWasOmen = true;

                return new FiringIncident(Lovecraft.Omens.RandomElement(), this)
                {
                    parms = parms
                };
            }

            Lovecraft.Data.lastEventWasOmen = false;
            return new FiringIncident(result, this)
            {
                parms = parms
            };
        }

        public override string ToString()
        {
            return base.ToString() + " (" +
                   (Props.incident != null ? Props.incident.defName : Props.IncidentCategory.defName) + ")";
        }
    }
}