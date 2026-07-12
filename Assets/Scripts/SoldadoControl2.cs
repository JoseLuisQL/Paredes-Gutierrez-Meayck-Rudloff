using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldadoControl2 : MonoBehaviour
{
    private Vector3 direccion;
    private CharacterController controlador;

    [Header("Configuración de Velocidad")]
    public float walkingSpeed = 5.0f;
    public float runningSpeed = 10.0f;
    public bool isWalking;

    [Header("Configuración de Cámara")]
    public float mouseSensitivity = 2.0f; // Sensibilidad del ratón

    private Vector3 momentoSpeed;
    Animator animador;

    void Start()
    {
        controlador = gameObject.GetComponent<CharacterController>();
        animador = gameObject.GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(0, mouseX, 0);
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 moveInput = new Vector3(horizontal, 0, vertical);
        direccion = transform.TransformDirection(moveInput);
        isWalking = !Input.GetKey(KeyCode.LeftShift);
        if (isWalking)
        {
            momentoSpeed = walkingSpeed * direccion;
        }
        else
        {
            momentoSpeed = runningSpeed * direccion;
        }

        controlador.SimpleMove(momentoSpeed);
        this.isWalking = !Input.GetKey(KeyCode.LeftShift);
        if (this.isWalking)
        {
            this.momentoSpeed = this.walkingSpeed * this.direccion;
        }
        else
        {
            this.momentoSpeed = this.runningSpeed * this.direccion;
        }
        this.controlador.SimpleMove(this.momentoSpeed);

        if (this.direccion != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(this.direccion);
        }
        Animate();



    }

    void Animate()
    {
        this.animador.SetBool("Caminando", this.isWalking);
        this.animador.SetBool("Corriendo", !this.isWalking);

        this.animador.SetFloat("Velocidad", this.momentoSpeed.magnitude);
    }
}
