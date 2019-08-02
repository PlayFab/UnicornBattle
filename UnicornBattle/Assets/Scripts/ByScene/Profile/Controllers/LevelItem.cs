using UnicornBattle.Models;
using UnityEngine;
using UnityEngine.UI;

namespace UnicornBattle.Controllers
{
    public class LevelItem : MonoBehaviour
    {
        public LevelPicker lPicker;
        public Image levelIcon;
        public Image levelMastery;
        public string levelName;
        public int difficulty = 0; // 0 = easy, 1 = medium 2 = hard 
        public UBLevelData levelData;
        public Button myButton;

        // Use this for initialization
        void Start()
        {
            lPicker = transform.GetComponentInParent<LevelPicker>();
            myButton.onClick.AddListener(() => { lPicker.LevelItemClicked(levelName); });
        }

        public LevelItemHelper GetLevelItemData()
        {
            return new LevelItemHelper
            {
                levelIcon = levelIcon.overrideSprite,
                    levelName = levelName,
                    difficulty = difficulty,
                    levelData = levelData
            };
        }
    }

    // temp fix for storing monobehavior refs in a static class, doh!
    public class LevelItemHelper
    {
        public Sprite levelIcon;
        public string levelName;
        public int difficulty = 0; // 0 = easy, 1 = medium 2 = hard 
        public UBLevelData levelData;
    }
}