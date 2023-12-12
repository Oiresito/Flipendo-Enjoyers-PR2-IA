using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovPowers : MonoBehaviour
{
    public OverlayTile activeTilePowerups;
    // Start is called before the first frame update
    private float velocidad = 2.0f; // Velocidad del movimiento
    private float amplitud = 0.05f; // Amplitud del movimiento
    private float tiempo = 0.0f;
    private float origen=0.0f;
    void Start(){
        origen=transform.position.y;
    }

    void Update()
    {
        tiempo += Time.deltaTime * velocidad; // Actualizar el tiempo basado en la velocidad
        // Calcular la posición en Y utilizando la función seno para el movimiento oscilatorio
        float offsetY = Mathf.Sin(tiempo) * amplitud;
        
        // Actualizar la posición del objeto
        transform.position = new Vector2(transform.position.x, origen+offsetY);
    }

    
}
