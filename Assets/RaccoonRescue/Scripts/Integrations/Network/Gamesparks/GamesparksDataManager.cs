using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Reflection;


#if GAMESPARKS
using GameSparks.Core;

public class GamesparksDataManager : IDataManager
{


    public static int LatestReachedLevel = 0;
    static int LevelScoreCurrentRecord = 0;

    #region Scores

    public void SetPlayerScore(int level, int score)
    {
        new GameSparks.Api.Requests.LogEventRequest()
            .SetEventKey("ScoreLevel")
            .SetEventAttribute("Level", level).SetEventAttribute("Score", score).Send((response) =>
            {
                if (!response.HasErrors)
                {
                    Debug.Log("Score Saved To GameSparks...");
                }
                else
                {
                    Debug.Log("Error Saving Score Data...");
                }
            });

    }

    public void GetPlayerScore(Action<int> Callback)
    {
        new GameSparks.Api.Requests.LeaderboardsEntriesRequest()
            .SetLeaderboards(new List<string> { "LB.Level." + PlayerPrefs.GetInt("OpenLevel") })
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    Debug.Log("Found Score Data...");
                    //				ShowFields (ref response);
                    //				Debug.Log (response.JSONData ["LB.Level." + mainscript.Instance.currentLevel]);
                    if (!response.JSONData.ContainsKey("LB.Level." + PlayerPrefs.GetInt("OpenLevel")))
                        return;
                    List<object> list = (List<object>)response.JSONData["LB.Level." + PlayerPrefs.GetInt("OpenLevel")];
                    if (list.Count > 0)
                    {
                        Dictionary<string, object> dic = (Dictionary<string, object>)list[0];
                        int score = 0;
                        foreach (var entry in dic)
                        {
                            object obj = null;
                            dic.TryGetValue("Score", out obj);

                            //					if (entry.Key == "LB.Level." + mainscript.Instance.currentLevel) {
                            //						var list = (Dictionary<string,object>)((List<object>)entry.Value) [0];
                            //						foreach (var item in list) {
                            //							if (item.Key == "Score") {
                            score = int.Parse(obj.ToString());
                            Callback(score);
                            //						Debug.Log (" Score:" + score);
                            //
                            //							}
                            //						}
                            //					}
                        }
                    }
                }
                else
                {
                    Debug.Log("Error Retrieving Score Data...");
                }
            });

    }

    #endregion

    #region Level

    public void SetPlayerLevel(int level)
    {
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("SetReachedLevel")
            .SetEventAttribute("Level", level).Send((response) =>
            {
                if (!response.HasErrors)
                {
                    Debug.Log("Level Saved To GameSparks...");
                }
                else
                {
                    Debug.Log("Error Saving Level Data...");

                }
            });
    }

    public void GetPlayerLevel(Action<int> Callback)
    {
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("GetReachedLevel").Send((response) =>
        {
            if (!response.HasErrors)
            {
                GSData data = response.ScriptData.GetGSData("player_Data");
                if (data != null)
                {
                    Callback(int.Parse(data.GetInt("level").ToString()));
                }
                else
                {
                    Callback(0);//1.2
                }
            }
            else
            {
                Debug.Log("Error Loading Level Data...");
            }
        });
    }

    #endregion

    #region Stars

    public void SetStars(int Stars, int Level)
    {
        Debug.Log("set stars " + " " + Stars + " level " + Level);
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("SetStars")
            .SetEventAttribute("Level", Level).SetEventAttribute("Stars", Stars).Send((response) =>
            {
                if (!response.HasErrors)
                {
                    Debug.Log("Stars Saved To GameSparks...");
                }
                else
                {
                    Debug.Log("Error Saving Stars Data...");
                }
            });

    }

    public void GetStars(Action<Dictionary<string, int>> Callback)
    {

        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("GetStars").Send((response) =>
        {

            if (!response.HasErrors)
            {
                Debug.Log("Found Stars Data...");
                Dictionary<string, int> starsDic = new Dictionary<string, int>();
                var cursor = response.ScriptData.GetGSDataList("stars_data");
                foreach (var item in cursor)
                {
                    string level = item.GetString("Level");
                    string stars = item.GetInt("Stars").ToString();
                    //					Debug.Log ("level: " + level + "; stars: " + stars);
                    starsDic.Add(level, int.Parse(stars));
                    Callback(starsDic);
                }
            }
            else
            {
                Debug.Log("Error Retrieving Stars Data...");
            }
        });

    }

    public void SetTotalStars()
    {
    }

    #endregion

    #region Boosters

    public void SetBoosterData(Dictionary<string, string> dic)
    {
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("SetBoosts").SetEventAttribute("Boosts", GSJson.To(dic)).Send((response) =>
        {
            if (!response.HasErrors)
            {
                Debug.Log("Boosts Saved To GameSparks...");
            }
            else
            {
                Debug.Log("Error Saving Boosts Data...");
            }
        });


    }

    public void GetBoosterData(Action<Dictionary<string, int>> Callback)
    {
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("GetBoosts").Send((response) =>
        {

            if (!response.HasErrors)
            {
                Debug.Log("Getting boosts");
                Dictionary<string, int> dicBoost = new Dictionary<string, int>();
                var cursor = response.ScriptData.GetGSData("boosts_Data");
                if (cursor == null)
                    return;

                foreach (var item in cursor.BaseData)
                {
                    if (item.Key == "Boosts")
                    {
                        //						Debug.Log (item.Value + " " + item.Value.GetType ());
                        //						Debug.Log (GSJson.From (item.Value.ToString ()));
                        var j = (Dictionary<string, object>)GSJson.From(item.Value.ToString());
                        foreach (var item2 in j)
                        {
                            dicBoost.Add(item2.Key, int.Parse(item2.Value.ToString()));
                        }
                    }
                    Callback(dicBoost);
                }
            }
            else
            {
                Debug.Log("Error Retrieving Boosts Data...");
            }
        });

    }

    #endregion

    public void Logout()
    {

    }


    void ShowFields<T>(ref T obj)
    {
        foreach (PropertyInfo prop in typeof(T).GetProperties())
        {
            Debug.Log(string.Format("{0} = {1}", prop.Name, prop.GetValue(obj, null)));
        }
        foreach (FieldInfo prop in typeof(T).GetFields())
        {
            Debug.Log(string.Format("{0} = {1}", prop.Name, prop.GetValue(obj)));
        }

    }


}
#endif
