using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player1 : Player
{
    // 狮子动画
    private Animator lionAnim;

    // 跳跃物体相关
    public int potScore;            // 跳跃火盆分数
    public int ringScore;           // 跳跃火圈分数
    public int doubleScore;         // 两个一起跳分数

    private bool passPot;           // 是否跳跃火盆
    private bool passRing;          // 是否跳跃火圈

    // 奖励相关
    public int moneyScore;          // 钱袋分数
    public int coinScore;           // 金币分数
    public int coinCount;           // 金币所需跳跃次数
    public float coinInitPosY;      // 金币初始纵坐标
    public GameObject coinPrefab;   // 金币预制体

    private bool hasCoin;           // 是否已经生成过金币
    private GameObject coin;        // 金币实体

    protected override void Awake()
    {
        // 调用父类Awake方法
        base.Awake();

        // 关卡状态初始化
        passPot = false;
        passRing = false;

        // 查理与狮子动画指定
        charlieAnim = GameObject.Find("charlie").GetComponent<Animator>();
        lionAnim = GameObject.Find("lion").GetComponent<Animator>();

        hasCoin = false;
    }

    protected override void Update()
    {
        base.Update();

        // 处理金币，如果掉落回一定高度就销毁
        if (coin != null && coin.transform.position.y < coinInitPosY)
        {
            Destroy(coin);
            coin = null;
        }

        // 跳火盆/火圈得分处理
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

    // stand方法，调用父类同名方法，随后更新狮子动画，下面forward、backward、jump、win和die方法同理
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
