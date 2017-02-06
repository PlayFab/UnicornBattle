
using System;
public class SupersonicPlacement
{
	private string rewardName;
	private int rewardAmount;
	private string placementName;


	public SupersonicPlacement (string pName,string rName,int rAmount)
	{
		this.placementName = pName;
		this.rewardName = rName;
		this.rewardAmount = rAmount;
	}

	public string getRewardName(){
		return rewardName;
	}

	public int getRewardAmount(){
		return rewardAmount;
	}

	public string getPlacementName(){
		return placementName;
	}

	public override string ToString()
	{
		return placementName + " : " + rewardName + " : " + rewardAmount;
	}


}






