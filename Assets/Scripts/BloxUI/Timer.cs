using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Based on https://medium.com/@brianmayrose/a-simple-script-for-a-simple-clock-unity-3d-game-development-187a9b0d2f40
/// This basically counts a MaxTime to zero.
/// </summary>
public class Timer : MonoBehaviour
{
    private bool TimerLoaded = false;
    private Text textClock;
    private double MaxTime;
    private DateTime endTime;
    
    // Start is called before the first frame update
    void Start()
    {
        textClock = GetComponent<Text>();
    }

    public void LoadTimer(double maxTime)
    {
        this.MaxTime = maxTime;
        endTime = DateTime.Now.AddMinutes(MaxTime);
        TimerLoaded = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (TimerLoaded)
        {
            DateTime time = DateTime.Now;
            TimeSpan dif = endTime - time;
            bool isNegative = dif.TotalMinutes < 0;
            string minute = LeadingZero(Convert.ToInt32(Math.Floor(Math.Abs(dif.TotalMinutes))));
            string second = LeadingZero(Math.Abs(isNegative ? 59 + dif.Seconds : dif.Seconds));
            textClock.text = (isNegative ? "-" : "") + minute + ":" + second;
        }
    }
    string LeadingZero(int n)
    {
        return n.ToString().PadLeft(2, '0');
    }
}
