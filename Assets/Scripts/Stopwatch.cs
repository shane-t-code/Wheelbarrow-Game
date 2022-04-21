using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Stopwatch : MonoBehaviour
{
    bool stopwatchActive = false;
    float currentTime;
    float finalTime;
    public TextMeshProUGUI currentTimeText;
    public TextMeshProUGUI finalTimeText;

    // Start is called before the first frame update
    void Start()
    {
        currentTime = 0;
        StartStopwatch();
    }

    // Update is called once per frame
    void Update()
    {
        if (stopwatchActive == true)
        {
            currentTime += Time.deltaTime;
        }
        TimeSpan time = TimeSpan.FromSeconds(currentTime);
        //Shows how minues, seconds, and milliseconds should be displayed v
        currentTimeText.text = time.ToString(@"mm\:ss\:fff");
        if (WB_Controller.gameEnd == true)
        {
            StopStopwatch();
            finalTimeText.text = time.ToString(@"mm\:ss\:fff");
            WB_Controller.gameEnd = false;
        }
    }

    public void StartStopwatch()
    {
        stopwatchActive = true;
    }

    public void StopStopwatch()
    {
        stopwatchActive = false;
    }
}
