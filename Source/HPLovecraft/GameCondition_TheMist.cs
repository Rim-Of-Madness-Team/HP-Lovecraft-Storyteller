using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;

namespace HPLovecraft
{
    public class GameCondition_TheMist : GameCondition
    {
        private static int interval = 3451;

        public GameCondition_TheMist()
        {
            /*
            ColorInt colorInt = new ColorInt(216, 255, 0);
            Color arg_50_0 = colorInt.ToColor;
            ColorInt colorInt2 = new ColorInt(234, 200, 255);
            this.ToxicFalloutColors = new SkyColorSet(arg_50_0, colorInt2.ToColor, new Color(0.6f, 0.8f, 0.5f), 0.85f);
                    
            this.overlays = new List<SkyOverlay>
            {
                new WeatherOverlay_Fog()
            };
            */
            //base.Map.weatherManager.TransitionTo(HPLDefOf.Fog);
        }

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
                if (pawnGroup?.Count > 0) mistCreatures.AddRange(pawnGroup);
            }
            if (mistCreatures?.Count > 0) foreach (Pawn p in mistCreatures) p.Kill(null);
            base.End();
        }

        public override void GameConditionTick()
        {
            if (Find.TickManager.TicksGame % interval == 0)
            {
                //Keep it fogged!
                foreach (var map in AffectedMaps)
                {
                   
                    if (map.weatherManager.curWeather != HPLDefOf.Fog) map.weatherManager.TransitionTo(HPLDefOf.Fog);

                    int maxMistCreatureCount = (int)((map.mapPawns.ColonistCount * 4) * (((float)map.Size.x) / 250f));
                    int curMistCreatureCount = map.mapPawns.AllPawnsSpawned.FindAll(x => x is PawnMistCreature).Count;

                    int i = 100;
                    while ((curMistCreatureCount < maxMistCreatureCount) && i > 0)
                    {
                        var intVec = default(IntVec3);
                        if (!Cthulhu.Utility.TryFindSpawnCell(HPLDefOf.HPLovecraft_MistCreature, map.Center, map, 60, out intVec))
                        {
                            continue;
                        }
                        PawnKindDef randomKind = (Rand.Value > 0.3f) ? HPLDefOf.HPLovecraft_MistStalker : HPLDefOf.HPLovecraft_MistStalkerTwo;
                        Pawn newThing = PawnGenerator.GeneratePawn(randomKind, null);
                        Thing stalker = GenSpawn.Spawn(newThing, intVec, map);
                        i--;
                    } 
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
