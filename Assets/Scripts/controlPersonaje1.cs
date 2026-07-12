using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controlPersonaje1 : MonoBehaviour
{
    public CharacterController controller;
    public float velocidad;
    public float gravedad;
    public Vector3 direccion;
    // Start is called before the first frame update
    void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        velocidad = 2f;
        gravedad = -9.8f;
    }

    // Update is called once per frame
    void Update()
    {
        this.direccion = Vector3.zero;
        this.direccion.y -= this.gravedad * Time.deltaTime;
        this.direccion.x = Input.GetAxis("Horizontal") * velocidad;
        this.direccion.z = Input.GetAxis("Vertical") * velocidad;
        this.controller.Move(direccion*Time.deltaTime);
    }

}
