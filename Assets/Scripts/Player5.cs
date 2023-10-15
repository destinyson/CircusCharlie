using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player5 : Player
{
    // 玩家动画
    public AnimationClip leftGrab;      // 玩家在锚点左侧摆动动画序列
    public AnimationClip rightGrab;     // 玩家在锚点右侧摆动动画序列

    private AnimatorOverrideController overrideController;      // 玩家动画控制器

    // 绳子相关
    public GameObject[] ropeList;       // 绳索列表
    public Vector2 handPos;             // 手的相对位置

    private GameObject grabRope;        // 抓着的绳索
    private GameObject lastRope;        // 上次抓着的绳索

    public float gravityScale;          // 玩家重力倍数

    public AudioClip springClip;        // 玩家被弹起音效

    private float cameraDiff;           // 玩家与摄像机距离

    protected override void Awake()
    {
        base.Awake();

        overrideController = new AnimatorOverrideController(charlieAnim.runtimeAnimatorController);
        charlieAnim.runtimeAnimatorController = overrideController;

        grabRope = ropeList[GlobalArg.playerPassCount];
        camera.transform.position = new Vector3(grabRope.GetComponent<Rope>().getCenter().x + 1.96f, 0, -10);
        followRope();
    }

    protected override void Update()
    {
        base.Update();

        if (transform.position.x < camera.transform.position.x - GlobalArg.window_width / 2 + 0.32f)
            transform.position = new Vector3(camera.transform.position.x - GlobalArg.window_width / 2 + 0.32f, transform.position.y);
        if (transform.position.x > maxPosX)
            transform.position = new Vector3(maxPosX, transform.position.y);

        // 摄像机移动
        if (!isGround)
        {
            float cameraTheoryPosX = transform.position.x + cameraDiff;
            // 摄像机本身也有横向坐标范围
            if (cameraTheoryPosX <= cameraMinPosX)
                cameraTheoryPosX = cameraMinPosX;
            if (cameraTheoryPosX >= cameraMaxPosX)
                cameraTheoryPosX = cameraMaxPosX;
            // 更新摄像机位置
            camera.transform.position = new Vector3(cameraTheoryPosX, camera.transform.position.y, camera.transform.position.z);
        }
            
        else
        {
            Vector3 target = new Vector3(grabRope.GetComponent<Rope>().getCenter().x + 1.96f, camera.transform.position.y, camera.transform.position.z);
            float cameraTheoryPosX = Vector3.MoveTowards(camera.transform.position, target,  speed * Time.deltaTime).x;
            // 摄像机本身也有横向坐标范围
            if (cameraTheoryPosX <= cameraMinPosX)
                cameraTheoryPosX = cameraMinPosX;
            if (cameraTheoryPosX >= cameraMaxPosX)
                cameraTheoryPosX = cameraMaxPosX;
            // 更新摄像机位置
            camera.transform.position = new Vector3(cameraTheoryPosX, camera.transform.position.y, camera.transform.position.z);
        }
    }

    protected override void stand()
    {
        base.stand();
        followRope();
    }

    protected override void forward()
    {
        base.stand();
        grabRope.GetComponent<Rope>().sendRequest(1);
        followRope();
    }

    protected override void backward()
    {
        base.stand();
        grabRope.GetComponent<Rope>().sendRequest(-1);
        followRope();
    }

    protected override void jump()
    {
        // 玩家状态切换为jump
        state = GlobalArg.playerState.jump;
        // 玩家准备起跳
        startJump = true;

        isGround = false;

        lastRope = grabRope;

        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        GetComponent<Rigidbody2D>().gravityScale = gravityScale;

        int hDir = grabRope.GetComponent<Rope>().getHDir();
        int vDir = grabRope.GetComponent<Rope>().getVDir();
        GetComponent<Rigidbody2D>().velocity = new Vector2(hDir * speed, vDir * speed);

        // 播放跳跃音效
        AudioSource.PlayClipAtPoint(jumpClip, transform.position);

        // 切换玩家动画
        charlieAnim.SetInteger("h", 0);
        charlieAnim.SetBool("jump", true);

        cameraDiff = camera.transform.position.x - transform.position.x;
    }

    protected override void die()
    {
        base.die();
        GlobalArg.playerPassCount = grabRope.GetComponent<Rope>().order;
    }

    private void followRope()
    {
        isGround = true;

        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        Vector3 knotPos = grabRope.transform.Find("knot").position;
        transform.position = new Vector3(knotPos.x - handPos.x, knotPos.y - handPos.y);

        if (transform.position.x <= grabRope.transform.position.x - handPos.x)
            overrideController["game5_charlie_grab"] = leftGrab;
        else
            overrideController["game5_charlie_grab"] = rightGrab;

        cameraDiff = 0;
    }

    protected override void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.tag == "Win" && collision.contacts[0].normal.y == 1)
            state = GlobalArg.playerState.win;
        else if (collision.collider.tag == "Ground")
            state = GlobalArg.playerState.die;
        else if (collision.collider.tag == "Spring")
        {
            grabRope = null;
            dir = Input.GetAxisRaw("Horizontal");
            AudioSource.PlayClipAtPoint(springClip, transform.position);
            GetComponent<Rigidbody2D>().velocity = new Vector2(dir * speed, 14.54f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Platform" && grabRope != collision.gameObject.transform.parent.gameObject && !startJump)
        {
            isGround = true;
            grabRope = collision.gameObject.transform.parent.gameObject;
            if (grabRope != lastRope)
                addScore(500, false, true);
        }
    }
}
