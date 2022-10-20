using System.Collections.Generic;
using Cthulhu;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI.Group;

namespace HPLovecraft
{
    public class IncidentWorker_LoneSurvivor : IncidentWorker
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Settings.DebugString("== Enter IncidentWorker_LoneSurvivor ==");
            var rand = Rand.Value;
            _ = new GlobalTargetInfo();
            GlobalTargetInfo target;
            string flavorDesc;
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
                {
                    PsychopathSurvivor(parms, out flavorDesc, out target);
                }
                else
                {
                    PlaguedSurvivor(parms, out flavorDesc, out target);
                }

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

            Find.LetterStack.ReceiveLetter(def.label.CapitalizeFirst(), flavorDesc,
                DefDatabase<LetterDef>.GetNamed("ROM_Omen"), target);
            return true;
        }


        /*
         * Dying Survivor
         *
         * 
         * 
         */
        public void DyingSurvivor(IncidentParms parms, out string flavorDesc, out GlobalTargetInfo target)
        {
            Settings.DebugString("Dying Survivor");
            var map = (Map) parms.target;
            RCellFinder.TryFindRandomPawnEntryCell(out var loc, map, CellFinder.EdgeRoadChance_Animal);
            var newThing = PawnGenerator.GeneratePawn(PawnKindDefOf.Drifter,
                Find.FactionManager.FirstFactionOfDef(FactionDef.Named("HPL_StrangersNeutral")));
            var survivor = (Pawn) GenSpawn.Spawn(newThing, loc, map);
            Utility.ApplySanityLoss(survivor);
            HealthUtility.DamageUntilDowned(survivor);
            flavorDesc = "ROM_OmenSurvivorDesc6".Translate();
            target = new GlobalTargetInfo(survivor);
        }

        /*
         * Plagued Survivor
         *
         * 
         * 
         */
        public void PlaguedSurvivor(IncidentParms parms, out string flavorDesc, out GlobalTargetInfo target)
        {
            Settings.DebugString("Plagued Survivor");
            var map = (Map) parms.target;
            RCellFinder.TryFindRandomPawnEntryCell(out var loc, map, CellFinder.EdgeRoadChance_Animal);
            var newThing = PawnGenerator.GeneratePawn(PawnKindDefOf.Drifter,
                Find.FactionManager.FirstFactionOfDef(FactionDef.Named("HPL_StrangersNeutral")));
            var survivor = (Pawn) GenSpawn.Spawn(newThing, loc, map);
            survivor.health.AddHediff(HediffDefOf.Plague);
            for (var i = 0; i < Rand.Range(1, 2); i++)
            {
                survivor.health.AddHediff(HediffDefOf.WoundInfection,
                    survivor.health.hediffSet.GetNotMissingParts().RandomElement());
            }

            survivor.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Wander_Sad);
            Utility.ApplySanityLoss(survivor);

            survivor.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Berserk);

            flavorDesc = "ROM_OmenSurvivorDesc5".Translate();
            target = new GlobalTargetInfo(survivor);
        }

        /*
         * Mad Survivor
         *
         * A person driven mad will wander the outskirts of the colony.
         *
         */
        public void MadSurvivor(IncidentParms parms, out string flavorDesc, out GlobalTargetInfo target)
        {
            Settings.DebugString("Mad Survivor");
            var map = (Map) parms.target;
            RCellFinder.TryFindRandomPawnEntryCell(out var loc, map, CellFinder.EdgeRoadChance_Animal);
            var newThing = PawnGenerator.GeneratePawn(PawnKindDefOf.Drifter,
                Find.FactionManager.FirstFactionOfDef(FactionDef.Named("HPL_StrangersNeutral")));
            var survivor = (Pawn) GenSpawn.Spawn(newThing, loc, (Map) parms.target);
            _ = Rand.Range(0, 100) > 50 ? DamageDefOf.Crush : DamageDefOf.Stab;
            _ = Rand.Range(0, 100) > 50 ? 3f : 5f;
            Utility.ApplySanityLoss(survivor, 0.7f);

            survivor.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Berserk);

            //SanityLossReport.ApplySanityLossAndShowReport(new List<Pawn> { survivor }, new FloatRange(0.7f, 0.9f), 0.02f);
            flavorDesc = "ROM_OmenSurvivorDesc4".Translate();
            target = new GlobalTargetInfo(survivor);
        }

        /*
        * Wild Survivor
        *
        * A survivor turned into a wild man will wander the outskirts of the colony.
        * 
        */
        public void WildSurvivor(IncidentParms parms, out string flavorDesc, out GlobalTargetInfo target)
        {
            Settings.DebugString("Wild Survivor");
            RCellFinder.TryFindRandomPawnEntryCell(out var loc, (Map) parms.target, CellFinder.EdgeRoadChance_Animal);
            var newThing = PawnGenerator.GeneratePawn(PawnKindDefOf.WildMan);
            var survivor = (Pawn) GenSpawn.Spawn(newThing, loc, (Map) parms.target);
            Utility.ApplySanityLoss(survivor, 0.7f);
            //SanityLossReport.ApplySanityLossAndShowReport(new List<Pawn> { survivor }, new FloatRange(0.7f, 0.9f), 0.02f);
            flavorDesc = "ROM_OmenSurvivorDesc3".Translate();
            target = new GlobalTargetInfo(survivor);
        }

        /*
         * Psychopath Survivor
         *
         * 
         * 
         */
        public void PsychopathSurvivor(IncidentParms parms, out string flavorDesc, out GlobalTargetInfo target)
        {
            Settings.DebugString("Psychopath Survivor");
            var map = (Map) parms.target;
            RCellFinder.TryFindRandomPawnEntryCell(out var loc, map, CellFinder.EdgeRoadChance_Animal);
            var newThing = PawnGenerator.GeneratePawn(PawnKindDefOf.Drifter,
                Find.FactionManager.FirstFactionOfDef(FactionDef.Named("HPL_StrangersHostile")));
            newThing.story.traits.allTraits = new List<Trait>();
            newThing.story.traits.GainTrait(new Trait(TraitDefOf.Psychopath, 0, true));
            if (Rand.Range(0, 100) > 60)
            {
                newThing.story.traits.GainTrait(new Trait(TraitDefOf.Cannibal));
            }

            if (Rand.Range(0, 100) > 95)
            {
                newThing.story.traits.GainTrait(new Trait(TraitDefOf.Pyromaniac));
            }

            if (Rand.Range(0, 100) > 60)
            {
                newThing.story.traits.GainTrait(new Trait(TraitDefOf.Bloodlust));
            }

            if (Rand.Range(0, 100) > 95)
            {
                newThing.story.traits.GainTrait(new Trait(TraitDefOf.CreepyBreathing));
            }

            var survivor = (Pawn) GenSpawn.Spawn(newThing, loc, map);
            _ = Rand.Range(0, 100) > 50 ? DamageDefOf.Cut : DamageDefOf.Burn;
            //survivor.mindState.mentalStateHandler.TryStartMentalState(DefDatabase<MentalStateDef>.GetNamed("MurderousRage"), otherPawn: ((Map)parms.target).mapPawns.FreeColonists.FirstOrDefault());
            //survivor.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Berserk);

            RCellFinder.TryFindRandomSpotJustOutsideColony(survivor, out var _);
            LordMaker.MakeNewLord(survivor.Faction, new LordJob_DefendPoint(map.Center), map,
                new List<Pawn> {survivor});

            flavorDesc = "ROM_OmenSurvivorDesc2".Translate();
            target = new GlobalTargetInfo(survivor);
        }

        public void DeadSurvivor(IncidentParms parms, out string flavorDesc, out GlobalTargetInfo target)
        {
            Settings.DebugString("Dead Survivor");
            RCellFinder.TryFindRandomPawnEntryCell(out var loc, (Map) parms.target, CellFinder.EdgeRoadChance_Animal);
            var newThing = PawnGenerator.GeneratePawn(PawnKindDefOf.Drifter,
                Find.FactionManager.FirstFactionOfDef(FactionDef.Named("HPL_StrangersNeutral")));
            var survivor = GenSpawn.Spawn(newThing, loc, (Map) parms.target);
            ((Pawn) survivor).Kill(null);
            flavorDesc = "ROM_OmenSurvivorDesc1".Translate();
            target = new GlobalTargetInfo(loc, (Map) parms.target);
        }
    }
}