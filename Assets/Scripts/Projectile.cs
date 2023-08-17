using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("References")]
    public Transform trans;
    [Header("Stats")]
    [Tooltip("O ile jednostek na sekundê do przodu bêdzie poruszaæ siê ten pocisk.")]
    public float speed = 34;
    [Tooltip("Odleg³oœæ, na jak¹ przemieœci siê pocisk, zanim siê zatrzyma.")]
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
        //Przesuñ pocisk wzd³u¿ jego lokalnej osi Z (do przodu):
        trans.Translate(0, 0, speed * Time.deltaTime, Space.Self);
        //Zniszcz pocisk, jesli przekroczy³ swój zakres:
        if (Vector3.Distance(trans.position, spawnPoint) >= range)
            Destroy(gameObject);
    }
}
