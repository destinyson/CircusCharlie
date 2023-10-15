using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player4 : Player
{
    // 玩家动作
    public float slowSpeedScale;            // 玩家减速速度倍数
    public float fastSpeedScale;            // 玩家加速速度倍数
    public float slowPosX;                  // 玩家减速位置，最后10m处会强行减速

    public bool follow;                     // 玩家是否需要马跟随
    public float springHeight;              // 玩家跳过的弹簧高度
    public Vector2 elasticity;              // 弹簧弹力

    public GameObject horse;                // 马实体

    // 玩家动画
    public AnimationClip lowJump;                               // 矮弹簧跳跃动画
    public AnimationClip highJump;                              // 高弹簧跳跃动画
    public AnimationClip dropJump;                              // 回到马上/掉落地面跳跃动画
    private AnimatorOverrideController overrideController;      // 玩家动画控制器

    public AudioClip springClip;            // 弹簧音效

    // 奖励设置
    public int initScore;                   // 初始分数
    private int score;                      // 实际分数

    protected override void Awake()
    {
        base.Awake();

        charlieAnim = GetComponent<Animator>();

        overrideController = new AnimatorOverrideController(charlieAnim.runtimeAnimatorController);
        charlieAnim.runtimeAnimatorController = overrideController;

        score = initScore;
    }

    protected override void Update()
    {
        base.Update();

        if (state == GlobalArg.playerState.win)
            follow = false;

        else if (state != GlobalArg.playerState.die && state != GlobalArg.playerState.drop)
        {
            if (!startJump)
            {
                // 此处，方向键不在控制方向，而是控制速度，因此与基类方法处理有区别
                if (transform.position.x >= slowPosX)
                    dir = -1;
                else
                    dir = Input.GetAxisRaw("Horizontal");
               
                if (isGround)
                {
                    jumpRequest = Input.GetKeyDown(GlobalArg.K_JUMP);
                    
                    if (jumpRequest)
                        jump();

                    else
                    {
                        if (dir < 0)
                            backward();
                        else if (dir > 0)
                            forward();
                        else
                            stand();
                    }
                }

                if (transform.position.y < springHeight)
                    overrideController["game2_charlie_jump"] = dropJump;

                if (follow)
                {
                    if (dir < 0)
                        horse.GetComponent<Horse>().state = Horse.horseState.slow;
                    else if (dir == 0)
                        horse.GetComponent<Horse>().state = Horse.horseState.normal;
                    else
                        horse.GetComponent<Horse>().state = Horse.horseState.fast;
                }
            }

            // 根据玩家方向调整玩家速度，移动玩家横向坐标
            if (dir < 0)
                transform.Translate(Vector2.right * slowSpeedScale * speed * Time.deltaTime);
            else if (dir == 0)
                transform.Translate(Vector2.right * speed * Time.deltaTime);
            else
                transform.Translate(Vector2.right * fastSpeedScale * speed * Time.deltaTime);

            if (transform.position.x < minPosX)
                transform.position = new Vector3(minPosX, transform.position.y);
            if (transform.position.x > maxPosX)
                transform.position = new Vector3(maxPosX, transform.position.y);

            if (camera.transform.position.x < transform.position.x + cameraPlayerMinDis)
            {
                float cameraTheoryPosX = transform.position.x + cameraPlayerMinDis;
                // 摄像机本身也有横向坐标范围
                if (cameraTheoryPosX <= cameraMinPosX)
                    cameraTheoryPosX = cameraMinPosX;
                if (cameraTheoryPosX >= cameraMaxPosX)
                    cameraTheoryPosX = cameraMaxPosX;
                // 更新摄像机位置
                camera.transform.position = new Vector3(cameraTheoryPosX, camera.transform.position.y, camera.transform.position.z);
            }
        }
    }

    protected override void drop()
    {
        base.drop();

        follow = false;
        overrideController["game2_charlie_jump"] = dropJump;

        if (GlobalArg.playerPassCount > 16)
            GlobalArg.playerPassCount = 17;
    }

    protected override void OnCollisionStay2D(Collision2D collision)
    {
        base.OnCollisionStay2D(collision);

        if (collision.collider.tag == "Platform" && collision.contacts[0].normal.y == 1)
        {
            isGround = true;
            springHeight = -4.48f;
            overrideController["game2_charlie_jump"] = highJump;
            score = initScore;
        }
        else if (collision.collider.tag == "Spring")
        {
            if (collision.contacts[0].normal.y > 0 && collision.contacts[0].normal.x == 0)
            {
                overrideController["game2_charlie_jump"] = collision.gameObject.GetComponent<Spring>().low ? lowJump : highJump;
                collision.collider.SendMessage("press");
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                GetComponent<Rigidbody2D>().AddForce(elasticity);
                springHeight = collision.gameObject.transform.position.y + 0.32f;
                AudioSource.PlayClipAtPoint(springClip, transform.position);
                addScore(score, false, true);
                score <<= 1;
            }
            else
                state = GlobalArg.playerState.drop; 
        }
    }
}
