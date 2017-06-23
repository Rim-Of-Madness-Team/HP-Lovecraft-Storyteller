using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using UnityEngine;

namespace HPLovecraft
{
    public class Building_Web : Building
    {
        private PawnWebSpinner spinner;
        public PawnWebSpinner Spinner { get { return spinner; } set { spinner = value; } }

        public void WebEffect(Pawn p)
        {
            float num = 20f;
            float num2 = Mathf.Lerp(0.85f, 1.15f, p.thingIDNumber ^ 74374237);
            num *= num2;
            p.TakeDamage(new DamageInfo(DamageDefOf.Stun, (int)num, -1, this));
            spinner.Notify_WebTouched(p);
            Messages.Message("ROM_SpiderWebsCrossed".Translate(p.LabelShort), MessageSound.Standard);
            spinner.Web = null;
            this.Destroy(DestroyMode.Vanish);
        }

        public override void Tick()
        {
            base.Tick();
            List<Thing> thingList = this.Position.GetThingList(base.Map);
            for (int i = 0; i < thingList.Count; i++)
            {
                if (thingList[i] is Pawn p && !(thingList[i] is PawnWebSpinner))
                {
                    WebEffect(p);
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look<PawnWebSpinner>(ref this.spinner, "spinner");
        }
    }
}
