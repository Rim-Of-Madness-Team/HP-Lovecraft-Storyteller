using System.Collections.Generic;
using System.Linq;
using Cthulhu;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace HPLovecraft
{
    public class IncidentWorker_Paranoia : IncidentWorker
    {
        public static readonly FloatRange SANITYLOSSRANGE = new FloatRange(0.2f, 0.4f);
        public static readonly float DIFFICULTYMODIFIER = 0.02f;

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Settings.DebugString("== Enter IncidentWorker_Paranoia ==");
            var rand = Rand.Value;
            if (rand < 0.25f)
            {
                /*
                 * Mass Paranoia
                 *
                 * Every colonist will experience a range of sanity loss,
                 * ranging from minor to severe.
                 * 10% chance that two colonists will trigger a social fight.
                 *
                 */
                Settings.DebugString("Mass Paranoia");
                if (parms.target is not Map map ||
                    map.mapPawns.FreeColonistsAndPrisoners is not IEnumerable<Pawn> pawns || !pawns.Any())
                {
                    return true;
                }

                var difficultyCalc = DIFFICULTYMODIFIER * Find.Storyteller.difficulty.threatScale;
                SanityLossReport.ApplySanityLossAndShowReport(new List<Pawn>(pawns), SANITYLOSSRANGE,
                    difficultyCalc);
                //bool socialFightStarted = false;
                //foreach (Pawn pawn in pawns)
                //{
                //    if (pawn?.story?.traits?.GetTrait(TraitDefOf.Nerves) is Trait nerves && nerves.Degree > 0)
                //    {
                //        continue;
                //    }
                //    Cthulhu.Utility.ApplySanityLoss(pawn, Rand.Range(0.3f, 0.7f), 1);

                //    if (!socialFightStarted && !pawn.IsPrisoner && Rand.Value > 0.9f && pawn?.mindState?.mentalStateHandler is MentalStateHandler mentalStateHandler)
                //    {
                //        socialFightStarted = true;
                //        var otherPawn = pawns.FirstOrDefault(x => x != pawn && !x.IsPrisoner);
                //        if (otherPawn != null && otherPawn?.mindState?.mentalStateHandler is MentalStateHandler otherMind)
                //        {
                //            mentalStateHandler.TryStartMentalState(MentalStateDefOf.SocialFighting, "ROM_OmenParanaoiaResult".Translate(), false, false, otherPawn);
                //            otherMind.TryStartMentalState(MentalStateDefOf.SocialFighting, "ROM_OmenParanaoiaResult".Translate(), false, false, pawn);
                //        }
                //    }
                //}
            }
            else if (rand < 0.5f)
            {
                /*
                 * Single Paranoia
                 *
                 * A single colonist will experience severe sanity loss.
                 * 80% chance of the character wandering in a psychotic state.
                 * If it's a prisoner, enter the berserk state.
                 *
                 */

                Settings.DebugString("Single Paranoia");
                if (parms.target is not Map map ||
                    map.mapPawns.FreeColonistsAndPrisoners is not IEnumerable<Pawn> pawns || !pawns.Any() ||
                    pawns.RandomElement() is not { } pawn)
                {
                    return true;
                }

                var difficultyCalc = DIFFICULTYMODIFIER * Find.Storyteller.difficulty.threatScale;
                SanityLossReport.ApplySanityLossAndShowReport(new List<Pawn> {pawn}, SANITYLOSSRANGE,
                    difficultyCalc);
                //if (pawn?.story?.traits?.GetTrait(TraitDefOf.Nerves) is Trait nerves && nerves.Degree > 0)
                //{
                //    flavorDesc = "ROM_OmenParanaoiaDesc2b".Translate(pawn);
                //}
                //else
                //{
                //    Cthulhu.Utility.ApplySanityLoss(pawn, Rand.Range(0.7f, 0.9f), 1);

                //    if (Rand.Value > 0.2f && pawn?.mindState?.mentalStateHandler is MentalStateHandler mentalStateHandler)
                //    {
                //        if (pawn.IsPrisoner) mentalStateHandler.TryStartMentalState(MentalStateDefOf.Berserk, "ROM_OmenParanaoiaResult".Translate());
                //        else mentalStateHandler.TryStartMentalState(MentalStateDefOf.Wander_Psychotic, "ROM_OmenParanaoiaResult".Translate());
                //    }
                //    flavorDesc = "ROM_OmenParanaoiaDesc2".Translate(pawn);
                //}
            }
            else if (rand < 0.75f)
            {
                /*
                 * Visions
                 *
                 * A single colonist will experience minor sanity loss.
                 * 90% chance of the character wandering in their own room.
                 *
                 */
                Settings.DebugString("Visions");
                if (parms.target is not Map map ||
                    map.mapPawns.FreeColonistsAndPrisoners is not IEnumerable<Pawn> pawns || !pawns.Any() ||
                    pawns.RandomElement() is not { } pawn)
                {
                    return true;
                }

                var difficultyCalc = DIFFICULTYMODIFIER * Find.Storyteller.difficulty.threatScale;
                SanityLossReport.ApplySanityLossAndShowReport(new List<Pawn> {pawn}, SANITYLOSSRANGE,
                    difficultyCalc, "HPLovecraft_Vision");
                if (Rand.Value > 0.1f && pawns.Count() > 3 &&
                    pawn.mindState?.mentalStateHandler is { } mentalStateHandler)
                {
                    mentalStateHandler.TryStartMentalState(DefDatabase<MentalStateDef>.GetNamed("WanderOwnRoom"),
                        "ROM_OmenParanaoiaResult".Translate());
                }
            }
            else
            {
                /*
                 * Plagued Senses
                 *
                 * A single colonist's eye, ear, nose, or mouth becomes "disoriented."
                 *
                 */
                Settings.DebugString("Plagued Senses");
                if (parms.target is not Map map || map.mapPawns.FreeColonists is not IEnumerable<Pawn> pawns ||
                    !pawns.Any() || pawns.RandomElement() is not { } pawn ||
                    pawn.health?.hediffSet is not { } parts)
                {
                    return true;
                }

                Utility.ApplySanityLoss(pawn, Rand.Range(0.3f, 0.5f));

                var unused = Rand.Value;
                var disorientedHediff =
                    HediffMaker.MakeHediff(DefDatabase<HediffDef>.GetNamed("ROM_Disoriented"), pawn);
                var senseParts = new List<BodyPartRecord>
                {
                    Utility.GetEar(parts),
                    Utility.GetEye(parts),
                    Utility.GetNose(parts),
                    Utility.GetMouth(parts)
                };
                disorientedHediff.Part = parts.GetNotMissingParts().FirstOrDefault(x => senseParts.Contains(x));
                disorientedHediff.Severity = Rand.Range(0.7f, 0.9f);
                parts.AddDirect(disorientedHediff);
                string flavorDesc = "ROM_OmenParanaoiaDesc4".Translate(pawn);
                Find.LetterStack.ReceiveLetter(def.label.CapitalizeFirst(), flavorDesc,
                    DefDatabase<LetterDef>.GetNamed("ROM_Omen"), new GlobalTargetInfo(pawn));
            }

            return true;
        }
    }
}