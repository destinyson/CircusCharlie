using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player3 : Player
{
    public int singleBallScore;
    public int doubleBallScore;

    private int passBallCount;

    protected override void Awake()
    {
        base.Awake();
        charlieAnim = GetComponent<Animator>();

        passBallCount = 0;
    }

    protected override void Update()
    {
        base.Update();

        if (state != GlobalArg.playerState.die)
        {
            if (state == GlobalArg.playerState.win || (!startJump && isGround))
            {
                if (passBallCount > 0)
                {
                    if (passBallCount == 1)
                        addScore(singleBallScore, false, transform.position);
                    else
                        addScore(doubleBallScore, true, new Vector3(transform.position.x, transform.position.y + 1));
                    passBallCount = 0;
                }
            }
        }
    }

    protected override void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.tag == "Win" && collision.contacts[0].normal.y == 1)
            state = GlobalArg.playerState.win;
        else if (collision.collider.tag == "Ground")
            state = GlobalArg.playerState.die;
        else if (collision.collider.tag == "Ball" && collision.contacts[0].normal.y == 1)
        {
            isGround = true;
            if (!startJump && isGround)
            {
                
                if (dir == 0)
                    collision.gameObject.GetComponent<Ball>().state = Ball.ballState.stand;
                else if (dir > 0)
                    collision.gameObject.GetComponent<Ball>().state = Ball.ballState.forward;
                else
                    collision.gameObject.GetComponent<Ball>().state = Ball.ballState.backward;
            }
        }
    }

    protected override void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ball")
        {
            isGround = false;

            if (dir > 0)
                collision.gameObject.GetComponent<Ball>().state = Ball.ballState.backrun;
            else if (dir < 0)
                collision.gameObject.GetComponent<Ball>().state = Ball.ballState.forrun;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ball")
            ++passBallCount;
    }
}
