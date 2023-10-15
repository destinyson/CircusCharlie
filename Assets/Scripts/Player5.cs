using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player5 : Player
{
    // ��Ҷ���
    public AnimationClip leftGrab;      // �����ê�����ڶ���������
    public AnimationClip rightGrab;     // �����ê���Ҳ�ڶ���������

    private AnimatorOverrideController overrideController;      // ��Ҷ���������

    // �������
    public GameObject[] ropeList;       // �����б�
    public Vector2 handPos;             // �ֵ����λ��

    private GameObject grabRope;        // ץ�ŵ�����
    private GameObject lastRope;        // �ϴ�ץ�ŵ�����

    public float gravityScale;          // �����������

    public AudioClip springClip;        // ��ұ�������Ч

    private float cameraDiff;           // ��������������

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

        // ������ƶ�
        if (!isGround)
        {
            float cameraTheoryPosX = transform.position.x + cameraDiff;
            // ���������Ҳ�к������귶Χ
            if (cameraTheoryPosX <= cameraMinPosX)
                cameraTheoryPosX = cameraMinPosX;
            if (cameraTheoryPosX >= cameraMaxPosX)
                cameraTheoryPosX = cameraMaxPosX;
            // ���������λ��
            camera.transform.position = new Vector3(cameraTheoryPosX, camera.transform.position.y, camera.transform.position.z);
        }
            
        else
        {
            Vector3 target = new Vector3(grabRope.GetComponent<Rope>().getCenter().x + 1.96f, camera.transform.position.y, camera.transform.position.z);
            float cameraTheoryPosX = Vector3.MoveTowards(camera.transform.position, target,  speed * Time.deltaTime).x;
            // ���������Ҳ�к������귶Χ
            if (cameraTheoryPosX <= cameraMinPosX)
                cameraTheoryPosX = cameraMinPosX;
            if (cameraTheoryPosX >= cameraMaxPosX)
                cameraTheoryPosX = cameraMaxPosX;
            // ���������λ��
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
        // ���״̬�л�Ϊjump
        state = GlobalArg.playerState.jump;
        // ���׼������
        startJump = true;

        isGround = false;

        lastRope = grabRope;

        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        GetComponent<Rigidbody2D>().gravityScale = gravityScale;

        int hDir = grabRope.GetComponent<Rope>().getHDir();
        int vDir = grabRope.GetComponent<Rope>().getVDir();
        GetComponent<Rigidbody2D>().velocity = new Vector2(hDir * speed, vDir * speed);

        // ������Ծ��Ч
        AudioSource.PlayClipAtPoint(jumpClip, transform.position);

        // �л���Ҷ���
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
