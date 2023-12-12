using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MovDialogo : MonoBehaviour
{
    private float velocidad = 2.0f;
    private float amplitud = 0.08f;
    private float tiempo = 0.0f;
    public TextMeshProUGUI ataque;
    public TextMeshProUGUI vida;
    private float origen = 0.0f;
    void Start()
    {
        origen = transform.position.y;
    }

    void Update()
    {
        tiempo += Time.deltaTime * velocidad;
        float offsetY = Mathf.Sin(tiempo) * amplitud;

        transform.position = new Vector2(transform.position.x, origen + offsetY);
    }

    public void SetValoresText(int a, int b)
    {
        ataque.text = a.ToString();
        vida.text = b.ToString();

    }
}