
//Regex.Replace("ThisIsMyCapsDelimitedString", @"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))", " $1");


// Sales
{
	"s200" : 
	{
		"SaleName" : "Happy Hour",
		"SaleDescription" : "Get Potions, Elyxirs & flasks 50% off everyday, all month long, Sale courtesy of The Consortium.",
		"StoreToUse" : "Happy Hour",
		"StartDate" : "09/01/2015",
		"EndDate" : "09/30/2015",
		"BundleId" : "sales/s200",
		"PromoteWithInterstitial" : true,
		"PromoteWithCarousel" : true,
		"Occurrence" : 
		{
			"Availability" : "Daily",
			"OpensAt" : "12:00",
			"ClosesAt" : "14:00"
		}
	},

	"s100" : 
	{
		"SaleName" : "Launch Party",
		"SaleDescription" : "Great prices on items for new players. Enjoy these benefits all month long, Sale courtesy of The Consortium. Thanks for playing!",
		"StoreToUse" : "LaunchParty",
		"StartDate" : "09/01/2015",
		"EndDate" : "09/30/2015",
		"BundleId" : "sales/s100",
		"PromoteWithInterstitial" : true,
		"PromoteWithCarousel" : true,
		"Occurrence" : 
		{
			"Availability" : "Daily",
			"OpensAt" : "12:00",
			"ClosesAt" : "14:00"
		}
	}
}


// OFFER ISSUE: how to tell what character got the award. Probably need a more complex offer history structure
//pending offers // redeemedOffers
{
	"GUID_OFFER_ID" : //character id or player id
	{
		"OfferId" : "", 
		"AppliesToCharacter" : null / "Character_Id",
		"Occurrence" : "Single",
		"AwardedOn" : 0,
		"RedeemedOn" : 0
	}
}


// Offers (will be saved to player data and evaluated on the next interstitial)
{
	"OFFER_001" : 
	{
		"OfferName" : "Level Up - 2 ",
		"OfferDescription" : "Congratulations on achieving Level 2. Its dangerous out there, here take this.",
		"StoreToUse" : null,
		"ItemToGrant" : "LevelUp2_Chest",
		"AppliesTo" : "Player",
		"OfferTrigger" :
		{
			"OnLevelGained" : 2,
			"OnAchievementGained" : null,
			"Occurrence" : "Single"
		}
	},

	"AD_CAMPAIGN" : 
	{
		"OfferName" : "Promotional Rewards",
		"OfferDescription" : "Congratulations on trying out Unicorn Battle",
		"StoreToUse" : null,
		"ItemToGrant" : "CrystalKey",
		"AppliesTo" : "Player",
		"OfferTrigger" :
		{
			"OnLevelGained" : null,
			"OnAchievementGained" : null,
			"Occurrence" : "Single"
		}
	}
}