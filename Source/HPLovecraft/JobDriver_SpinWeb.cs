using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
using RimWorld;

namespace HPLovecraft
{
    public class JobDriver_SpinWeb : JobDriver
    {
        private const int LayEgg = 500;

        private const TargetIndex LaySpotInd = TargetIndex.A;

        [DebuggerHidden]
        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);
            yield return new Toil
            {
                defaultCompleteMode = ToilCompleteMode.Delay,
                defaultDuration = 500
            };
            yield return new Toil
            {
                initAction = delegate
                {
                    var spinner = this.GetActor() as PawnWebSpinner;
                    if (spinner != null)
                    {
                        var web = (Building_Web)GenSpawn.Spawn(HPLDefOf.HPLovecraft_Web, spinner.Position, spinner.Map);
                        spinner.Web = web;
                        spinner.WebsMade++;
                        web.Spinner = spinner;
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
        }
    }
}
