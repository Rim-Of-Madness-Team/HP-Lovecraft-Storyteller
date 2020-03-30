using System;
using Verse;
using Verse.AI;
using RimWorld;

namespace HPLovecraft
{
    public class JobGiver_StalkCharacters : JobGiver_Wander
    {
        public Pawn ClosestCharacter(Pawn pawn)
        {
            //return (Pawn)AttackTargetFinder.BestAttackTarget(pawn, TargetScanFlags.NeedLOSToPawns | TargetScanFlags.NeedLOSToNonPawns | TargetScanFlags.NeedReachableIfCantHitFromMyPos, x => x is Pawn && !(x is PawnMistCreature), 0f, 12f, default(IntVec3), 3.40282347E+38f, false);
            return (Pawn)GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.Touch, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 30f, (Thing x) => x is Pawn p && !(x is PawnMistCreature) && !x.DestroyedOrNull() && !p.RaceProps.Animal, null, 0, 30, false, RegionType.Set_Passable, false);
        }

    public JobGiver_StalkCharacters()
        {
            this.wanderRadius = 3f;
            this.ticksBetweenWandersRange = new IntRange(125, 200);
        }

        protected override IntVec3 GetWanderRoot(Pawn pawn)
        {
            return (ClosestCharacter(pawn) is Pawn otherPawn) ? WanderUtility.BestCloseWanderRoot(otherPawn.PositionHeld, pawn) : pawn.PositionHeld;
        }
    }
}
