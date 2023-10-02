using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Player1 : MonoBehaviour
{
    public Animator charlieAnim;
    public Animator lionAnim;
    public GameObject camera;
    public float speed;
    public float cameraMaxPosX;
    public float cameraMinPosX;
    public Vector2 jumpForce;
    public Vector3 winPosition;
    public AudioClip jumpClip;
    public AudioClip bonusClip;

    private float dir;
    private bool jumpRequest;
    private bool isGround;
    private bool startJump;
    private float playerCameraPosXDiff;
    private bool passPot;
    private bool passRing;

    private GlobalArg.playerState state;

    void Start()
    {
        dir = 0;
        jumpRequest = false;
        isGround = true;
        startJump = false;
        playerCameraPosXDiff = transform.position.x;
        passPot = false;
        passRing = false;
        state = GlobalArg.playerState.stand;
    }

    void Update()
    {
        if (state == GlobalArg.playerState.win)
            win();
        else if (state == GlobalArg.playerState.die)
            die();

        else if (startJump)
        {
            if (!isGround)
                startJump = false;
        }

        else
        {
            jumpRequest = Input.GetKeyDown(GlobalArg.K_JUMP);

            if (jumpRequest && isGround)
                jump();

            else if (isGround)
            {
                if (passPot && passRing)
                    GlobalArg.playerScore[GlobalArg.playerOrder] += 400;
                else if (passPot)
                    GlobalArg.playerScore[GlobalArg.playerOrder] += 200;
                else if (passRing)
                    GlobalArg.playerScore[GlobalArg.playerOrder] += 100;
                if (GlobalArg.hiScore < GlobalArg.playerScore[GlobalArg.playerOrder])
                    GlobalArg.hiScore = GlobalArg.playerScore[GlobalArg.playerOrder];
                passPot = false;
                passRing = false;

                dir = Input.GetAxisRaw("Horizontal");
                if (dir < 0)
                    backward();
                else if (dir > 0)
                    forward();
                else
                    stand();
            }
        }

        transform.Translate(Vector2.right * dir * speed * Time.deltaTime);

        float cameraTheoryPosX = transform.position.x - playerCameraPosXDiff;
        if (cameraTheoryPosX <= cameraMinPosX)
            cameraTheoryPosX = cameraMinPosX;
        if (cameraTheoryPosX >= cameraMaxPosX)
            cameraTheoryPosX = cameraMaxPosX;
        camera.transform.position = new Vector3(cameraTheoryPosX, camera.transform.position.y, camera.transform.position.z);
    }

    void stand()
    {
        if (state != GlobalArg.playerState.stand)
        {
            state = GlobalArg.playerState.stand;

            charlieAnim.SetInteger("h", 0);
            lionAnim.SetBool("jump", false);
            lionAnim.SetInteger("h", 0);
        }
    }

    void forward()
    {
        if (state != GlobalArg.playerState.forward)
        {
            state = GlobalArg.playerState.forward;

            charlieAnim.SetInteger("h", 1);
            lionAnim.SetBool("jump", false);
            lionAnim.SetInteger("h", 1);
        }
    }

    void backward()
    {
        if (state != GlobalArg.playerState.backward)
        {
            state = GlobalArg.playerState.backward;

            charlieAnim.SetInteger("h", -1);
            lionAnim.SetBool("jump", false);
            lionAnim.SetInteger("h", -1);
        }
    }

    void jump()
    {
        if (state != GlobalArg.playerState.jump)
        {
            state = GlobalArg.playerState.jump;
            AudioSource.PlayClipAtPoint(jumpClip, transform.position);
            startJump = true;
            GetComponent<Rigidbody2D>().AddForce(jumpForce);

            charlieAnim.SetInteger("h", 0);
            lionAnim.SetBool("jump", true);
            lionAnim.SetInteger("h", 0);
        }
    }

    void die()
    {
        GlobalArg.isPlayerDie = true;

        charlieAnim.SetTrigger("die");
        lionAnim.SetTrigger("die");
    }

    void win()
    {
        transform.position = winPosition;
        GlobalArg.isPlayerWin = true;

        charlieAnim.SetTrigger("win");
        lionAnim.SetBool("jump", false);
        lionAnim.SetInteger("h", 0);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.tag == "Win")
            state = GlobalArg.playerState.win;
        else if (collision.collider.tag == "Danger")
            state = GlobalArg.playerState.die;
        else if (collision.collider.tag == "Ground")
            isGround = true;
        else if (collision.collider.tag == "Bonus")
        {
            Destroy(collision.gameObject);
            AudioSource.PlayClipAtPoint(bonusClip, transform.position);
            GlobalArg.playerScore[GlobalArg.playerOrder] += 500;
            if (GlobalArg.hiScore < GlobalArg.playerScore[GlobalArg.playerOrder])
                GlobalArg.hiScore = GlobalArg.playerScore[GlobalArg.playerOrder];
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground")
            isGround = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Pot")
            passPot = true;
        if (collision.tag == "Ring")
            passRing = true;
    }
}
