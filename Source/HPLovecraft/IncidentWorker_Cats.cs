using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using RimWorld.Planet;

namespace HPLovecraft
{
    public class IncidentWorker_Cats : IncidentWorker
    {
        public override bool TryExecute(IncidentParms parms)
        {
            string flavorDesc = "";
            float rand = Rand.Value;
            GlobalTargetInfo target = new GlobalTargetInfo();

            if (rand < 0.25f)
            {
                DeadCat(parms, out flavorDesc, out target);
            }
            else if (rand < 0.5f)
            {
                //Non-hostile Period is the first three seasons of omens.
                if (GenDate.DaysPassed > 45) WildCats(parms, out flavorDesc, out target);
                else
                {
                    StrayCat(parms, out flavorDesc, out target);
                }
            }
            else if (rand < 0.75f)
            {
                StrayCat(parms, out flavorDesc, out target);
            }
            else
            {
                AffectionateCat(parms, out flavorDesc, out target);
            }
            Find.LetterStack.ReceiveLetter(def.label.CapitalizeFirst(), flavorDesc, DefDatabase<LetterDef>.GetNamed("ROM_Omen"), target);
            return true;
        }


        /*
         * Affectionate Cat
         *
         * A single cat joins the colony.
         *
         */
        public void AffectionateCat(IncidentParms parms, out string flavorDesc, out GlobalTargetInfo target)
        {
            Log.Message("Affectionate Cat");
            //Affectionate cat
            IntVec3 loc;
            RCellFinder.TryFindRandomPawnEntryCell(out loc, (Map)parms.target, CellFinder.EdgeRoadChance_Animal, null);
            Pawn newThing = PawnGenerator.GeneratePawn(HPLDefOf.HPLovecraft_CatKind_Black, null);
            Thing cat = GenSpawn.Spawn(newThing, loc, (Map)parms.target);
            InteractionWorker_RecruitAttempt.DoRecruit(cat.Map.mapPawns.FreeColonists.FirstOrDefault<Pawn>(), (Pawn)cat, 1f, true);
            flavorDesc = "ROM_OmenCatDesc4".Translate();
            target = new RimWorld.Planet.GlobalTargetInfo(cat);
        }

        /*
        * Stray Cat
        *
        * A single cat wanders onto the colony's border.
        * 
        */
        public void StrayCat(IncidentParms parms, out string flavorDesc, out GlobalTargetInfo target)
        {
            Log.Message("Stray Cat");
            //Stray cat
            IntVec3 loc;
            RCellFinder.TryFindRandomPawnEntryCell(out loc, (Map)parms.target, CellFinder.EdgeRoadChance_Animal, null);
            Pawn newThing = PawnGenerator.GeneratePawn(HPLDefOf.HPLovecraft_CatKind_Black, null);
            Thing cat = GenSpawn.Spawn(newThing, loc, (Map)parms.target);
            flavorDesc = "ROM_OmenCatDesc3".Translate();
            target = new RimWorld.Planet.GlobalTargetInfo(cat);
        }

        /*
         * Wild Cats
         *
         * Multiple manhunter cats wander onto the colony's border.
         * 
         */
        public void WildCats(IncidentParms parms, out string flavorDesc, out GlobalTargetInfo target)
        {
            Log.Message("Wild Cats");
            //Wild cats
            IntVec3 loc;
            GlobalTargetInfo? newTarget = null;
            RCellFinder.TryFindRandomPawnEntryCell(out loc, (Map)parms.target, CellFinder.EdgeRoadChance_Animal, null);
            var numberOfCats = (GenDate.YearsPassed > 0) ? GenDate.YearsPassed : 1;
            List<Thing> cats = new List<Thing>();
            for (int i = 0; i < numberOfCats; i++)
            {
                Pawn newCat = PawnGenerator.GeneratePawn(HPLDefOf.HPLovecraft_CatKind_Black, null);
                cats.Add(GenSpawn.Spawn(newCat, loc, (Map)parms.target));
            }
            foreach (Pawn single in cats)
            {
                single.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Manhunter);
                if (newTarget == null) newTarget = new GlobalTargetInfo(single);
            }
            target = newTarget.Value;
            flavorDesc = "ROM_OmenCatDesc2".Translate();

        }

        public void DeadCat(IncidentParms parms, out string flavorDesc, out GlobalTargetInfo target)
        {
            Log.Message("Dead Cat");
            //Dead cat
            IntVec3 loc;
            RCellFinder.TryFindRandomPawnEntryCell(out loc, (Map)parms.target, CellFinder.EdgeRoadChance_Animal, null);
            Pawn newThing = PawnGenerator.GeneratePawn(HPLDefOf.HPLovecraft_CatKind_Black, null);
            Thing cat = GenSpawn.Spawn(newThing, loc, (Map)parms.target);
            ((Pawn)cat).Kill(null);
            flavorDesc = "ROM_OmenCatDesc1".Translate();
            target = new GlobalTargetInfo(loc, (Map)parms.target);
        }
    }
}
