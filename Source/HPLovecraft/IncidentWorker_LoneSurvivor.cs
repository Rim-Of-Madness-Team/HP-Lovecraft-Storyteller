using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using RimWorld.Planet;
using Verse.AI.Group;

namespace HPLovecraft
{
    public class IncidentWorker_LoneSurvivor : IncidentWorker
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            string flavorDesc = "";
            float rand = Rand.Value;
            GlobalTargetInfo target = new GlobalTargetInfo();

            if (rand < 0.1f)
            {
                DeadSurvivor(parms, out flavorDesc, out target);
            }
            else if (rand < 0.5f)
            {
                //Non-hostile Period is the first three seasons of omens.
                //if (GenDate.DaysPassed > 45)
                //{
                    if (Rand.Range(0, 100) > 50)
                        PsychopathSurvivor(parms, out flavorDesc, out target);
                    else
                        PlaguedSurvivor(parms, out flavorDesc, out target);
                //}
                //else
                //{
                //    WildSurvivor(parms, out flavorDesc, out target);
                //}
            }
            else if (rand < 0.9f)
            {
                MadSurvivor(parms, out flavorDesc, out target);
            }
            else
            {
                WildSurvivor(parms, out flavorDesc, out target);

            }
            Find.LetterStack.ReceiveLetter(def.label.CapitalizeFirst(), flavorDesc, DefDatabase<LetterDef>.GetNamed("ROM_Omen"), target);
            return true;
        }

        /*
         * Plagued Survivor
         *
         * 
         * 
         */
        public void PlaguedSurvivor(IncidentParms parms, out string flavorDesc, out GlobalTargetInfo target)
        {
            IntVec3 loc;
            Map map = (Map)parms.target;
            RCellFinder.TryFindRandomPawnEntryCell(out loc, map, CellFinder.EdgeRoadChance_Animal, false, null);
            Pawn newThing = PawnGenerator.GeneratePawn(PawnKindDefOf.Drifter, null);
            Pawn survivor = (Pawn)GenSpawn.Spawn(newThing, loc, map);
            survivor.health.AddHediff(HediffDefOf.Plague);
            for (int i = 0; i < Rand.Range(1, 2); i++)
                survivor.health.AddHediff(HediffDefOf.WoundInfection, survivor.health.hediffSet.GetNotMissingParts().RandomElement());
            survivor.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Wander_Sad);
            Cthulhu.Utility.ApplySanityLoss(survivor, 0.3f);

            survivor.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Berserk);

            flavorDesc = "ROM_OmenSurvivorDesc5".Translate();
            target = new RimWorld.Planet.GlobalTargetInfo(survivor);
        }

        /*
         * Mad Survivor
         *
         * A person driven mad will wander the outskirts of the colony.
         *
         */
        public void MadSurvivor(IncidentParms parms, out string flavorDesc, out GlobalTargetInfo target)
        {
            IntVec3 loc;
            Map map = (Map)parms.target;
            RCellFinder.TryFindRandomPawnEntryCell(out loc, map, CellFinder.EdgeRoadChance_Animal, false, null);
            Pawn newThing = PawnGenerator.GeneratePawn(PawnKindDefOf.Drifter, null);
            Pawn survivor = (Pawn)GenSpawn.Spawn(newThing, loc, (Map)parms.target);
            DamageDef damageType = (Rand.Range(0, 100) > 50) ? DamageDefOf.Crush : DamageDefOf.Stab;
            var damageNum = (Rand.Range(0, 100) > 50) ? 3f : 5f;
            Cthulhu.Utility.ApplySanityLoss(survivor, 0.7f);

            survivor.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Berserk);

            //SanityLossReport.ApplySanityLossAndShowReport(new List<Pawn> { survivor }, new FloatRange(0.7f, 0.9f), 0.02f);
            flavorDesc = "ROM_OmenSurvivorDesc4".Translate();
            target = new RimWorld.Planet.GlobalTargetInfo(survivor);
        }

        /*
        * Wild Survivor
        *
        * A survivor turned into a wild man will wander the outskirts of the colony.
        * 
        */
        public void WildSurvivor(IncidentParms parms, out string flavorDesc, out GlobalTargetInfo target)
        {
            IntVec3 loc;
            RCellFinder.TryFindRandomPawnEntryCell(out loc, (Map)parms.target, CellFinder.EdgeRoadChance_Animal, false, null);
            Pawn newThing = PawnGenerator.GeneratePawn(PawnKindDefOf.WildMan, null);
            Pawn survivor = (Pawn)GenSpawn.Spawn(newThing, loc, (Map)parms.target);
            Cthulhu.Utility.ApplySanityLoss(survivor, 0.7f);
            //SanityLossReport.ApplySanityLossAndShowReport(new List<Pawn> { survivor }, new FloatRange(0.7f, 0.9f), 0.02f);
            flavorDesc = "ROM_OmenSurvivorDesc3".Translate();
            target = new RimWorld.Planet.GlobalTargetInfo(survivor);
        }

        /*
         * Psychopath Survivor
         *
         * 
         * 
         */
        public void PsychopathSurvivor(IncidentParms parms, out string flavorDesc, out GlobalTargetInfo target)
        {
            IntVec3 loc;
            Map map = (Map)parms.target;
            RCellFinder.TryFindRandomPawnEntryCell(out loc, map, CellFinder.EdgeRoadChance_Animal, false, null);
            Pawn newThing = PawnGenerator.GeneratePawn(PawnKindDefOf.Drifter, null);
            newThing.story.traits.allTraits = new List<Trait>();
            newThing.story.traits.GainTrait(new Trait(TraitDefOf.Psychopath, 0, true));
            if (Rand.Range(0, 100) > 60) newThing.story.traits.GainTrait(new Trait(TraitDefOf.Cannibal));
            if (Rand.Range(0, 100) > 95) newThing.story.traits.GainTrait(new Trait(TraitDefOf.Pyromaniac));
            if (Rand.Range(0, 100) > 60) newThing.story.traits.GainTrait(new Trait(TraitDefOf.Bloodlust));
            if (Rand.Range(0, 100) > 95) newThing.story.traits.GainTrait(new Trait(TraitDefOf.CreepyBreathing));
            Pawn survivor = (Pawn)GenSpawn.Spawn(newThing, loc, map);
            DamageDef damageType = (Rand.Range(0, 100) > 50) ? DamageDefOf.Cut : DamageDefOf.Burn;
            //survivor.mindState.mentalStateHandler.TryStartMentalState(DefDatabase<MentalStateDef>.GetNamed("MurderousRage"), otherPawn: ((Map)parms.target).mapPawns.FreeColonists.FirstOrDefault());
            survivor.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Berserk);

            RCellFinder.TryFindRandomSpotJustOutsideColony(survivor, out IntVec3 result);
            var lordJob = new LordJob_AssaultColony();
            LordMaker.MakeNewLord(survivor.Faction, lordJob, map, new List<Pawn>() { survivor });

            flavorDesc = "ROM_OmenSurvivorDesc2".Translate();
            target = new RimWorld.Planet.GlobalTargetInfo(survivor);
        }

        public void DeadSurvivor(IncidentParms parms, out string flavorDesc, out GlobalTargetInfo target)
        {

            IntVec3 loc;
            RCellFinder.TryFindRandomPawnEntryCell(out loc, (Map)parms.target, CellFinder.EdgeRoadChance_Animal, false, null);
            Pawn newThing = PawnGenerator.GeneratePawn(PawnKindDefOf.Drifter, null);
            Thing survivor = GenSpawn.Spawn(newThing, loc, (Map)parms.target);
            ((Pawn)survivor).Kill(null);
            flavorDesc = "ROM_OmenSurvivorDesc1".Translate();
            target = new GlobalTargetInfo(loc, (Map)parms.target);
        }
    }
}
