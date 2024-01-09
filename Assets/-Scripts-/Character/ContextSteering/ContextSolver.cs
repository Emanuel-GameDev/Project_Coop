using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextSolver : MonoBehaviour
{
    public Vector2 GetDirectionToMove(List<SteeringBehaviour> behaviours)
    {
        float[] danger = new float[8];
        float[] interest = new float[8];

        //Loop dei behaviour
        foreach (SteeringBehaviour behaviour in behaviours)
        {
            (danger, interest) = behaviour.GetSteering(danger, interest);
        }

        //sottrae i valori di danger dai valori di interest
        for (int i = 0; i < danger.Length; i++)
        {
            interest[i] = Mathf.Clamp01(interest[i] - danger[i]);
        }

        //get direction media
        Vector2 outputDirection = Vector2.zero;
        for(int i = 0;i < interest.Length;i++)
        {
            outputDirection += Directions.eightDirections[i] * interest[i];
        }
        
        return outputDirection.normalized;

    }
}

public static class Directions
{
    public static List<Vector2> eightDirections = new List<Vector2>{
            new Vector2(0,1).normalized,
            new Vector2(1,1).normalized,
            new Vector2(1,0).normalized,
            new Vector2(1,-1).normalized,
            new Vector2(0,-1).normalized,
            new Vector2(-1,-1).normalized,
            new Vector2(-1,0).normalized,
            new Vector2(-1,1).normalized
        };
}
