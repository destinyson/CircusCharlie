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
    public int doubleScore;         // ����һ��������

    private bool passPot;           // �Ƿ���Ծ����
    private bool passRing;          // �Ƿ���Ծ��Ȧ

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
        passRing = false;

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
        if (state != GlobalArg.playerState.die)
        {
            if (state == GlobalArg.playerState.win || (!startJump && isGround))
            {
                if (passPot && passRing)
                    addScore(doubleScore, false, new Vector3(0, 0));
                else if (passPot)
                    addScore(potScore, false, new Vector3(0, 0));
                else if (passRing)
                    addScore(ringScore, false, new Vector3(0, 0));

                passPot = false;
                passRing = false;
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

        if (collision.collider.tag == "Fire")
            state = GlobalArg.playerState.die;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Bonus")
        {
            Destroy(collision.gameObject);
            addScore(500, true, collision.gameObject.transform.position);
        }

        if (collision.tag == "Coin")
        {
            Destroy(collision.gameObject);
            addScore(5000, true, collision.gameObject.transform.position);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Pot")
        {
            passPot = true;
            if (!hasCoin)
            {
                collision.SendMessage("addCount");
                if (collision.gameObject.GetComponent<FirePot>().getCount() >= coinCount)
                {
                    GameObject coin = Instantiate(coinPrefab, collision.gameObject.transform);
                    hasCoin = true;
                }
            }  
        }
            
        if (collision.tag == "Ring")
            passRing = true;
    }
}
