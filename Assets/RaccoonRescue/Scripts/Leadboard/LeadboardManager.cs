using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

#if FACEBOOK
using Facebook.Unity;
#endif


public class LeadboardManager : MonoBehaviour
{
    public GameObject playerIconPrefab;
    List<LeadboardObject> playerIconsList = new List<LeadboardObject>();
    public GameObject button;

    void OnEnable()
    {
        GetComponent<Image>().enabled = false;
#if PLAYFAB || GAMESPARKS
        //if (FB.IsLoggedIn)
        //	button.SetActive(false);
        //else
        //button.SetActive(true);
        //PlayFabManager.OnLevelLeadboardLoaded += ShowLeadboard;
        Debug.Log("leadboard enable");
        NetworkManager.leadboardList.Clear();
        StartCoroutine(WaitForLeadboard());
#endif
    }

    void OnDisable()
    {
        //		Debug.Log ("Leadboard disable");		
#if PLAYFAB || GAMESPARKS
        //PlayFabManager.OnLevelLeadboardLoaded -= ShowLeadboard;
#endif
        ResetLeadboard();
    }

    void ResetLeadboard()
    {
        //		transform.position = new Vector3 (0, -40f, 0);
        foreach (LeadboardObject item in playerIconsList)
        {
            Destroy(item.gameObject);
        }
        playerIconsList.Clear();
    }

#if PLAYFAB || GAMESPARKS
    IEnumerator WaitForLeadboard()
    {
        //		NetworkManager.friendsManager.GetLeadboardOnLevel ();
        yield return new WaitForSeconds(0.5f);
        // Debug.Log("player in leadboard " + NetworkManager.leadboardList.Count);
        yield return new WaitUntil(() => NetworkManager.leadboardList.Count > 0);
        // print(NetworkManager.leadboardList.Count);
        if (FB.IsLoggedIn)
        {
            GetComponent<Image>().enabled = true;
            ShowLeadboard();
        }
    }

    void ShowLeadboard()
    {
        //		GetComponent<Animation> ().Play ();
        Vector2 leftPosition = new Vector2(-270f, -12f);

        float width = 270;

        NetworkManager.leadboardList.Sort(CompareByScore);
        int i = 0;

        foreach (var item in NetworkManager.leadboardList)
        {
            if (item.score <= 0)
                continue;
            GameObject gm = Instantiate(playerIconPrefab) as GameObject;
            LeadboardObject lo = gm.GetComponent<LeadboardObject>();
            item.position = i + 1;
            lo.PlayerData = item;
            playerIconsList.Add(lo);
            gm.transform.SetParent(transform);
            gm.transform.localScale = Vector3.one;
            gm.GetComponent<RectTransform>().anchoredPosition = leftPosition + Vector2.right * (width * i);
            i++;
            Debug.Log(item.score);
        }
    }


    private int CompareByScore(LeadboardPlayerData x, LeadboardPlayerData y)
    {
        int retval = y.score.CompareTo(x.score);

        if (retval != 0)
        {
            return retval;
        }
        else
        {
            return y.score.CompareTo(x.score);
        }
    }
#endif
}
