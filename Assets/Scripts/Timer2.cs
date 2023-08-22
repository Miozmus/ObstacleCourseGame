using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer2 : MonoBehaviour
{
    public TextMeshProUGUI Timertext;

    public float currentTime;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        Timertext.text = currentTime.ToString(); ;
    }
}
