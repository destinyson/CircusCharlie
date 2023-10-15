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

    private void Awake()
    {
        startJump = false;
        isGround = true;
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
        
        if (!GlobalArg.isPlayerDrop && !GlobalArg.isPlayerDie)
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
        if (collision.collider.tag == "Platform")
            isGround = true;
        if (collision.collider.tag == "Monkey")
        {
            if (isBonus && isGround && !startJump)
                jump();
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Platform")
            isGround = false;
    }
}
