using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using Verse;
using RimWorld;

namespace HPLovecraft
{
    [StaticConstructorOnStartup]
    static class LovecraftHarmony
    {
        static LovecraftHarmony()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.jecrell.lovecraftstoryteller");
            harmony.Patch(AccessTools.Method(typeof(Dialog_DebugActionsMenu), "DoExecuteIncidentDebugAction"),
                new HarmonyMethod(typeof(LovecraftHarmony), "DoExecuteIncidentDebugAction_PreFix"), null);
        }

        public static bool DoExecuteIncidentDebugAction_PreFix(Dialog_DebugActionsMenu __instance, IIncidentTarget target, IIncidentTarget altTarget)
        {
            //Log.Message("1");
            if (Find.Storyteller?.storytellerComps?.FirstOrDefault(x => x is StorytellerComp_OmenThreatCycle) != null)
            {
                //Log.Message("2");
                AccessTools.Method(typeof(Dialog_DebugActionsMenu), "DebugAction").Invoke(__instance, new object[]
                {
                "Execute incident...", new Action(delegate
                {
                    //Log.Message("3");
                    List<DebugMenuOption> list = new List<DebugMenuOption>();
                    foreach (IncidentDef current in from d in DefDatabase<IncidentDef>.AllDefs
                                                    where d.TargetAllowed(target) || (altTarget != null && d.TargetAllowed(altTarget))
                                                    orderby !d.TargetAllowed(target), d.defName
                                                    select d)
                    {
                        IIncidentTarget arg_98_1;
                        if (current.TargetAllowed(target))
                        {
                            IIncidentTarget target2 = target;
                            arg_98_1 = target2;
                        }
                        else
                        {
                            arg_98_1 = altTarget;
                        }
                        var thisIncidentTarget = arg_98_1;

                        string text = current.defName;
                        if (!current.Worker.CanFireNow(thisIncidentTarget))
                        {
                            text += " [NO]";
                        }
                        if (thisIncidentTarget == altTarget)
                        {
                            text = text + " (" + altTarget.GetType().Name.Truncate(52f, null) + ")";
                        }
                        list.Add(new DebugMenuOption(text, DebugMenuOptionMode.Action, delegate
                        {
                            IncidentParms incidentParms = StorytellerUtility.DefaultParmsNow(Find.Storyteller.def, current.category, thisIncidentTarget);
                            if (current.pointsScaleable)
                            {
                                StorytellerComp storytellerComp = Find.Storyteller.storytellerComps.First((StorytellerComp x) => x is StorytellerComp_ThreatCycle || x is StorytellerComp_RandomMain || x is StorytellerComp_OmenThreatCycle);
                                incidentParms = storytellerComp.GenerateParms(current.category, incidentParms.target);
                            }
                        current.Worker.TryExecute(incidentParms);
                        }));
                    };
                    Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
                })
                });
                return false;
            }
            return true;
        }
    }
   
}
