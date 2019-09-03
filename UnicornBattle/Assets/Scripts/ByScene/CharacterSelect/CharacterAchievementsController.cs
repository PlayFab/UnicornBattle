using System;
using System.Collections.Generic;
using UnicornBattle.Managers;
using UnicornBattle.Models;
using UnityEngine;
using UnityEngine.UI;

namespace UnicornBattle.Controllers
{
    public class CharacterAchievementsController : MonoBehaviour
    {
        public List<Image> colorize = new List<Image>();
        public AchievementItem achievementItemPrefab;
        public CharacterSelectionController cPicker;
        public List<AchievementItem> achievementList;
        public Transform listView;

        public void Init()
        {
            StopAllCoroutines();

            MainManager l_mManager = MainManager.Instance;
            if (null == l_mManager) return;

            GameDataManager l_gameDataManager = l_mManager.getGameDataManager();
            if (null == l_gameDataManager) return;

            l_gameDataManager.Refresh(false,
                (string s) =>
                {
                    CharacterManager l_characterMgr = l_mManager.getCharacterManager();
                    if (null == l_characterMgr) return;

                    l_characterMgr.Refresh(false,
                        (string s2) =>
                        {
                            UBAchievement[] l_achievements = l_gameDataManager.GetAllAchievements();
                            if (null == l_achievements || l_achievements.Length == 0) return;

                            if (cPicker.selectedSlot.saved != null)
                            {
                                while (achievementList.Count < l_achievements.Length)
                                {
                                    var l_achieveItem = Instantiate(achievementItemPrefab);
                                    l_achieveItem.transform.SetParent(listView, false);
                                    achievementList.Add(l_achieveItem);
                                }

                                for (var i = 0; i < achievementList.Count; i++)
                                {
                                    UBAchievement l_achievement = l_achievements[i];
                                    if (null == l_achievement) continue;

                                    // check here for already awarded
                                    achievementList[i].progressBar.currentValue = 0;
                                    colorize.Add(achievementList[i].coloredBar);

                                    achievementList[i].icon.overrideSprite = GameController.Instance.iconManager.GetIconById(l_achievement.Icon, IconManager.IconTypes.Misc);
                                    achievementList[i].Name.text = l_achievement.AchievementName;
                                    achievementList[i].progressBar.maxValue = l_achievement.Threshold;

                                    if (l_characterMgr.DoesCharacterHaveAchievement(cPicker.selectedSlot.saved.CharacterId, l_achievement.AchievementName))
                                    {
                                        achievementList[i].CompleteAchievement();
                                        StartCoroutine(achievementList[i].progressBar.UpdateBar(l_achievement.Threshold));
                                        achievementList[i].display.text = GlobalStrings.COMPLETE_MSG;
                                    }
                                    else
                                    {
                                        achievementList[i].ResetImage();

                                        int progress = CalcProgress(l_achievement);
                                        achievementList[i].display.text = string.Format("{0:n0}/{1:n0} {2}",
                                            AchievementDisplayNumber(progress),
                                            AchievementDisplayNumber(l_achievement.Threshold),
                                            l_achievement.MatchingStatistic);

                                        StartCoroutine(achievementList[i].progressBar.UpdateBar(progress));
                                    }
                                }
                            }

                            var classType = (CharacterClassTypes) Enum.Parse(typeof(CharacterClassTypes), cPicker.selectedSlot.saved.baseClass.CatalogCode);

                            switch ((int) classType)
                            {
                                case 0:
                                    foreach (var item in colorize)
                                        item.color = UBGamePlay.ClassColor1;
                                    break;
                                case 1:
                                    foreach (var item in colorize)
                                        item.color = UBGamePlay.ClassColor2;
                                    break;
                                case 2:
                                    foreach (var item in colorize)
                                        item.color = UBGamePlay.ClassColor3;
                                    break;
                                default:
                                    Debug.LogWarning("Unknown Class type detected...");
                                    break;
                            }
                        },
                        (string e) =>
                        {
                            Debug.LogError(this.GetType() + ":" + e);
                        }
                    );
                }
            );
        }

        private string AchievementDisplayNumber(int input)
        {
            if (input < 900)
                return input.ToString();
            else if (input < 1000)
                return "<1K";
            else if (input < 900000)
                return (input / 1000).ToString() + "K";
            else if (input < 1000000)
                return "<1M";
            else
                return (input / 1000000).ToString() + "M";
        }

        public int CalcProgress(UBAchievement p_achieveItem)
        {
            CharacterManager l_characterMgr = MainManager.Instance.getCharacterManager();
            if (null == l_characterMgr) return 0;

            PlayerManager l_playerManager = MainManager.Instance.getPlayerManager();
            if (null == l_playerManager) return 0;

            var l_charId = cPicker.selectedSlot.saved.CharacterId;
            Dictionary<string, int> stats = l_characterMgr.GetCharacterStatistics(l_charId);

            if (stats == null)
                return 0;

            if (p_achieveItem.SingleStat)
                return stats.ContainsKey(p_achieveItem.MatchingStatistic) ? stats[p_achieveItem.MatchingStatistic] : 0;

            var total = 0;
            foreach (var stat in stats)
                if (stat.Key.Contains(p_achieveItem.MatchingStatistic))
                    total += stat.Value;
            return total;
        }
    }
}