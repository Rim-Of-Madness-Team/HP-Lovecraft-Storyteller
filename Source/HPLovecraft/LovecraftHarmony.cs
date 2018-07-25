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
            harmony.Patch(AccessTools.Method(typeof(Dialog_DebugActionsMenu), "DoIncidentDebugAction"),
                new HarmonyMethod(typeof(LovecraftHarmony), nameof(DoIncidentDebugAction_Prefix)), null);
        }

        public static bool DoIncidentDebugAction_Prefix(Dialog_DebugActionsMenu __instance, IIncidentTarget target)
        {
            //Log.Message("1");
            if (Find.Storyteller?.storytellerComps?.FirstOrDefault(x => x is StorytellerComp_OmenThreatCycle) != null)
            {
                //Log.Message("2");
                AccessTools.Method(typeof(Dialog_DebugActionsMenu), "DebugAction").Invoke(__instance, new object[]
                {
                    "Execute incident...", new Action(delegate
                    {
                        List<DebugMenuOption> list = new List<DebugMenuOption>();
                        foreach (IncidentDef localDef2 in from d in DefDatabase<IncidentDef>.AllDefs
                            where d.TargetAllowed(target)
                            orderby d.defName
                            select d)
                        {
                            IncidentDef localDef = localDef2;
                            string text = localDef.defName;
                            IncidentParms parms = StorytellerUtility.DefaultParmsNow(localDef.category, target);
                            if (!localDef.Worker.CanFireNow(parms))
                            {
                                text += " [NO]";
                            }
                            list.Add(new DebugMenuOption(text, DebugMenuOptionMode.Action, delegate
                            {
                                if (localDef.pointsScaleable)
                                {
                                    StorytellerComp storytellerComp = Find.Storyteller.storytellerComps.First(
                                        (StorytellerComp x) =>
                                            x is StorytellerComp_OnOffCycle || x is StorytellerComp_RandomMain ||
                                            x is StorytellerComp_OmenThreatCycle);
                                    parms = storytellerComp.GenerateParms(localDef.category, parms.target);
                                }
                                localDef.Worker.TryExecute(parms);
                            }));
                        }
                        Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));

                    })
                });
                return false;
            }
            return true;
        }
        
//                //Log.Message("2");
//                AccessTools.Method(typeof(Dialog_DebugActionsMenu), "DebugAction").Invoke(__instance, new object[]
//                {
//                "Execute incident...", new Action(delegate
//                {
//                    //Log.Message("3");
//                    List<DebugMenuOption> list = new List<DebugMenuOption>();
//                    foreach (IncidentDef current in from d in DefDatabase<IncidentDef>.AllDefs
//                                                    where d.TargetAllowed(target) || (altTarget != null && d.TargetAllowed(altTarget))
//                                                    orderby !d.TargetAllowed(target), d.defName
//                                                    select d)
//                    {
//                        IIncidentTarget arg_98_1;
//                        if (current.TargetAllowed(target))
//                        {
//                            IIncidentTarget target2 = target;
//                            arg_98_1 = target2;
//                        }
//                        else
//                        {
//                            arg_98_1 = altTarget;
//                        }
//                        var thisIncidentTarget = arg_98_1;
//
//                        string text = current.defName;
//                        if (!current.Worker.CanFireNow(target))
//                        {
//                            text += " [NO]";
//                        }
//                        if (thisIncidentTarget == altTarget)
//                        {
//                            text = text + " (" + altTarget.GetType().Name.Truncate(52f, null) + ")";
//                        }
//                        list.Add(new DebugMenuOption(text, DebugMenuOptionMode.Action, delegate
//                        {
//                            IncidentParms incidentParms = StorytellerUtility.DefaultParmsNow(current.category, thisIncidentTarget);
//                            if (current.pointsScaleable)
//                            {
//                                StorytellerComp storytellerComp = Find.Storyteller.storytellerComps.First((StorytellerComp x) => x is StorytellerComp_ThreatCycle || x is StorytellerComp_RandomMain || x is StorytellerComp_OmenThreatCycle);
//                                incidentParms = storytellerComp.GenerateParms(current.category, incidentParms.target);
//                            }
//                        current.Worker.TryExecute(incidentParms);
//                        }));
//                    };
//                    Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
//                })
//                });
    }
}