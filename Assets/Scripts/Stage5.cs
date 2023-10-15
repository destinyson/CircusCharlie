using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage5 : Stage
{
    public GameObject[] ropeList;

    protected override void Update()
    {
        base.Update();

        for (int i = 0; i < ropeList.Length; i++)
        {
            if (ropeList[i].GetComponent<Rope>().getCenter().x < camera.transform.position.x - GlobalArg.window_width / 2 - 0.04f)
                ropeList[i].SetActive(false);
        }
    }
}
