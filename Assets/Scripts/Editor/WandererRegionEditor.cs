using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WandererRegion))]

public class WandererRegionEditor : Editor
{
    //Szybkie odwo³anie do obiektu docelowego poprzez rzutowanie typu
    private WandererRegion Target
    {
        get
        {
            return (WandererRegion)target;
        }
    }

    //Wysokoœæ rysowanego obszaru
    private const float BoxHeight = 10f;

    void OnSceneGUI()
    {
        //Bia³y kolor uchwytów
        Handles.color = Color.white;
        //Rysuj szkielet szeœcianu odpowiadaj¹cy regionowi wêdrowania
        Handles.DrawWireCube(Target.transform.position + (Vector3.up * BoxHeight * .5f), new Vector3(Target.size.x, BoxHeight, Target.size.z));
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
