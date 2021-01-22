#if PLAYFAB || GAMESPARKS
using UnityEngine;
using System.Collections;

#if PLAYFAB
using PlayFab.ClientModels;
using PlayFab;
#endif


public class NetworkFriendsManager
{
    IFriendsManager friendsManager;

    public NetworkFriendsManager()
    {
#if PLAYFAB
		friendsManager = new PlayFabFriendsManager ();
#elif GAMESPARKS
        friendsManager = new GameSparksFriendsManager();

#endif
        NetworkManager.OnLoginEvent += GetFriends;

        GameEvent.OnMapState += PlaceFriendsPositionsOnMap;
        MenuManager.OnMenuLeadboard += GetLeadboardOnLevel;
        NetworkManager.OnLogoutEvent += Logout;//1.3.3
    }

    public void Logout()
    {//1.3.3
        NetworkManager.OnLoginEvent -= GetFriends;

        GameEvent.OnMapState -= PlaceFriendsPositionsOnMap;
        MenuManager.OnMenuLeadboard -= GetLeadboardOnLevel;

        NetworkManager.OnLogoutEvent -= Logout;
        FacebookManager.Friends.Clear();
        friendsManager.Logout();
    }


    /// <summary>
    /// Gets the friends list.
    /// </summary>
    public void GetFriends()
    {
        if (!NetworkManager.THIS.IsLoggedIn)
            return;
        Debug.Log("GetFriends");
        if (friendsManager != null)
        {
            friendsManager.GetFriends((dic) =>
            {

                FacebookManager.THIS.AddFriend(FacebookManager.THIS.GetCurrentUserAsFriend());//1.4.4

                foreach (var item in dic)
                {
                    FriendData friend = new FriendData()
                    {
                        FacebookID = item.Key,
                        userID = item.Value
                    };
                    Debug.Log("found friend " + friend.userID);
                    FacebookManager.THIS.AddFriend(friend);//1.4.4
                }
                FacebookManager.THIS.GetFriendsPicture();
                PlaceFriendsPositionsOnMap();

            });
        }
    }

    /// <summary>
    /// Place the friends on map.
    /// </summary>
    public void PlaceFriendsPositionsOnMap()
    {
        Debug.Log("friends on map");
        if (!NetworkManager.THIS.IsLoggedIn)
            return;

        if (friendsManager != null)
        {
            friendsManager.PlaceFriendsPositionsOnMap((list) =>
            {
                foreach (var item in list)
                {
                    FriendData friend = FacebookManager.Friends.Find(delegate (FriendData bk)
                    {
                        return bk.userID == item.Key && bk.userID != NetworkManager.UserID;
                    });
                    if (friend != null)
                    {
                        friend.level = item.Value;
                    }
                }
                NetworkManager.FriendsOnMapLoaded();

            });
        }
    }

    /// <summary>
    /// Gets the leadboard on level.
    /// </summary>
    public void GetLeadboardOnLevel()
    {
        InitScriptName.InitScript.Instance.StartCoroutine(GetLeadboardCor());
    }

    IEnumerator GetLeadboardCor()
    {
        yield return new WaitUntil(() => NetworkManager.THIS.IsLoggedIn == true);
        Debug.Log("getting leadboard");
        if (friendsManager != null)
        {
            int LevelNumber = PlayerPrefs.GetInt("OpenLevel");
            NetworkManager.leadboardList.Clear();
            friendsManager.GetLeadboardOnLevel(LevelNumber, (list) =>
            {
                Debug.Log(list.Count);
                foreach (var pl in list)
                {
                    FriendData friend = FacebookManager.Friends.Find(delegate (FriendData bk)
                    {
                        return bk.userID == pl.userID;
                    }
                                        );
                    if (friend != null)
                    {
                        pl.friendData = friend;
                        pl.picture = friend.picture;
                    }

                    LeadboardPlayerData leadboardPlayerData = NetworkManager.leadboardList.Find(delegate (LeadboardPlayerData bk)
                    {
                        return bk.userID == pl.userID;
                    }
                                                              );
                    if (leadboardPlayerData != null)
                        leadboardPlayerData = pl;
                    else
                        NetworkManager.leadboardList.Add(pl);

                    Debug.Log(pl.Name + " " + pl.userID + " " + pl.position + " " + pl.score);
                }

                if (NetworkManager.leadboardList.Count > 0)
                {
                    NetworkManager.LevelLeadboardLoaded();
                }

            });
        }

    }
}

#endif