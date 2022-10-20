using System.Collections.Generic;
using System.Diagnostics;
using RimWorld;
using UnityEngine;
using Verse;

namespace HPLovecraft
{
    public class StorytellerComp_OmenThreatCycle : StorytellerComp
    {
        protected StorytellerCompProperties_OmenThreatCycle Props => (StorytellerCompProperties_OmenThreatCycle) props;

        protected int QueueIntervalsPassed => Find.TickManager.TicksGame / 1000;


        public float OmenIncidentChanceFinal(IncidentDef def)
        {
            var num = def.Worker.BaseChanceThisGame;
            num *= IncidentChanceFactor_CurrentPopulation(def);
            num *= IncidentChanceFactor_PopulationIntent(def);
            return Mathf.Max(0f, num);
        }

        [DebuggerHidden]
        public override IEnumerable<FiringIncident> MakeIntervalIncidents(IIncidentTarget target)
        {
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
            for (var i = 0; i < incCount; i++)
            {
                var firingIncident = GenerateIncident(target);
                if (firingIncident != null)
                {
                    yield return firingIncident;
                }
            }
            /*
             * 
             *  ((1.0))
                        float difficultyFactor = (!this.Props.applyRaidBeaconThreatMtbFactor)
                            ? 1f
                            : Find.Storyteller.difficulty. raidBeaconThreatCountFactor;
                        int incCount = IncidentCycleUtility.IncidentCountThisInterval(target,
                            Find.Storyteller.storytellerComps.IndexOf(this), this.Props.minDaysPassed, this.Props.onDays,
                            this.Props.offDays, this.Props.minSpacingDays, this.Props.numIncidentsRange.min * difficultyFactor,
                            this.Props.numIncidentsRange.max * difficultyFactor, 1f);
                        for (int i = 0; i < incCount; i++)
                        {
                            FiringIncident fi = this.GenerateIncident(target);
                            if (fi != null)
                            {
                                yield return fi;
                            }
                        }
                        yield break;*/

            //        (( B18 ))
            //            float curCycleDays = (GenDate.DaysPassedFloat - this.Props.minDaysPassed) % this.Props.ThreatCycleTotalDays;
            //            if (curCycleDays > this.Props.threatOffDays)
            //            {
            //                float daysSinceThreatBig =
            //                    (float) (Find.TickManager.TicksGame - target.StoryState.LastThreatBigTick) / 60000f;
            //                if (daysSinceThreatBig > this.Props.minDaysBetweenThreatBigs &&
            //                    ((daysSinceThreatBig > this.Props.ThreatCycleTotalDays * 0.9f &&
            //                      curCycleDays > this.Props.ThreatCycleTotalDays * 0.95f) ||
            //                     Rand.MTBEventOccurs(this.Props.mtbDaysThreatBig, 60000f, 1000f)))
            //                {
            //                    FiringIncident st = this.GenerateQueuedThreatBig(target);
            //                    if (st != null)
            //                    {
            //                        yield return st;
            //                    }
            //                }
            //
            //                if (Rand.MTBEventOccurs(this.Props.mtbDaysThreatSmall, 60000f, 1000f))
            //                {
            //                    FiringIncident st = this.GenerateQueuedThreatSmall(target);
            //                    if (st != null)
            //                    {
            //                        yield return st;
            //                    }
            //                }
            //            }
        }
        //
        //        (( B18 ))
        //        private FiringIncident GenerateQueuedThreatSmall(IIncidentTarget target)
        //        {
        //            IncidentDef incidentDef;
        //            if (!this.UsableIncidentsInCategory(IncidentCategoryDefOf.ThreatSmall, target)
        //                .TryRandomElementByWeight(new Func<IncidentDef, float>(base.IncidentChanceFinal), out incidentDef))
        //            {
        //                return null;
        //            }
        //            return new FiringIncident(incidentDef, this, null)
        //            {
        //                parms = this.GenerateParms(incidentDef.category, target)
        //            };
        //        }


        private FiringIncident GenerateIncident(IIncidentTarget target)
        {
            var parms = GenerateParms(Props.IncidentCategory, target);
            return new FiringIncident(IncidentDef.Named("HPLovecraft_OmenIncident"), this)
            {
                parms = parms
            };
        }

        //
        //        private FiringIncident GenerateQueuedThreatBig(IIncidentTarget target, bool isCosmicHorrorEvent = false)
        //        {
        //            IncidentParms parms = this.GenerateParms(IncidentCategoryDefOf.ThreatBig, target);
        //            IncidentDef omenIncident = IncidentDef.Named("HPLovecraft_OmenIncident");
        //            if (isCosmicHorrorEvent) omenIncident = IncidentDef.Named("HPLovecraft_OmenIncidentCosmicHorror");
        //
        //            //Vanilla code
        //            if (GenDate.DaysPassed < 20)
        //            {
        //                if (!IncidentDefOf.RaidEnemy.Worker.CanFireNow(parms))
        //                {
        //                    return null;
        //                }
        //            }
        //
        //            //Throw an omen instead of a big incident
        //            return new FiringIncident(omenIncident, this, null)
        //            {
        //                parms = parms
        //            };
        //        }
    }
}