using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePot : MonoBehaviour
{
    public int coinCount;
    public float countTimeVal;
    public GameObject coinPrefab;

    private int passCount;
    private float countTime;
    private GameObject coin;

    void Start()
    {
        passCount = 0;
        countTime = countTimeVal;
        coin = null;
    }

    void Update()
    {
        if (coin != null && coin.transform.position.y < transform.position.y)
        {
            Destroy(coin);
            coin = null;
        }

        if (passCount == coinCount)
        {
            coin = Instantiate(coinPrefab, transform);
            passCount = 0;
            coinCount += 4;
        }

        else if (countTime <= 0)
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

    public void resetCount()
    {
        passCount = 0;
    }
}
