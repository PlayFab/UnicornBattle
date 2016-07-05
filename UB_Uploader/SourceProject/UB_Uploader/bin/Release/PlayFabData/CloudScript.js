var defaultCatalog = "CharacterClasses";

handlers.CreateCharacter = function(args)
{
	var params = {};
	params.PlayFabId = currentPlayerId;
	params.CatalogVersion = defaultCatalog;
	params.ItemIds = [args.catalogCode];
	
	// this needs to change when you have limited & locked characters
	// this should perform a prereqs check method
	server.GrantItemsToUser(params);

	params = {};
	params.PlayFabId = currentPlayerId;
	params.CharacterName = args.characterName;
	params.CharacterType = args.catalogCode;

	
	// will contain the newly created character ID
	var result = {};
	result = server.GrantCharacterToUser(params);

	// set up default character data here.
	InitializeNewCharacterData(result.CharacterId, args.catalogCode);

	// maybe return the character data or maybe just the id
	return true;
}

handlers.DeleteCharacter = function(args)
{
	var params = {};
	params.PlayFabId = currentPlayerId;
	params.CharacterId = args.characterId;
	params.SaveCharacterInventory = false;
	
	server.DeleteCharacterFromUser(params);
	return true;
}

handlers.GetBaseClassForType = function(args)
{
	var params = {};
	params.Keys = ["Classes"];

	var result = server.GetTitleData(params);
	var classes = JSON.parse(result.Data.Classes);

	for(var each in classes)
	{
		if(classes[each].CatalogCode == args.cCode)
		{
			return classes[each];
		}
	}

	return null;	
}

handlers.SaveProgress = function(args)
{
	args.CurrentPlayerData =  args.CurrentPlayerData == undefined || args.CurrentPlayerData == null ? "" : args.CurrentPlayerData;
	args.QuestProgress = args.QuestProgress == undefined || args.QuestProgress == null ? "" : args.QuestProgress;
	args.LevelRamp = args.LevelRamp == undefined || args.LevelRamp == null ? "" : args.LevelRamp;
	//log.info(retObj);

	// could have a complex validation here to make sure the data the client is sending is within reasonable ranges
	// for now we will trust our client data.

	//check for level up
	var baseStats = args.CurrentPlayerData.baseClass;
	var characterData = args.CurrentPlayerData.characterData;
	var vitals = args.CurrentPlayerData.PlayerVitals;
	var QuestProgress = args.QuestProgress;


	var experienceLevel = "" + characterData.CharacterLevel;
	var experienceTarget = args.LevelRamp[experienceLevel]; // int

	if(vitals.didLevelUp == true)
	{

		// increment the spell
		if(vitals.skillSelected == 0)
		{
			characterData.Spell1_Level++;
		}
		else if(vitals.skillSelected == 1)
		{
			characterData.Spell2_Level++;
		}
		else
		{
			characterData.Spell3_Level++;
		}
		
		//Update stats
		characterData.CharacterLevel++;
		characterData.Defense += baseStats.DPLevelBonus; 
		characterData.Speed += baseStats.SPLevelBonus; 
		characterData.Mana += baseStats.MPLevelBonus; 
		characterData.Health+= baseStats.HPLevelBonus; 
		characterData.TotalExp += QuestProgress.XpCollected;

		characterData.ExpThisLevel = characterData.TotalExp - experienceTarget;

	}
	else
	{
		characterData.ExpThisLevel += QuestProgress.XpCollected;
	}

	//check for achievements & offers
	var evalParams = {};
	evalParams.CharacterId = args.CurrentPlayerData.characterDetails.CharacterId;

	EvaluateAchievements(evalParams);


	// API params
	var params = {};
	params.PlayFabId = currentPlayerId;
	params.CharacterId = args.CurrentPlayerData.characterDetails.CharacterId;
	params.Data = {};
	params.Data.CharacterData = JSON.stringify(characterData);
	params.Permission = "Public";

	server.UpdateCharacterReadOnlyData(params);


	// set up Gold VC
	var balanceGoldVC = {};
	balanceGoldVC.PlayFabId = currentPlayerId;
	balanceGoldVC.CharacterId = args.CurrentPlayerData.characterDetails.CharacterId;
	balanceGoldVC.VirtualCurrency = "G";
	balanceGoldVC.Amount = QuestProgress.GoldCollected;

	server.AddCharacterVirtualCurrency(balanceGoldVC);
}

