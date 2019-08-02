using System;

namespace UnicornBattle.Models
{
    [Serializable]
    public class UBAchievement 
    {
        // Name is the ID
        public string AchievementName;  
        public string MatchingStatistic;
        public bool SingleStat;
        public int Threshold;
        public string Icon;

        public UBAchievement() 
        {
            AchievementName = string.Empty;
            MatchingStatistic = string.Empty;
            SingleStat = true;
            Threshold = 0;
            Icon = string.Empty;
        }

        public UBAchievement( UBAchievement p_copy )
        {
            AchievementName = p_copy.AchievementName;
            MatchingStatistic = p_copy.MatchingStatistic;
            SingleStat = p_copy.SingleStat;
            Threshold = p_copy.Threshold;
            Icon = p_copy.Icon;
        }
    }
}