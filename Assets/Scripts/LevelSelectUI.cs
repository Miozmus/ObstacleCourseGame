using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectUI : MonoBehaviour
{
    //Indeks budowania aktualnie za�adowanej sceny
    private int currentScene = 0;

    //Kamera podgl�du poziomu dla bie��cej sceny, o ile istnieje.
    private GameObject levelViewCamera;

    //Trwaj�ca obecnie operacja �adowania sceny, o ile istnieje
    private AsyncOperation currentLoadOperation;

    void OnGUI()
    { 
        GUILayout.Label("OBSTACLE COURSE");
        //Je�li nie jest to menu g��wne
        if (currentScene != 0)
        {
            GUILayout.Label("Currently viewing level" + currentScene);
            //Poka� przycisk play
            if (GUILayout.Button("PLAY"))
            {
                //Je�li przycisk zosta� klikni�ty, rozpocznij odtwarzanie poziomu
                PlayCurrentLevel();
            }
        }
        else //Je�li jest to menu g��wne
            GUILayout.Label("Selecet a level to preview it.");
        //Zaczynaj�c od indeksu budowania sceny 1, przechodzimy przez pozosta�e sceny
        for (int i = 1; i < SceneManager.sceneCountInBuildSettings;i++)
            {
            //Poka� przycisk z tekstem "Level [numer poziomu]"
            if (GUILayout.Button("Level " + i))
            { //Je�li ten przycisk zostanie naci�ni�ty i nie oczekujemu aktualnie na za�adowanie sceny
                if (currentLoadOperation == null)
                {
                    //Rozpocznij asynchroniczne �adowanie poziomu
                    currentLoadOperation = SceneManager.LoadSceneAsync(i);
                    //Ustaw bie��c� scen�
                    currentScene = i;
                }
            }
        }
    
    }

    private void PlayCurrentLevel()
    {
        //Dezaktywuj kamere podgl�du poziomu
        levelViewCamera.SetActive(false);
        //Spr�buj znale�� obiekt Player
        var playerGobj = GameObject.Find("Player");
        if(playerGobj == null)
        {
            Debug.LogError("Couldn't find a Player in the level!");

        }
        else
        {
            //Uzyskaj do��czony skrypt Player i w��cz go
            var playerScript = playerGobj.GetComponent<Player>();
            playerScript.enabled = true;
            //Poprzez skrypt Player uzyskaj dost�p do obiektu kamera i  aktywuj j� 
            playerScript.UpViewCam.SetActive(true);
            //Zniszcz obiekt, do kt�rego jest przypisany ten skrypt; obiekt pojawi si� ponownie, gdy g��wna scena b�dzie znowu �adowana
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Zapewniamy, aby ten obiekt nadal istnia�, gdy scena si� zmieni
        DontDestroyOnLoad(gameObject);

    }

    // Update is called once per frame
    void Update()
    {
        //Je�li mamy aktualn� operacj� �adowania i jest ona zako�czona 
        if(currentLoadOperation != null && currentLoadOperation.isDone)
        {
            //Przypisz null do operacji �adowania
            currentLoadOperation = null;
            //Znajd� kamer� podgl�du poziomu na scenie
            levelViewCamera = GameObject.Find("Level View Camera");
            //Zarejestruj b��d, je�li nie mogli�my znale�� kamery
            if (levelViewCamera == null)
                Debug.LogError("No level view camera was found in the scene!");
        }
    }
}
