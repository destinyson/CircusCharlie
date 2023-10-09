using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public float scrollSpeed;
    public float moveSpeed;

    public enum ballState { scroll, stand, forward, backward, backrun, forrun };
    public ballState state;
    public int dir;

    private GameObject player;

    void Awake()
    {
        state = ballState.scroll;

        player = GameObject.Find("player");
        dir = -1;
    }
    
    void Update()
    {
        switch (state)
        {
            case ballState.stand: dir = 0; transform.position = new Vector3(player.transform.position.x, transform.position.y); break;
            case ballState.forward: dir = 1; transform.position = new Vector3(player.transform.position.x, transform.position.y); break;
            case ballState.backward: dir = -1; transform.position = new Vector3(player.transform.position.x, transform.position.y); break;
            case ballState.scroll: dir = -1; transform.Translate(Vector2.left * scrollSpeed * Time.deltaTime); break;
            case ballState.backrun: dir = -1; transform.Translate(Vector2.left * moveSpeed * Time.deltaTime); break;
            case ballState.forrun: dir = 1; transform.Translate(Vector2.right * moveSpeed * Time.deltaTime); break;
            default: break;
        }
        
        switch (state)
        {
            case ballState.stand:
                {
                    GetComponent<Animator>().SetTrigger("step");
                    GetComponent<Animator>().SetInteger("h", 0);
                    break;
                }
            case ballState.forward:
                {
                    GetComponent<Animator>().SetTrigger("step");
                    GetComponent<Animator>().SetInteger("h", 1);
                    break;
                }
            case ballState.backward:
                {
                    GetComponent<Animator>().SetTrigger("step");
                    GetComponent<Animator>().SetInteger("h", -1);
                    break;
                }
            case ballState.backrun:
                {
                    GetComponent<Animator>().SetTrigger("kick");
                    GetComponent<Animator>().SetInteger("h", -1);
                    gameObject.layer = LayerMask.NameToLayer("Remains");
                    break;
                }
            case ballState.forrun:
                {
                    GetComponent<Animator>().SetTrigger("kick");
                    GetComponent<Animator>().SetInteger("h", 1);
                    gameObject.layer = LayerMask.NameToLayer("Remains");
                    break;
                }
            default: break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ball" && gameObject.layer != LayerMask.NameToLayer("Remains") &&
            collision.gameObject.transform.parent.gameObject.layer != LayerMask.NameToLayer("Remains"))
        {
            if (dir >= 0)
            {
                state = ballState.backrun;
                collision.gameObject.transform.parent.gameObject.GetComponent<Ball>().state = ballState.forrun;
            }
            else
            {
                state = ballState.forrun;
                collision.gameObject.transform.parent.gameObject.GetComponent<Ball>().state = ballState.backrun;
            }

            gameObject.layer = LayerMask.NameToLayer("Remains");
            collision.gameObject.transform.parent.gameObject.layer = LayerMask.NameToLayer("Remains");
        }
    }
}
