using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    private enum State
    { Lowered, Lowering, Raising, Raised}
    private State state = State.Lowered;
    private const float SpikeHeight = 3.6f;
    private const float LoweredSpikeHeight = .5f;

    [Header("Stats")]
    [Tooltip("Czas w wsekundach po opuszczeniu kolc�w, zanim zostan� ponownie podniesione.")]
    public float interval = 2f;

    [Tooltip("Czas w sekundach po podniesieniu kolc�w, zanim zaczn� by� ponownie opuszczane.")]
    public float raiseWaitTime = .3f;

    [Tooltip("Czas w sekundach, ile zajmuje pe�ne opuszczenie kolc�w.")]
    public float lowerTime = .6f;

    [Tooltip("Czas w sekundach, ile zajmuje pe�ne podniesienie kolc�w.")]
    public float raiseTime = .8f;

    private float lastSwitchTime = Mathf.NegativeInfinity;

    [Header("References")]
    [Tooltip("Odwo�anie do obiektu nadrz�dnego dla wszystkich kolc�w.")]
    public Transform spikeHolder;
    public GameObject hitboxGameObject;
    public GameObject colliderGameObject;

    void StartRaising()
    {
        lastSwitchTime = Time.time;
        state = State.Raising;
        hitboxGameObject.SetActive(true);
    }

    void StartLowering()
    {
        lastSwitchTime = Time.time;
        state = State.Lowering;
        hitboxGameObject.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        //Kolce domy�lnie s� opuszczone
        //Rozpoczniemy ich podnoszenie po 'interval' sekund od metody Start
        Invoke("StartRaising", interval);
    }

    // Update is called once per frame
    void Update()
    {
        if(state == State.Lowering)
        {
            //Uzyskaj lokalne skalowanie pojemnika na kolce
            Vector3 scale = spikeHolder.localScale;

            //Zaktualizuj skalowanie Y przez liniow� interpolacj� od wysoko�ci maksymalnej do minimalnej
            scale.y = Mathf.Lerp(SpikeHeight, LoweredSpikeHeight, (Time.time - lastSwitchTime) / lowerTime);

            //Zastosuj zaaktualizowane skalowanie wobec pojemnika na kolce
            spikeHolder.localScale = scale;

            //Je�li kolce zako�czy�y opuszczanie 
            if(scale.y == LoweredSpikeHeight)
            {
                //Zaktualizuj stan i przygotuj wywo�anie nast�pnego podnoszenia kolc�w za 'interval' sekund
                Invoke("StartRaising", interval);
                hitboxGameObject.SetActive(false);
                colliderGameObject.SetActive(false);
                state = State.Lowered;
            }
        }
        else if(state == State.Raising)
        {
            //Uzyskaj lokalne skalowanie pojemnika na kolce
            Vector3 scale = spikeHolder.localScale;
            //Zaaktualizuj skalowanie Y przez liniow� interpolacj� od wysoko�ci minimalnej do maksymalnej
            scale.y = Mathf.Lerp(LoweredSpikeHeight, SpikeHeight, (Time.time - lastSwitchTime) / raiseTime);

            //Zastosuj zaktualizowane skalowanie wobec pojemnika na kolce
            spikeHolder.localScale = scale;

            //Je�li kolce zako�czy�y podnoszenie
            if(scale.y == SpikeHeight)
            {
                //Zaaktualizuj stan i przygotuj wywo�anie nast�pnego opuszcznania kolc�w za 'raiseWaitTime' sekund
                Invoke("StartLowering", raiseWaitTime);
                state = State.Raised;
                //Aktywuj bry�� ograniczaj�c�, kt�ra blokuje gracza
                colliderGameObject.SetActive(true);
                //Dezaktywuj obszar trafienia, aby nie zabija� gracza
                hitboxGameObject.SetActive(false);
            }
        }
    }
}
