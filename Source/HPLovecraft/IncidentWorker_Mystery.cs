using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HPLovecraft
{
    public class IncidentWorker_Mystery : IncidentWorker
    {
        public override bool TryExecute(IncidentParms parms)
        {
            Cthulhu.Utility.DebugReport("Started Blood Moon");
            HPLDefOf.HPLovecraft_BloodMoon.Worker.TryExecute(parms);
            return true;
        }
    }
}
