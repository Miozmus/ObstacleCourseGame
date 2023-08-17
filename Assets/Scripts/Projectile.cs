using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("References")]
    public Transform trans;
    [Header("Stats")]
    [Tooltip("O ile jednostek na sekund� do przodu b�dzie porusza� si� ten pocisk.")]
    public float speed = 34;
    [Tooltip("Odleg�o��, na jak� przemie�ci si� pocisk, zanim si� zatrzyma.")]
    public float range = 70;
    private Vector3 spawnPoint;
    // Start is called before the first frame update
    void Start()
    {
        spawnPoint = trans.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Przesu� pocisk wzd�u� jego lokalnej osi Z (do przodu):
        trans.Translate(0, 0, speed * Time.deltaTime, Space.Self);
        //Zniszcz pocisk, jesli przekroczy� sw�j zakres:
        if (Vector3.Distance(trans.position, spawnPoint) >= range)
            Destroy(gameObject);
    }
}
