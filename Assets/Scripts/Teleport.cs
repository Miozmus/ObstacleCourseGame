using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Miejsce, do którego teleport teleportuje.")]
    private Vector3 teleportPosition;
    public Transform teleporter;


    private void Teleporting(Transform t)
    {
        Debug.Log("Teleporting to:" + teleportPosition);
        Debug.Log("Player pos:" + t.position);
        t.position = teleportPosition;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Kolizja!");
        if (other.gameObject.layer == 8) { 
            Transform ptrans = other.GetComponent<Transform>();
            other.GetComponent<CharacterController>().enabled = false;
            Debug.Log("ptrans: "+ ptrans.position);
            if (ptrans != null)
            {
                Teleporting(ptrans);
                other.GetComponent<CharacterController>().enabled = true;
            }
            else
                Debug.Log("Nie ma gracza");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        teleportPosition = teleporter.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
