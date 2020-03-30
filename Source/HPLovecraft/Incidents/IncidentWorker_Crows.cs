using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using RimWorld.Planet;

namespace HPLovecraft
{
    public class IncidentWorker_Crows : IncidentWorker
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {

            Settings.DebugString("== Enter IncidentWorker_Crows ==");
            string flavorDesc = "";
            Thing crow = null;
            float rand = Rand.Value;
            GlobalTargetInfo target = new GlobalTargetInfo();
            if (rand < 0.25f)
            {
                /*
                 * Dead Crow
                 *
                 * One dead crow appears.
                 * 
                 */
                Settings.DebugString("Dead Crow");
                IntVec3 loc;
                RCellFinder.TryFindRandomPawnEntryCell(out loc, (Map)parms.target, CellFinder.EdgeRoadChance_Animal, false, null);
                Pawn newThing = PawnGenerator.GeneratePawn(HPLDefOf.HPLovecraft_CrowKind, null);
                crow = GenSpawn.Spawn(newThing, loc, (Map)parms.target);
                ((Pawn)crow).Kill(null);
                flavorDesc = "ROM_OmenCatDesc1".Translate();
                target = new GlobalTargetInfo(loc, (Map)parms.target);
            }
            else if (rand < 0.5f)
            {
                /*
                 * Murder of Crows
                 *
                 * A group of crows appears outside your colony.
                 * One crow is dead in the center.
                 * 
                 */
                Settings.DebugString("Murder of Crows");
                IntVec3 loc;
                RCellFinder.TryFindRandomPawnEntryCell(out loc, (Map)parms.target, CellFinder.EdgeRoadChance_Animal, false, null);
                var newThing = PawnGenerator.GeneratePawn(HPLDefOf.HPLovecraft_CrowKind, null);
                crow = GenSpawn.Spawn(newThing, loc, (Map)parms.target);
                List<Thing> crows = new List<Thing>();
                for (int i = 0; i < 2; i++)
                {
                    Pawn newCrow = PawnGenerator.GeneratePawn(HPLDefOf.HPLovecraft_CrowKind, null);
                    crows.Add(GenSpawn.Spawn(newCrow, loc, (Map)parms.target));
                }
                crows.Add(crow);
                crow.Kill(null);
                flavorDesc = "ROM_OmenCrowDesc1".Translate();
                target = new RimWorld.Planet.GlobalTargetInfo(crows.FirstOrDefault(x => x is Pawn y && !y.Dead));
            }
            else if (rand < 0.75f)
            {
                /*
                 * Flock of Crows
                 *
                 * A group of crows spawn outside the colony.
                 * 
                 */
                Settings.DebugString("Flock of Crows");
                IntVec3 loc;
                RCellFinder.TryFindRandomPawnEntryCell(out loc, (Map)parms.target, CellFinder.EdgeRoadChance_Animal, false, null);
                Pawn newThing = PawnGenerator.GeneratePawn(HPLDefOf.HPLovecraft_CrowKind, null);
                crow = GenSpawn.Spawn(newThing, loc, (Map)parms.target);
                List<Thing> crows = new List<Thing>();
                for (int i = 0; i < 2; i++)
                {
                    Pawn newCrow = PawnGenerator.GeneratePawn(HPLDefOf.HPLovecraft_CrowKind, null);
                    crows.Add(GenSpawn.Spawn(newCrow, loc, (Map)parms.target));
                }
                crows.Add(crow);
                flavorDesc = "ROM_OmenCrowDesc2".Translate();
                target = new RimWorld.Planet.GlobalTargetInfo(crow);
            }
            else
            {
                /*
                 * Solitary Crow
                 *
                 * A single crow watches the colony.
                 *
                 */
                Settings.DebugString("Solitary Crow");
                IntVec3 loc;
                RCellFinder.TryFindRandomPawnEntryCell(out loc, (Map)parms.target, CellFinder.EdgeRoadChance_Animal, false, null);
                Pawn newThing = PawnGenerator.GeneratePawn(HPLDefOf.HPLovecraft_CrowKind, null);
                crow = GenSpawn.Spawn(newThing, loc, (Map)parms.target);
                flavorDesc = "ROM_OmenCrowDesc3".Translate();
                target = new RimWorld.Planet.GlobalTargetInfo(crow);
            }
            Find.LetterStack.ReceiveLetter(def.label.CapitalizeFirst(), flavorDesc, DefDatabase<LetterDef>.GetNamed("ROM_Omen"), target);
            return true;
        }
    }
}
