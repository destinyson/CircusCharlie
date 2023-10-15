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
    public int potSingleRingScore;  // 火盆和单个火圈一起跳分数
    public int potDoubleRingScore;  // 火盆和两个火圈一起跳分数

    private bool passPot;           // 是否跳跃火盆
    private int passRing;           // 是否跳跃火圈

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
        passRing = 0;

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

        // 如果玩家触火，判定死亡
        if (collision.collider.tag == "Fire")
            state = GlobalArg.playerState.die;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 钱袋与金币加分触发
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
        // 跳火盆触发
        if (collision.tag == "Pot")
        {
            passPot = true;
            // 如果本局尚未出现金币，统计火盆被跳次数
            if (!hasCoin)
            {
                collision.SendMessage("addCount");
                // 如果火盆被跳次数达到阈值，生成金币，并标记本局已生成金币
                if (collision.gameObject.GetComponent<FirePot>().getCount() >= coinCount)
                {
                    GameObject coin = Instantiate(coinPrefab, collision.gameObject.transform);
                    hasCoin = true;
                }
            }  
        }
        // 跳火圈触发
        if (collision.tag == "Ring")
            ++passRing;
    }
}
