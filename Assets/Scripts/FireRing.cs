using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRing : MonoBehaviour
{
    public float speed;

    void Update()
    {
        if (!GlobalArg.isPlayerDie)
            transform.Translate(Vector2.left * speed * Time.deltaTime);
    }

    public void stop()
    {
        transform.Find("left").GetComponent<Animator>().speed = 0;
        transform.Find("right").GetComponent<Animator>().speed = 0;
        transform.Find("bottom").GetComponent<Animator>().speed = 0;
    }
}
