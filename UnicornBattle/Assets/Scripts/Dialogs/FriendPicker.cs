using UnityEngine;
using System.Collections;

public class FriendPicker : MonoBehaviour {
	public Transform friendItemPrefab;
	public Transform friendList;
	public Transform friendStats;
	
	// Use this for initialization
	void Start () {
        // itterate and add friends from PF
        ReturnToFriendList();
    }

	public void FriendItemClicked(FriendItem item)
	{
		DisplaySelectedFriendStats();
	}
	
	public void ReturnToFriendList()
	{
		friendStats.gameObject.SetActive(false);
		friendList.gameObject.SetActive(true);
	}
	
	public void DisplaySelectedFriendStats()
	{
		friendStats.gameObject.SetActive(true);
		friendList.gameObject.SetActive(false);
	}
}

