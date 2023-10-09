using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage1 : Stage
{
    // ��Ȧ����
    public float bigFireRingDis;                // ��Ȧ���ϴ�ֵ
    public float smallFireRingDis;              // ��Ȧ����Сֵ
    public float fireRingWidth;                 // ��Ȧ���
    public float fireRingPosY;                  // ��ͨ��Ȧ�߶�
    public float bonusFireRingPosY;             // Ǯ����Ȧ�߶�
    public float fireRingInitPosX;              // ��һ����Ȧ��������

    // ��Ȧ�ز���ʵ��
    public GameObject fireRingPrefab;           // ��ͨ��ȦԤ����
    public GameObject bonusFireRingPrefab;      // Ǯ����ȦԤ����

    private GameObject oldFireRing;             // �ɻ�Ȧʵ��
    private GameObject newFireRing;             // �»�Ȧʵ��

    protected override void Start()
    {
        base.Start();

        // ��һ����Ȧ������λ���趨�������ɻ�Ȧ���»�Ȧ��ֵnull
        oldFireRing = Instantiate(fireRingPrefab, transform);
        oldFireRing.transform.position = new Vector3(camera.transform.position.x + fireRingInitPosX, fireRingPosY);
        newFireRing = null;

        // ʧ����Ч����
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
