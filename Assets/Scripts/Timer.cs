using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI Timertext;

    public float currentTimeInts;
    public float currentTimeSeconds;
    public float currentTimeMillis;
    public string FinalText;


   

    // Update is called once per frame
    void Update()
    {
       
            Debug.Log("Timer dzia³a");
            currentTimeMillis += Time.deltaTime;
            if (currentTimeMillis >= 1)
            {
                Debug.Log("Zwiêkszam sekundy    " + currentTimeMillis.ToString());
                currentTimeSeconds += 1;
                currentTimeMillis = 0;
            }
            if (currentTimeSeconds >= 60)
            {
                Debug.Log("Zwiêkszam minuty    " + currentTimeSeconds.ToString());
                currentTimeInts += 1;
                currentTimeSeconds = 0;
            }
            FinalText = currentTimeInts.ToString() + ":" + currentTimeSeconds.ToString();
            Timertext.text = FinalText;
    }
}
