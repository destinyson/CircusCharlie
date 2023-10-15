using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : Player
{
    // 死亡掉落相关
    public float dropDelayTimeVal;      // 掉落延迟时间阈值

    // 奖励相关
    public int monkeyScore;             // 普通猴子得分
    public int bonusMonkeyScore;        // 奖励猴子得分

    private int passMonkey;             // 跳过普通猴子数量
    private int passBonusMonkey;        // 跳过奖励猴子数量


    protected override void Awake()
    {
        base.Awake();

        // 获取玩家动画组件
        charlieAnim = GetComponent<Animator>();
        
        // 跳过普通猴子和奖励猴子数量初始化为0
        passMonkey = 0;
        passBonusMonkey = 0;
    }

    protected override void Update()
    {
        base.Update();

        // 玩家得分规则设定
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

        // 改变玩家为站立动画
        charlieAnim.SetInteger("h", 0);
        charlieAnim.SetBool("jump", false);

        // 取消玩家重力，速度设为0，使其静止
        GetComponent<Rigidbody2D>().gravityScale = 0;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        // 延迟一段时间后掉落
        Invoke("down", dropDelayTimeVal);
    }

    private void down()
    {
        // 玩家掉落
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
        // 统计玩家跳过猴子数量
        if (collision.tag == "Monkey")
        {
            if (collision.gameObject.GetComponent<Monkey>().isBonus)
                ++passBonusMonkey;
            else
                ++passMonkey;
        }
    }
}
