using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Async : MonoBehaviour
{
    private void Start()
    {
        Timer timer = new Timer(TimeOut, null, 5000, 0);
    }

    private void TimeOut(object state)
    {
        Debug.Log("铃铃铃");
    }
}