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



}
