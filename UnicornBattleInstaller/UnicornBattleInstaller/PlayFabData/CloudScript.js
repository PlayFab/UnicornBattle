var defaultCatalog = "CharacterClasses";
var GEM_CURRENCY_CODE = "GM";
var GOLD_CURRENCY_CODE = "AU";
var HEART_CURRENCY_CODE = "HT";

// this is a simple change

///////////////////////// Cloud Script Handler Functions /////////////////////////
function CreateCharacter(args) {
    var grantItemsRequest = {
        PlayFabId: currentPlayerId,
        CatalogVersion: defaultCatalog,
        ItemIds: [args.catalogCode]
    };
    server.GrantItemsToUser(grantItemsRequest);
    var grantCharRequest = {
        PlayFabId: currentPlayerId,
        CharacterName: args.characterName,
        CharacterType: args.catalogCode
    };
    var result = server.GrantCharacterToUser(grantCharRequest);
    InitializeNewCharacterData(result.CharacterId, args.catalogCode); // set up default character data
    return true;
}
function DeleteCharacter(args) {
    var deleteRequest = {
        PlayFabId: currentPlayerId,
        CharacterId: args.characterId,
        SaveCharacterInventory: false
    };
    server.DeleteCharacterFromUser(deleteRequest);
    return true;
}
function GetBaseClassForType(args) {
    var getTitleDataRequest = { Keys: ["Classes"] };
    var result = server.GetTitleData(getTitleDataRequest);
    var classes = JSON.parse(result.Data["Classes"]);
    for (var each in classes)
        if (classes[each].CatalogCode === args.cCode)
            return classes[each];
    return null;
}
function SaveProgress(args) {
    args.CurrentPlayerData = !args.CurrentPlayerData ? {} : args.CurrentPlayerData;
    args.QuestProgress = !args.QuestProgress ? {} : args.QuestProgress;
    args.LevelRamp = !args.LevelRamp ? {} : args.LevelRamp;
    //check for level up
    var baseStats = args.CurrentPlayerData.baseClass;
    var characterData = args.CurrentPlayerData.characterData;
    var vitals = args.CurrentPlayerData.PlayerVitals;
    var questProgress = args.QuestProgress;
    var experienceLevel = "" + characterData.CharacterLevel;
    var experienceTarget = args.LevelRamp[experienceLevel]; // int
    if (vitals.didLevelUp) {
        // increment the spell
        if (vitals.skillSelected === 0)
            characterData.Spell1_Level++;
        else if (vitals.skillSelected === 1)
            characterData.Spell2_Level++;
        else
            characterData.Spell3_Level++;
        //Update stats
        characterData.CharacterLevel++;
        characterData.Defense += baseStats.DPLevelBonus;
        characterData.Speed += baseStats.SPLevelBonus;
        characterData.Mana += baseStats.MPLevelBonus;
        characterData.Health += baseStats.HPLevelBonus;
        characterData.TotalExp += questProgress.XpCollected;
        characterData.ExpThisLevel = characterData.TotalExp - experienceTarget;
    }
    else {
        characterData.ExpThisLevel += questProgress.XpCollected;
    }
    //check for achievements & offers
    EvaluateAchievements(args.CurrentPlayerData.characterDetails.CharacterId);
    // API params
    var updateDataRequest = {
        PlayFabId: currentPlayerId,
        CharacterId: args.CurrentPlayerData.characterDetails.CharacterId,
        Data: { CharacterData: JSON.stringify(characterData) },
        Permission: "Public"
    };
    server.UpdateCharacterReadOnlyData(updateDataRequest);
    // set up Gold VC
    var addVcRequest = {
        PlayFabId: currentPlayerId,
        VirtualCurrency: GOLD_CURRENCY_CODE,
        Amount: questProgress.GoldCollected
    };
    server.AddUserVirtualCurrency(addVcRequest);
}
function RetriveQuestItems(args) {
    var grantRequest = {
        PlayFabId: currentPlayerId,
        ItemIds: args.ItemIds
    };
    var response = server.GrantItemsToUser(grantRequest);
    return JSON.stringify(response.ItemGrantResults);
}
function SubtractLife() {
    var subtractVcRequest = {
        PlayFabId: currentPlayerId,
        VirtualCurrency: HEART_CURRENCY_CODE,
        Amount: 1
    };
    return server.SubtractUserVirtualCurrency(subtractVcRequest);
}
function SetEventStatus(args) {
    SetEventActive(args.eventName, args.status)
}

