using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    //Odwo³ania
    [Header("References")]
    public Transform trans;
    public Transform modelTrans;
    public CharacterController characterController;
    public GameObject UpViewCam;
    public GameObject BackViewCam;
    public DashBar dashBar;

    //Ruch
    [Header("Movement")]
    [Tooltip("Liczba jednostek przesuniêcia w ci¹gu sekundy przy maksymalnej prêdkoœci.")]
    public float movespeed = 24;
    [Tooltip("Czas w sekundach potrzebny na osi¹gniêcie maksymalnej prêdkoœci.")]
    public float timeToMaxSpeed = .26f;
    private float VelocityGainPerSecond { get { return movespeed / timeToMaxSpeed; } }
    [Tooltip("Czas w sekundach potrzebny na przejœcie od maksymalnej prêdkoœci do zatrzymania.")]
    public float timeToLoseMaxSpeed = .2f;
    private float VelocityLossPerSecond {  get { return movespeed / timeToLoseMaxSpeed; } }
    [Tooltip("Mno¿nik dla prêdkoœci przy ruchu w kierunku przeciwnym do aktualnego kierunku poruszania " +
        "(np. przy próbie ruchu w prawo, gdy aktualnie poruszamy siê w lewo).")]
    public float reverseMomentumMultiplier = 2.2f;
    private Vector3 movementVelocity = Vector3.zero;

    //Œmieræ i odradzanie
    [Header("Death and Respawning")]
    [Tooltip("Po jakim czasie ( w sekundach) po smierci gracz siê odradza")]
    public float respawnWaitTime = 2f;
    private bool dead = false;
    private Vector3 spawnPoint;
    private Quaternion spawnRotation;

    //Zryw
    [Header("Dashing")]
    [Tooltip("Ca³kowita liczba jednostek pokonywanych przy wykonywaniu zrywu.")]
    public float dashDistance = 17;
    [Tooltip("Czas, jaki zajmuje zryw w sekundach.")]
    public float dashTime = .26f;
    [Tooltip("Czas po zakoñczeniu zrywu, zanim mo¿na go u¿yæ ponownie.")]
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
            //----- RUCH PRZÓD-TY£ -----
            //Jeœli przytrzymany jest klawisz W lub strza³ka w górê
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                if (movementVelocity.z >= 0) //Jeœli poruszamy siê do przodu
                                             //Zwiêkszamy prêdkoœæ Z o VelocityGainPerSecond, ale nie przekraczamy movespeed
                    movementVelocity.z = Mathf.Min(movespeed, movementVelocity.z + VelocityGainPerSecond * Time.deltaTime);
                else //lub jeœli poruszamy siê do ty³u
                     //Zwiêkszamy prêdkoœæ Z o VelocityGainPerSecond, wykorzystuj¹c reverseMomentumMultiplier
                    movementVelocity.z = Mathf.Min(0, movementVelocity.z + VelocityGainPerSecond * reverseMomentumMultiplier * Time.deltaTime);

            }
            //Jeœli przytrzymany jest klawisz S lub strza³ka w dó³
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                if (movementVelocity.z > 0) //Jeœli poruszamy siê do przodu
                                            //Zwiêkszamy prêdkoœæ Z o VelocityGainPerSecond, ale nie przekraczamy movespeed
                    movementVelocity.z = Mathf.Max(0, movementVelocity.z - VelocityGainPerSecond * reverseMomentumMultiplier * Time.deltaTime);
                else //lub jeœli poruszamy siê do ty³u albo nie poruszamy w ogóle
                    movementVelocity.z = Mathf.Max(-movespeed, movementVelocity.z - VelocityGainPerSecond * Time.deltaTime);

            }
            else // nie s¹ wciœniête ani klawisze do przodu ani do ty³u
            {
                //stopniowe zmierzanie wartoœci prêdkoœci do 0
                if (movementVelocity.z > 0) //Poruszamy siê do przodu
                                            //Zmniejsz prêdkoœæ Z o VelocityLossPerSecond, ale nie poni¿ej 0
                    movementVelocity.z = Mathf.Max(0, movementVelocity.z - VelocityLossPerSecond * Time.deltaTime);
                else // Poruszamy siê do ty³u
                     //Zwiêksz prêdkoœæ Z o VelocityLossPerSecond, ale nie powy¿ej 0
                    movementVelocity.z = Mathf.Min(0, movementVelocity.z + VelocityLossPerSecond * Time.deltaTime);
            }

            //----- RUCH LEWO-PRAWO -----
            //Jeœli przytrzymany jest klawisz D lub strza³ka w prawo
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                if (movementVelocity.x >= 0) //Jeœli poruszamy siê w prawo
                                             //Zwiêkszamy prêdkoœæ X o VelocityGainPerSecond, ale nie przekraczamy movespeed
                    movementVelocity.x = Mathf.Min(movespeed, movementVelocity.x + VelocityGainPerSecond * Time.deltaTime);
                else //lub jeœli poruszamy siê w lewo
                     //Zwiêkszamy prêdkoœæ x o VelocityGainPerSecond, wykorzystuj¹c reverseMomentumMultiplier
                    movementVelocity.x = Mathf.Min(0, movementVelocity.x + VelocityGainPerSecond * reverseMomentumMultiplier * Time.deltaTime);

            }
            //Jeœli przytrzymany jest klawisz A lub strza³ka w lewo
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                if (movementVelocity.x > 0) //Jeœli poruszamy siê w lewo
                                            //Zwiêkszamy prêdkoœæ Z o VelocityGainPerSecond, ale nie przekraczamy movespeed
                    movementVelocity.x = Mathf.Max(0, movementVelocity.x - VelocityGainPerSecond * reverseMomentumMultiplier * Time.deltaTime);
                else //lub jeœli poruszamy siê w prawo albo nie poruszamy w ogóle
                    movementVelocity.x = Mathf.Max(-movespeed, movementVelocity.x - VelocityGainPerSecond * Time.deltaTime);

            }
            else // nie s¹ wciœniête ani klawisze w prawo ani w lewo
            {
                //stopniowe zmierzanie wartoœci prêdkoœci do 0
                if (movementVelocity.x > 0) //Poruszamy siê w prawo
                                            //Zmniejsz prêdkoœæ x o VelocityLossPerSecond, ale nie poni¿ej 0
                    movementVelocity.x = Mathf.Max(0, movementVelocity.x - VelocityLossPerSecond * Time.deltaTime);
                else // Poruszamy siê w lewo
                     //Zwiêksz prêdkoœæ x o VelocityLossPerSecond, ale nie powy¿ej 0
                    movementVelocity.x = Mathf.Min(0, movementVelocity.x + VelocityLossPerSecond * Time.deltaTime);
            }

            //----- RUCH I OBRÓT GRACZA -----
            //Jeœli gracz porusza siê w którymœ kierunku
            if (movementVelocity.x != 0 || movementVelocity.z != 0)
            {
                //Stosowanie wektora prêdkoœci ruchu
                characterController.Move(movementVelocity * Time.deltaTime);
                //Utrzymujemy obrót elementu przetrzymuj¹cego model  w kierunku ostatniego ruchu
                modelTrans.rotation = Quaternion.Slerp(modelTrans.rotation, Quaternion.LookRotation(movementVelocity), .18F);
            }
        }
    }

    private void Dashing()
    {
        if (!CanDashNow)//Jeœli nie mo¿na wykonaæ 
        {
            dashBar.SetCurrentValue(Time.time - dashBeginTime);
        }
        if(!IsDashing) //Nie wykonujemy zrywu
        {
            if(CanDashNow && Input.GetKey(KeyCode.Space))
            {
                //ZnjadŸ kierunek okreœlany przez wciœniête klawisze ruchu
                Vector3 movementDir = Vector3.zero;
                //Jêsli wciœniêty jest klawisz W lub strza³ak w góre
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
                //Jêsli wciœniêty jest przynajmniej jeden klawisz ruchu
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
            //Prze³¹cz stan wstrzymania
            paused = !paused;
            //Jeœli gra jest wstrzymana, ustaw timeScale na o
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