handlers.RetriveQuestItems = function(args)
{
	var params = {};
	params.CharacterId = args.CharacterId;
	params.PlayFabId = currentPlayerId;
	params.ItemIds = args.ItemIds;

	var response = server.GrantItemsToCharacter(params);
	return JSON.stringify(response.ItemGrantResults);
}

handlers.SubtractLife = function(args)
{
	// CharacterId

	var params = {};
	params.CharacterId = args.CharacterId;
	params.PlayFabId = currentPlayerId;
	params.VirtualCurrency = "HT";
	params.Amount = 1;

	return server.SubtractCharacterVirtualCurrency(params);
}

handlers.GetCharacterStatistics = function(args)
{
	//CharacterId[]
	var retValue = {};
	
	var params = {};
	params.PlayFabId = currentPlayerId;
	for(var index in args.CharacterId)
	{
		params.CharacterId = args.CharacterId[index];
		var result = server.GetCharacterStatistics(params);
		retValue[args.CharacterId[index]] = result.CharacterStatistics;
	}

	return retValue;
}

handlers.UpdateCharacterStats = function(args)
{
	// CharacterStatistics (Dict)
	// CharacterId

	var params = {};
	params.PlayFabId = currentPlayerId;
	params.CharacterId = args.CharacterId;
	params.CharacterStatistics = args.CharacterStatistics;

	var result = server.UpdateCharacterStatistics(params);

	var psReq = {};
	psReq.EventName = "CharacterStatisticsUpdated";
	psReq.PlayFabId = currentPlayerId;
	psReq.Body = { apiResult: result };
	server.WritePlayerEvent(psReq);

	return {};
}

handlers.TransferItemToPlayer = function(args)
{
	if(args.sourceId == undefined || args.instanceId == undefined)
		throw "Invalid parameters.";

	var moveItemRequest = {};
	moveItemRequest.PlayFabId = currentPlayerId;
	moveItemRequest.CharacterId = args.sourceId;
	moveItemRequest.ItemInstanceId = args.instanceId;

	server.MoveItemToUserFromCharacter(moveItemRequest);
	
	return;
}

handlers.TransferItemToCharacter = function(args)
{
	if(	args.sourceId == undefined || args.sourceType == undefined || args.instanceId == undefined || args.destId == undefined)
	{
		throw "Invalid parameters.";
	}

	if(args.sourceType == "Player")
	{
		var moveItemRequest = {};
		moveItemRequest.PlayFabId = currentPlayerId;
		moveItemRequest.CharacterId = args.destId;
		moveItemRequest.ItemInstanceId = args.instanceId;

		server.MoveItemToCharacterFromUser(moveItemRequest);
	}
	else
	{
		var moveItemRequest = {};
		moveItemRequest.PlayFabId = currentPlayerId;
		moveItemRequest.GivingCharacterId = args.sourceId;
		moveItemRequest.ReceivingCharacterId = args.destId;
		moveItemRequest.ItemInstanceId = args.instanceId;

		server.MoveItemToCharacterFromCharacter(moveItemRequest);
	}

	return;
}

handlers.TransferVcToPlayer = function(args)
{
	if(	args.sourceId == undefined || args.cCode == undefined || args.amount == undefined)
	{
		throw "Invalid parameters.";
	}

	var inventoryParams = {};
	inventoryParams.CharacterId = args.sourceId;
	inventoryParams.PlayFabId = currentPlayerId;

	var inventoryResult = server.GetCharacterInventory(inventoryParams);

	// ensure that the account has enough VC to transfer
	if(!inventoryResult.VirtualCurrency.hasOwnProperty(args.cCode))
	{
		throw "User has no currency of type " + args.cCode;
	}
	else if(inventoryResult.VirtualCurrency.hasOwnProperty(args.cCode) && args.amount > inventoryResult.VirtualCurrency[args.cCode])
	{
		throw "User does not have enough currency to complete this transfer.";
	}

	var subtractVcParms = {};
	subtractVcParms.CharacterId = args.sourceId;
	subtractVcParms.PlayFabId = currentPlayerId;
	subtractVcParms.VirtualCurrency = args.cCode;
	subtractVcParms.Amount = args.amount;
	
	server.SubtractCharacterVirtualCurrency(subtractVcParms);

	var addVcParams = {};
	addVcParams.PlayFabId = currentPlayerId;
	addVcParams.VirtualCurrency = args.cCode;
	addVcParams.Amount = args.amount;

	server.AddUserVirtualCurrency(addVcParams);

	return;
}

