using System.Collections.Generic;
using System.Text;
using Cthulhu;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace HPLovecraft
{
    public static class SanityLossReport
    {
        /// Create a report on sanity loss.
        /// Shows a letter to the player that displays along these lines.
        /// 
        /// SANITY LOSS REPORT
        /// Sanity Loss
        /// 
        /// Some colonists have experienced a fevered nightmare.
        /// In it they saw visions of a blackened creature whose monstrous forehead
        /// shone in moonlight and whose vile hooves surely pawed hellish oozes miles
        /// below the surface with eyes hidden, leering, and waiting.
        /// 
        /// Afflicted colonists.
        /// (+7% difficulty modifier)
        /// - Dave experienced +30% sanity loss.
        /// - Jerry experienced +25% sanity loss.
        /// - Steely Dan resisted with iron will.
        public static void ApplySanityLossAndShowReport(List<Pawn> pawns, FloatRange lossRange,
            float difficultyModifier, string dreamOverride = "")
        {
            if (pawns.NullOrEmpty())
            {
                return;
            }

            var target = new GlobalTargetInfo();
            var s = new StringBuilder();
            var map = pawns[0]?.MapHeld;
            if (map != null)
            {
                //
                // Prefix
                //
                string dream = GenLocalDate.DayPercent(map) < 0.2f || GenLocalDate.DayPercent(map) > 0.7f
                    ? "HPLovecraft_Daydream".Translate()
                    : "HPLovecraft_Nightmare".Translate();
                if (dreamOverride != "")
                {
                    dream = dreamOverride.Translate();
                }

                if (pawns.Count > 1)
                {
                    s.AppendLine("HPLovecraft_SanityLossPrefixPlural".Translate(dream));
                }
                else if (pawns.Count == 1)
                {
                    s.AppendLine("HPLovecraft_SanityLossPrefix".Translate(pawns[0].LabelShort, dream));
                }
                else
                {
                    Log.Message("Can't choose sanity loss prefix. This should never happen.");
                }

                //
                // Flavor Text
                // 
                var flavorKey = "HPLovecraft_SanityLossFlavorText" + new IntRange(1, 13).RandomInRange;
                s.AppendLine(flavorKey.Translate());

                // Seperator
                s.AppendLine();

                //
                // Inidividual Reports
                // 
                s.AppendLine("HPLovecraft_AfflictedColonists".Translate());
                s.AppendLine(
                    "HPLovecraft_DifficultyModifier".Translate((int) (difficultyModifier * 100))
                        .ToString()); //0.07 => 7
                foreach (var p in pawns)
                {
                    if (!target.HasThing)
                    {
                        target = new GlobalTargetInfo(p);
                    }

                    if (p?.story?.traits?.GetTrait(TraitDefOf.Nerves) is {Degree: 2})
                    {
                        s.AppendLine("  -" + "HPLovecraft_SanityLossResistedWithIronWill".Translate(p.LabelShort));
                    }
                    else
                    {
                        var sanityLossToApply = lossRange.RandomInRange + difficultyModifier;
                        s.AppendLine("  -" +
                                     "HPLovecraft_ExperiencedSanityLoss".Translate(p?.LabelShort,
                                         (int) (sanityLossToApply * 100)));
                        Utility.ApplySanityLoss(p, sanityLossToApply);
                    }
                }
            }

            Find.LetterStack.ReceiveLetter("HPLovecraft_SanityLossReport".Translate(),
                s.ToString().TrimEndNewlines(), LetterDefOf.ThreatSmall, target);
        }
    }
}