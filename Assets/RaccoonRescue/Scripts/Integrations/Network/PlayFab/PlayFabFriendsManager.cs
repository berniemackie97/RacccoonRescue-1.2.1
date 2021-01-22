#if PLAYFAB
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using System;

public class PlayFabFriendsManager : IFriendsManager
{

	public PlayFabFriendsManager()
	{
	}

	public void Logout()
	{//1.3.3
	}


	/// <summary>
	/// Gets the friends list.
	/// </summary>
	public void GetFriends(Action<Dictionary<string, string>> Callback)
	{

		PlayFab.ClientModels.GetFriendsListRequest request = new PlayFab.ClientModels.GetFriendsListRequest()
		{
			IncludeFacebookFriends = true
		};

		PlayFabClientAPI.GetFriendsList(request, (result) =>
		{
			Dictionary<string, string> dic = new Dictionary<string, string>();
			foreach (var item in result.Friends)
			{
				dic.Add(item.FacebookInfo.FacebookId, item.FriendPlayFabId);
			}
			Callback(dic);
		}, (error) =>
		{
			Debug.Log(error.ErrorDetails);
		});

	}

	/// <summary>
	/// Place the friends on map.
	/// </summary>
	public void PlaceFriendsPositionsOnMap(Action<Dictionary<string, int>> Callback)
	{
		Debug.Log("place friends");
		PlayFab.ClientModels.GetFriendLeaderboardRequest request = new PlayFab.ClientModels.GetFriendLeaderboardRequest()
		{
			StatisticName = "Level",
			IncludeFacebookFriends = true
		};

		PlayFabClientAPI.GetFriendLeaderboard(request, (result) =>
		{
			Dictionary<string, int> dic = new Dictionary<string, int>();
			foreach (var item in result.Leaderboard)
			{
				dic.Add(item.PlayFabId, item.StatValue);
			}
			Callback(dic);
		}, (error) =>
		{
			Debug.Log(error.ErrorDetails);
		});
	}

	/// <summary>
	/// Gets the leadboard on level.
	/// </summary>
	public void GetLeadboardOnLevel(int LevelNumber, Action<List<LeadboardPlayerData>> Callback)
	{
		PlayFab.ClientModels.GetFriendLeaderboardAroundPlayerRequest request = new PlayFab.ClientModels.GetFriendLeaderboardAroundPlayerRequest()
		{
			StatisticName = "Level_" + LevelNumber,
			MaxResultsCount = 5,
			PlayFabId = NetworkManager.UserID,
			IncludeFacebookFriends = true
		};

		PlayFabClientAPI.GetFriendLeaderboardAroundPlayer(request, (result) =>
		{
			if (GameEvent.Instance.GameStatus == GameState.Map)
				NetworkManager.leadboardList.Clear();
			List<LeadboardPlayerData> list = new List<LeadboardPlayerData>();
			foreach (var item in result.Leaderboard)
			{
				LeadboardPlayerData pl = new LeadboardPlayerData();
				pl.Name = item.DisplayName;
				pl.userID = item.PlayFabId;
				pl.position = item.Position;
				pl.score = item.StatValue;

				list.Add(pl);
				Debug.Log(item.DisplayName + " " + item.PlayFabId + " " + item.Position + " " + item.StatValue);
			}
			Callback(list);

		}, (error) =>
		{
			Debug.Log(error.ErrorDetails);
		});



	}



}

#endif