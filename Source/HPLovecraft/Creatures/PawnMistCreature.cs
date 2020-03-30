using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;

namespace HPLovecraft
{
    public class PawnMistCreature : Pawn
    {
        private bool DestroyInProgress;

        public override void TickRare()
        {
            base.TickRare();
            if (Spawned && !Dead && !Destroyed)
            {
                ObservationEffect();
                DissipateCheck();   
            }
        }

        //During unfavorable weather conditions, destroy the mist creature.
        public void DissipateCheck()
        {
            if (!DestroyInProgress)
            {
                if (PawnUtility.EverBeenColonistOrTameAnimal(this))
                {
                    var cat = GenSpawn.Spawn(ThingDef.Named("HPLovecraft_CatRace"), this.PositionHeld, this.MapHeld);
                    cat.SetFaction(Faction.OfPlayer);
                    Messages.Message("ROM_ItWasACat".Translate(), cat, MessageTypeDefOf.PositiveEvent);

                    DestroyMe();
                    
                    return;
                }
                if ((Downed && !Dead) || this.MapHeld.weatherManager.curWeather != HPLDefOf.Fog || this.PositionHeld.Roofed(this.MapHeld)) {
                    DestroyMe();
                    return;
                }
            }

        }

        // Any damage dealt results in the mist creature's dissipation.
        public override void PostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            if (totalDamageDealt > 0f)
            {
                if (!DestroyInProgress)
                {
                    if (!Dead || !Destroyed)
                    {
                        DestroyMe();
                    }   
                }
            }
        }

        private void DestroyMe()
        {
            DestroyInProgress = true;
            this.Destroy();
            //LongEventHandler.QueueLongEvent(() => { this.Destroy(); }, "destroyMist", true, null);
        }

        public Predicate<Thing> Predicate => delegate (Thing t)
        {
            if (t == null)
                return false;
            if (t == this)
                return false;
            if (!t.Spawned)
                return false;
            Pawn pawn1 = t as Pawn;
            if (pawn1 == null)
                return false;
            if (pawn1.Dead)
                return false;
            if (pawn1 is PawnMistCreature)
                return false;
            if (pawn1.Faction == null)
                return false;
            if (this.Faction != null && pawn1.Faction != null)
            {
                if (this.Faction == pawn1.Faction)
                    return false;
                if (!this.Faction.HostileTo(pawn1.Faction))
                    return false;
            }

            if (pawn1.needs == null)
                return false;
            if (pawn1.needs.mood == null)
                return false;
            if (pawn1.needs.mood.thoughts == null)
                return false;
            if (pawn1.needs.mood.thoughts.memories == null)
                return false;
            return true;
        };

        /// <summary>
        /// Repurposed Observation Effect code from Cosmic Horrors.
        /// Only processes LIVE creatures.
        /// 
        /// Checks around the cosmic horror for pawns to give sanity loss.
        /// Also gives a bad memory of the experinece
        /// </summary>
        public void ObservationEffect()
        {
            try
            {
                //This finds a suitable target pawn.
                Predicate<Thing> predicate = this.Predicate;

                Thing thing2 = GenClosest.ClosestThingReachable(this.PositionHeld, this.MapHeld, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.OnCell, TraverseParms.For(this, Danger.Deadly, TraverseMode.PassDoors, false), 15, predicate);
                if (thing2 != null && thing2.Position != IntVec3.Invalid)
                {
                    if (GenSight.LineOfSight(thing2.Position, this.PositionHeld, this.MapHeld))
                    {
                        if (thing2 is Pawn target)
                        {
                            if (target.RaceProps != null)
                            {
                                if (!target.RaceProps.IsMechanoid)
                                {
                                    if (!this.Dead && this.MapHeld != null)
                                    {
                                        if (this.StoringThing() == null && target.RaceProps.Humanlike)
                                        {
                                            if (target?.story?.traits?.GetTrait(HPLDefOf.PsychicSensitivity) is Trait psy && psy.Degree > -1)
                                            {
                                                Thought_MemoryObservation thought_MemoryObservation;
                                                thought_MemoryObservation = (Thought_MemoryObservation)ThoughtMaker.MakeThought(DefDatabase<ThoughtDef>.GetNamed("ROM_ObservedMistCreaturePsychic"));
                                                thought_MemoryObservation.Target = this;
                                                target.needs.mood.thoughts.memories.TryGainMemory(thought_MemoryObservation);
                                            }
                                            else
                                            {
                                                Thought_MemoryObservation thought_MemoryObservation;
                                                thought_MemoryObservation = (Thought_MemoryObservation)ThoughtMaker.MakeThought(DefDatabase<ThoughtDef>.GetNamed("ROM_ObservedMistCreature"));
                                                thought_MemoryObservation.Target = this;
                                                target.needs.mood.thoughts.memories.TryGainMemory(thought_MemoryObservation);
                                            }
                                        }

                                        Cthulhu.Utility.ApplySanityLoss(target, 0.003f, 0.8f);
                                    }
                                }
                            }
                        }
                    }

                }
            }
            catch (NullReferenceException)
            { }
        }

    }
}
