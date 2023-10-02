using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage1 : MonoBehaviour
{
    public GameObject camera;
    public GameObject firstFireRing;
    public GameObject fireRingPrefab;
    public GameObject bonusFireRingPrefab;
    public Animator[] audienceAnim;
    public float bigFireRingDis;
    public float smallFireRingDis;
    public float fireRingWidth;
    public float fireRingPosY;
    public float bonusFireRingPosY;
    public AudioClip bgm1;
    public AudioClip die;
    public AudioClip gameOver;
    public AudioClip applause;
    public AudioClip score;

    private GameObject oldFireRing;
    private GameObject newFireRing;
    private AudioSource audio;
    private bool gameOverPlay;
    private bool bgmPlay;
    private float timeLessVal;
    private float timeLess;
    void Start()
    {
        oldFireRing = firstFireRing;
        newFireRing = null;
        gameOverPlay = false;
        bgmPlay = true;
        timeLessVal = GlobalArg.fps * 16;
        timeLess = timeLessVal;
        audio = GameObject.Find("AudioManager").GetComponent<AudioSource>();
        audio.clip = bgm1;
        audio.loop = true;
        audio.Play();
    }

    void Update()
    {
        if (GlobalArg.isPlayerWin)
        {
            if (oldFireRing)
            {
                Destroy(oldFireRing);
                oldFireRing = null;
            }
            if (newFireRing)
            {
                Destroy(newFireRing);
                newFireRing = null;
            }

            for (int i = 0; i < audienceAnim.Length; ++i)
                audienceAnim[i].SetTrigger("win");

            if (GlobalArg.time > 0)
            {
                GlobalArg.time -= 10;
                GlobalArg.playerScore[GlobalArg.playerOrder] += 10;
                if (!audio.isPlaying)
                {
                    audio.clip = score;
                    audio.loop = false;
                    audio.Play();
                }
            }

            if (audio.isPlaying)
            {
                if (bgmPlay)
                {
                    bgmPlay = false;
                    audio.Stop();
                    audio.clip = applause;
                    audio.loop = false;
                    audio.Play();
                }
            }

            else if (GlobalArg.time == 0)
                SceneManager.LoadScene(1);
        }

        else if (GlobalArg.isPlayerDie)
        {
            Time.timeScale = 0;
            if (audio.isPlaying)
            {
                if (bgmPlay)
                {
                    bgmPlay = false;
                    audio.Stop();
                    audio.clip = die;
                    audio.loop = false;
                    audio.Play();
                }
            }

            else
            {
                if (!gameOverPlay)
                {
                    gameOverPlay = true;
                    audio.clip = gameOver;
                    audio.loop = false;
                    audio.Play();
                }
                else
                    SceneManager.LoadScene(1);
            }
        }

        else
        {
            if (timeLess <= 0)
            {
                GlobalArg.time -= 10;
                timeLess = timeLessVal;
            }
            else
                timeLess -= Time.deltaTime;

            if (oldFireRing.transform.position.x + smallFireRingDis <= camera.transform.position.x + GlobalArg.window_width / 2 && !newFireRing)
            {
                int disRand = Random.Range(0, 10);
                int kindRand = Random.Range(0, 10);
                newFireRing = Instantiate(kindRand == 0 ? bonusFireRingPrefab : fireRingPrefab,
                                          new Vector3(oldFireRing.transform.position.x + (disRand < 3 ? smallFireRingDis : bigFireRingDis),
                                                      kindRand == 0 ? bonusFireRingPosY : fireRingPosY),
                                          Quaternion.identity, transform);
            }

            else if (oldFireRing.transform.position.x + fireRingWidth <= camera.transform.position.x - GlobalArg.window_width / 2)
            {
                Destroy(oldFireRing);
                oldFireRing = newFireRing;
                newFireRing = null;
            }
        }
    }
}
