using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage4 : Stage
{
    // 弹簧信息
    public float[] springSignDis;           // 弹簧与路标距离
    public GameObject[] springPrefabs;      // 弹簧预制体
    public float startSignPosX;             // 开始生成弹簧对应的路标位置
    public float springLowPosY;             // 矮弹簧纵向坐标
    public float springHighPosY;            // 高弹簧纵向坐标
    public int springGroupNum;              // 弹簧组数


    public AudioClip drop;                  // 掉落音效


    protected override void Start()
    {
        base.Start();

        // 弹簧生成
        for (int i = GlobalArg.playerPassCount; i < springGroupNum; ++i)
        {
            GameObject spring1 = Instantiate(springPrefabs[Random.Range(0, springPrefabs.Length)], transform);
            GameObject spring2 = Instantiate(springPrefabs[Random.Range(0, springPrefabs.Length)], transform);
            //GameObject spring1 = Instantiate(springPrefabs[0], transform);
            //GameObject spring2 = Instantiate(springPrefabs[0], transform);
            int posYRandom = Random.Range(0, 4);
            if (posYRandom > 1)
            {
                spring1.transform.position = new Vector3(startSignPosX + 10.24f * i + springSignDis[0], springHighPosY);
                spring1.GetComponent<Spring>().low = false;
            }
            else
            {
                spring1.transform.position = new Vector3(startSignPosX + 10.24f * i + springSignDis[0], springLowPosY);
                spring1.GetComponent<Spring>().low = true;
            }
            if ((posYRandom & 1) == 1)
            {
                spring2.transform.position = new Vector3(startSignPosX + 10.24f * i + springSignDis[1], springHighPosY);
                spring2.GetComponent<Spring>().low = false;
            }
            else
            {
                spring2.transform.position = new Vector3(startSignPosX + 10.24f * i + springSignDis[1], springLowPosY);
                spring2.GetComponent<Spring>().low = true;
            }
        }
    }

    protected override void Update()
    {
        base.Update();

        // 掉落时音效播放
        if (GlobalArg.isPlayerDrop)
        {
            if (am.GetComponent<AudioSource>().clip == BGM || am.GetComponent<AudioSource>().clip == warningBGM)
            {
                am.GetComponent<AudioSource>().Stop();
                am.GetComponent<AudioSource>().clip = drop;
                am.GetComponent<AudioSource>().loop = false;
                am.GetComponent<AudioSource>().Play();
            }
        }
    }
}
