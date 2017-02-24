using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NextEncounterController : MonoBehaviour
{
    public EncounterPlaceholder encounterN1;
    public EncounterPlaceholder encounterN2;
    public EncounterPlaceholder encounterN3;
    public Image remainingIcon;
    public Text remainingText;

    public void UpdateNextEncounters()
    {
        if (PF_GamePlay.encounters != null)
        {
            List<UB_GamePlayEncounter> nextSet = PF_GamePlay.encounters.Skip(1).Take(3).ToList();

            //
            int frameCount = 3;
            foreach (var item in nextSet)
            {
                if (frameCount == 3)
                {
                    // change banner color
                    if (item.Data.EncounterType.ToString().Contains(GlobalStrings.ENCOUNTER_CREEP))
                    {
                        this.encounterN1.encounterBanner.color = Color.red;
                    }
                    else if (item.Data.EncounterType.ToString().Contains(GlobalStrings.ENCOUNTER_STORE))
                    {
                        this.encounterN1.encounterBanner.color = Color.green;
                    }
                    else if (item.Data.EncounterType.ToString().Contains(GlobalStrings.ENCOUNTER_HERO))
                    {
                        this.encounterN1.encounterBanner.color = Color.cyan;
                    }

                    // change icon
                    this.encounterN1.encounterIcon.overrideSprite = GameController.Instance.iconManager.GetIconById(item.Data.Icon, IconManager.IconTypes.Encounter);

                    // change name
                    this.encounterN1.encounterName.text = item.DisplayName;

                }
                else if (frameCount == 2)
                {
                    // change banner color
                    if (item.Data.EncounterType.ToString().Contains(GlobalStrings.ENCOUNTER_CREEP))
                    {
                        this.encounterN2.encounterBanner.color = Color.red;
                    }
                    else if (item.Data.EncounterType.ToString().Contains(GlobalStrings.ENCOUNTER_STORE))
                    {
                        this.encounterN2.encounterBanner.color = Color.green;
                    }
                    else if (item.Data.EncounterType.ToString().Contains(GlobalStrings.ENCOUNTER_HERO))
                    {
                        this.encounterN2.encounterBanner.color = Color.cyan;
                    }

                    // change icon
                    this.encounterN2.encounterIcon.overrideSprite = GameController.Instance.iconManager.GetIconById(item.Data.Icon, IconManager.IconTypes.Encounter);

                    // change name
                    this.encounterN2.encounterName.text = item.DisplayName;
                }
                else if (frameCount == 1)
                {
                    // change banner color
                    if (item.Data.EncounterType.ToString().Contains(GlobalStrings.ENCOUNTER_CREEP))
                    {
                        this.encounterN3.encounterBanner.color = Color.red;
                    }
                    else if (item.Data.EncounterType.ToString().Contains(GlobalStrings.ENCOUNTER_STORE))
                    {
                        this.encounterN3.encounterBanner.color = Color.green;
                    }
                    else if (item.Data.EncounterType.ToString().Contains(GlobalStrings.ENCOUNTER_HERO))
                    {
                        this.encounterN3.encounterBanner.color = Color.cyan;
                    }

                    // change icon
                    this.encounterN3.encounterIcon.overrideSprite = GameController.Instance.iconManager.GetIconById(item.Data.Icon, IconManager.IconTypes.Encounter);

                    // change name
                    this.encounterN3.encounterName.text = item.DisplayName;
                }
                frameCount--;
            }


            for (int z = frameCount; z > 0; z--)
            {
                if (z == 3)
                {
                    this.encounterN1.encounterIcon.overrideSprite = GameController.Instance.iconManager.GetIconById(GlobalStrings.DARKSTONE_STAR_ICON, IconManager.IconTypes.Misc);
                    this.encounterN1.encounterBanner.color = new Color(188, 0, 255, 255);
                    this.encounterN1.encounterName.text = string.Empty;
                }
                else if (z == 2)
                {
                    this.encounterN2.encounterIcon.overrideSprite = GameController.Instance.iconManager.GetIconById(GlobalStrings.DARKSTONE_STAR_ICON, IconManager.IconTypes.Misc);
                    this.encounterN2.encounterBanner.color = new Color(188, 0, 255, 255);
                    this.encounterN2.encounterName.text = string.Empty;
                }
                else if (z == 1)
                {
                    this.encounterN3.encounterIcon.overrideSprite = GameController.Instance.iconManager.GetIconById(GlobalStrings.DARKSTONE_STAR_ICON, IconManager.IconTypes.Misc);
                    this.encounterN3.encounterBanner.color = new Color(188, 0, 255, 255);
                    this.encounterN3.encounterName.text = string.Empty;
                }
            }

            int encountersRemaining = PF_GamePlay.encounters.Count - 4 > 0 ? PF_GamePlay.encounters.Count - 4 : 0;
            this.remainingText.text = "x" + encountersRemaining;
        }
    }


}
