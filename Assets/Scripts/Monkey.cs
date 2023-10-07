using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Monkey : MonoBehaviour
{
    public float speed;
    public Vector2 jumpForce;
    public bool isBonus;

    private bool startJump;
    private bool isGround;

    private void Start()
    {
        startJump = false;
        isGround = true;

        if (isBonus)
            Invoke("jump", 1.2f);
    }

    void Update()
    {
        if (isBonus)
        {
            if (startJump)
            {
                if (!isGround)
                    startJump = false;
            }

            else if (isGround)
                GetComponent<Animator>().SetBool("jump", false);
        }
        
        transform.Translate(Vector2.left * speed * Time.deltaTime);

    }

    void jump()
    {
        startJump = true;
        GetComponent<Rigidbody2D>().AddForce(jumpForce);

        GetComponent<Animator>().SetBool("jump", true);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground")
            isGround = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground")
            isGround = false;
    }
}
