using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage1 : Stage
{
    // 火圈参数
    public float bigFireRingDis;                // 火圈间距较大值
    public float smallFireRingDis;              // 火圈间距较小值
    public float fireRingWidth;                 // 火圈宽度
    public float fireRingPosY;                  // 普通火圈高度
    public float bonusFireRingPosY;             // 钱袋火圈高度
    public float fireRingInitPosX;              // 第一个火圈横向坐标

    // 火圈素材与实体
    public GameObject fireRingPrefab;           // 普通火圈预制体
    public GameObject bonusFireRingPrefab;      // 钱袋火圈预制体

    private GameObject oldFireRing;             // 旧火圈实体
    private GameObject newFireRing;             // 新火圈实体

    protected override void Start()
    {
        base.Start();

        // 第一个火圈生成与位置设定，赋给旧火圈，新火圈赋值null
        oldFireRing = Instantiate(fireRingPrefab, transform);
        oldFireRing.transform.position = new Vector3(camera.transform.position.x + fireRingInitPosX, fireRingPosY);
        newFireRing = null;

        // 失败音效序列
        loseClipList = new AudioClip[] { die, gameOver };
    }

    protected override void Update()
    {
        base.Update();

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
        }

        else if (!GlobalArg.isPlayerDie)
        {
            if (!isPause)
            {
                if (oldFireRing.transform.position.x + smallFireRingDis - fireRingWidth / 2 <= camera.transform.position.x + GlobalArg.window_width / 2 && !newFireRing)
                {
                    int disRand = Random.Range(0, 10);
                    int kindRand = Random.Range(0, 10);
                    newFireRing = Instantiate(kindRand == 0 ? bonusFireRingPrefab : fireRingPrefab,
                                              new Vector3(oldFireRing.transform.position.x + (disRand < 3 ? smallFireRingDis : bigFireRingDis),
                                                          kindRand == 0 ? bonusFireRingPosY : fireRingPosY),
                                              Quaternion.identity, transform);
                }

                else if (oldFireRing.transform.position.x + fireRingWidth / 2 <= camera.transform.position.x - GlobalArg.window_width / 2)
                {
                    Destroy(oldFireRing);
                    oldFireRing = newFireRing;
                    newFireRing = null;
                }
            }
        }
    }
}
