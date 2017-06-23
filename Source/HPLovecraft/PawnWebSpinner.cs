using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using UnityEngine;

namespace HPLovecraft
{
    public class PawnWebSpinner : Pawn
    {
        private int webPeriod = -1;
        public int WebPeriod
        {
            get
            {
                if (webPeriod == -1)
                {
                    webPeriod = (int)Mathf.Lerp(3451f, 4000f, Rand.ValueSeeded(this.thingIDNumber ^ 74374237));
                }
                return webPeriod;
            }
        }

        private Thing web;
        public Thing Web { get { return web; } set { web = value; } }
        private int websMade = 0;
        public int WebsMade { get; set; }
        public bool IsMakingWeb => this?.CurJob?.def == HPLDefOf.HPLovecraft_SpinWeb;

        public void MakeWeb()
        {
            if (web == null && !this.Dead && !IsMakingWeb)
            {
                var cell = RCellFinder.RandomWanderDestFor(this, this.Position, 5f, null, Danger.Some);
                this.jobs.TryTakeOrderedJob(new Job(HPLDefOf.HPLovecraft_SpinWeb, cell));
            }
        }

        public void Notify_WebTouched(Pawn toucher)
        {
            if (web != null && !this.Dead)
            {
                if ((toucher?.RaceProps?.Animal ?? false) || Rand.Value > 0.4)
                {
                    this.jobs.TryTakeOrderedJob(new Job(JobDefOf.AttackMelee, toucher), Verse.AI.JobTag.Misc);
                }
            }
        }

        #region Overrides
        public override void Tick()
        {
            base.Tick();
            if (Find.TickManager.TicksGame % WebPeriod == 0)
            {

                MakeWeb();
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look<Thing>(ref this.web, "web");
            Scribe_Values.Look<int>(ref this.websMade, "websMade", 0);
        }
        #endregion Overrides
    }
}
