using System;
using Cthulhu;
using RimWorld;
using Verse;
using Verse.AI;

namespace HPLovecraft
{
    public class PawnMistCreature : Pawn
    {
        private bool DestroyInProgress;

        public Predicate<Thing> Predicate => delegate(Thing t)
        {
            if (t == null)
            {
                return false;
            }

            if (t == this)
            {
                return false;
            }

            if (!t.Spawned)
            {
                return false;
            }

            if (!(t is Pawn pawn1))
            {
                return false;
            }

            if (pawn1.Dead)
            {
                return false;
            }

            if (pawn1 is PawnMistCreature)
            {
                return false;
            }

            if (pawn1.Faction == null)
            {
                return false;
            }

            if (Faction != null && pawn1.Faction != null)
            {
                if (Faction == pawn1.Faction)
                {
                    return false;
                }

                if (!Faction.HostileTo(pawn1.Faction))
                {
                    return false;
                }
            }

            if (pawn1.needs?.mood?.thoughts?.memories == null)
            {
                return false;
            }

            return true;
        };

        public override void TickRare()
        {
            base.TickRare();
            if (!Spawned || Dead || Destroyed)
            {
                return;
            }

            ObservationEffect();
            DissipateCheck();
        }

        //During unfavorable weather conditions, destroy the mist creature.
        public void DissipateCheck()
        {
            if (DestroyInProgress)
            {
                return;
            }

            if (PawnUtility.EverBeenColonistOrTameAnimal(this))
            {
                var cat = GenSpawn.Spawn(ThingDef.Named("HPLovecraft_CatRace"), PositionHeld, MapHeld);
                cat.SetFaction(Faction.OfPlayer);
                Messages.Message("ROM_ItWasACat".Translate(), cat, MessageTypeDefOf.PositiveEvent);

                DestroyMe();

                return;
            }

            if (Downed && !Dead || MapHeld.weatherManager.curWeather != HPLDefOf.Fog ||
                PositionHeld.Roofed(MapHeld))
            {
                DestroyMe();
            }
        }

        // Any damage dealt results in the mist creature's dissipation.
        public override void PostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            if (!(totalDamageDealt > 0f))
            {
                return;
            }

            if (DestroyInProgress)
            {
                return;
            }

            if (!Dead || !Destroyed)
            {
                DestroyMe();
            }
        }

        private void DestroyMe()
        {
            DestroyInProgress = true;
            Destroy();
            //LongEventHandler.QueueLongEvent(() => { this.Destroy(); }, "destroyMist", true, null);
        }

        /// <summary>
        ///     Repurposed Observation Effect code from Cosmic Horrors.
        ///     Only processes LIVE creatures.
        ///     Checks around the cosmic horror for pawns to give sanity loss.
        ///     Also gives a bad memory of the experinece
        /// </summary>
        public void ObservationEffect()
        {
            try
            {
                //This finds a suitable target pawn.
                var predicate = Predicate;

                var thing2 = GenClosest.ClosestThingReachable(PositionHeld, MapHeld,
                    ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.OnCell,
                    TraverseParms.For(this, Danger.Deadly, TraverseMode.PassDoors), 15, predicate);
                if (thing2 == null || thing2.Position == IntVec3.Invalid)
                {
                    return;
                }

                if (!GenSight.LineOfSight(thing2.Position, PositionHeld, MapHeld))
                {
                    return;
                }

                if (thing2 is not Pawn target)
                {
                    return;
                }

                if (target.RaceProps == null)
                {
                    return;
                }

                if (target.RaceProps.IsMechanoid)
                {
                    return;
                }

                if (Dead || MapHeld == null)
                {
                    return;
                }

                if (this.StoringThing() == null && target.RaceProps.Humanlike)
                {
                    if (target.story?.traits?.GetTrait(HPLDefOf.PsychicSensitivity) is {Degree: > -1})
                    {
                        var thought_MemoryObservation = (Thought_MemoryObservation) ThoughtMaker.MakeThought(
                            DefDatabase<ThoughtDef>.GetNamed(
                                "ROM_ObservedMistCreaturePsychic"));
                        thought_MemoryObservation.Target = this;
                        target.needs.mood.thoughts.memories.TryGainMemory(
                            thought_MemoryObservation);
                    }
                    else
                    {
                        var thought_MemoryObservation = (Thought_MemoryObservation) ThoughtMaker.MakeThought(
                            DefDatabase<ThoughtDef>.GetNamed("ROM_ObservedMistCreature"));
                        thought_MemoryObservation.Target = this;
                        target.needs.mood.thoughts.memories.TryGainMemory(
                            thought_MemoryObservation);
                    }
                }

                Utility.ApplySanityLoss(target, 0.003f, 0.8f);
            }
            catch
            {
                // ignored
            }
        }
    }
}