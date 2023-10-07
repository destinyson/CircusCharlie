using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : Player
{
    public float dropSpeed;
    public float dropPosY;
    public float dropDelayTimeVal;

    private float dropDelayTime;

    public int monkeyScore;
    public int bonusMonkeyScore;

    private int passMonkey;
    private int passBonusMonkey;


    protected override void Awake()
    {
        base.Awake();
        charlieAnim = GetComponent<Animator>();

        dropDelayTime = dropDelayTimeVal;
        
        passMonkey = 0;
        passBonusMonkey = 0;
    }

    protected override void Update()
    {
        base.Update();

        if (state != GlobalArg.playerState.die)
        {
            if (state == GlobalArg.playerState.win || (!startJump && isGround))
            {
                if (!(passMonkey == 0 && passBonusMonkey == 0))
                {
                    addScore(monkeyScore * passMonkey, false, transform.position);
                    if (passBonusMonkey > 0)
                        addScore(bonusMonkeyScore * passBonusMonkey, true, new Vector3(transform.position.x, transform.position.y + 1));
                    passMonkey = 0;
                    passBonusMonkey = 0;
                }
            }
        }
    }

    protected override void die()
    {
        base.die();

        if (dropDelayTime > 0)
            dropDelayTime -= GlobalArg.fps;

        else
        {
            GetComponent<Rigidbody2D>().gravityScale = 0;
            gameObject.layer = LayerMask.NameToLayer("Remains");

            if (transform.position.y > dropPosY)
            {
                charlieAnim.SetInteger("h", 0);
                charlieAnim.SetBool("jump", false);
                transform.Translate(Vector2.down * 0.1f);
            }

            else
                charlieAnim.SetTrigger("die");
        }
    }

    protected override void OnCollisionStay2D(Collision2D collision)
    {
        base.OnCollisionStay2D(collision);

        if (collision.collider.tag == "Monkey")
            state = GlobalArg.playerState.die;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Monkey")
        {
            if (collision.gameObject.GetComponent<Monkey>().isBonus)
                ++passBonusMonkey;
            else
                ++passMonkey;
        }
    }
}
