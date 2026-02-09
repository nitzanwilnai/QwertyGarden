using UnityEngine;
using Steamworks;

namespace QwertyGarden
{
    public static class SteamAchievements
    {
        public static void UnlockAchievement(string apiName)
        {
            Debug.Log("UnlockAchievement(" + apiName + ")");

            if (!SteamManager.Initialized)
            {
                Debug.LogWarning("Steam not initialized. Cannot unlock achievement.");
                return;
            }

            // 1. Check if it's already unlocked to avoid redundant calls
            bool isUnlocked;
            if (SteamUserStats.GetAchievement(apiName, out isUnlocked) && !isUnlocked)
            {
                // 2. Set the achievement to 'true'
                bool success = SteamUserStats.SetAchievement(apiName);

                if (success)
                {
                    // 3. IMPORTANT: Store stats to push the change to Steam immediately
                    // Without this, the 'toast' notification might not pop up.
                    SteamUserStats.StoreStats();
                    Debug.Log($"Achievement '{apiName}' unlocked successfully!");
                }
            }
        }
    }
}