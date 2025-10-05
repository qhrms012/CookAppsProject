using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackBox : MonoBehaviour
{
    private Animator anim;
    private int clownCount = 0;
    private const int MAX_CLOWN = 100;

    private void Awake()
    {
        anim = GetComponent<Animator>();
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
