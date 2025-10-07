using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackBox : MonoBehaviour
{
    private Animator anim;
    private int clownCount = 0;
    public int MAX_CLOWN = 10;
    private int totalClownCount = 0;
    public Transform clown;
    private RectTransform goalUI;


    public GameObject clownTarget;

    public static event Action<int> onClownSpawned;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    public void Initialize(RectTransform ui)
    {
        goalUI = ui;
    }
    public void MoveClownToGoal()
    {
        Vector3 screenPos = goalUI.position;
        Vector3 goalWorldPos = Camera.main.ScreenToWorldPoint(screenPos);
        goalWorldPos.z = 0;

        StartCoroutine(MoveClown(clown, goalWorldPos, 2f));
    }

    IEnumerator MoveClownAfterAnim()
    {
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

        anim.enabled = false;

        MoveClownToGoal();
    }
    IEnumerator MoveClown(Transform target, Vector3 goal, float time)
    {
        Vector3 start = target.position;
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime / time;
            target.position = Vector3.Lerp(start, goal, t);
            
            if(t >= 1 - (0.4f / time))
            {
                clownTarget.SetActive(false);
            }


            yield return null;
        }
        target.position = goal;
    }
    public void OnNearbyMatch()
    {
        clownCount = 0;
        if(totalClownCount < MAX_CLOWN)
        {
            clownCount++;
            totalClownCount += clownCount;
            onClownSpawned?.Invoke(clownCount);

            if(!anim.enabled)
                anim.enabled = true;

            anim.SetTrigger("SpawnClown");
            StartCoroutine(MoveClownAfterAnim());
        }
    }
}