handlers.TransferVCToCharacter = function(args)
{
	if(	args.sourceId == undefined || args.sourceType == undefined || args.cCode == undefined || args.amount == undefined || args.destId == undefined)
	{
		throw "Invalid parameters.";
	}

	if(args.sourceType == "Player")
	{
		var inventoryParams = {};
		inventoryParams.PlayFabId = currentPlayerId;

		var inventoryResult = server.GetUserInventory(inventoryParams);

		// ensure that the account has enough VC to transfer
		if(!inventoryResult.VirtualCurrency.hasOwnProperty(args.cCode))
		{
			throw "User has no currency of type " + args.cCode;
		}
		else if(inventoryResult.VirtualCurrency.hasOwnProperty(args.cCode) && args.amount > inventoryResult.VirtualCurrency[args.cCode])
		{
			throw "User does not have enough currency to complete this transfer.";
		}

		var subtractVcParms = {};
		subtractVcParms.PlayFabId = currentPlayerId;
		subtractVcParms.VirtualCurrency = args.cCode;
		subtractVcParms.Amount = args.amount;

		server.SubtractUserVirtualCurrency(subtractVcParms);
	}
	else
	{
		var inventoryParams = {};
		inventoryParams.CharacterId = args.sourceId;
		inventoryParams.PlayFabId = currentPlayerId;

		var inventoryResult = server.GetCharacterInventory(inventoryParams);

		// ensure that the account has enough VC to transfer
		if(!inventoryResult.VirtualCurrency.hasOwnProperty(args.cCode))
		{
			throw "User has no currency of type " + args.cCode;
		}
		else if(inventoryResult.VirtualCurrency.hasOwnProperty(args.cCode) && args.amount > inventoryResult.VirtualCurrency[args.cCode])
		{
			throw "User does not have enough currency to complete this transfer.";
		}

		var subtractVcParms = {};
		subtractVcParms.CharacterId = args.sourceId;
		subtractVcParms.PlayFabId = currentPlayerId;
		subtractVcParms.VirtualCurrency = args.cCode;
		subtractVcParms.Amount = args.amount;

		server.SubtractCharacterVirtualCurrency(subtractVcParms);
	}

	var addVcParams = {};
	addVcParams.PlayFabId = currentPlayerId;
	addVcParams.CharacterId = args.destId;
	addVcParams.VirtualCurrency = args.cCode;
	addVcParams.Amount = args.amount;
	
	server.AddCharacterVirtualCurrency(addVcParams);

	return;
}

// HELPER FUNCTIONS (NOT DIRECTLY CALLABLE FROM THE CLIENT)
function InitializeNewCharacterData(id, catalogCode)
{
	var cDetails = handlers.GetBaseClassForType( {"cCode": catalogCode} );
	
	// default character properties
	var CharacterData = {};
	CharacterData.ClassDetails = cDetails;
	CharacterData.TotalExp = 0;
	CharacterData.ExpThisLevel = 0;
	CharacterData.Health = cDetails.BaseHP;
	CharacterData.Mana = cDetails.BaseMP;
	CharacterData.Defense = cDetails.BaseDP;
	CharacterData.Speed = cDetails.BaseSP;
	CharacterData.CharacterLevel = 1;
	CharacterData.Spell1_Level = 0;
	CharacterData.Spell2_Level = 0;
	CharacterData.Spell3_Level = 0;
	CharacterData.CustomAvatar = null;

	// API params
	var params = {};
	params.PlayFabId = currentPlayerId;
	params.CharacterId = id;
	params.Data = {};
	params.Permission = "Public";
	params.Data.CharacterData = JSON.stringify(CharacterData);

	server.UpdateCharacterReadOnlyData(params);

	// set up Heart VC
	var balanceHeartVC = {};
	balanceHeartVC.PlayFabId = currentPlayerId;
	balanceHeartVC.CharacterId = id;
	balanceHeartVC.VirtualCurrency = "HT";
	balanceHeartVC.Amount = 0;

	server.AddCharacterVirtualCurrency(balanceHeartVC);
	
	// set up Gold VC
	var balanceGoldVC = {};
	balanceGoldVC.PlayFabId = currentPlayerId;
	balanceGoldVC.CharacterId = id;
	balanceGoldVC.VirtualCurrency = "G";
	balanceGoldVC.Amount = 0;

	server.AddCharacterVirtualCurrency(balanceGoldVC);
	
	// set up Gem VC
	var balanceGemVC = {};
	balanceGemVC.PlayFabId = currentPlayerId;
	balanceGemVC.CharacterId = id;
	balanceGemVC.VirtualCurrency = "Gm";
	balanceGemVC.Amount = 0;

	server.AddCharacterVirtualCurrency(balanceGemVC);
}

