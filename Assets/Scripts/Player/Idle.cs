using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Idle
{
    public static bool IsIdle
    {
        get { return Time.time - lastInteractionTime > idleTimer; }
        private set {}
    }

    private static float idleTimer = 3;
    private static float lastInteractionTime;

    public static void ReportAction()
    {
        lastInteractionTime = Time.time;
    }

}
