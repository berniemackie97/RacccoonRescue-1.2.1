using UnityEngine;
using System.Collections;
using InitScriptName;

#if GAMESPARKS
using GameSparks.Core;

public class GamesparksCurrencyManager : ICurrencyManager
{



    #region ICurrencyManager implementation

    public void IncBalance(int amount)
    {
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("AddCurrency")
            .SetEventAttribute("Value", amount).Send((response) =>
            {
                if (!response.HasErrors)
                {
                    Debug.Log("Currency Saved To GameSparks...");
                }
                else
                {
                    SetBalance(InitScript.Gems);
                }
            });

    }

    public void DecBalance(int amount)
    {
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("SpendCurrency")
            .SetEventAttribute("Value", amount).Send((response) =>
            {
                if (!response.HasErrors)
                {
                    Debug.Log("Currency Saved To GameSparks...");
                }
                else
                {
                    Debug.Log("Error Saving Currency Data...");
                }
            });

    }

    public void SetBalance(int newbalance)
    {
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("SetCurrency")
            .SetEventAttribute("Value", newbalance).Send((response) =>
            {
                if (!response.HasErrors)
                {
                    Debug.Log("Currency Saved To GameSparks...");
                }
                else
                {
                    Debug.Log("Error Saving Currency Data...");
                }
            });

    }

    public void GetBalance(System.Action<int> Callback)
    {
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("GetCurrency")
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    GSData data = response.ScriptData.GetGSData("currency_Data");
                    if (data != null)
                    {
                        Callback(int.Parse(data.GetInt("Value").ToString()));
                        Debug.Log("Currency Received From GameSparks...");
                    }
                }
                else
                {
                    Debug.Log("Error Getting Currency Data...");
                }
            });

    }




    #endregion



}
#endif