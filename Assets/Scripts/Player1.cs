using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public GameObject bonusText;
    public float bonusActiveTimeVal;

    private float dir;
    private bool jumpRequest;
    private bool isGround;
    private bool startJump;
    private float playerCameraPosXDiff;
    private bool passPot;
    private bool passRing;
    private float bonusActiveTime;

    private GlobalArg.playerState state;

    void Awake()
    {
        dir = 0;
        jumpRequest = false;
        isGround = true;
        startJump = false;
        playerCameraPosXDiff = transform.position.x;
        passPot = false;
        passRing = false;
        state = GlobalArg.playerState.stand;
        bonusText.SetActive(false);
        bonusActiveTime = bonusActiveTimeVal;
        transform.position = new Vector3(GlobalArg.playerPosX[GlobalArg.playerOrder], -2.7f);
    }

    void Update()
    {
        if (bonusText.activeSelf)
        {
            if (bonusActiveTime <= 0)
            {
                bonusText.SetActive(false);
                bonusActiveTime = bonusActiveTimeVal;
            }

            else
                bonusActiveTime -= Time.deltaTime;
        }

        if (state == GlobalArg.playerState.win)
            win();
        else if (state == GlobalArg.playerState.die)
            die();

        else if (GlobalArg.time == 0)
            state = GlobalArg.playerState.die;

        else if (startJump)
        {
            if (!isGround)
                startJump = false;
        }

        else
        {
            if (isGround)
            {
                jumpRequest = Input.GetKeyDown(GlobalArg.K_JUMP);

                if (jumpRequest)
                    jump();

                else
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

        if (GlobalArg.time > 0)
        {
            charlieAnim.SetTrigger("die");
            lionAnim.SetTrigger("die");
        }

        int passSignCount = (int)((transform.position.x - GlobalArg.playerInitPosX[GlobalArg.playerOrder]) / 10.24f);
        GlobalArg.playerPosX[GlobalArg.playerOrder] = GlobalArg.playerInitPosX[GlobalArg.playerOrder] + passSignCount * 10.24f;
    }

    void win()
    {
        transform.position = winPosition;
        GlobalArg.isPlayerWin = true;

        charlieAnim.SetTrigger("win");
        lionAnim.SetBool("jump", false);
        lionAnim.SetInteger("h", 0);

        GlobalArg.playerPosX[GlobalArg.playerOrder] = GlobalArg.playerInitPosX[GlobalArg.playerOrder];
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.tag == "Win")
            state = GlobalArg.playerState.win;
        else if (collision.collider.tag == "Danger")
            state = GlobalArg.playerState.die;
        else if (collision.collider.tag == "Ground")
            isGround = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground")
            isGround = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Bonus")
        {
            Destroy(collision.gameObject);
            AudioSource.PlayClipAtPoint(bonusClip, transform.position);
            GlobalArg.playerScore[GlobalArg.playerOrder] += 500;
            bonusText.transform.position = collision.gameObject.transform.position;
            bonusText.GetComponent<Text>().text = "500";
            bonusText.SetActive(true);
            if (GlobalArg.hiScore < GlobalArg.playerScore[GlobalArg.playerOrder])
                GlobalArg.hiScore = GlobalArg.playerScore[GlobalArg.playerOrder];
        }

        if (collision.tag == "Coin")
        {
            Destroy(collision.gameObject);
            AudioSource.PlayClipAtPoint(bonusClip, transform.position);
            GlobalArg.playerScore[GlobalArg.playerOrder] += 5000;
            bonusText.transform.position = collision.gameObject.transform.position;
            bonusText.GetComponent<Text>().text = "5000";
            bonusText.SetActive(true);
            if (GlobalArg.hiScore < GlobalArg.playerScore[GlobalArg.playerOrder])
                GlobalArg.hiScore = GlobalArg.playerScore[GlobalArg.playerOrder];
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Pot")
        {
            passPot = true;
            collision.SendMessage("addCount");
        }
            
        if (collision.tag == "Ring")
            passRing = true;
    }
}
