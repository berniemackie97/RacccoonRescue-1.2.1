using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightContainerLamp : MonoBehaviour
{
    public GameObject lightningPrefab;
    public ParticleSystem ps;
    public delegate void OnFinishedEffect();
    public static OnFinishedEffect OnFinished;
    Transform catapult;
    void Start()
    {
        catapult = GameObject.Find("boxCatapult").transform;
        ps = GetComponentInChildren<ParticleSystem>();
    }
    // Use this for initialization
    void OnEnable()
    {
        // GameObject obj= Instantiate(lightningPrefab, transform.position, Quaternion.Euler(0,0,0));
        // obj.transform.SetParent(transform);
        // obj.transform.localScale = Vector3.one;
        // obj.transform.localRotation = Quaternion.Euler(0,0,0);
        // SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.lightning);

        // Invoke("Hide", 1.5f);
    }

    void Update()
    {
        Vector3 targetPos = catapult.position - transform.position;
        transform.rotation = Quaternion.LookRotation(targetPos, Vector3.back);
        if (ps.isStopped)
            Hide();
    }

    void Hide()
    {
        //OnFinished();
        gameObject.SetActive(false);
    }

}