// use this for setting player stats; better than opening up direct client access
// also this gives the opportunity to set bounds checking or other fraud detection
function SetPlayerStats(args) {
    // TBD add some limit checking or inspection
    args.statistics = !args.statistics ? {} : args.statistics;

    var updateStatRequest = {
        PlayFabId: currentPlayerId,
        Statistics: args.statistics
    };
    return server.UpdatePlayerStatistics(updateStatRequest);
}

function SetCharacterStats(args) {
    // TBD add some limit checking or inspection
    args.statistics = !args.statistics ? {} : args.statistics;
    args.characterId = !args.characterId ? {} : args.characterId;

    var updateStatRequest = {
        PlayFabId: currentPlayerId,
        CharacterId: args.characterId,
        CharacterStatistics: args.statistics
    };
    return server.UpdateCharacterStatistics(updateStatRequest);
}

function EnableValentinesEvent() {
    SetEventActive("evalentine", true);
}
function DisableValentinesEvent() {
    SetEventActive("evalentine", false);
}
function EnablePresEvent() {
    SetEventActive("epresident", true);
}
function DisablePresEvent() {
    SetEventActive("epresident", false);
}

//// Functions to be called by rules/actions to drive real time messaging to the client
function SendMessageToPlayer (args, context) {
    var entityEvent = {};
    entityEvent.EventNamespace = "custom.UnicornBattle";
    entityEvent.Name = "MessageToPlayer";
    entityEvent.Payload =
        {
            "title": args.title,
            "message": args.message,
            "showStore": args.showStore,
            "refreshEvents": args.refreshEvents
        };


    if (context.currentEntity == null) {
        //do accountInfo to look up title_player_account        
        entityEvent.Entity = server.GetUserAccountInfo({ PlayFabId: currentPlayerId }).UserInfo.TitleInfo.TitlePlayerAccount
    }
    else {
        entityEvent.Entity = context.currentEntity.Entity;
    }


    var eventResult = entity.WriteEvents({ Events: [entityEvent] });
    log.info("Write Events Result", eventResult);
}

// not working yet; also has title ID fixed
function BroadcastMessageToAllPlayers (args, context) {
    var entityEvent = {};
    entityEvent.EventNamespace = "custom.UnicornBattle";
    entityEvent.Name = "MessageToAllPlayers";

    entityEvent.Entity =
        {
            "type": "title",
            "Id": "A5F3"
        };
    entityEvent.Payload =
    {
        "title": args.title,
        "message": args.message,
        "showStore": args.showStore,
        "refreshEvents": args.refreshEvents
    };

    var eventResult = entity.WriteEvents({ Events: [entityEvent] });
    log.info("Write Events Result", eventResult);

}


