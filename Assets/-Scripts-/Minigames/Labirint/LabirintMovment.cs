using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabirintMovment : MonoBehaviour
{
    public float velocitaMovimento = 5f; // Velocità di movimento dell'oggetto
    public Grid grid; // Riferimento al componente Grid

    void Update()
    {
        // Esempio di movimento orizzontale
        float inputOrizzontale = Input.GetAxis("Horizontal");
        float inputVerticale = Input.GetAxis("Vertical");

        // Calcola la direzione del movimento
        Vector3 direzioneMovimento = new Vector3(inputOrizzontale, inputVerticale, 0f);

        // Normalizza la direzione per evitare movimenti diagonali più veloci
        direzioneMovimento.Normalize();

        // Calcola la posizione desiderata sulla griglia
        Vector3 posizioneDesiderata = transform.position + direzioneMovimento * grid.cellSize.x;

        // Quantizza la posizione alla griglia utilizzando il componente Grid
        posizioneDesiderata = grid.GetCellCenterWorld(grid.WorldToCell(posizioneDesiderata));

        // Sposta l'oggetto sulla posizione quantizzata
        transform.position = Vector3.MoveTowards(transform.position, posizioneDesiderata, velocitaMovimento * Time.deltaTime);
    }
}