function HasAchievement(achievement, playerStats)
{
	if(achievement.SingleStat == true)
	{
		for (var stat in playerStats)
		{
			if (playerStats.hasOwnProperty(stat) && stat.indexOf(achievement.MatchingStatistic) > -1) 
			{
				if(playerStats[stat] >= achievement.Threshold)
				{
		       		// Stat found and exceeds the achievement threshold
		       		return true;
		       	}
		       }
		   }
		}
		else
		{
		// process aggregate stats
		var statTotal = 0;
		for (var stat in playerStats)
		{
			if (playerStats.hasOwnProperty(stat) && stat.indexOf(achievement.MatchingStatistic) > -1) 
			{
				statTotal += playerStats[stat];
			}
		}

		if(statTotal >= achievement.Threshold)
		{
       		// sum of stats found exceeds the achievement threshold
       		return true;
       	}
       }

       return false;
   }

	EvaluateAchievements = function(args)
   {
	//  CharacterId

	// get the achievement thresholds set by TitleData
	var achivementParams = {};
	achivementParams.Keys = ["Achievements"];
	
	var ServerAchievements = server.GetTitleData(achivementParams)

	if(ServerAchievements.Data.hasOwnProperty("Achievements"))
	{
		ServerAchievements = JSON.parse(ServerAchievements.Data["Achievements"]);
	}
	else
	{
		throw "Achievements not found on Server. Check TitleData[\"Achievements\"]";
	}

	// get the character stats
	var playerStatsParams = {};
	playerStatsParams.PlayFabId = currentPlayerId;
	playerStatsParams.CharacterId = args.CharacterId;
	
	var PlayerStats = server.GetCharacterStatistics(playerStatsParams);
	PlayerStats = PlayerStats.CharacterStatistics;


	// get the unlocked stats for the character
	var awardedDataParams = {};
	awardedDataParams.PlayFabId = currentPlayerId;
	awardedDataParams.CharacterId = args.CharacterId;
	awardedDataParams.Keys = ["Achievements"];
	
	var AwardedAchievements = server.GetCharacterReadOnlyData(awardedDataParams);
	
	if(AwardedAchievements.Data.hasOwnProperty("Achievements"))
	{
		AwardedAchievements = JSON.parse(AwardedAchievements.Data["Achievements"].Value);
	}
	else
	{
		// no achievements 
		AwardedAchievements = [];
	}


	// TODO -- need to track new achievements and toss them over to the offer evaluator
	var arrayCount = AwardedAchievements.length;
	for(var property in ServerAchievements)
	{
		// if the player does not already have the achievement, evaluate progress
		if(AwardedAchievements.indexOf(property) == -1)
		{
			var result = HasAchievement(ServerAchievements[property], PlayerStats);

			if(result == true)
			{
				//log.info(property + " -- Awarded");
				AwardedAchievements.push(property);
			}
		}
	}

	// if we added some new achievements, save details to character data
	if(arrayCount < AwardedAchievements.length)
	{
		var characterDataParams = {};
		characterDataParams.PlayFabId = currentPlayerId;
		characterDataParams.CharacterId = args.CharacterId;
		characterDataParams.Data = {};
		characterDataParams.Permission = "Public";

		characterDataParams.Data.Achievements = JSON.stringify(AwardedAchievements);
		server.UpdateCharacterReadOnlyData(characterDataParams);

		//log.info("New Achievements Added!");
	}
}

function isObjectEmpty(obj)
{
	if(typeof obj === 'undefined')
	{
		return true;
	}

	if(Object.getOwnPropertyNames(obj).length === 0)
	{
		return true;
	}
	else
	{
		return false;
	}
}

function CreateGUID()
{
	//http://stackoverflow.com/questions/105034/create-guid-uuid-in-javascript
	return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {var r = Math.random()*16|0,v=c=='x'?r:r&0x3|0x8;return v.toString(16);});
}