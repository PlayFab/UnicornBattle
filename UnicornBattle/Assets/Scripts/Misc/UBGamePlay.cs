using UnityEngine;

public static class UBGamePlay
{
    public static bool UseRaidMode = true; // TODO default to false - or make it a setting in the player's data
    public static bool isHardMode = false;

    // class colors used to colorize UI based on active character.
    public static Color ClassColor1;
    public static Color ClassColor2;
    public static Color ClassColor3;

    public enum ShakeEffects { None = 0, DecreaseHealth, DecreaseMana, IncreaseHealth, IncreaseMana }
    public enum PlayerSpellInputs { Null = 0, Spell1, Spell2, Spell3, Flee, UseItem }
    public enum PlayerEncounterInputs { Null = 0, Attack, UseItem, Evade, ViewStore, Rescue }
    public enum GameplayEventTypes { Null, StartQuest, EndQuest, IntroQuest, OutroQuest, IntroAct, OutroAct, IntroEncounter, OutroEncounter, PlayerTurnBegins, PlayerTurnEnds, EnemyTurnBegins, EnemyTurnEnds, StartBossBattle, PlayerDied, PlayerRespawn } // MAY need more types here for things like store, heros and bosses, perhaps even one for spelleffects
    public enum TurnStates { Null, Player, Enemy }
}