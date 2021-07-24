using System.Collections.Generic;
using RimWorld;
using Verse;

namespace HPLovecraft
{
    public class GameCondition_TheMist : GameCondition
    {
        private int ticksInFog;
        //private static int interval = 3451;

        public override void Init()
        {
            LessonAutoActivator.TeachOpportunity(ConceptDefOf.ForbiddingDoors, OpportunityType.Critical);
            LessonAutoActivator.TeachOpportunity(ConceptDefOf.AllowedAreas, OpportunityType.Critical);
        }

        public override void End()
        {
            var mistCreatures = new HashSet<Pawn>();
            foreach (var map in AffectedMaps)
            {
                var pawnGroup = map?.mapPawns?.AllPawnsSpawned?.FindAll(x => x is PawnMistCreature);
                if (pawnGroup?.Count > 0)
                {
                    mistCreatures.AddRange(pawnGroup);
                }
            }

            if (mistCreatures.Count > 0)
            {
                foreach (var p in mistCreatures)
                {
                    p.Kill(null);
                }
            }

            base.End();
        }

        public override void GameConditionTick()
        {
            //Keep it fogged!
            ticksInFog++;
            if (ticksInFog % 1000 != 0)
            {
                return;
            }

            if (!(AffectedMaps?.Count > 0))
            {
                return;
            }

            foreach (var map in AffectedMaps)
            {
                if (map.weatherManager.curWeather != HPLDefOf.Fog)
                {
                    map.weatherManager.TransitionTo(HPLDefOf.Fog);
                }

                var maxMistCreatureCount = (int) (map.mapPawns.ColonistCount * 4 * (map.Size.x / 250f));
                var curMistCreatureCount = map.mapPawns.AllPawnsSpawned.FindAll(x => x is PawnMistCreature).Count;

                var i = 100;
                while (curMistCreatureCount < maxMistCreatureCount && i > 0)
                {
                    CellFinder.TryFindRandomCellNear(map.Center, map, 60,
                        c => c.Standable(map) && !map.roofGrid.Roofed(c) && map.reachability.CanReachColony(c),
                        out var spawnLoc, 100);
                    var randomKind = Rand.Value > 0.3f
                        ? HPLDefOf.HPLovecraft_MistStalker
                        : HPLDefOf.HPLovecraft_MistStalkerTwo;
                    var newThing = PawnGenerator.GeneratePawn(randomKind);
                    var unused = GenSpawn.Spawn(newThing, spawnLoc, map);
                    i--;
                }
            }
        }

        public override void DoCellSteadyEffects(IntVec3 c, Map map)
        {
        }
        //public override void GameConditionDraw()
        //{
        //    Map map = base.Map;
        //    for (int i = 0; i < this.overlays.Count; i++)
        //    {
        //        this.overlays[i].DrawOverlay(map);
        //    }
        //}

        //public override float SkyTargetLerpFactorMap map()
        //{
        //    return GameConditionUtility.LerpInOutValue((float)base.TicksPassed, (float)base.TicksLeft, 5000f, 0.5f);
        //}

        //public override SkyTarget? SkyTarget()
        //{
        //    return HPLDefOf.Fog.Worker.CurSkyTarget(base.Map);
        //}

        //public override float AnimalDensityFactor()
        //{
        //    return 0f;
        //}

        //public override float PlantDensityFactor()
        //{
        //    return 0f;
        //}

        public override bool AllowEnjoyableOutsideNow(Map map)
        {
            return false;
        }

        //public override List<SkyOverlay> SkyOverlays()
        //{
        //    return this.overlays;
        //}
    }
}