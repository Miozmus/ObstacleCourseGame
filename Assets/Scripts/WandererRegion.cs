using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandererRegion : MonoBehaviour
{
    [Tooltip("Rozmiar obszaru")]
    public Vector3 size;
    public Vector3 GetRandomPointWithin()
    {
        float x = transform.position.x + Random.Range(size.x * -.5f, size.x * .5f);
        float z = transform.position.z + Random.Range(size.z * -.5f, size.z * .5f);
        return new Vector3(x, transform.position.y, z);
    }
    void Awake()
    {
        //Uzyskaj wszystkie obiekty podrzêdne typu Wanderer
        var wanderers = gameObject.GetComponentsInChildren<Wanderer>();
        //PrzejdŸ w pêtli przez te obiekty podrzêdne
        for(int i = 0; i < wanderers.Length; i++)
        {
            //Ustaw ich odwo³anie .region na wyst¹pienie 'this' tego skryptu
            wanderers[i].region = this;
        }
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
