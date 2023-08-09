using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patroller : MonoBehaviour
{
    //Sta�e:
    private const float rotationSlerpAmount = .68f;
    [Header("References")]
    public Transform trans;
    public Transform modelHolder;
    [Header("Stats")]
    public float movespeed = 30;

    //Zmiennie prywatne:
    private int currentPointIndex;
    private Transform currentPoint;

    private Transform[] patrolPoints;

    //Zwraca list� zawieraj�c� sk�adniki Transform dla ka�dego obiektu podrz�dnego
    //o nazwie zaczynaj�cej si� od "Patrol Point"
    private List<Transform> GetUnsortedPatrolPoints()
    {
        //Uzyskad sk�adnik Transform dla ka�dego obiektu podzednego w Patroller
        Transform[] children = gameObject.GetComponentsInChildren<Transform> ();
        //Zadeklaruj lokaln� list� do przechowywania sk�adnik�w Transform:
        List<Transform> points = new List<Transform>();
        //Przejd� w p�tli przez sk�adniki Transform obiekt�w podrz�dnych
        for(int i=0;i<children.Length;i++)
        {
            //Sprawd� czy nazwa obiektu podrz�dnego zaczyna si� od "Patrol Point"
            if (children[i].gameObject.name.StartsWith("Patrol Point ("))
            {
                //Jesli tak, dodaj do listy 'points':
                points.Add(children[i]);
            }
        }
        return points;
    }
    private void SetCurrentPatrolPoint(int index)
    {
        currentPointIndex = index;
        currentPoint = patrolPoints[index];
    }

    // Start is called before the first frame update
    void Start()
    {
        //Uzyskaj nieposortowana tablic� punkt�w patrolowych
        List<Transform> points = GetUnsortedPatrolPoints();

        //Kontynuuj tylko je�li lista nie jest pusta
        if(points.Count > 0)
        {
            //Przygotuj tablice punkt�w patrolowych
            patrolPoints = new Transform[points.Count];
            for(int i = 0; i < points.Count; i++)
            {
                //Szybkie odwo�anie do bie��cego punktu
                Transform point = points[i];
                //Wyizoluj numer punktu patrolowego z jego nazwy;
                int closingParenthesisIndex = point.gameObject.name.IndexOf(')');
                string indexSubstring = point.gameObject.name.Substring(14, closingParenthesisIndex - 14);
                //przekonwertuj ten numer z �a�cucha znak�w na liczb� ca�kowit�
                int index = Convert.ToInt32(indexSubstring);
                //Wstaw odwo�anie do punktu do naszej tablicy patrolPoints
                patrolPoints[index] = point;
                //Od��cz ka�dy punkt patrolowy od obiektu nadrz�dnego, aby nie porusza� si� wraz z przeszkod�
                point.SetParent(null);
                //Ukryj punkt patrolowy w oknie Hierarchy
                point.gameObject.hideFlags = HideFlags.HideInHierarchy;
            }
            //Zacznij patrolowanie od pierwszego punktu w tablicy;
            SetCurrentPatrolPoint(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(currentPoint != null)
        {
            //Przesu� g��wny element GameObject w kierunku bie��cego punktu
            trans.position = Vector3.MoveTowards(trans.position, currentPoint.position, movespeed * Time.deltaTime);
            //Je�li ju� dotarli�my do tego punktu, zmieniamy bie��cy punkt
            if(trans.position == currentPoint.position)
            {
                //Je�li jeste�my w ostatnim punkcie
                if(currentPointIndex >= patrolPoints.Length - 1)
                {
                    //Ustawiamy jako bie��cy pierwszy punkt patrolowy
                    SetCurrentPatrolPoint(0);
                }
                else
                {
                    SetCurrentPatrolPoint(currentPointIndex + 1);
                }
            }
            //Je�li jeszcze nie dotar� do punktu
            else
            {
                Quaternion lookRotation = Quaternion.LookRotation((currentPoint.position - trans.position).normalized);
                modelHolder.rotation = Quaternion.Slerp(modelHolder.rotation, lookRotation, rotationSlerpAmount);
            }
        }
    }
}
