using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wanderer : MonoBehaviour
{
    [HideInInspector] public WandererRegion region;

    private enum State
    {
        Idle,
        Rotating,
        Moving
    }

    private State state = State.Idle;

    [Header("Refrences")]
    public Transform trans;
    public Transform modelTrans;

    [Header("Stats")]
    public float movespeed = 18;

    [Tooltip("Minimalny czas oczekiwania przed ponownym wyznaczeniem celu.")]
    public float minRetargetInterval = 4.4f;

    [Tooltip("Maksymalny czas oczekiwania przed ponownym wyznaczeniem celu.")]
    public float maxRetargetInterval = 6.2f;

    [Tooltip("Czas w sekundach po�wi�cony na obr�cenie si� po wyznaczeniu celu.")]
    public float rotationTime = .6f;

    [Tooltip("Czas w sekundach po zako�czeniu obracania si�, a przed ruchem.")]
    public float postRotationWaitTime = .3f;

    private Vector3 currenTarget;
    private Quaternion initialRotation;
    private Quaternion targetRotation;
    private float rotationStartTime;

    //Wywo�ana przy starcie i planowana do ponownego wywo�ania przy ka�dym swoim wywo�aniu.
    //Nast�pne wywo�anie b�dzie zaplanowane po losowym czasie z wyznaczonego zakresu.
    void Retarget()
    {
        //Ustaw bie��cy cel na nowy, losowy punkt w regionie:
        currenTarget = region.GetRandomPointWithin();
        //Zanotuj pocz�tkowy obr�t
        initialRotation = modelTrans.rotation;
        //Zanotuj obr�t potrzebny, aby patrzec na cel
        targetRotation = Quaternion.LookRotation((currenTarget - trans.position).normalized);

        //Rozpocznij obracanie
        state = State.Rotating;
        rotationStartTime = Time.time;

        //Rozpocznij ruch po 'postRotationWaitTime' sekund po zako�czeniu obracania
        Invoke("BeginMoving", rotationTime + postRotationWaitTime);
    }

    //Wywo�ywana przez Retarget w celu zainicjowania ruchu
    void BeginMoving()
    {
        //Upewnij si�, �e patrzymy w kierunku tergetRotation
        modelTrans.rotation = targetRotation;
        //Ustaw stan na Moving
        state = State.Moving;
    }

    // Start is called before the first frame update
    void Start()
    {
        Retarget();
    }

    // Update is called once per frame
    void Update()
    {
        if(state == State.Moving)
        {
            //Zmierz odleg�o��, na jak� przemie�cimy si� w tej klatce
            float delta = movespeed * Time.deltaTime;
            //Wykonaj ruch w kierunku punktu docelowego o warto�� delta
            trans.position = Vector3.MoveTowards(trans.position, currenTarget, delta);

            //Po osi�gni�ciu punktu docelowego zatrzymaj si� i zaplanuj nast�pne wywo�anie Retarget
            if(trans.position == currenTarget)
            {
                state = State.Idle;
                Invoke("Retarget", Random.Range(minRetargetInterval, maxRetargetInterval));
            }
        }
        else if (state == State.Rotating)
        {
            //Zmierz czas dotychczasowego obracania sie w sekundach
            float timeSpentRotating = Time.time - rotationStartTime;

            //Obracaj sie od initialRotation do targetRotation
            modelTrans.rotation = Quaternion.Slerp(initialRotation, targetRotation, timeSpentRotating / rotationTime);
        }
    }
}
