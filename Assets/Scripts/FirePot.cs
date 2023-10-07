using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePot : MonoBehaviour
{
    public float countTimeVal;

    private int passCount;
    private float countTime;

    void Start()
    {
        passCount = 0;
        countTime = countTimeVal;
    }

    void Update()
    {
        if (countTime <= 0)
        {
            passCount = 0;
            countTime = countTimeVal;
        }
            
        else
            countTime -= Time.deltaTime;
    }

    public void addCount()
    {
        ++passCount;
        countTime = countTimeVal;
    }

    public int getCount()
    {
        return passCount;
    }
}
