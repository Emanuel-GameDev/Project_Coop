using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Transform startPoint; // Punto di partenza
    public Transform endPoint;   // Punto di arrivo
    public Transform controlPoint; // Punto di controllo per la curva di Bezier
    public float speed = 1f;     // Velocità di movimento

    private float startTime;
    private Vector2[] pathPoints;

    void Start()
    {
        startTime = Time.time;

        // Calcoliamo i punti sulla curva di Bezier
        pathPoints = new Vector2[3];
        pathPoints[0] = startPoint.position;
        pathPoints[1] = controlPoint.position;
        pathPoints[2] = endPoint.position;
    }

    void Update()
    {
        // Calcoliamo la distanza percorsa dall'inizio del movimento
        float distCovered = (Time.time - startTime) * speed;

        // Calcoliamo la percentuale completata del movimento
        float fracJourney = distCovered / Vector2.Distance(startPoint.position, endPoint.position);

        // Utilizziamo la curva di Bezier per ottenere la posizione intermedia
        transform.position = CalculateBezierPoint(fracJourney, pathPoints[0], pathPoints[1], pathPoints[2]);

        // Se l'oggetto ha raggiunto la destinazione, puoi eseguire delle azioni
        if (fracJourney >= 1.0f)
        {
            // E.g., Puoi distruggere l'oggetto o eseguire altre azioni
            Destroy(gameObject);
        }
    }

    // Funzione per calcolare il punto sulla curva di Bezier
    Vector2 CalculateBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector2 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;
        return p;
    }

}
