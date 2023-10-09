using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // 玩家位置相关设定
    public Vector2 initPos;                     // 玩家初始位置
    public Vector3 winPos;                      // 玩家获胜位置
    public float minPosX;                       // 玩家横向坐标最小值
    public float maxPosX;                       // 玩家横向坐标最大值

    // 玩家节点动作
    public float speed;                         // 玩家移动速度
    public Vector2 jumpForce;                   // 玩家跳跃的力

    protected GlobalArg.playerState state;      // 玩家状态
    protected bool startJump;                   // 玩家是否准备起跳
    protected bool jumpRequest;                 // 跳跃键是否按下
    protected bool isGround;                    // 玩家是否在地面上
    protected float dir;                        // 玩家方向
    protected Animator charlieAnim;             // 玩家动画

    // 摄像机（需要跟随玩家）
    protected GameObject camera;                // 摄像机实体
    public float cameraMaxPosX;                 // 摄像机最小横坐标
    public float cameraMinPosX;                 // 摄像机最大横坐标
    public float cameraPlayerMinDis;            // 摄像机与玩家最小距离
    public float cameraPlayerMaxDis;            // 摄像机与玩家最大距离

    // 音效
    public AudioClip jumpClip;                  // 跳跃音效
    public AudioClip bonusClip;                 // 奖励音效

    // 奖励得分文本相关参数
    public GameObject bonusText;                // 奖励得分文本实体
    public float bonusActiveTimeVal;            // 奖励得分文本显示时间阈值

    protected float bonusActiveTime;            // 奖励得分文本显示剩余时间

    protected virtual void Awake()
    {
        // 玩家位置设定
        transform.position = new Vector3(GlobalArg.playerPassCount[GlobalArg.playerOrder] * 10.24f + initPos.x, initPos.y);

        // 玩家状态设定
        state = GlobalArg.playerState.stand;
        startJump = false;
        jumpRequest = false;
        isGround = true;
        dir = 0;
        // 摄像机实体获取与位置设定
        camera = GameObject.Find("Main Camera");
        camera.transform.position = new Vector3(transform.position.x + cameraPlayerMinDis, 0, -10);
        // 奖励得分文本隐藏，其显示剩余时间初始化为显示时间阈值
        bonusText.SetActive(false);
        bonusActiveTime = bonusActiveTimeVal;
    }

    protected virtual void Update()
    {
        // 奖励得分文本显示判定
        if (bonusText.activeSelf)
        {
            // 如果显示剩余时间耗尽，文本隐藏，显示剩余时间重置为显示时间阈值
            if (bonusActiveTime <= 0)
            {
                bonusText.SetActive(false);
                bonusActiveTime = bonusActiveTimeVal;
            }

            // 否则显示剩余时间减少
            else
                bonusActiveTime -= Time.deltaTime;
        }

        // 如果玩家获胜，执行win()
        if (state == GlobalArg.playerState.win)
            win();

        // 如果玩家死亡，执行die()
        else if (state == GlobalArg.playerState.die)
            die();

        // 如果关卡时间耗尽，玩家状态切换为die
        else if (GlobalArg.time == 0)
            state = GlobalArg.playerState.die;

        // 否则，玩家正在游戏，可操作
        else
        {
            // 如果玩家处于准备起跳状态
            if (startJump)
            {
                // 如果玩家已经不在地面上，说明已经跳起，不再处于准备起跳状态
                if (!isGround)
                    startJump = false;
            }

            else
            {
                // 否则，如果玩家在地面上
                if (isGround)
                {
                    jumpRequest = Input.GetKeyDown(GlobalArg.K_JUMP);
                    // 如果跳跃键按下，执行jump方法
                    if (jumpRequest)
                        jump();

                    // 否则根据按键情况确定方向
                    else
                    {
                        dir = Input.GetAxisRaw("Horizontal");
                        if (dir < 0)
                            backward();
                        else if (dir > 0)
                            forward();
                        else
                            stand();
                    }
                }
            }

            // 根据玩家方向移动玩家横向坐标
            transform.Translate(Vector2.right * dir * speed * Time.deltaTime);
            if (transform.position.x < minPosX)
                transform.position = new Vector3(minPosX, transform.position.y);
            if (transform.position.x > maxPosX)
                transform.position = new Vector3(maxPosX, transform.position.y);

            // 摄像机移动，如果玩家没有横向移动，摄像机不需要移动
            if (dir != 0)
            {
                // 如果玩家向左移动，玩家与摄像机距离不得大于最大距离阈值，否则摄像机需要移动
                if (dir < 0 && camera.transform.position.x > transform.position.x + cameraPlayerMaxDis)
                {
                    float cameraTheoryPosX = transform.position.x + cameraPlayerMaxDis;
                    // 摄像机本身也有横向坐标范围
                    if (cameraTheoryPosX <= cameraMinPosX)
                        cameraTheoryPosX = cameraMinPosX;
                    if (cameraTheoryPosX >= cameraMaxPosX)
                        cameraTheoryPosX = cameraMaxPosX;
                    // 更新摄像机位置
                    camera.transform.position = new Vector3(cameraTheoryPosX, camera.transform.position.y, camera.transform.position.z);
                }

                // 如果玩家向右移动，玩家与摄像机距离不得小于最小距离阈值，否则摄像机需要移动
                else if (dir > 0 && camera.transform.position.x < transform.position.x + cameraPlayerMinDis)
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
    }

    // stand方法，切换玩家状态为stand，并切换玩家对应动画，下面forward方法和backward方法同理
    protected virtual void stand()
    {
        state = GlobalArg.playerState.stand;

        charlieAnim.SetInteger("h", 0);
        if ((GlobalArg.playerStage[GlobalArg.playerOrder] - 1) % 5 != 0)
            charlieAnim.SetBool("jump", false);
    }

    protected virtual void forward()
    {
        state = GlobalArg.playerState.forward;

        charlieAnim.SetInteger("h", 1);
        if ((GlobalArg.playerStage[GlobalArg.playerOrder] - 1) % 5 != 0)
            charlieAnim.SetBool("jump", false);
    }

    protected virtual void backward()
    {
        state = GlobalArg.playerState.backward;

        charlieAnim.SetInteger("h", -1);
        if ((GlobalArg.playerStage[GlobalArg.playerOrder] - 1) % 5 != 0)
            charlieAnim.SetBool("jump", false);
    }

    // jump方法
    protected virtual void jump()
    {
        // 玩家状态切换为jump
        state = GlobalArg.playerState.jump;
        // 玩家准备起跳
        startJump = true;
        // 对玩家施加起跳的力
        GetComponent<Rigidbody2D>().AddForce(jumpForce);

        // 播放跳跃音效
        AudioSource.PlayClipAtPoint(jumpClip, transform.position);

        // 切换玩家动画
        charlieAnim.SetInteger("h", 0);
        if ((GlobalArg.playerStage[GlobalArg.playerOrder] - 1) % 5 != 0)
            charlieAnim.SetBool("jump", true);
    }

    protected virtual void win()
    {
        // 玩家获胜
        GlobalArg.isPlayerWin = true;

        // 更新玩家位置，切换玩家动画
        transform.position = winPos;
        charlieAnim.SetTrigger("win");

        // 玩家跨越路牌数量重置
        GlobalArg.playerPassCount[GlobalArg.playerOrder] = 0;
    }

    protected virtual void die()
    {
        // 玩家死亡
        GlobalArg.isPlayerDie = true;

        // 若非时间耗尽死亡，则切换玩家动画
        if ((GlobalArg.playerStage[GlobalArg.playerOrder] - 1) % 5 != 1)
        {
            if (GlobalArg.time > 0)
                charlieAnim.SetTrigger("die");
        }

        // 计算玩家跨越路牌数量（用于存档）
        GlobalArg.playerPassCount[GlobalArg.playerOrder] = (int)((transform.position.x - initPos.x) / 10.24f);
    }

    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.tag == "Win" && collision.contacts[0].normal.y == 1)
            state = GlobalArg.playerState.win;
        else if (collision.collider.tag == "Ground")
            isGround = true;
    }

    protected virtual void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground")
            isGround = false;
    }

    protected void addScore(int score, bool isBonus, Vector3 bonusTextPos)
    {
        GlobalArg.playerScore[GlobalArg.playerOrder] += score;
        if (GlobalArg.hiScore < GlobalArg.playerScore[GlobalArg.playerOrder])
            GlobalArg.hiScore = GlobalArg.playerScore[GlobalArg.playerOrder];

        if (isBonus)
        {
            AudioSource.PlayClipAtPoint(bonusClip, transform.position);
            bonusText.transform.position = bonusTextPos;
            bonusText.GetComponent<Text>().text = score.ToString();
            bonusText.SetActive(true);
        }
    }

}
