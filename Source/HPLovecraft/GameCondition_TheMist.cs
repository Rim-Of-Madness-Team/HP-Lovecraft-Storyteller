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
            if (base.Map?.mapPawns?.AllPawnsSpawned?.FindAll(x => x is PawnMistCreature) is List<Pawn> mistCreatures)
            {
                if (!mistCreatures.NullOrEmpty()) foreach (Pawn p in mistCreatures) p.Kill(null);
            }
            base.End();
        }

        public override void GameConditionTick()
        {
            if (Find.TickManager.TicksGame % interval == 0)
            {
                //Keep it fogged!
                if (base.Map.weatherManager.curWeather != HPLDefOf.Fog) base.Map.weatherManager.TransitionTo(HPLDefOf.Fog);

                int maxMistCreatureCount = (int)((base.Map.mapPawns.ColonistCount * 4) * (((float)base.Map.Size.x) / 250f));
                int curMistCreatureCount = base.Map.mapPawns.AllPawnsSpawned.FindAll(x => x is PawnMistCreature).Count;

                int i = 100;
                while ((curMistCreatureCount < maxMistCreatureCount) && i > 0)
                {
                    var intVec = default(IntVec3);
                    if (!Cthulhu.Utility.TryFindSpawnCell(HPLDefOf.HPLovecraft_MistCreature, base.Map.Center, base.Map, 60, out intVec))
                    {
                        continue;
                    }
                    PawnKindDef randomKind = (Rand.Value > 0.3f) ? HPLDefOf.HPLovecraft_MistStalker : HPLDefOf.HPLovecraft_MistStalkerTwo;
                    Pawn newThing = PawnGenerator.GeneratePawn(randomKind, null);
                    Thing stalker = GenSpawn.Spawn(newThing, intVec, base.Map);
                    i--;
                }

                //List<Pawn> allPawnsSpawned = base.Map.mapPawns.AllPawnsSpawned;
                //for (int i = 0; i < allPawnsSpawned.Count; i++)
                //{
                //    Pawn pawn = allPawnsSpawned[i];
                //    if (!pawn.Position.Roofed(base.Map) && pawn.def.race.IsFlesh)
                //    {
                //        float num = 0.1f;
                //        num *= pawn?.GetStatValue(StatDefOf.PsychicSensitivity, true) ?? 1;
                //        if (num != 0f)
                //        {
                //            float num2 = Mathf.Lerp(0.85f, 1.15f, Rand.ValueSeeded(pawn.thingIDNumber ^ 74374237));
                //            num *= num2;
                //            Cthulhu.Utility.ApplySanityLoss(pawn, num, 1f);
                //        }
                //    }
                //}
            }
        }

        public override void DoCellSteadyEffects(IntVec3 c)
        {
            //if (!c.Roofed(base.Map))
            //{
            //    List<Thing> thingList = c.GetThingList(base.Map);
            //    for (int i = 0; i < thingList.Count; i++)
            //    {
            //        Thing thing = thingList[i];
            //        if (thing is Pawn pawn)
            //        {
            //            if (Rand.Value < 0.05f)
            //            {

            //            }
            //        }
            //        //if (thing is Plant)
            //        //{
            //        //    if (Rand.Value < 0.0065f)
            //        //    {
            //        //        thing.Kill(null);
            //        //    }
            //        //}
            //        //else if (thing.def.category == ThingCategory.Item)
            //        //{
            //        //    CompRottable compRottable = thing.TryGetComp<CompRottable>();
            //        //    if (compRottable != null && compRottable.Stage < RotStage.Dessicated)
            //        //    {
            //        //        compRottable.RotProgress += 3000f;
            //        //    }
            //        //}
            //    }
            //}
        }

        //public override void GameConditionDraw()
        //{
        //    Map map = base.Map;
        //    for (int i = 0; i < this.overlays.Count; i++)
        //    {
        //        this.overlays[i].DrawOverlay(map);
        //    }
        //}

        //public override float SkyTargetLerpFactor()
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

        public override bool AllowEnjoyableOutsideNow()
        {
            return false;
        }

        //public override List<SkyOverlay> SkyOverlays()
        //{
        //    return this.overlays;
        //}
    }
}
