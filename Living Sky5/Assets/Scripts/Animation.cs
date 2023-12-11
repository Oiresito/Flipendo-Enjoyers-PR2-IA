using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation : MonoBehaviour
{
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        animator.SetBool("Ataque", true);
        //Invoke("FinAtaque", 1.0f);
    }

    public void FinAtaque()
    {
        animator.SetBool("Ataque", false);
    }
}
