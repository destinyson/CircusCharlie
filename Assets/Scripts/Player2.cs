using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : Player
{
    // �����������
    public float dropDelayTimeVal;      // �����ӳ�ʱ����ֵ

    // �������
    public int monkeyScore;             // ��ͨ���ӵ÷�
    public int bonusMonkeyScore;        // �������ӵ÷�

    private int passMonkey;             // ������ͨ��������
    private int passBonusMonkey;        // ����������������


    protected override void Awake()
    {
        base.Awake();

        // ��ȡ��Ҷ������
        charlieAnim = GetComponent<Animator>();
        
        // ������ͨ���Ӻͽ�������������ʼ��Ϊ0
        passMonkey = 0;
        passBonusMonkey = 0;
    }

    protected override void Update()
    {
        base.Update();

        // ��ҵ÷ֹ����趨
        if (state != GlobalArg.playerState.die && state != GlobalArg.playerState.win && state != GlobalArg.playerState.drop)
        {
            if (!startJump && isGround)
            {
                if (!(passMonkey == 0 && passBonusMonkey == 0))
                {
                    addScore(monkeyScore * passMonkey, false, false);
                    if (passBonusMonkey > 0)
                        addScore(bonusMonkeyScore * passBonusMonkey, true, true);
                    passMonkey = 0;
                    passBonusMonkey = 0;
                }
            }
        }
    }

    protected override void drop()
    {
        GlobalArg.isPlayerDrop = true;

        // �ı����Ϊվ������
        charlieAnim.SetInteger("h", 0);
        charlieAnim.SetBool("jump", false);

        // ȡ������������ٶ���Ϊ0��ʹ�侲ֹ
        GetComponent<Rigidbody2D>().gravityScale = 0;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        // �ӳ�һ��ʱ������
        Invoke("down", dropDelayTimeVal);
    }

    private void down()
    {
        // ��ҵ���
        gameObject.layer = LayerMask.NameToLayer("Remains");
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, -dropSpeed);
    }

    protected override void OnCollisionStay2D(Collision2D collision)
    {
        base.OnCollisionStay2D(collision);

        if (collision.collider.tag == "Monkey")
            state = GlobalArg.playerState.drop;
        else if (collision.collider.tag == "Ground")
            state = GlobalArg.playerState.die;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // ͳ�����������������
        if (collision.tag == "Monkey")
        {
            if (collision.gameObject.GetComponent<Monkey>().isBonus)
                ++passBonusMonkey;
            else
                ++passMonkey;
        }
    }
}
