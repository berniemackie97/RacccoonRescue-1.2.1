using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeginLeafAnimation : MonoBehaviour
{

    bool finished;


    public void OnFinishedAnimation()
    {
        finished = true;
    }

    public void OnFinished(System.Action callback)
    {
        StartCoroutine(OnFinishedCoroutine(callback));
    }

    IEnumerator OnFinishedCoroutine(System.Action callback)
    {
        while (!finished)
        {
            yield return new WaitForFixedUpdate();
        }
        callback();
    }
}
