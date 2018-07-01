using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace HPLovecraft
{
    public class GameCondition_BloodMoon : GameCondition
    {
        private const int LerpTicks = 200;

        private SkyColorSet BloodSkyColors = new SkyColorSet(new Color(0.9f, 0.103f, 0.182f), Color.white, new Color(0.7f, 0.1f, 0.1f), 1f);

        private bool firstTick = true;

        public override float SkyTargetLerpFactor(Map map)
        {
            return GameConditionUtility.LerpInOutValue((float)base.TicksPassed, (float)base.TicksLeft, 200f, 1f);
        }


        public override void GameConditionTick()
        {
            base.GameConditionTick();
            if (firstTick)
            {
                var affectedPawns = new List<Pawn>();
                foreach (var map in this.AffectedMaps)
                {
                    affectedPawns.AddRange(map.mapPawns.FreeColonistsAndPrisoners);
                }
                
                foreach (Pawn pawn in affectedPawns)
                {
                    if (ThoughtUtility.CanGetThought(pawn, HPLDefOf.HPLovecraft_SawBloodMoonSad))
                    {
                        pawn.needs.mood.thoughts.memories.TryGainMemory(HPLDefOf.HPLovecraft_SawBloodMoonSad);
                    }
                    else if (ThoughtUtility.CanGetThought(pawn, HPLDefOf.HPLovecraft_SawBloodMoonHappy))
                    {
                        pawn.needs.mood.thoughts.memories.TryGainMemory(HPLDefOf.HPLovecraft_SawBloodMoonHappy);
                    }
                }
                firstTick = false;
            }
        }

        public override SkyTarget? SkyTarget(Map map)
        {
            return new SkyTarget?(new SkyTarget(0f, this.BloodSkyColors, 1f, 0f));
        }
    }
}
