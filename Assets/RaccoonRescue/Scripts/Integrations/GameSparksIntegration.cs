//#if GAMESPARKS
//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using Facebook.Unity;
//using GameSparks.Api.Requests;
//
//public class GameSparksIntegration : MonoBehaviour
//{
//    public static GameSparksIntegration THIS;
//    private string lastResponse = string.Empty;
//
//    protected string LastResponse
//    {
//        get
//        {
//            return this.lastResponse;
//        }
//
//        set
//        {
//            this.lastResponse = value;
//        }
//    }
//
//    private string status = "Ready";
//
//    protected string Status
//    {
//        get
//        {
//            return this.status;
//        }
//
//        set
//        {
//            this.status = value;
//        }
//    }
//
//    // Use this for initialization
//    void Start()
//    {
//        THIS = this;
//    }
//
//
//    //public void CallFBLogin()
//    //{
//    //    FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email", "user_friends" }, this.HandleResult);
//    //}
//
//    public void GameSparkLogin()
//    {
//        //If so, we can use that acces token to log in to Facebook
//        new FacebookConnectRequest().SetAccessToken(AccessToken.CurrentAccessToken.TokenString).Send((response) =>
//        {
//            //If our response has errors we can check what went wrong
//            if (response.HasErrors)
//            {
//                Debug.Log("Something failed when connecting with Facebook " + response.Errors);
//            }
//            else
//            {
//                //Otherwise we are successfully logged in!
//                Debug.Log("Gamesparks Facebook Login Successful");
//                //Since we successfully logged in, we can get our account information.
//                UserManager.instance.UpdateInformation();
//            }
//        });
//    }
//
//
//}
//
//#endif