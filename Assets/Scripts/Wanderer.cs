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

    [Tooltip("Czas w sekundach poœwiêcony na obrócenie siê po wyznaczeniu celu.")]
    public float rotationTime = .6f;

    [Tooltip("Czas w sekundach po zakoñczeniu obracania siê, a przed ruchem.")]
    public float postRotationWaitTime = .3f;

    private Vector3 currenTarget;
    private Quaternion initialRotation;
    private Quaternion targetRotation;
    private float rotationStartTime;

    //Wywo³ana przy starcie i planowana do ponownego wywo³ania przy ka¿dym swoim wywo³aniu.
    //Nastêpne wywo³anie bêdzie zaplanowane po losowym czasie z wyznaczonego zakresu.
    void Retarget()
    {
        //Ustaw bie¿¹cy cel na nowy, losowy punkt w regionie:
        currenTarget = region.GetRandomPointWithin();
        //Zanotuj pocz¹tkowy obrót
        initialRotation = modelTrans.rotation;
        //Zanotuj obrót potrzebny, aby patrzec na cel
        targetRotation = Quaternion.LookRotation((currenTarget - trans.position).normalized);

        //Rozpocznij obracanie
        state = State.Rotating;
        rotationStartTime = Time.time;

        //Rozpocznij ruch po 'postRotationWaitTime' sekund po zakoñczeniu obracania
        Invoke("BeginMoving", rotationTime + postRotationWaitTime);
    }

    //Wywo³ywana przez Retarget w celu zainicjowania ruchu
    void BeginMoving()
    {
        //Upewnij siê, ¿e patrzymy w kierunku tergetRotation
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
            //Zmierz odleg³oœæ, na jak¹ przemieœcimy siê w tej klatce
            float delta = movespeed * Time.deltaTime;
            //Wykonaj ruch w kierunku punktu docelowego o wartoœæ delta
            trans.position = Vector3.MoveTowards(trans.position, currenTarget, delta);

            //Po osi¹gniêciu punktu docelowego zatrzymaj siê i zaplanuj nastêpne wywo³anie Retarget
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
