using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player4 : Player
{
    // ��Ҷ���
    public float slowSpeedScale;            // ��Ҽ����ٶȱ���
    public float fastSpeedScale;            // ��Ҽ����ٶȱ���
    public float slowPosX;                  // ��Ҽ���λ�ã����10m����ǿ�м���

    public bool follow;                     // ����Ƿ���Ҫ�����
    public float springHeight;              // ��������ĵ��ɸ߶�
    public Vector2 elasticity;              // ���ɵ���

    public GameObject horse;                // ��ʵ��

    // ��Ҷ���
    public AnimationClip lowJump;                               // ��������Ծ����
    public AnimationClip highJump;                              // �ߵ�����Ծ����
    public AnimationClip dropJump;                              // �ص�����/���������Ծ����
    private AnimatorOverrideController overrideController;      // ��Ҷ���������

    public AudioClip springClip;            // ������Ч

    // ��������
    public int initScore;                   // ��ʼ����
    private int score;                      // ʵ�ʷ���

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
                // �˴�����������ڿ��Ʒ��򣬶��ǿ����ٶȣ��������෽������������
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

            // ������ҷ����������ٶȣ��ƶ���Һ�������
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
                // ���������Ҳ�к������귶Χ
                if (cameraTheoryPosX <= cameraMinPosX)
                    cameraTheoryPosX = cameraMinPosX;
                if (cameraTheoryPosX >= cameraMaxPosX)
                    cameraTheoryPosX = cameraMaxPosX;
                // ���������λ��
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
