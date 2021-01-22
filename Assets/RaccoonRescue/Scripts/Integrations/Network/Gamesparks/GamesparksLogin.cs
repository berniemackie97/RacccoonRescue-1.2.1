using UnityEngine;
using System.Collections;
using System.Collections.Generic;


#if GAMESPARKS
using GameSparks.Api.Requests;
using GameSparks.Core;

public class GamesparksLogin : ILoginManager
{
    #region ILoginManager implementation

    string userID;

    public GamesparksLogin()
    {

    }

    public void LoginWithFB(string accessToken, string titleId)
    {
        // Debug.Log("login to Gamesparks... " + " token " + accessToken);
        new FacebookConnectRequest().SetSwitchIfPossible(true).SetAccessToken(accessToken).Send((response) =>//1.2
        {
            if (!response.HasErrors)
            {
                Debug.Log("Player id : " + response.UserId);
                userID = response.UserId;
                NetworkManager.UserID = userID;
                NetworkManager.THIS.IsLoggedIn = true;
            }
            else
            {
                IDictionary<string, object> errors = response.Errors.BaseData;
                Debug.Log("Authentification error:");
                foreach (var item in errors)
                {
                    Debug.Log(item.Key + " : " + item.Value);
                }
                //				Debug.Log ("errors " + response.Errors);
            }
        });

    }

    public void UpdateName(string userName)
    {
        //		new AccountDetailsRequest ()
        //			.Send ((response) => {
        //			if (!response.HasErrors) {
        //
        //				Debug.Log ("Player id :" + response.UserId);
        //				Debug.Log ("Player name :" + response.DisplayName);
        //			} else {
        //				Debug.Log ("errors " + response.Errors);
        //			}
        //
        //		});
    }

    public bool IsYou(string id)
    {
        if (id == userID)
            return true;
        return false;
    }

    #endregion


}

#endif
