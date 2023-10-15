using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage1 : Stage
{
    // 火圈参数
    public float[] fireRingDis;                     // 火圈间距几种可能值
    public float fireRingNeighborDis;               // 如果生成双火圈，两个火圈的间距
    public float fireRingWidth;                     // 火圈宽度
    public float fireRingInitPosX;                  // 第一个火圈横向坐标
    public float fireRingPosY;                      // 普通火圈纵向坐标
    public float bonusFireRingPosY;                 // 钱袋火圈纵向坐标

    // 火圈、火盆素材与实体
    public GameObject fireRingPrefab;               // 普通火圈预制体
    public GameObject bonusFireRingPrefab;          // 钱袋火圈预制体

    private LinkedList<GameObject> fireRingList;    // 火圈实体列表

    public GameObject[] firePotList;                // 火盆实体列表

    protected override void Start()
    {
        base.Start();

        fireRingList = new LinkedList<GameObject>();
        // 生成第一个火圈
        createNextFireRing(camera.transform.position.x + fireRingInitPosX);
    }

    protected override void Update()
    {
        base.Update();

        // 如果获胜，删除所有火圈
        if (GlobalArg.isPlayerWin)
        {
            while (fireRingList.Count > 0)
            {
                GameObject obj = fireRingList.First.Value;
                fireRingList.RemoveFirst();
                Destroy(obj);
                obj = null;
            }
        }

        else if (!GlobalArg.isPlayerDie)
        {
            if (!isPause)
            {
                // 如果列表最后一个火圈距离屏幕右侧达到火圈间距最小值，则在右侧生成新火圈
                if (fireRingList.Last.Value.transform.position.x + fireRingDis[2] - fireRingWidth / 2 <= camera.transform.position.x + GlobalArg.window_width / 2)
                {
                    // 火圈间距随机数，三种火圈间距从大到小概率比为 6 ：3 ：1
                    int disRand = Random.Range(0, 10);
                    float dis = 0;
                    if (disRand == 0)
                        dis = fireRingDis[2];
                    else if (disRand <= 3)
                        dis = fireRingDis[1];
                    else
                        dis = fireRingDis[0];
                    // 生成新火圈
                    createNextFireRing(fireRingList.Last.Value.transform.position.x + dis);
                }

                // 若列表第一个火圈移出屏幕外，则删除第一个火圈
                if (fireRingList.First.Value.transform.position.x + fireRingWidth / 2 <= camera.transform.position.x - GlobalArg.window_width / 2)
                {
                    GameObject ring = fireRingList.First.Value;
                    fireRingList.RemoveFirst();
                    Destroy(ring);
                    ring = null;
                }
            }
        }
    }

    protected override void stopAnim()
    {
        // 停止火盆火苗动画
        for (int i = 0; i < firePotList.Length; ++i)
            firePotList[i].transform.Find("fire").gameObject.GetComponent<Animator>().speed = 0;

        // 停止火圈动画
        foreach (GameObject ring in fireRingList)
            ring.GetComponent<FireRing>().stop();
    }

    // 该方法在指定横坐标位置生成火圈
    private void createNextFireRing(float posX)
    {
        // 火圈数量随机数，单/双火圈概率比 4 ：1
        int ringNumRand = Random.Range(0, 5);

        if (ringNumRand == 0)
        {
            // 生成双火圈
            // 火圈种类随机数，大/小火圈概率比 4 ：1，下同
            // 为避免双火圈都是小火圈，随机范围不包括0
            int ringKindRand = Random.Range(1, 25);
            int firstRingKindRand = ringKindRand / 5;
            int secondRingKindRand = ringKindRand % 5;
            // 生成两个火圈实体
            GameObject ring1 = Instantiate(firstRingKindRand == 0 ? bonusFireRingPrefab : fireRingPrefab, transform);
            GameObject ring2 = Instantiate(secondRingKindRand == 0 ? bonusFireRingPrefab : fireRingPrefab, transform);
            // 指定位置
            ring1.transform.position = new Vector3(posX, firstRingKindRand == 0 ? bonusFireRingPosY : fireRingPosY);
            ring2.transform.position = new Vector3(posX + fireRingNeighborDis, secondRingKindRand == 0 ? bonusFireRingPosY : fireRingPosY);
            // 两个火圈实体加入列表
            fireRingList.AddLast(ring1);
            fireRingList.AddLast(ring2);
        }

        else
        {
            // 生成单火圈
            // 火圈种类随机数
            int ringKindRand = Random.Range(0, 5);
            // 生成火圈实体
            GameObject ring = Instantiate(ringKindRand == 0 ? bonusFireRingPrefab : fireRingPrefab, transform);
            // 指定位置
            ring.transform.position = new Vector3(posX, ringKindRand == 0 ? bonusFireRingPosY : fireRingPosY);
            // 火圈实体加入列表
            fireRingList.AddLast(ring);
        }
    }
}
