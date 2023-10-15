using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public GameObject[] stageList;

    private GameObject stage;

    private void Start()
    {
        stage = Instantiate(stageList[(GlobalArg.playerStage - 1) % stageList.Length], transform);
    }
}
