using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player3 : Player
{
    // �������
    public int singleBallScore;     // ��һ��Ƥ��÷�
    public int doubleBallScore;     // �����Ƥ��÷�

    private int passBallCount;      // ����Ƥ������

    protected override void Awake()
    {
        base.Awake();

        // ��ȡ��Ҷ������
        charlieAnim = GetComponent<Animator>();

        // ����Ƥ��������ʼ��Ϊ0
        passBallCount = 0;
    }

    protected override void Update()
    {
        base.Update();

        // ��ҵ÷ֹ����趨
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
            // ������ҷ����趨Ƥ��״̬
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
            // ������ҷ����趨Ƥ��״̬�����dir == 0��˵���Ǵ�ֱ������Ƥ������ı�״̬
            if (dir > 0)
                collision.gameObject.GetComponent<Ball>().state = Ball.ballState.backrun;
            else if (dir < 0)
                collision.gameObject.GetComponent<Ball>().state = Ball.ballState.forrun;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // ͳ���������Ƥ�����
        if (collision.tag == "Platform")
            ++passBallCount;
    }
}
