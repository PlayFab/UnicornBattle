//Unicorn Battle FB App ID: 403608796495072
var defaultCatalog = "CharacterClasses";

handlers.CreateCharacter = function(args)
{
	//log.info(args.classDetails);
	var params = {};
	params.PlayFabId = currentPlayerId;
	params.CatalogVersion = defaultCatalog;
	params.ItemIds = [args.catalogCode];
	
	try
	{
		// this needs to change when you have limited & locked characters
		// will need a check prereqs method
		server.GrantItemsToUser(params);
	}
	catch(ex)
	{
		//log.error(ex);
		return false;
	}

	params = {};
	params.PlayFabId = currentPlayerId;
	params.CharacterName = args.characterName;
	params.CharacterType = args.catalogCode;

	
	// will contain the newly created character ID
	var result = {};
	try
	{
		result = server.GrantCharacterToUser(params);
		//log.info(result);
		// set up default character data here.
		InitializeNewCharacterData(result.CharacterId, args.catalogCode);
	}
	catch(ex)
	{
		//log.error(ex);
		return false;
	}


	// maybe return the character data or maybe just the id
	return true;
}


handlers.DeleteCharacter = function(args)
{
	var params = {};
	params.PlayFabId = currentPlayerId;
	params.CharacterId = args.characterId;
	params.SaveCharacterInventory = false;
	
	try
	{
		server.DeleteCharacterFromUser(params);
	}
	catch(ex)
	{
		//log.error(ex);
		return false;
	}
	return true;
}



handlers.GetCharacters = function(args)
{
	var chars = {};
	var params = {};
	params.PlayFabId = currentPlayerId;
	try
	{
		chars = server.GetAllUsersCharacters(params);
	}
	catch(ex)
	{
		//log.error(ex);
	}
	return chars.Characters;
}


function InitializeNewCharacterData(id, catalogCode)
{

	try
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

		log.info("Ran the intializeNewCharacterData with no errors");
	}
	catch(ex)
	{
		//log.error(ex);
	}
}

