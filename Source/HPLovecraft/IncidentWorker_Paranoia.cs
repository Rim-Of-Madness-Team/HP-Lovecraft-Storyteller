using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using RimWorld.Planet;
using Verse.AI;

namespace HPLovecraft
{
    public class IncidentWorker_Paranoia : IncidentWorker
    {
        public override bool TryExecute(IncidentParms parms)
        {
            string flavorDesc = "";
            float rand = Rand.Value;
            GlobalTargetInfo target = new GlobalTargetInfo();
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
                Log.Message("Mass Paranoia");
                if (parms.target is Map map && map.mapPawns.FreeColonistsAndPrisoners is IEnumerable<Pawn> pawns && pawns.Count<Pawn>() > 0)
                {
                    bool socialFightStarted = false;
                    foreach (Pawn pawn in pawns)
                    {
                        if (pawn?.story?.traits?.GetTrait(TraitDefOf.Nerves) is Trait nerves && nerves.Degree > 0)
                        {
                            continue;
                        }
                        Cthulhu.Utility.ApplySanityLoss(pawn, Rand.Range(0.3f, 0.7f), 1);

                        if (!socialFightStarted && !pawn.IsPrisoner && Rand.Value > 0.9f && pawn?.mindState?.mentalStateHandler is MentalStateHandler mentalStateHandler)
                        {
                            socialFightStarted = true;
                            var otherPawn = pawns.FirstOrDefault(x => x != pawn && !x.IsPrisoner);
                            if (otherPawn != null && otherPawn?.mindState?.mentalStateHandler is MentalStateHandler otherMind)
                            {
                                mentalStateHandler.TryStartMentalState(MentalStateDefOf.SocialFighting, "ROM_OmenParanaoiaResult".Translate(), false, false, otherPawn);
                                otherMind.TryStartMentalState(MentalStateDefOf.SocialFighting, "ROM_OmenParanaoiaResult".Translate(), false, false, pawn);
                            }
                        }
                    }
                    target = new GlobalTargetInfo(pawns.RandomElement<Pawn>());
                }
                flavorDesc = "ROM_OmenParanaoiaDesc1".Translate();
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

                Log.Message("Single paranoia");
                if (parms.target is Map map && map.mapPawns.FreeColonistsAndPrisoners is IEnumerable<Pawn> pawns && pawns.Count<Pawn>() > 0 &&
                    pawns.RandomElement<Pawn>() is Pawn pawn)
                {
                    if (pawn?.story?.traits?.GetTrait(TraitDefOf.Nerves) is Trait nerves && nerves.Degree > 0)
                    {
                        flavorDesc = "ROM_OmenParanaoiaDesc2b".Translate(pawn);
                    }
                    else
                    {
                        Cthulhu.Utility.ApplySanityLoss(pawn, Rand.Range(0.7f, 0.9f), 1);

                        if (Rand.Value > 0.2f && pawn?.mindState?.mentalStateHandler is MentalStateHandler mentalStateHandler)
                        {
                            if (pawn.IsPrisoner) mentalStateHandler.TryStartMentalState(MentalStateDefOf.Berserk, "ROM_OmenParanaoiaResult".Translate());
                            else mentalStateHandler.TryStartMentalState(MentalStateDefOf.WanderPsychotic, "ROM_OmenParanaoiaResult".Translate());
                        }
                        flavorDesc = "ROM_OmenParanaoiaDesc2".Translate(pawn);
                    }
                    target = new GlobalTargetInfo(pawn);
                }
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
                Log.Message("Visions");
                if (parms.target is Map map && map.mapPawns.FreeColonistsAndPrisoners is IEnumerable<Pawn> pawns && pawns.Count<Pawn>() > 0 &&
                    pawns.RandomElement<Pawn>() is Pawn pawn)
                {
                    Cthulhu.Utility.ApplySanityLoss(pawn, Rand.Range(0.3f, 0.5f), 1);

                    if (Rand.Value > 0.1f && pawn?.mindState?.mentalStateHandler is MentalStateHandler mentalStateHandler)
                    {
                        mentalStateHandler.TryStartMentalState(DefDatabase<MentalStateDef>.GetNamed("WanderOwnRoom"), "ROM_OmenParanaoiaResult".Translate());
                    }
                    target = new GlobalTargetInfo(pawn);
                    flavorDesc = "ROM_OmenParanaoiaDesc3".Translate(pawn);
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
                Log.Message("Plagued senses");
                if (parms.target is Map map && map.mapPawns.FreeColonistsAndPrisoners is List<Pawn> pawns && pawns.Count > 0 &&
                    pawns.RandomElement<Pawn>() is Pawn pawn && pawn?.health?.hediffSet is HediffSet parts)
                {
                    Cthulhu.Utility.ApplySanityLoss(pawn, Rand.Range(0.3f, 0.5f), 1);

                    var chance = Rand.Value;
                    var disorientedHediff = HediffMaker.MakeHediff(DefDatabase<HediffDef>.GetNamed("ROM_Disoriented"), pawn);
                    var senseParts = new List<BodyPartRecord>();
                    senseParts.Add(Cthulhu.Utility.GetEar(parts));
                    senseParts.Add(Cthulhu.Utility.GetEye(parts));
                    senseParts.Add(Cthulhu.Utility.GetNose(parts));
                    senseParts.Add(Cthulhu.Utility.GetMouth(parts));
                    disorientedHediff.Part = parts.GetNotMissingParts().FirstOrDefault(x => senseParts.InRandomOrder<BodyPartRecord>().Contains(x));
                    disorientedHediff.Severity = Rand.Range(0.7f, 0.9f);
                    parts.AddDirect(disorientedHediff, null);
                    target = new GlobalTargetInfo(pawn);
                    flavorDesc = "ROM_OmenParanaoiaDesc4".Translate(pawn);
                }
            }
            Find.LetterStack.ReceiveLetter(def.label.CapitalizeFirst(), flavorDesc, DefDatabase<LetterDef>.GetNamed("ROM_Omen"), target);
            return true;
        }
    }
}
