using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    public int order;

    public int rotateMaxAngle;
    public float[] rotateTimeVal;
    public float length;

    private Vector2 center;
    private int rotateAngle;
    private int dir;
    private int speedRank;
    private float rotateTime;

    private int request;

    void Awake()
    {
        center = new Vector2(transform.position.x, transform.position.y + length / 2);
        rotateAngle = 15 * Random.Range(-5, 6);
        dir = 1;
        speedRank = 0;
        rotateTime = rotateTimeVal[speedRank];
        transform.RotateAround(center, Vector3.forward, rotateAngle);
        request = 0;
    }

    void Update()
    {
        if (rotateTime <= 0)
        {
            if (rotateAngle == rotateMaxAngle)
            {
                dir = -1;
                if (request > 0)
                    speedUp();
                else if (request < 0)
                    speedDown();
                request = 0;
            }
            else if (rotateAngle == -rotateMaxAngle)
            {
                dir = 1;
                if (request > 0)
                    speedDown();
                else if (request < 0)
                    speedUp();
                request = 0;
            }
            transform.RotateAround(center, Vector3.forward, dir * 15);
            rotateAngle += dir * 15;
            rotateTime = rotateTimeVal[speedRank];
        }
        else
            rotateTime -= Time.deltaTime;
    }

    public int getHDir()
    {
        if (rotateAngle == rotateMaxAngle || rotateAngle == -rotateMaxAngle)
            return 0;
        return dir;
    }

    public int getVDir()
    {
        if (rotateAngle == 0 || rotateAngle == rotateMaxAngle || rotateAngle == -rotateMaxAngle)
            return 0;
        else if (rotateAngle > 0)
            return dir;
        else
            return -dir;
    }

    public Vector2 getCenter()
    {
        return center;
    }

    public void sendRequest(int dir)
    {
        request += dir;
    }

    private void speedUp()
    {
        if (speedRank < rotateTimeVal.Length - 1)
            ++speedRank;
    }

    private void speedDown()
    {
        if (speedRank > 0)
            --speedRank;
    }
}
