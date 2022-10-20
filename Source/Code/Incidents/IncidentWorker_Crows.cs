using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace HPLovecraft
{
    public class IncidentWorker_Crows : IncidentWorker
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Settings.DebugString("== Enter IncidentWorker_Crows ==");
            string flavorDesc;
            Thing crow;
            var rand = Rand.Value;
            GlobalTargetInfo target;
            if (rand < 0.25f)
            {
                /*
                 * Dead Crow
                 *
                 * One dead crow appears.
                 * 
                 */
                Settings.DebugString("Dead Crow");
                RCellFinder.TryFindRandomPawnEntryCell(out var loc, (Map) parms.target,
                    CellFinder.EdgeRoadChance_Animal);
                var newThing = PawnGenerator.GeneratePawn(HPLDefOf.HPLovecraft_CrowKind);
                crow = GenSpawn.Spawn(newThing, loc, (Map) parms.target);
                ((Pawn) crow).Kill(null);
                flavorDesc = "ROM_OmenCatDesc1".Translate();
                target = new GlobalTargetInfo(loc, (Map) parms.target);
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
                RCellFinder.TryFindRandomPawnEntryCell(out var loc, (Map) parms.target,
                    CellFinder.EdgeRoadChance_Animal);
                var newThing = PawnGenerator.GeneratePawn(HPLDefOf.HPLovecraft_CrowKind);
                crow = GenSpawn.Spawn(newThing, loc, (Map) parms.target);
                var crows = new List<Thing>();
                for (var i = 0; i < 2; i++)
                {
                    var newCrow = PawnGenerator.GeneratePawn(HPLDefOf.HPLovecraft_CrowKind);
                    crows.Add(GenSpawn.Spawn(newCrow, loc, (Map) parms.target));
                }

                crows.Add(crow);
                crow.Kill();
                flavorDesc = "ROM_OmenCrowDesc1".Translate();
                target = new GlobalTargetInfo(crows.FirstOrDefault(x => x is Pawn {Dead: false}));
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
                RCellFinder.TryFindRandomPawnEntryCell(out var loc, (Map) parms.target,
                    CellFinder.EdgeRoadChance_Animal);
                var newThing = PawnGenerator.GeneratePawn(HPLDefOf.HPLovecraft_CrowKind);
                crow = GenSpawn.Spawn(newThing, loc, (Map) parms.target);
                var crows = new List<Thing>();
                for (var i = 0; i < 2; i++)
                {
                    var newCrow = PawnGenerator.GeneratePawn(HPLDefOf.HPLovecraft_CrowKind);
                    crows.Add(GenSpawn.Spawn(newCrow, loc, (Map) parms.target));
                }

                crows.Add(crow);
                flavorDesc = "ROM_OmenCrowDesc2".Translate();
                target = new GlobalTargetInfo(crow);
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
                RCellFinder.TryFindRandomPawnEntryCell(out var loc, (Map) parms.target,
                    CellFinder.EdgeRoadChance_Animal);
                var newThing = PawnGenerator.GeneratePawn(HPLDefOf.HPLovecraft_CrowKind);
                crow = GenSpawn.Spawn(newThing, loc, (Map) parms.target);
                flavorDesc = "ROM_OmenCrowDesc3".Translate();
                target = new GlobalTargetInfo(crow);
            }

            Find.LetterStack.ReceiveLetter(def.label.CapitalizeFirst(), flavorDesc,
                DefDatabase<LetterDef>.GetNamed("ROM_Omen"), target);
            return true;
        }
    }
}