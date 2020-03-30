using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace HPLovecraft
{
    public static class Settings
    {
        public static bool DEBUG_MODE = true;

        public static void DebugString(string s)
        {
            if (DEBUG_MODE)
                Log.Message(s);
        }
    }
}
