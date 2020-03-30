// RimWorld.StorytellerComp_OnOffCycle
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace HPLovecraft
{

	public class StorytellerComp_OmenOnOffCycle : StorytellerComp
	{
		protected StorytellerCompProperties_OmenOnOffCycle Props => (StorytellerCompProperties_OmenOnOffCycle)props;

		public override IEnumerable<FiringIncident> MakeIntervalIncidents(IIncidentTarget target)
		{
			//Settings.DebugString("== Enter StorytellerComp_OmenOnOffCycle.MakeIntervalIncidents ==");
			float num = 1f;
			if (Props.acceptFractionByDaysPassedCurve != null)
			{
				num *= Props.acceptFractionByDaysPassedCurve.Evaluate(GenDate.DaysPassedFloat);
			}
			if (Props.acceptPercentFactorPerThreatPointsCurve != null)
			{
				num *= Props.acceptPercentFactorPerThreatPointsCurve.Evaluate(StorytellerUtility.DefaultThreatPointsNow(target));
			}
			int incCount = IncidentCycleUtility.IncidentCountThisInterval(target, Find.Storyteller.storytellerComps.IndexOf(this), Props.minDaysPassed, Props.onDays, Props.offDays, Props.minSpacingDays, Props.numIncidentsRange.min, Props.numIncidentsRange.max, num);

			//Settings.DebugString($"Accept Fraction: {num} + Incident Count: {incCount}");
			for (int i = 0; i < incCount; i++)
			{
				FiringIncident firingIncident = GenerateIncident(target);
				if (firingIncident != null)
				{
					Settings.DebugString($"Make Incident: {firingIncident.def.defName}");
					yield return firingIncident;
				}
			}
			//Settings.DebugString("== Exit StorytellerComp_OmenOnOffCycle.MakeIntervalIncidents ==");
		}

		private FiringIncident GenerateIncident(IIncidentTarget target)
		{

			IncidentParms parms = GenerateParms(Props.IncidentCategory, target);
			IncidentDef result;

			//OTHER INCIDENTS

			//If no cosmic horrors, keep throwing raids...
			if ((float)GenDate.DaysPassed < (Cthulhu.Utility.IsCosmicHorrorsLoaded() ? Props.forceRaidEnemyBeforeDaysPassed : Props.forceRaidEnemyBeforeDaysPassed * 2f))
			{
				if (!IncidentDefOf.RaidEnemy.Worker.CanFireNow(parms))
				{
					return null;
				}
				result = IncidentDefOf.RaidEnemy;
			}
			//Cosmic horrors? Well, after the first week, they will come...
			else if (Cthulhu.Utility.IsCosmicHorrorsLoaded() && (float)GenDate.DaysPassed < Props.forceCosmicHorrorRaidBeforeDaysPassed)
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
					result = IncidentDef.Named("ROM_RaidCosmicHorrors");
			}
			else if (Props.incident != null)
			{
				if (!Props.incident.Worker.CanFireNow(parms))
				{
					return null;
				}
				result = Props.incident;
			}
			else if (!UsableIncidentsInCategory(Props.IncidentCategory, parms).TryRandomElementByWeight(base.IncidentChanceFinal, out result))
			{
				return null;
			}

			// OMENS
			
			if (!Lovecraft.Data.lastEventWasOmen)
			{
				Lovecraft.Data.lastEventWasOmen = true;

				return new FiringIncident(Lovecraft.Omens.RandomElement(), this, null)
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
			return base.ToString() + " (" + ((Props.incident != null) ? Props.incident.defName : Props.IncidentCategory.defName) + ")";
		}
	}
}