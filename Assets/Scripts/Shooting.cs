using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    [Header("References")]
    public Transform spawnPoint;
    public GameObject projectilePrefab;
    [Header("Stats")]
    [Tooltip("Czas (w sekundach) pomiêdzy wystrzeleniem kolejnych pocisków.")]
    public float fireRate = 1;
    private float lastFireTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time >= lastFireTime +  fireRate)
        {
            lastFireTime = Time.time;
            Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
