using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldadoControl : MonoBehaviour
{
    private Vector3 direccion;
    private CharacterController controlador;
    public float walkingSpeed;
    public float runningSpeed;
    public bool iswalking;

    public Vector3 momentoSpeed;
    Animator animator;

    void Start()
    {
        controlador = gameObject.GetComponent<CharacterController>();
        animator = gameObject.GetComponent<Animator>();

    }
    void Animate() {
        animator.SetBool("Caminando", iswalking);
        animator.SetBool("Corriendo", !iswalking);
        animator.SetFloat("Velocidad", momentoSpeed.magnitude);

    }

    void Update()
    {
        direccion = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        iswalking = !Input.GetKey(KeyCode.LeftShift);
        if (iswalking)
        {
            momentoSpeed = walkingSpeed * direccion;
        }
        else {
            momentoSpeed = runningSpeed * direccion;
        }
        controlador.SimpleMove(momentoSpeed);
        Animate();
    }

}
