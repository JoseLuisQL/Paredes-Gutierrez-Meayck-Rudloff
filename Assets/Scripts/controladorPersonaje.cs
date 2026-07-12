using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class controladorPersonaje : MonoBehaviour
{
    public CharacterController controlador;
    public float velocidad = 5f;
    public Vector3 direccion;
    public float gravedad = -9.8f;
    public float rotacion;
    public float velrotacion = 5f;
    public float life = 100f;
    public float salto = 10f;
    public death death;
    public bool isDead =false;
    public AudioClip drown;
    AudioSource aSource;
    // Start is called before the first frame update
    public Slider sldVida;
    void Start()
    {
        controlador = gameObject.GetComponent<CharacterController>();
        aSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (controlador.isGrounded)
        {
            if (!isDead)
            {
                direccion = gameObject.transform.TransformDirection(new Vector3(Input.GetAxis("Vertical"), 0, 0)) * velocidad;
                rotacion = Input.GetAxis("Horizontal") * velrotacion;
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    direccion.y += salto * Time.deltaTime * velocidad;
                    controlador.transform.Rotate(Vector3.zero);
                }
            }
            else
            {
                direccion = gameObject.transform.TransformDirection(0, 0, 0);
            }
            
        }
        //controlador.SimpleMove(Vector3.right*velocidad*Time.deltaTime);
        
        direccion -= new Vector3(0,gravedad*Time.deltaTime,0);
        
        controlador.transform.Rotate(0f,rotacion,0f);
        //direccion.z = Input.GetAxis("Vertical")*velocidad;
        controlador.Move(direccion*Time.deltaTime);
        sldVida.value = life;
    }
    public void perderVida(float cantidad)
    {
        life -= cantidad;
        if (life < 0)
        {
            life = 0;
        }
        if(life <= 0)
        {
            isDead = true;
            aSource.PlayOneShot(drown);
            death.setActive();
        }
    }

}
