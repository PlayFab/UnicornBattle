using System;
using System.Collections.Generic;

namespace UnicornBattle.Models
{
    [Serializable]
    public class UBSavedCharacter
    {
        /// <summary>
        /// The id for this character on this player.
        /// </summary>
        public string CharacterId { get; set; }
        /// <summary>
        /// The name of this character.
        /// </summary>
        public string CharacterName { get; set; }
        /// <summary>
        /// The type-string that was given to this character on creation.
        /// </summary>
        public string CharacterType { get; set; }
        ///  <summary>
        /// 
        /// </summary>
        public UBCharacterClassDetail baseClass;
        ///  <summary>
        /// 
        /// </summary>
        public UBCharacterData characterData;
        ///  <summary>
        /// 
        /// </summary>
        public UBPlayerVitals PlayerVitals;

        public void SetMaxVitals()
        {
            PlayerVitals.MaxHealth = characterData.Health;
            PlayerVitals.MaxMana = characterData.Mana;
            PlayerVitals.MaxSpeed = characterData.Speed;
            PlayerVitals.MaxDefense = characterData.Defense;

            PlayerVitals.Health = characterData.Health;
            PlayerVitals.Mana = characterData.Mana;
            PlayerVitals.Speed = characterData.Speed;
            PlayerVitals.Defense = characterData.Defense;

            PlayerVitals.ActiveStati.Clear();
            PlayerVitals.didLevelUp = false;
            PlayerVitals.skillSelected = 0;
        }

        public void RefillVitals()
        {
            PlayerVitals.ActiveStati.Clear();
            PlayerVitals.didLevelUp = false;
            PlayerVitals.skillSelected = 0;

            PlayerVitals.Health = PlayerVitals.MaxHealth;
            PlayerVitals.Mana = PlayerVitals.MaxMana;
            PlayerVitals.Speed = PlayerVitals.MaxSpeed;
            PlayerVitals.Defense = PlayerVitals.MaxDefense;
        }

        public void LevelUpCharacterStats()
        {
            //TODO add in this -- needs to have a level up table from title data
        }

        public void UpdateCharacterData( UBCharacterData p_data)
        {
            if(null != p_data) {
                characterData = new UBCharacterData(p_data);
            }
        }

        public UBSavedCharacter(PlayFab.ClientModels.CharacterResult p_result, UBCharacterClassDetail p_details, UBCharacterData p_data)
        {
            CharacterId = p_result.CharacterId;
            CharacterName = p_result.CharacterName;
            CharacterType = p_result.CharacterType;

            baseClass =  new UBCharacterClassDetail(p_details);
            characterData = new UBCharacterData(p_data);
            PlayerVitals = new UBPlayerVitals { ActiveStati = new List<UBSpellStatus>() };
        }

        public UBSavedCharacter(UBSavedCharacter p_copy)
        {
            CharacterId = p_copy.CharacterId;
            CharacterName = p_copy.CharacterName;
            CharacterType = p_copy.CharacterType;

            baseClass =  new UBCharacterClassDetail(p_copy.baseClass);
            characterData = new UBCharacterData(p_copy.characterData);
            PlayerVitals = new UBPlayerVitals(p_copy.PlayerVitals);
        }
    }
}