handlers.GetBaseClassForType = function(args)
{
	var params = {};
	params.Keys = ["Classes"];

	try
	{
		var result = server.GetTitleData(params);
		var classes = JSON.parse(result.Data.Classes);

		for(var each in classes)
		{
			if(classes[each].CatalogCode == args.cCode)
			{
				return classes[each];
			}
		}
	}
	catch(ex)
	{
		//log.error(ex);
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
	//log.info(evalParams);
	handlers.EvaluateAchievements(evalParams);
	//EvaluateOffers(); // on hold, pending removal


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


	//return args;
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


handlers.PurchaseItem = function(args)
{
	// ItemPrice / CurrencyCode
	// CharacterId
	// ItemId

	var balance = 0;

	var inventoryParams = {};
	inventoryParams.CharacterId = args.CharacterId;
	inventoryParams.PlayFabId = currentPlayerId;

	var inventoryResult = server.GetCharacterInventory(inventoryParams);

	for(var key in inventoryResult.VirtualCurrency)
	{
		if (inventoryResult.VirtualCurrency.hasOwnProperty(key)) 
		{
			if(key == args.CurrencyCode)
			{
				balance = inventoryResult.VirtualCurrency[key];
			}
		}
	}


	if(balance >= args.ItemPrice)
	{
		var grantParams = {};
		grantParams.CharacterId = args.CharacterId;
		grantParams.PlayFabId = currentPlayerId;
		grantParams.ItemIds = [];
		grantParams.ItemIds.push(args.ItemId);

		var grantItemsResult = server.GrantItemsToCharacter(grantParams);
		//log.info(grantItemsResult);

		if(grantItemsResult.ItemGrantResults.length > 0)
		{
			var subtractVcParms = {};
			subtractVcParms.CharacterId = args.CharacterId;
			subtractVcParms.PlayFabId = currentPlayerId;
			subtractVcParms.VirtualCurrency = args.CurrencyCode;
			subtractVcParms.Amount = args.ItemPrice;
			server.SubtractCharacterVirtualCurrency(subtractVcParms);
			return true;
		}	
		else
		{
			return false;
		}
		
		// successful purchase 
		
	}
	else
	{
		// not enough VC to make purchase
		return false;
	}
}

handlers.ConsumeItem = function(args)
{
	// CharacterId
	// ItemId

	var itemToConsume = {};

	var inventoryParams = {};
	inventoryParams.CharacterId = args.CharacterId;
	inventoryParams.PlayFabId = currentPlayerId;

	var inventoryResult = server.GetCharacterInventory(inventoryParams);
	
	for(var key in inventoryResult.Inventory)
	{
		if(inventoryResult.Inventory[key].ItemId == args.ItemId)
		{
			// Item found
			itemToConsume = inventoryResult.Inventory[key];
		}
	}
	
	// Item not found in character inventory
	if(isObjectEmpty(itemToConsume))
	{
		return false
	}
	else
	{
		// Workaround for platform shortcommings
		try
		{
			var charToUserParams = {};
			charToUserParams.PlayFabId = currentPlayerId;
			charToUserParams.CharacterId = args.CharacterId;
			charToUserParams.ItemInstanceId = itemToConsume.ItemInstanceId;

			server.MoveItemToUserFromCharacter(charToUserParams);

			var modifyUsesParams = {};
			modifyUsesParams.PlayFabId = currentPlayerId;
			modifyUsesParams.ItemInstanceId = itemToConsume.ItemInstanceId;
			modifyUsesParams.UsesToAdd = -1;

			var modifyResponse = server.ModifyItemUses(modifyUsesParams);

			if(modifyResponse.RemainingUses > 0)
			{
				// move item back to character inventory
				var userToCharParams = {};
				userToCharParams.PlayFabId = currentPlayerId;
				userToCharParams.CharacterId = args.CharacterId;
				userToCharParams.ItemInstanceId = itemToConsume.ItemInstanceId;

				server.MoveItemToCharacterFromUser(userToCharParams);
			}
		}
		catch(ex)
		{
			//log.error(JSON.stringify(ex);
		}
	}

}

handlers.UnlockContainer = function(args)
{
	// CharacterId
	// ItemId

	var itemToConsume = {};

	var inventoryParams = {};
	inventoryParams.CharacterId = args.CharacterId;
	inventoryParams.PlayFabId = currentPlayerId;

	var inventoryResult = server.GetCharacterInventory(inventoryParams);
	
	for(var key in inventoryResult.Inventory)
	{
		if(inventoryResult.Inventory[key].ItemId == args.ItemId)
		{
			// Item found
			itemToConsume = inventoryResult.Inventory[key];
		}
	}
	
	// Item not found in character inventory
	if(isObjectEmpty(itemToConsume))
	{
		return false
	}
	else
	{
		// Workaround for platform shortcommings
		try
		{
			var charToUserParams = {};
			charToUserParams.PlayFabId = currentPlayerId;
			charToUserParams.CharacterId = args.CharacterId;
			charToUserParams.ItemInstanceId = itemToConsume.ItemInstanceId;

			server.MoveItemToUserFromCharacter(charToUserParams);

			var modifyUsesParams = {};
			modifyUsesParams.PlayFabId = currentPlayerId;
			modifyUsesParams.ItemInstanceId = itemToConsume.ItemInstanceId;
			modifyUsesParams.UsesToAdd = -1;

			var modifyResponse = server.ModifyItemUses(modifyUsesParams);

			if(modifyResponse.RemainingUses > 0)
			{
				// move item back to character inventory
				var userToCharParams = {};
				userToCharParams.PlayFabId = currentPlayerId;
				userToCharParams.CharacterId = args.CharacterId;
				userToCharParams.ItemInstanceId = itemToConsume.ItemInstanceId;

				server.MoveItemToCharacterFromUser(userToCharParams);
			}
		}
		catch(ex)
		{
			//log.error(ex);
		}
	}
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

	//EVALUATE OFFER / ACHIEVEMENTS
	return {};
 
}


handlers.EvaluateAchievements = function(args)
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


	// TODO -- need to track new achievemnets and toss them over to the offer evaluator
	var arrayCount = AwardedAchievements.length;
	for(var property in ServerAchievements)
	{
		// if the player does not already have the achievement, evaluate progress
		if(AwardedAchievements.indexOf(property) == -1)
		{
			var result = AssertAchievement(ServerAchievements[property], PlayerStats);

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


// base evaluation of offers will be done on the client
// these will run automatically on levelup & on achievement

handlers.EvaluateLevelUpOffers = function(args)
{



}

handlers.EvaluateAchievementOffers = function(args)
{



}


handlers.RemoveOfferItem = function(args)
{
	// args.PFID
	// args.InstanceToRemove
	
	var params = {}
	params.PlayFabId = args.PFID;
	params.ItemInstanceId = args.InstanceToRemove;
	params.UsesToAdd = -1; // make sure we consume all uses

	//  NEED TO DELETE THE ITEM FROM THE USERS INVENTORY
	server.ModifyItemUses(params);
	return true;
}



handlers.RedeemItemOffer = function(args)
{
	// args.PFID
	// args.Offer
	// args.SingleUse

	if(typeof(args.PFID) !== "undefined" && typeof(args.Offer) !== "undefined" && typeof(args.SingleUse) !== "undefined")
	{
		if(args.SingleUse == true)
		{
			var getParams = {};
			getParams.PlayFabId = args.PFID;
			getParams.Keys = ["RedeemedOffers"];

			var currentData = server.GetUserReadOnlyData(getParams);
			var redeemed = [];

			if (currentData.Data.hasOwnProperty("RedeemedOffers"))
			{
				redeemed = JSON.parse(currentData.Data["RedeemedOffers"].Value);
			}

			redeemed.push(args.Offer.ItemId);

			// write back data changes to the server
			var updateParams = {};
			updateParams.PlayFabId = args.PFID;
			updateParams.Data = {};
			updateParams.Data.RedeemedOffers = JSON.stringify(redeemed);
			updateParams.Permission = "Public";

			server.UpdateUserReadOnlyData(updateParams);
		}

		// determine the item grant:
		var customData = JSON.parse(args.Offer.CustomData);

		if(customData.hasOwnProperty("itemAwarded"))
		{
			//GRANT player the item in the custom data.
			var grantParams = {};
			grantParams.CatalogVersion = defaultCatalog;
			grantParams.PlayFabId = args.PFID;
			grantParams.Annotation = "Granted by Offer:" + args.Offer.DisplayName;
			grantParams.ItemIds = [customData["itemAwarded"]];

			server.GrantItemsToUser(grantParams);
			
			return customData["itemAwarded"]; // tell the player about the item granted... so the item viewer popup can trigger
		}
		else
		{
			return null;
		}
	}
	else
	{
		//log.info("Invalid parameters");
		return null;
	}
}




handlers.ConsumeOffer = function(args)
{
	//args.OfferGUID = ""
	//args.PFID = ""

	if(typeof(args.PFID) !== "undefined" && typeof(args.OfferGUID) !== "undefined")
	{
		var offerInstance = {};
		// get the user data that contains pending and consumed offers
		var getParams = {};
		getParams.PlayFabId = args.PFID;
		getParams.Keys = ["PendingOffers", "ConsumedSingleOffers"];

		var currentData = server.GetUserReadOnlyData(getParams);
		var pending = {};
		var consumed = {};
    	
    	if (currentData.Data.hasOwnProperty("ConsumedSingleOffers"))
		{
			consumed = JSON.parse(currentData.Data["ConsumedSingleOffers"].Value);
		}

		if (currentData.Data.hasOwnProperty("PendingOffers"))
		{
			pending = JSON.parse(currentData.Data["PendingOffers"].Value);
		}

		if(!isObjectEmpty(pending))
		{
			if(pending.hasOwnProperty(args.OfferGUID))
			{
				offerInstance = pending[args.OfferGUID];

				// get the active offer details from server
				var titleDataParams = {};
				titleDataParams.Keys = ["Offers"];

				var titleDataResult = server.GetTitleData(titleDataParams);
				var serverOffers = null;

				if (titleDataResult.Data.hasOwnProperty("Offers"))
				{
					serverOffers = JSON.parse(titleDataResult.Data["Offers"]);
				}

				// cross ref our offers data to determine what to do next. 
				if (serverOffers.hasOwnProperty(offerInstance.OfferId)) 
				{
					if(serverOffers[offerInstance.OfferId].ItemToGrant != null || serverOffers[offerInstance.OfferId].ItemToGrant != "")
					{
						//log.info(serverOffers[offerInstance.OfferId].AppliesTo);
						if(serverOffers[offerInstance.OfferId].AppliesTo == "Player")
						{
							var grantParams = {}
							grantParams.CatalogVersion = defaultCatalog;
							grantParams.PlayFabId = args.PFID;
							grantParams.Annotation = "Awarded by Offer";
							grantParams.ItemIds = [ serverOffers[offerInstance.OfferId].ItemToGrant ];

							var itemGrantResult = server.GrantItemsToUser(grantParams);
							//log.info(itemGrantResult);
						}
						else if(serverOffers[offerInstance.OfferId].AppliesTo == "Character")
						{	
							var grantParams = {};
							grantParams.CatalogVersion = defaultCatalog;
							grantParams.PlayFabId = args.PFID;
							params.CharacterId = args.CharacterId;
							grantParams.Annotation = "Awarded by" + args.Offer;
							grantParams.ItemIds = [ serverOffers[offerInstance.OfferId].ItemToGrant ];

							var itemGrantResult = server.GrantItemsToCharacter(grantParams);
							//log.info(itemGrantResult);
						}
						else
						{
							//log.info("Invalid AppliesTo property on Offer: " + args.OfferGUID);
							return false;
						}
					}

					// if this is a one time offer, save it so we know not to add again.
	    			if (serverOffers[offerInstance.OfferId].OfferTrigger.Occurrence == "Single")
					{
						//found the offer, now move it to consumed.

						var offerGUID = args.OfferGUID;
						consumed[offerGUID] = {};
						consumed[offerGUID].OfferId = offerInstance.OfferId;
						consumed[offerGUID].Occurrence = offerInstance.Occurrence;
						consumed[offerGUID].AwardedOn = offerInstance.AwardedOn;
						consumed[offerGUID].RedeemedOn = Date.now();
						consumed[offerGUID].AppliesToCharacter = offerInstance.CharacterId;
						//log.info("After add to consumed: " +  consumed[offerGUID]);
					}

					// consume the offer.
					delete pending[args.OfferGUID];

					// write back data changes to the server
					var updateParams = {};
					updateParams.PlayFabId = args.PFID;
					updateParams.Data = {};
					updateParams.Data.PendingOffers = JSON.stringify(pending);
					updateParams.Data.ConsumedSingleOffers = JSON.stringify(consumed);
					updateParams.Permission = "Public";

					server.UpdateUserReadOnlyData(updateParams);

					return true;
	  			}
	  			else
	  			{
	  				//log.info(args.Offer + " not found on server");
	  				return false;
	  			}
			}
			else
			{
				//log.info(args.Offer + " not found on player");
				return false;
			}
		}
		else
		{
			// no pending offers, quit...
			//log.info("Player has no pending offers");
			return false;
		}
	}
	else
	{
		///log.info("Invalid Parmeters");
		return false
	}
}

handlers.GrantOffer = function(args)
{
	//args.Offer = ""
	//args.PFID

	if(typeof(args.PFID) !== "undefined" && typeof(args.Offer) !== "undefined")
	{
		// get the user data that contains pending and consumed offers
		var getParams = {};
		getParams.PlayFabId = args.PFID;
		getParams.Keys = ["PendingOffers", "ConsumedSingleOffers"];

		var currentData = server.GetUserReadOnlyData(getParams);
		var pending = {};
		var consumed = {};

    	if (currentData.Data.hasOwnProperty("ConsumedSingleOffers"))
		{
			consumed = JSON.parse(currentData.Data["ConsumedSingleOffers"].Value);
		}

		if (currentData.Data.hasOwnProperty("PendingOffers"))
		{
			
			pending = JSON.parse(currentData.Data["PendingOffers"].Value);
		}


		// make sure the offer is not already pending
		if(!isObjectEmpty(pending))
		{
			for(var key in pending)
			{
				if(pending[key].OfferId == args.Offer && pending[key].AppliesTo != null)
				{
					if(pending[key].AppliesTo == args.CharacterId)
					{
						// already pending...
						//log.info(args.Offer + " for character " + args.CharacterId + " is already pending. Will not add again.");
						return false;
					}
				}
				else if(pending[key].OfferId == args.Offer)
				{
					// already pending...
					//log.info(args.Offer + " is already pending. Will not add again.");
					return false;
				}
			}
		}

		// get the active offer details from server
		var titleDataParams = {};
		titleDataParams.Keys = ["Offers"];

		var titleDataResult = server.GetTitleData(titleDataParams);
		var serverOffers = null;

		if (titleDataResult.Data.hasOwnProperty("Offers"))
		{
			serverOffers = JSON.parse(titleDataResult.Data["Offers"]);
		}

		if (serverOffers.hasOwnProperty(args.Offer)) 
		{
			if(serverOffers[args.Offer].OfferTrigger.Occurrence == "Single")
			{
				if (!isObjectEmpty(consumed))
				{
					for(var key in consumed)
					{
						if(consumed[key].OfferId == args.Offer && consumed[key].AppliesToCharacter != null)
						{
							if(consumed[key].AppliesToCharacter == args.CharacterId)
							{
								// already redeemed...
								//log.info(args.Offer + " for character " + args.CharacterId + " is already redeemed. Will not add again.");
								return false;
							}
						}
						else if(consumed[key].OfferId == args.Offer)
						{
							// already redeemed...
							//log.info(args.Offer + " is already redeemed. Will not add again.");
							return false;
						}
					}
				}
			}
			}
			else
			{
				// no corresponding offer found in title data
				//log.info(args.Offer + ": No corresponding offer found in title data");
				return false;
			}


		// ok checks out, lets add a pending offer
		// {
		// 	"GUID_OFFER_ID" : //character id or player id
		// 	{
		// 		"OfferId" : "", 
		// 		"AppliesToCharacter" : null / "Character_Id",
		// 		"Occurrence" : "Single",
		// 		"AwardedOn" : 0,
		// 		"RedeemedOn" : 0
		// 	}
		// }

		var offerGUID = CreateGUID();
		pending[offerGUID] = {};
		pending[offerGUID].OfferId = args.Offer;
		pending[offerGUID].Occurrence = serverOffers[args.Offer].OfferTrigger.Occurrence;
		pending[offerGUID].AwardedOn = Date.now();
		pending[offerGUID].RedeemedOn = 0;

		if(typeof(args.CharacterId) !== "undefined" )
		{
			// check to make sure player owns character:
			// var characterParams = {};
			// characterParams.PlayFabId = args.PFID;
			// var playerCharacters = server.GetAllUsersCharacters(params); //[] of CharacterResult -- CharacterId; CharacterName; CharacterType;

			pending[offerGUID].AppliesToCharacter = args.CharacterId;
		}
		else
		{
			pending[offerGUID].AppliesToCharacter = null;
		}	

		// write data back to server
		var updateParams = {};
		updateParams.PlayFabId = args.PFID;
		updateParams.Data = {};
		updateParams.Data.PendingOffers = JSON.stringify(pending);
		updateParams.Permission = "Public";

		server.UpdateUserReadOnlyData(updateParams);
		return true;

	}
	else
	{
		//log.info("Invalid Parmeters");
		return false
	}
}



handlers.TransferItemToPlayer = function(args)
{
	if(args.sourceId == undefined || args.instanceId == undefined)
		return "Invalid parameters.";

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
		return "Invalid parameters.";
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

	if(inventoryResult.VirtualCurrency.hasOwnProperty(args.cCode))
	{
		// ensure that the account has enough VC to transfer
		if(args.amount > inventoryResult.VirtualCurrency[args.cCode])
		{
			throw "User does not have enough currency to complete this transfer.";
		}
	}
	else
	{
		throw "User has no currency of type " + args.cCode;
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

		if(inventoryResult.VirtualCurrency.hasOwnProperty(args.cCode))
		{
			// ensure that the account has enough VC to transfer
			if(args.amount > inventoryResult.VirtualCurrency[args.cCode])
			{
				throw "User does not have enough currency to complete this transfer.";
			}
		}
		else
		{
			throw "User has no currency of type " + args.cCode;
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

		if(inventoryResult.VirtualCurrency.hasOwnProperty(args.cCode))
		{
			// ensure that the account has enough VC to transfer
			if(args.amount > inventoryResult.VirtualCurrency[args.cCode])
			{
				throw "User does not have enough currency to complete this transfer.";
			}
		}
		else
		{
			throw "User has no currency of type " + args.cCode;
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
function AssertAchievement(achievement, playerStats)
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

