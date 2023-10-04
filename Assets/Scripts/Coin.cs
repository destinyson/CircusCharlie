using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Coin : MonoBehaviour
{
    public AudioClip coinClip;
    public Vector2 force;
    void Start()
    {
        AudioSource.PlayClipAtPoint(coinClip, transform.position);
        GetComponent<Rigidbody2D>().AddForce(force);
    }

    void FixedUpdate()
    {
        transform.Rotate(new Vector3(0, 0, 10));
    }
}
