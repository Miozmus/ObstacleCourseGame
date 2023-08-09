using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectUI : MonoBehaviour
{
    //Indeks budowania aktualnie za³adowanej sceny
    private int currentScene = 0;

    //Kamera podgl¹du poziomu dla bie¿¹cej sceny, o ile istnieje.
    private GameObject levelViewCamera;

    //Trwaj¹ca obecnie operacja ³adowania sceny, o ile istnieje
    private AsyncOperation currentLoadOperation;

    void OnGUI()
    { 
        GUILayout.Label("OBSTACLE COURSE");
        //Jeœli nie jest to menu g³ówne
        if (currentScene != 0)
        {
            GUILayout.Label("Currently viewing level" + currentScene);
            //Poka¿ przycisk play
            if (GUILayout.Button("PLAY"))
            {
                //Jeœli przycisk zosta³ klikniêty, rozpocznij odtwarzanie poziomu
                PlayCurrentLevel();
            }
        }
        else //Jeœli jest to menu g³ówne
            GUILayout.Label("Selecet a level to preview it.");
        //Zaczynaj¹c od indeksu budowania sceny 1, przechodzimy przez pozosta³e sceny
        for (int i = 1; i < SceneManager.sceneCountInBuildSettings;i++)
            {
            //Poka¿ przycisk z tekstem "Level [numer poziomu]"
            if (GUILayout.Button("Level " + i))
            { //Jeœli ten przycisk zostanie naciœniêty i nie oczekujemu aktualnie na za³adowanie sceny
                if (currentLoadOperation == null)
                {
                    //Rozpocznij asynchroniczne ³adowanie poziomu
                    currentLoadOperation = SceneManager.LoadSceneAsync(i);
                    //Ustaw bie¿¹c¹ scenê
                    currentScene = i;
                }
            }
        }
    
    }

    private void PlayCurrentLevel()
    {
        //Dezaktywuj kamere podgl¹du poziomu
        levelViewCamera.SetActive(false);
        //Spróbuj znaleŸæ obiekt Player
        var playerGobj = GameObject.Find("Player");
        if(playerGobj == null)
        {
            Debug.LogError("Couldn't find a Player in the level!");

        }
        else
        {
            //Uzyskaj do³¹czony skrypt Player i w³¹cz go
            var playerScript = playerGobj.GetComponent<Player>();
            playerScript.enabled = true;
            //Poprzez skrypt Player uzyskaj dostêp do obiektu kamera i  aktywuj j¹ 
            playerScript.UpViewCam.SetActive(true);
            //Zniszcz obiekt, do którego jest przypisany ten skrypt; obiekt pojawi siê ponownie, gdy g³ówna scena bêdzie znowu ³adowana
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Zapewniamy, aby ten obiekt nadal istnia³, gdy scena siê zmieni
        DontDestroyOnLoad(gameObject);

    }

    // Update is called once per frame
    void Update()
    {
        //Jeœli mamy aktualn¹ operacjê ³adowania i jest ona zakoñczona 
        if(currentLoadOperation != null && currentLoadOperation.isDone)
        {
            //Przypisz null do operacji ³adowania
            currentLoadOperation = null;
            //ZnajdŸ kamerê podgl¹du poziomu na scenie
            levelViewCamera = GameObject.Find("Level View Camera");
            //Zarejestruj b³¹d, jeœli nie mogliœmy znaleŸæ kamery
            if (levelViewCamera == null)
                Debug.LogError("No level view camera was found in the scene!");
        }
    }
}
