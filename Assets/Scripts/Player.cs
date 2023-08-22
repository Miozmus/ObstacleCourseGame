using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    //Odwo�ania
    [Header("References")]
    public Transform trans;
    public Transform modelTrans;
    public CharacterController characterController;
    public GameObject UpViewCam;
    public GameObject BackViewCam;
    public DashBar dashBar;

    //Ruch
    [Header("Movement")]
    [Tooltip("Liczba jednostek przesuni�cia w ci�gu sekundy przy maksymalnej pr�dko�ci.")]
    public float movespeed = 24;
    [Tooltip("Czas w sekundach potrzebny na osi�gni�cie maksymalnej pr�dko�ci.")]
    public float timeToMaxSpeed = .26f;
    private float VelocityGainPerSecond { get { return movespeed / timeToMaxSpeed; } }
    [Tooltip("Czas w sekundach potrzebny na przej�cie od maksymalnej pr�dko�ci do zatrzymania.")]
    public float timeToLoseMaxSpeed = .2f;
    private float VelocityLossPerSecond {  get { return movespeed / timeToLoseMaxSpeed; } }
    [Tooltip("Mno�nik dla pr�dko�ci przy ruchu w kierunku przeciwnym do aktualnego kierunku poruszania " +
        "(np. przy pr�bie ruchu w prawo, gdy aktualnie poruszamy si� w lewo).")]
    public float reverseMomentumMultiplier = 2.2f;
    private Vector3 movementVelocity = Vector3.zero;

    //�mier� i odradzanie
    [Header("Death and Respawning")]
    [Tooltip("Po jakim czasie ( w sekundach) po smierci gracz si� odradza")]
    public float respawnWaitTime = 2f;
    private bool dead = false;
    private Vector3 spawnPoint;
    private Quaternion spawnRotation;

    //Zryw
    [Header("Dashing")]
    [Tooltip("Ca�kowita liczba jednostek pokonywanych przy wykonywaniu zrywu.")]
    public float dashDistance = 17;
    [Tooltip("Czas, jaki zajmuje zryw w sekundach.")]
    public float dashTime = .26f;
    [Tooltip("Czas po zako�czeniu zrywu, zanim mo�na go u�y� ponownie.")]
    public float dashCooldown = 1.8f;
    
    private bool CanDashNow { get { return (Time.time > dashBeginTime + dashTime + dashCooldown); } }
    private bool IsDashing { get { return (Time.time < dashBeginTime + dashTime); } }
    private Vector3 dashDirection;
    private float dashBeginTime = Mathf.NegativeInfinity;

    //pauza
    private bool paused = false;


    private void Movement()
    {
        if (!IsDashing)
        {
            //----- RUCH PRZ�D-TY� -----
            //Je�li przytrzymany jest klawisz W lub strza�ka w g�r�
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                if (movementVelocity.z >= 0) //Je�li poruszamy si� do przodu
                                             //Zwi�kszamy pr�dko�� Z o VelocityGainPerSecond, ale nie przekraczamy movespeed
                    movementVelocity.z = Mathf.Min(movespeed, movementVelocity.z + VelocityGainPerSecond * Time.deltaTime);
                else //lub je�li poruszamy si� do ty�u
                     //Zwi�kszamy pr�dko�� Z o VelocityGainPerSecond, wykorzystuj�c reverseMomentumMultiplier
                    movementVelocity.z = Mathf.Min(0, movementVelocity.z + VelocityGainPerSecond * reverseMomentumMultiplier * Time.deltaTime);

            }
            //Je�li przytrzymany jest klawisz S lub strza�ka w d�
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                if (movementVelocity.z > 0) //Je�li poruszamy si� do przodu
                                            //Zwi�kszamy pr�dko�� Z o VelocityGainPerSecond, ale nie przekraczamy movespeed
                    movementVelocity.z = Mathf.Max(0, movementVelocity.z - VelocityGainPerSecond * reverseMomentumMultiplier * Time.deltaTime);
                else //lub je�li poruszamy si� do ty�u albo nie poruszamy w og�le
                    movementVelocity.z = Mathf.Max(-movespeed, movementVelocity.z - VelocityGainPerSecond * Time.deltaTime);

            }
            else // nie s� wci�ni�te ani klawisze do przodu ani do ty�u
            {
                //stopniowe zmierzanie warto�ci pr�dko�ci do 0
                if (movementVelocity.z > 0) //Poruszamy si� do przodu
                                            //Zmniejsz pr�dko�� Z o VelocityLossPerSecond, ale nie poni�ej 0
                    movementVelocity.z = Mathf.Max(0, movementVelocity.z - VelocityLossPerSecond * Time.deltaTime);
                else // Poruszamy si� do ty�u
                     //Zwi�ksz pr�dko�� Z o VelocityLossPerSecond, ale nie powy�ej 0
                    movementVelocity.z = Mathf.Min(0, movementVelocity.z + VelocityLossPerSecond * Time.deltaTime);
            }

            //----- RUCH LEWO-PRAWO -----
            //Je�li przytrzymany jest klawisz D lub strza�ka w prawo
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                if (movementVelocity.x >= 0) //Je�li poruszamy si� w prawo
                                             //Zwi�kszamy pr�dko�� X o VelocityGainPerSecond, ale nie przekraczamy movespeed
                    movementVelocity.x = Mathf.Min(movespeed, movementVelocity.x + VelocityGainPerSecond * Time.deltaTime);
                else //lub je�li poruszamy si� w lewo
                     //Zwi�kszamy pr�dko�� x o VelocityGainPerSecond, wykorzystuj�c reverseMomentumMultiplier
                    movementVelocity.x = Mathf.Min(0, movementVelocity.x + VelocityGainPerSecond * reverseMomentumMultiplier * Time.deltaTime);

            }
            //Je�li przytrzymany jest klawisz A lub strza�ka w lewo
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                if (movementVelocity.x > 0) //Je�li poruszamy si� w lewo
                                            //Zwi�kszamy pr�dko�� Z o VelocityGainPerSecond, ale nie przekraczamy movespeed
                    movementVelocity.x = Mathf.Max(0, movementVelocity.x - VelocityGainPerSecond * reverseMomentumMultiplier * Time.deltaTime);
                else //lub je�li poruszamy si� w prawo albo nie poruszamy w og�le
                    movementVelocity.x = Mathf.Max(-movespeed, movementVelocity.x - VelocityGainPerSecond * Time.deltaTime);

            }
            else // nie s� wci�ni�te ani klawisze w prawo ani w lewo
            {
                //stopniowe zmierzanie warto�ci pr�dko�ci do 0
                if (movementVelocity.x > 0) //Poruszamy si� w prawo
                                            //Zmniejsz pr�dko�� x o VelocityLossPerSecond, ale nie poni�ej 0
                    movementVelocity.x = Mathf.Max(0, movementVelocity.x - VelocityLossPerSecond * Time.deltaTime);
                else // Poruszamy si� w lewo
                     //Zwi�ksz pr�dko�� x o VelocityLossPerSecond, ale nie powy�ej 0
                    movementVelocity.x = Mathf.Min(0, movementVelocity.x + VelocityLossPerSecond * Time.deltaTime);
            }

            //----- RUCH I OBR�T GRACZA -----
            //Je�li gracz porusza si� w kt�rym� kierunku
            if (movementVelocity.x != 0 || movementVelocity.z != 0)
            {
                //Stosowanie wektora pr�dko�ci ruchu
                characterController.Move(movementVelocity * Time.deltaTime);
                //Utrzymujemy obr�t elementu przetrzymuj�cego model  w kierunku ostatniego ruchu
                modelTrans.rotation = Quaternion.Slerp(modelTrans.rotation, Quaternion.LookRotation(movementVelocity), .18F);
            }
        }
    }

    private void Dashing()
    {
        if (!CanDashNow)//Je�li nie mo�na wykona� 
        {
            dashBar.SetCurrentValue(Time.time - dashBeginTime);
        }
        if(!IsDashing) //Nie wykonujemy zrywu
        {
            if(CanDashNow && Input.GetKey(KeyCode.Space))
            {
                //Znjad� kierunek okre�lany przez wci�ni�te klawisze ruchu
                Vector3 movementDir = Vector3.zero;
                //J�sli wci�ni�ty jest klawisz W lub strza�ak w g�re
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                {
                    movementDir.z = 1;
                }
                else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                {
                    movementDir.z = -1;
                }
                if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                {
                    movementDir.x = 1;
                }
                else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                {
                    movementDir.x = -1;
                }
                //J�sli wci�ni�ty jest przynajmniej jeden klawisz ruchu
                if(movementDir.x != 0 || movementDir.z != 0)
                {
                    //Rozpocznij zryw
                    dashDirection = movementDir;
                    dashBeginTime = Time.time;
                    movementVelocity = dashDirection * movespeed;
                    modelTrans.forward = dashDirection;
                    dashBar.ResetTime();
                }
            }
        }
        else //Zryw jest w trackie wykonywania
        {
            characterController.Move(dashDirection * (dashDistance / dashTime) * Time.deltaTime);
        }
    }

    public void Die()
    {
        if (!dead)
        {
            dead = true;
            Invoke("Respawn", respawnWaitTime);
            movementVelocity = Vector3.zero;
            enabled = false;
            characterController.enabled = false;
            modelTrans.gameObject.SetActive(false);
            dashBeginTime = Mathf.NegativeInfinity;
            dashBar.ResetTime();
        }
    }

    public void Respawn()
    {
        dead = false;
        trans.position = spawnPoint;
        enabled = true;
        characterController.enabled = true;
        modelTrans.gameObject.SetActive(true);
        modelTrans.rotation = spawnRotation;

    }
    private void Pausing()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            //Prze��cz stan wstrzymania
            paused = !paused;
            //Je�li gra jest wstrzymana, ustaw timeScale na o
            if (paused)
                Time.timeScale = 0;
            else
                Time.timeScale = 1;
        }
    }

    void OnGUI()
    {
        if(paused)
        {
            float boxWidth = Screen.width * .4f;
            float boxHeight = Screen.height * .4f;
            GUILayout.BeginArea(new Rect((Screen.width * .5f) - (boxWidth * .5f),(Screen.height * .5f) - (boxHeight * .5f), boxWidth, boxHeight));

            if (GUILayout.Button("RESUME GAME", GUILayout.Height(boxHeight * .5f)))
            {
                paused = false;
                Time.timeScale = 1;
            }
            if(GUILayout.Button("RETURN TO MAIN MENU", GUILayout.Height(boxHeight * .5f)))
            {
                Time.timeScale = 1;
                SceneManager.LoadScene(0);
            }
            GUILayout.EndArea();
        }
    }

    private void ChangeCamera()
    {
        if (Input.GetKey(KeyCode.C))
        {
            UpViewCam.SetActive(false);
            UpViewCam.GetComponent<AudioListener>().enabled = false;
            BackViewCam.SetActive(true);
            BackViewCam.GetComponent<AudioListener>().enabled = true;
        }
        else
        {
            UpViewCam.SetActive(true);
            UpViewCam.GetComponent<AudioListener>().enabled = true;
            BackViewCam.SetActive(false);
            BackViewCam.GetComponent<AudioListener>().enabled = false;

        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        spawnPoint = trans.position;
        spawnRotation = modelTrans.rotation;
        dashBar.SetMaxCooldown(dashCooldown);
        dashBar.StartdashBar();
    }

    // Update is called once per frame
    void Update()
    {
        if (!paused)
        {
            Movement();
            Dashing();
            ChangeCamera();
        }
        Pausing();
    }
}
