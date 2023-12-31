using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalArg : MonoBehaviour
{
    public static float fps = 1.0f / 60;
    public static float window_width = 10.24f;
    public static float window_height = 8.96f;

    public static KeyCode K_START = KeyCode.Return;
    public static KeyCode K_LEFT = KeyCode.A;
    public static KeyCode K_RIGHT = KeyCode.D;
    public static KeyCode K_JUMP = KeyCode.K;
    public static KeyCode K_PAUSE = KeyCode.Return;

    public static int mode = 0;
    public static int playerLife = 3;
    public static int playerScore = 0;
    public static int playerStage = 1;
    public static int playerPassCount = 0;
    public static int hiScore = 20000;
    public static int addLifeScore = 20000;
    public static int time = 5000;
    public static int warningTime = 1000;

    public enum playerState { stand, forward, backward, jump, drop, die, win};
    public static bool isPlayerWin = false;
    public static bool isPlayerDrop = false;
    public static bool isPlayerDie = false;
}
