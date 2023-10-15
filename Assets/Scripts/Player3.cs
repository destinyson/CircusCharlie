using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player3 : Player
{
    // 奖励相关
    public int singleBallScore;     // 跳一个皮球得分
    public int doubleBallScore;     // 跳多个皮球得分

    private int passBallCount;      // 跳过皮球数量

    protected override void Awake()
    {
        base.Awake();

        // 获取玩家动画组件
        charlieAnim = GetComponent<Animator>();

        // 跳过皮球数量初始化为0
        passBallCount = 0;
    }

    protected override void Update()
    {
        base.Update();

        // 玩家得分规则设定
        if (state != GlobalArg.playerState.die && state != GlobalArg.playerState.win && state != GlobalArg.playerState.drop)
        {
            if (!startJump && isGround)
            {
                if (passBallCount > 0)
                {
                    if (passBallCount == 1)
                        addScore(singleBallScore, false, false);
                    else
                        addScore(doubleBallScore, true, true);
                    passBallCount = 0;
                }
            }
        }
    }

    protected override void OnCollisionStay2D(Collision2D collision)
    {
        base.OnCollisionStay2D(collision);

        if (collision.collider.tag == "Platform" && collision.contacts[0].normal.y == 1)
        {
            // 根据玩家方向设定皮球状态
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
        base.OnCollisionExit2D(collision);

        if (collision.collider.tag == "Platform")
        {
            // 根据玩家方向设定皮球状态，如果dir == 0，说明是垂直起跳，皮球无需改变状态
            if (dir > 0)
                collision.gameObject.GetComponent<Ball>().state = Ball.ballState.backrun;
            else if (dir < 0)
                collision.gameObject.GetComponent<Ball>().state = Ball.ballState.forrun;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 统计玩家跳过皮球个数
        if (collision.tag == "Platform")
            ++passBallCount;
    }
}
