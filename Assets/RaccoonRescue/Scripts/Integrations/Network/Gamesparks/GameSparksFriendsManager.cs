using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if GAMESPARKS
using GameSparks.Api.Requests;
using GameSparks.Core;

public class GameSparksFriendsManager : IFriendsManager
{

    #region IFriendsManager implementation

    public void GetFriends(System.Action<System.Collections.Generic.Dictionary<string, string>> Callback)
    {
        new GameSparks.Api.Requests.SocialLeaderboardDataRequest().SetLeaderboardShortCode("Level").SetDontErrorOnNotSocial(true).SetEntryCount(100).Send((response) =>
        {
            if (!response.HasErrors)
            {
                Debug.Log("Found friends Data..." + response.Data);
                Dictionary<string, string> dic = new Dictionary<string, string>();
                foreach (GameSparks.Api.Responses.LeaderboardDataResponse._LeaderboardData entry in response.Data)
                {
                    Debug.Log(entry.UserName);
                    int rank = (int)entry.Rank;
                    string playerName = entry.UserName;
                    var FBidArray = entry.ExternalIds.BaseData;
                    foreach (var item2 in FBidArray)
                    {
                        Debug.Log(item2);
                        dic.Add(item2.Value.ToString(), entry.UserId);
                    }

                }
                Callback(dic);
            }
            else
            {
                Debug.Log("Error Retrieving friends Data...");
            }
        });

    }

    public void PlaceFriendsPositionsOnMap(System.Action<System.Collections.Generic.Dictionary<string, int>> Callback)
    {
        Debug.Log("place friends");
        new GameSparks.Api.Requests.SocialLeaderboardDataRequest().SetLeaderboardShortCode("Level").SetDontErrorOnNotSocial(true).SetEntryCount(100).Send((response) =>
        {
            if (!response.HasErrors)
            {
                Dictionary<string, int> dic = new Dictionary<string, int>();
                foreach (GameSparks.Api.Responses.LeaderboardDataResponse._LeaderboardData entry in response.Data)
                {
                    int rank = (int)entry.Rank;
                    string playerName = entry.UserName;
                    Debug.Log(entry.UserId + " " + entry.JSONData["Level"]);
                    dic.Add(entry.UserId, int.Parse(entry.JSONData["Level"].ToString()));

                }
                Callback(dic);

            }
            else
            {
                Debug.Log("Error Retrieving friends Data...");
            }
        });

    }

    public void GetLeadboardOnLevel(int LevelNumber, System.Action<System.Collections.Generic.List<LeadboardPlayerData>> Callback)
    {
        new GameSparks.Api.Requests.SocialLeaderboardDataRequest().SetDontErrorOnNotSocial(true).SetLeaderboardShortCode("LB.Level." + PlayerPrefs.GetInt("OpenLevel")).SetEntryCount(2).Send((response) =>
        {
            if (!response.HasErrors)
            {
                List<LeadboardPlayerData> list = new List<LeadboardPlayerData>();
                foreach (GameSparks.Api.Responses.LeaderboardDataResponse._LeaderboardData item in response.Data)
                {
                    LeadboardPlayerData pl = new LeadboardPlayerData();
                    pl.Name = item.UserName;
                    pl.userID = item.UserId;
                    pl.position = int.Parse(item.Rank.ToString());
                    pl.score = int.Parse(item.JSONData["Score"].ToString());

                    list.Add(pl);
                    Debug.Log(item.UserName + " " + item.UserId + " " + item.Rank + " " + item.JSONData["Score"]);

                }
                Callback(list);

            }
            else
            {
                Debug.Log("Error Retrieving leadboard Data...");
            }
        });

    }

    public void Logout()
    {

    }


    #endregion



}
#endif