///////////////////////// HELPER FUNCTIONS (NOT DIRECTLY CALLABLE FROM THE CLIENT) /////////////////////////
function InitializeNewCharacterData(characterId, catalogItemId) {
    var cDetails = GetBaseClassForType({ cCode: catalogItemId });
    // default character properties
    var CharacterData = {
        ClassDetails: cDetails,
        TotalExp: 0,
        ExpThisLevel: 0,
        Health: cDetails.BaseHP,
        Mana: cDetails.BaseMP,
        Defense: cDetails.BaseDP,
        Speed: cDetails.BaseSP,
        CharacterLevel: 1,
        Spell1_Level: 0,
        Spell2_Level: 0,
        Spell3_Level: 0,
        CustomAvatar: null
    };
    // Char Data
    var updateDataRequest = {
        PlayFabId: currentPlayerId,
        CharacterId: characterId,
        Data: { CharacterData: JSON.stringify(CharacterData) },
        Permission: "Public"
    };
    server.UpdateCharacterReadOnlyData(updateDataRequest);
    // set up Heart VC
    var vcHeartRequest = {
        PlayFabId: currentPlayerId,
        VirtualCurrency: HEART_CURRENCY_CODE,
        Amount: 0
    };
    server.AddUserVirtualCurrency(vcHeartRequest);
    // set up Gold VC
    var vcGoldRequest = {
        PlayFabId: currentPlayerId,
        VirtualCurrency: GOLD_CURRENCY_CODE,
        Amount: 0
    };
    server.AddUserVirtualCurrency(vcGoldRequest);
    // set up Gem VC
    var vcGemRequest = {
        PlayFabId: currentPlayerId,
        VirtualCurrency: GEM_CURRENCY_CODE,
        Amount: 0
    };
    server.AddUserVirtualCurrency(vcGemRequest);
}
function HasAchievement(achievement, playerStats) {
    if (achievement.SingleStat) {
        for (var stat in playerStats)
            if (playerStats.hasOwnProperty(stat) && stat.indexOf(achievement.MatchingStatistic) > -1
                && playerStats[stat] >= achievement.Threshold)
                // Stat found and exceeds the achievement threshold
                return true;
    }
    else {
        // process aggregate stats
        var statTotal = 0;
        for (var stat in playerStats)
            if (playerStats.hasOwnProperty(stat) && stat.indexOf(achievement.MatchingStatistic) > -1)
                statTotal += playerStats[stat];
        if (statTotal >= achievement.Threshold)
            return true; // sum of stats found exceeds the achievement threshold
    }
    return false;
}
function EvaluateAchievements(characterId) {
    // get the achievement thresholds set by TitleData
    var getTitleAchievementsRequest = { Keys: ["Achievements"] };
    var titleDataResult = server.GetTitleData(getTitleAchievementsRequest);
    var serverAchievements;
    if (titleDataResult.Data.hasOwnProperty("Achievements"))
        serverAchievements = JSON.parse(titleDataResult.Data["Achievements"]);
    else
        throw "Achievements not found on Server. Check TitleData[\"Achievements\"]";
    // get the character stats
    var statsRequest = {
        PlayFabId: currentPlayerId,
        CharacterId: characterId,
    };
    var statsResult = server.GetCharacterStatistics(statsRequest);
    var charStats = statsResult.CharacterStatistics;
    // get the unlocked stats for the character
    var getCharDataRequest = {
        PlayFabId: currentPlayerId,
        CharacterId: characterId,
        Keys: ["Achievements"]
    };
    var charDataResult = server.GetCharacterReadOnlyData(getCharDataRequest);
    var awardedAchievements;
    if (charDataResult.Data.hasOwnProperty("Achievements"))
        awardedAchievements = JSON.parse(charDataResult.Data["Achievements"].Value);
    else
        awardedAchievements = []; // no achievements 
    // TODO -- need to track new achievements and toss them over to the offer evaluator
    var arrayCount = awardedAchievements.length;
    for (var achievementName in serverAchievements)
        // if the player does not already have the achievement, evaluate progress
        if (awardedAchievements.indexOf(achievementName) === -1 && HasAchievement(serverAchievements[achievementName], charStats))
            awardedAchievements.push(achievementName);
    // if we added some new achievements, save details to character data
    if (arrayCount < awardedAchievements.length) {
        var updateCharDataRequest = {
            PlayFabId: currentPlayerId,
            CharacterId: characterId,
            Data: { Achievements: JSON.stringify(awardedAchievements) },
            Permission: "Public"
        };
        server.UpdateCharacterReadOnlyData(updateCharDataRequest);
    }
}
var EVENT_TITLE_DATA_KEY = "ActiveEventKeys";
function SetEventActive(eventKey, isActive) {
    var getRequest = { Keys: [EVENT_TITLE_DATA_KEY] };
    var serverData = server.GetTitleData(getRequest);
    var eventKeys = JSON.parse(serverData.Data[EVENT_TITLE_DATA_KEY]);
    if (isActive)
        eventKeys.push(eventKey);
    else {
        var temp = [];
        for (var idx in eventKeys)
            if (eventKeys[idx] != eventKey)
                temp.push(eventKeys[idx]);
        eventKeys = temp;
    }
    var setRequest = {
        Key: EVENT_TITLE_DATA_KEY,
        Value: JSON.stringify(eventKeys)
    };
    server.SetTitleData(setRequest);
}

///////////////////////// Define the handlers /////////////////////////
handlers.GetBaseClassForType = GetBaseClassForType;
handlers.CreateCharacter = CreateCharacter;
handlers.DeleteCharacter = DeleteCharacter;
handlers.SaveProgress = SaveProgress;
handlers.RetriveQuestItems = RetriveQuestItems;
handlers.SubtractLife = SubtractLife;
handlers.SetEventStatus = SetEventStatus;
handlers.SendMessageToPlayer = SendMessageToPlayer;
handlers.SetPlayerStats = SetPlayerStats;
handlers.SetCharacterStats = SetCharacterStats;

//# sourceMappingURL=UnicornBattle.js.map

