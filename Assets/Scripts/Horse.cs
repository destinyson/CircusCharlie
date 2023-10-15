using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Horse : MonoBehaviour
{
    public GameObject player;
    public float playerHorseDis;

    public enum horseState { slow, normal, fast };
    public horseState state;
    public float runSpeed;

    private void Awake()
    {
        state = horseState.normal;
    }

    void Update()
    {
        switch (state)
        {
            case horseState.slow: GetComponent<Animator>().SetInteger("h", -1); break;
            case horseState.normal: GetComponent<Animator>().SetInteger("h", 0); break;
            case horseState.fast: GetComponent<Animator>().SetInteger("h", 1); break;
            default: break;
        }

        if (player.GetComponent<Player4>().follow && player.transform.position.x < player.GetComponent<Player>().maxPosX)
            transform.position = new Vector3(player.transform.position.x + playerHorseDis, transform.position.y);
           
        else
            transform.Translate(Vector2.right * runSpeed * Time.deltaTime);
    }
}
