using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//1.1
public class PriceLocalisation : MonoBehaviour
{
    public Text[] prices;


    // Update is called once per frame
    void Update()
    {
#if UNITY_INAPPS
        if (UnityInAppsIntegration.m_StoreController == null) return;
        for (int i = 0; i < prices.Length; i++)
        {
            if (UnityInAppsIntegration.m_StoreController.products.WithID(LevelEditorBase.THIS.InAppIDs[i]).metadata.localizedPrice > new decimal(0.01))
                prices[i].text = UnityInAppsIntegration.m_StoreController.products.WithID(LevelEditorBase.THIS.InAppIDs[i]).metadata.localizedPriceString;
        }
#endif
    }
}
