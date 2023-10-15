using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player1 : Player
{
    // ʨ�Ӷ���
    private Animator lionAnim;

    // ��Ծ�������
    public int potScore;            // ��Ծ�������
    public int ringScore;           // ��Ծ��Ȧ����
    public int potSingleRingScore;  // ����͵�����Ȧһ��������
    public int potDoubleRingScore;  // �����������Ȧһ��������

    private bool passPot;           // �Ƿ���Ծ����
    private int passRing;           // �Ƿ���Ծ��Ȧ

    // �������
    public int moneyScore;          // Ǯ������
    public int coinScore;           // ��ҷ���
    public int coinCount;           // ���������Ծ����
    public float coinInitPosY;      // ��ҳ�ʼ������
    public GameObject coinPrefab;   // ���Ԥ����

    private bool hasCoin;           // �Ƿ��Ѿ����ɹ����
    private GameObject coin;        // ���ʵ��

    protected override void Awake()
    {
        // ���ø���Awake����
        base.Awake();

        // �ؿ�״̬��ʼ��
        passPot = false;
        passRing = 0;

        // ������ʨ�Ӷ���ָ��
        charlieAnim = GameObject.Find("charlie").GetComponent<Animator>();
        lionAnim = GameObject.Find("lion").GetComponent<Animator>();

        hasCoin = false;
    }

    protected override void Update()
    {
        base.Update();

        // �����ң���������һ���߶Ⱦ�����
        if (coin != null && coin.transform.position.y < coinInitPosY)
        {
            Destroy(coin);
            coin = null;
        }

        // ������/��Ȧ�÷ִ���
        if (state != GlobalArg.playerState.die && state != GlobalArg.playerState.win && state != GlobalArg.playerState.drop)
        {
            if (!startJump && isGround)
            {
                if (!passPot)
                {
                    if (passRing > 0)
                        addScore(ringScore * (passRing >> 1), false, false);
                }

                else
                {
                    switch (passRing)
                    {
                        case 0: addScore(potScore, false, false); break;
                        case 2: addScore(potSingleRingScore, false, false); break;
                        case 4: addScore(potDoubleRingScore, false, true); break;
                        default: break;
                    }
                }

                passPot = false;
                passRing = 0;
            }
        }
    }

    // stand���������ø���ͬ��������������ʨ�Ӷ���������forward��backward��jump��win��die����ͬ��
    protected override void stand()
    {
        base.stand();

        lionAnim.SetBool("jump", false);
        lionAnim.SetInteger("h", 0);
    }

    protected override void forward()
    {
        base.forward();

        lionAnim.SetBool("jump", false);
        lionAnim.SetInteger("h", 1);
    }

    protected override void backward()
    {
        base.backward();

        lionAnim.SetBool("jump", false);
        lionAnim.SetInteger("h", -1);
    }

    protected override void jump()
    {
        base.jump();

        lionAnim.SetBool("jump", true);
        lionAnim.SetInteger("h", 0);
    }

    protected override void die()
    {
        base.die();

        GetComponent<Rigidbody2D>().gravityScale = 0;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        if (GlobalArg.time > 0)
            lionAnim.SetTrigger("die");
    }

    protected override void win()
    {
        base.win();

        lionAnim.SetBool("jump", false);
        lionAnim.SetInteger("h", 0);
    }

    protected override void OnCollisionStay2D(Collision2D collision)
    {
        base.OnCollisionStay2D(collision);

        // �����Ҵ����ж�����
        if (collision.collider.tag == "Fire")
            state = GlobalArg.playerState.die;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ǯ�����Ҽӷִ���
        if (collision.tag == "Bonus")
        {
            Destroy(collision.gameObject);
            addScore(moneyScore, true, true);
        }

        if (collision.tag == "Coin")
        {
            Destroy(collision.gameObject);
            addScore(coinScore, true, true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // �����败��
        if (collision.tag == "Pot")
        {
            passPot = true;
            // ���������δ���ֽ�ң�ͳ�ƻ��豻������
            if (!hasCoin)
            {
                collision.SendMessage("addCount");
                // ������豻�������ﵽ��ֵ�����ɽ�ң�����Ǳ��������ɽ��
                if (collision.gameObject.GetComponent<FirePot>().getCount() >= coinCount)
                {
                    GameObject coin = Instantiate(coinPrefab, collision.gameObject.transform);
                    hasCoin = true;
                }
            }  
        }
        // ����Ȧ����
        if (collision.tag == "Ring")
            ++passRing;
    }
}
