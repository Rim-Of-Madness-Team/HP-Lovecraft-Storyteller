// RimWorld.StorytellerCompProperties_OnOffCycle
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace HPLovecraft
{
	public class StorytellerCompProperties_OmenOnOffCycle : StorytellerCompProperties
	{
		public float onDays;

		public float offDays;

		public float minSpacingDays;

		public FloatRange numIncidentsRange = FloatRange.Zero;

		public SimpleCurve acceptFractionByDaysPassedCurve;

		public SimpleCurve acceptPercentFactorPerThreatPointsCurve;

		public IncidentDef incident;

		private IncidentCategoryDef category;

		public int cosmicHorrorRaidPercentage;

		public float forceCosmicHorrorRaidBeforeDaysPassed;

		public float forceRaidEnemyBeforeDaysPassed;

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

		public StorytellerCompProperties_OmenOnOffCycle()
		{
			compClass = typeof(StorytellerComp_OmenOnOffCycle);
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