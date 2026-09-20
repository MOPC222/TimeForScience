using System.Collections.Generic;

namespace TimeForScience
{
    /// <summary>Per-experimentID exclusions from timing and/or EC, loaded
    /// lazily from Config/Exclusions.cfg so there's no dependency on
    /// GameDatabase load order.</summary>
    internal static class ScienceExclusions
    {
        private static readonly HashSet<string> timerExclusions = new HashSet<string>();
        private static readonly HashSet<string> ecExclusions = new HashSet<string>();
        private static readonly HashSet<string> bankingExclusions = new HashSet<string>();
        private static bool loaded;

        internal static bool IsExcludedFromTimer(string experimentId)
        {
            EnsureLoaded();
            return timerExclusions.Contains(experimentId);
        }

        internal static bool IsExcludedFromEC(string experimentId)
        {
            EnsureLoaded();
            return ecExclusions.Contains(experimentId);
        }

        /// <summary>Only meaningful for a timed experiment: timer-excluded ones
        /// never register a run, so they never bank in the first place.</summary>
        internal static bool IsExcludedFromBanking(string experimentId)
        {
            EnsureLoaded();
            return bankingExclusions.Contains(experimentId);
        }

        private static void EnsureLoaded()
        {
            if (loaded)
            {
                return;
            }
            loaded = true;

            foreach (ConfigNode node in GameDatabase.Instance.GetConfigNodes("TIMEFORSCIENCE_EXCLUSION"))
            {
                string id = node.GetValue("experimentID");
                if (string.IsNullOrEmpty(id))
                {
                    continue;
                }

                bool excludeTimer = false;
                bool excludeEc = false;
                bool excludeBanking = false;
                node.TryGetValue("excludeFromTimer", ref excludeTimer);
                node.TryGetValue("excludeFromEC", ref excludeEc);
                node.TryGetValue("excludeFromBanking", ref excludeBanking);

                if (excludeTimer)
                {
                    timerExclusions.Add(id);
                }
                if (excludeEc)
                {
                    ecExclusions.Add(id);
                }
                if (excludeBanking)
                {
                    bankingExclusions.Add(id);
                }
            }
        }
    }
}
