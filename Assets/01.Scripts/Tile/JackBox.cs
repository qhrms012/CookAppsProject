using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackBox : MonoBehaviour
{
    private Animator anim;
    private Camera mainCam;
    private int clownCount = 0;
    public int MAX_CLOWN = 100;

    public Transform clown;
    private RectTransform goalUI;

    private void Awake()
    {
        mainCam = Camera.main;
        anim = GetComponent<Animator>();
    }

    public void Initialize(RectTransform ui)
    {
        goalUI = ui;
    }
    public void MoveClownToGoal()
    {
        Vector3 goalWorldPos = Camera.main.ScreenToWorldPoint(goalUI.position);
        goalWorldPos.z = 0;

        StartCoroutine(MoveClown(clown, goalWorldPos, 1f));
    }
    IEnumerator MoveClown(Transform target, Vector3 goal, float time)
    {
        Vector3 start = target.position;
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime / time;
            target.position = Vector3.Lerp(start, goal, t);
            yield return null;
        }
        target.position = goal;
    }
    public void OnNearbyMatch()
    {
        if(clownCount < MAX_CLOWN)
        {
            clownCount++;
            anim.SetTrigger("SpawnClown");
        }
    }
}
