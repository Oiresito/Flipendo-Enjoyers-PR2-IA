using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAInfo : MonoBehaviour
{
    public OverlayTile activeTileIA; 
    public int rangoMov = 0;
    public int rangoAtaque = 0;
    public int ataque = 0;
    public int vida = 0;
    public GameObject cuadroDeTextoPrefab;
    private GameObject cuadroDeTexto;
    private bool cuadroActivo = false;

    private float changeTime=0.1f;
    private int parpadeos = 0;

    private void OnMouseOver()
    {

        if (Input.GetMouseButtonDown(1) && cuadroActivo == false) // 1 representa el bot�n derecho del rat�n
        {
            // Acciones que deseas que ocurran al hacer clic derecho sobre el personaje
            Vector2 posicionClic = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            posicionClic += new Vector2(0f, 0.3f);

            // Crear el cuadro de texto en la posici�n del clic
            cuadroDeTexto = Instantiate(cuadroDeTextoPrefab, posicionClic, Quaternion.identity);//genero desde el pstart y lo destruyo
            MovDialogo scriptCuadro = cuadroDeTexto.GetComponent<MovDialogo>();

            scriptCuadro.SetValoresText(ataque, vida);
            cuadroActivo = true;
        }
        else if (Input.GetMouseButtonDown(1) && cuadroActivo == true)
        {
            Destroy(cuadroDeTexto);
            cuadroActivo = false;
        }
    }
    public int GetAtaque()
    {
        return ataque;
    }
    public void Pupa(int daño)
    {


        InvokeRepeating("ChangeVisibility", 0f, changeTime);
        vida -= daño;
        //scriptCuadro.SetValoresText(ataque,vida);
        if (vida <= 0)
        {
            Destroy(this.gameObject);
            Destroy(cuadroDeTexto);
        }

    }
    void ChangeVisibility()
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);
        parpadeos++;
        if (parpadeos >= 6)
        {

            parpadeos = 0;
            CancelInvoke("ChangeVisibility");
            gameObject.SetActive(true);
        }
    }

}
