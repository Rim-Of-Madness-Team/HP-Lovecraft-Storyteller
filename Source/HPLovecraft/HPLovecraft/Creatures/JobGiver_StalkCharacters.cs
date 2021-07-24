using Verse;
using Verse.AI;

namespace HPLovecraft
{
    public class JobGiver_StalkCharacters : JobGiver_Wander
    {
        public JobGiver_StalkCharacters()
        {
            wanderRadius = 3f;
            ticksBetweenWandersRange = new IntRange(125, 200);
        }

        public Pawn ClosestCharacter(Pawn pawn)
        {
            //return (Pawn)AttackTargetFinder.BestAttackTarget(pawn, TargetScanFlags.NeedLOSToPawns | TargetScanFlags.NeedLOSToNonPawns | TargetScanFlags.NeedReachableIfCantHitFromMyPos, x => x is Pawn && !(x is PawnMistCreature), 0f, 12f, default(IntVec3), 3.40282347E+38f, false);
            return (Pawn) GenClosest.ClosestThingReachable(pawn.Position, pawn.Map,
                ThingRequest.ForGroup(ThingRequestGroup.Pawn), PathEndMode.Touch, TraverseParms.For(pawn), 30f,
                x => x is Pawn p && !(x is PawnMistCreature) && !x.DestroyedOrNull() && !p.RaceProps.Animal, null, 0,
                30);
        }

        protected override IntVec3 GetWanderRoot(Pawn pawn)
        {
            return ClosestCharacter(pawn) is Pawn otherPawn
                ? WanderUtility.BestCloseWanderRoot(otherPawn.PositionHeld, pawn)
                : pawn.PositionHeld;
        }
    }
}