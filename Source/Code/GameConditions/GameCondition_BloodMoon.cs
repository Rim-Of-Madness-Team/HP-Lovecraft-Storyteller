using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace HPLovecraft
{
    public class GameCondition_BloodMoon : GameCondition
    {
        //private const int LerpTicks = 200;

        private readonly SkyColorSet BloodSkyColors = new SkyColorSet(new Color(0.9f, 0.103f, 0.182f), Color.white,
            new Color(0.7f, 0.1f, 0.1f), 1f);

        private bool firstTick = true;

        public override float SkyTargetLerpFactor(Map map)
        {
            return GameConditionUtility.LerpInOutValue(TicksPassed, TicksLeft, 200f);
        }


        public override void GameConditionTick()
        {
            base.GameConditionTick();
            if (!firstTick)
            {
                return;
            }

            firstTick = false;

            var affectedPawns = new List<Pawn>();
            foreach (var map in AffectedMaps)
            {
                affectedPawns.AddRange(map.mapPawns.FreeColonistsAndPrisoners);

                //Add a wolf pack
                var wolfType = map.mapTemperature.OutdoorTemp > 0f
                    ? PawnKindDef.Named("Wolf_Timber")
                    : PawnKindDef.Named("Wolf_Arctic");
                RCellFinder.TryFindRandomPawnEntryCell(out var loc, map, CellFinder.EdgeRoadChance_Animal);
                var numberOfWolves = Rand.Range(3, 6);
                for (var i = 0; i < numberOfWolves; i++)
                {
                    var newWolf = PawnGenerator.GeneratePawn(wolfType);
                    GenSpawn.Spawn(newWolf, loc, map);
                }
            }

            foreach (var pawn in affectedPawns)
            {
                if (ThoughtUtility.CanGetThought(pawn, HPLDefOf.HPLovecraft_SawBloodMoonSad))
                {
                    pawn.needs.mood.thoughts.memories.TryGainMemory(HPLDefOf.HPLovecraft_SawBloodMoonSad);
                    continue;
                }

                if (ThoughtUtility.CanGetThought(pawn, HPLDefOf.HPLovecraft_SawBloodMoonHappy))
                {
                    pawn.needs.mood.thoughts.memories.TryGainMemory(HPLDefOf.HPLovecraft_SawBloodMoonHappy);
                }
            }
        }

        public override SkyTarget? SkyTarget(Map map)
        {
            return new SkyTarget(0f, BloodSkyColors, 1f, 0f);
        }
    }
}