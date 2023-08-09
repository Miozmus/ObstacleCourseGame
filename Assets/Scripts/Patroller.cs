using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patroller : MonoBehaviour
{
    //Sta³e:
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

    //Zwraca listê zawieraj¹c¹ sk³adniki Transform dla ka¿dego obiektu podrzêdnego
    //o nazwie zaczynaj¹cej siê od "Patrol Point"
    private List<Transform> GetUnsortedPatrolPoints()
    {
        //Uzyskad sk³adnik Transform dla ka¿dego obiektu podzednego w Patroller
        Transform[] children = gameObject.GetComponentsInChildren<Transform> ();
        //Zadeklaruj lokaln¹ listê do przechowywania sk³adników Transform:
        List<Transform> points = new List<Transform>();
        //PrzejdŸ w pêtli przez sk³adniki Transform obiektów podrzêdnych
        for(int i=0;i<children.Length;i++)
        {
            //SprawdŸ czy nazwa obiektu podrzêdnego zaczyna siê od "Patrol Point"
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
        //Uzyskaj nieposortowana tablicê punktów patrolowych
        List<Transform> points = GetUnsortedPatrolPoints();

        //Kontynuuj tylko jeœli lista nie jest pusta
        if(points.Count > 0)
        {
            //Przygotuj tablice punktów patrolowych
            patrolPoints = new Transform[points.Count];
            for(int i = 0; i < points.Count; i++)
            {
                //Szybkie odwo³anie do bie¿¹cego punktu
                Transform point = points[i];
                //Wyizoluj numer punktu patrolowego z jego nazwy;
                int closingParenthesisIndex = point.gameObject.name.IndexOf(')');
                string indexSubstring = point.gameObject.name.Substring(14, closingParenthesisIndex - 14);
                //przekonwertuj ten numer z ³añcucha znaków na liczbê ca³kowit¹
                int index = Convert.ToInt32(indexSubstring);
                //Wstaw odwo³anie do punktu do naszej tablicy patrolPoints
                patrolPoints[index] = point;
                //Od³¹cz ka¿dy punkt patrolowy od obiektu nadrzêdnego, aby nie porusza³ siê wraz z przeszkod¹
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
            //Przesuñ g³ówny element GameObject w kierunku bie¿¹cego punktu
            trans.position = Vector3.MoveTowards(trans.position, currentPoint.position, movespeed * Time.deltaTime);
            //Jeœli ju¿ dotarliœmy do tego punktu, zmieniamy bie¿¹cy punkt
            if(trans.position == currentPoint.position)
            {
                //Jeœli jesteœmy w ostatnim punkcie
                if(currentPointIndex >= patrolPoints.Length - 1)
                {
                    //Ustawiamy jako bie¿¹cy pierwszy punkt patrolowy
                    SetCurrentPatrolPoint(0);
                }
                else
                {
                    SetCurrentPatrolPoint(currentPointIndex + 1);
                }
            }
            //Jeœli jeszcze nie dotar³ do punktu
            else
            {
                Quaternion lookRotation = Quaternion.LookRotation((currentPoint.position - trans.position).normalized);
                modelHolder.rotation = Quaternion.Slerp(modelHolder.rotation, lookRotation, rotationSlerpAmount);
            }
        }
    }
}
