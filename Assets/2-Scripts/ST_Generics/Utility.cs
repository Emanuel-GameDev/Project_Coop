using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public static class Utility
{

    /// <summary>
    /// Controlla se un oggetto è nella layer mask specificata.
    /// </summary>
    /// <param name="obj">L'oggetto da controllare.</param>
    /// <param name="layerMask">La layer mask da verificare.</param>
    /// <returns>Restituisce true se l'oggetto è nella layer mask, altrimenti restituisce false.</returns>
    public static bool IsInLayerMask(GameObject obj, LayerMask layerMask)
    {
        int objLayerMask = 1 << obj.layer;
        return (layerMask.value & objLayerMask) > 0;
    }

    /// <summary>
    /// Verifica se il layer di un oggetto è presente in una LayerMask.
    /// </summary>
    /// <param name="targetMask">La LayerMask da controllare.</param>
    /// <param name="gameObject">L'oggetto il cui layer deve essere verificato.</param>
    /// <returns>Restituisce true se il layer dell'oggetto è presente nella LayerMask, altrimenti restituisce false.</returns>
    public static bool IsInLayerMask(int layer, LayerMask targetMask)
    {
        int layerbit = 1 << layer;
        return (targetMask & layerbit) != 0;
    }

  
    public static void DebugTrace()
    {
        // Ottieni il nome dello script e della funzione
        StackTrace stackTrace = new StackTrace();
        string callingFunction = stackTrace.GetFrame(1).GetMethod().Name;
        string callingScript = stackTrace.GetFrame(1).GetMethod().ReflectedType.Name;

        // Scrivi nel file di log di debug
        UnityEngine.Debug.Log($"{callingScript} {callingFunction}");
    }
    public static void DebugTrace(string extraText)
    {
        // Ottieni il nome dello script e della funzione
        StackTrace stackTrace = new StackTrace();
        string callingFunction = stackTrace.GetFrame(1).GetMethod().Name;
        string callingScript = stackTrace.GetFrame(1).GetMethod().ReflectedType.Name;

        // Scrivi nel file di log di debug
        UnityEngine.Debug.Log($"{callingScript} {callingFunction} | {extraText}");
    }

    public static T InstantiateCondition<T>() where T : Condition
    {
        GameObject conditionGO = new GameObject();
        conditionGO.name = typeof(T).Name;
        T newCondition = conditionGO.AddComponent<T>();
        return newCondition;
    }

    public static List<T> Shuffle<T>(List<T> list) 
    {
        int count = list.Count;
        int last = count - 1;
        for (int i = 0; i < last; ++i)
        {
            int r = UnityEngine.Random.Range(i, count);
            T tmp = list[i];
            list[i] = list[r];
            list[r] = tmp;
        }
        return list;
    }

    public static Vector2 ZtoY(Vector3 vector)
    {
        return new Vector2(vector.x, vector.z);
    }

    public static Vector3 YtoZ(Vector2 vector)
    {
        return new Vector3(vector.x, 0, vector.y);
    }

    public static float DistanceV3toV2(Vector3 v1, Vector3 v2)
    {
        Vector2 first = ZtoY(v1);
        Vector2 second = ZtoY(v2);
        return Vector2.Distance(first, second);
    }

    public static IEnumerator WaitForPlayers(Action functionToCall, bool waitNextFrame = false)
    {
        yield return new WaitUntil(() => CoopManager.Instance.GetActiveHandlers() != null && CoopManager.Instance.GetActiveHandlers().Count > 0);
        if(waitNextFrame )
            yield return new WaitForEndOfFrame();
        functionToCall();
    }

    public static IEnumerator WaitForPlayers(Action functionToCall, float extraWaitTime)
    {
        yield return new WaitUntil(() => CoopManager.Instance.GetActiveHandlers() != null && CoopManager.Instance.GetActiveHandlers().Count > 0);
        yield return new WaitForSeconds(extraWaitTime);
        functionToCall();
    }
    public static IEnumerator WaitForLoadingDone(Action functionToCall)
    {
        yield return new WaitUntil(() => !GameManager.Instance.IsLoading);
        functionToCall();
    }
    public static IEnumerator WaitForLoadingDone(Action functionToCall, bool waitNextFrame = false)
    {
        yield return new WaitUntil(() => !GameManager.Instance.IsLoading);
        if (waitNextFrame)
            yield return new WaitForEndOfFrame();
        functionToCall();
    }

    public static IEnumerator WaitForLoadingDone(Action functionToCall, float extraWaitTime)
    {
        yield return new WaitUntil(() => !GameManager.Instance.IsLoading);
        yield return new WaitForSeconds(extraWaitTime);
        functionToCall();
    }

    public static Color HexToColor(string hex)
    {
        // Converti il valore esadecimale in un oggetto Color
        Color color = Color.white;
        ColorUtility.TryParseHtmlString(hex, out color);
        return color;
    }

    public static bool FindNearestObjectWithComponent<T>(Vector3 centerPosition, float searchRadius, out T componentFound) where T : Component
    {
        // Ottieni tutti i collider all'interno del raggio di ricerca
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(centerPosition, searchRadius);

        componentFound = null;

        if (hitColliders.Length == 0)
        {
            return false;
        }

        // Variabili per tenere traccia dell'oggetto più vicino
        T nearestObject = null;
        float nearestDistance = Mathf.Infinity;

        // Cerca l'oggetto più vicino con il componente desiderato
        foreach (Collider2D collider in hitColliders)
        {
            T component = collider.GetComponent<T>();
            if (component != null)
            {
                float distance = Vector2.Distance(centerPosition, collider.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestObject = component;
                }
            }
        }

        // Output dell'oggetto più vicino
        if (nearestObject != null)
        {
            componentFound = nearestObject;
            return true;
        }
        else
            return false;
    }

}
