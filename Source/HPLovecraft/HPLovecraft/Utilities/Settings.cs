using Verse;

namespace HPLovecraft
{
    public static class Settings
    {
#if DEBUG
        public static bool DEBUG_MODE = true;
#else
        public static bool DEBUG_MODE = false;
#endif
        public static void DebugString(string s)
        {
            if (DEBUG_MODE)
            {
                Log.Message(s);
            }
        }
    }
}