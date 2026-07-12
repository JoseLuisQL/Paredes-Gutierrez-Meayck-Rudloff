using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ctrlPuerta2 : MonoBehaviour
{
    bool abriendo = false;
    bool cerrando = false;
    bool isAbierta = false;
    public Transform puertaPos;
    //public ParticleSystem particle;
    public Vector3 posicionAbierta;
    public Vector3 posicionCerrada;
    public Vector3 posicionFinal;
    public float recorrido, tiempoInicio, tiempoApertura;
    public AudioClip aOpen;
    public AudioClip aClose;
    AudioSource aSource;
    bool sound = false;
    bool sound2 = false;
    // Start is called before the first frame update
    void Start()
    {
        puertaPos = gameObject.transform;
        posicionCerrada = puertaPos.transform.localPosition;
        posicionFinal = new Vector3(4f, 0f, 0f);
        posicionAbierta = posicionCerrada + posicionFinal;
        aSource = gameObject.GetComponent<AudioSource>();
        //particle.Pause();
    }
    private void OnTriggerEnter(Collider collision)
    {
        tiempoInicio = Time.time;
        abriendo = true;
        cerrando = false;
        //sound = false;
        sound2 = true;
        Debug.Log(abriendo);
    }


    private void OnTriggerExit(Collider other)
    {
        tiempoInicio = Time.time;
        cerrando = true;
        abriendo = false;
        //sound2 = false;
        sound = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (abriendo)
        {
            //particle.Play();
            if (!sound)
            {
                aSource.PlayOneShot(aOpen);
                sound = true;
                sound2 = false;
            }

            recorrido = (Time.time - tiempoInicio) / tiempoApertura;
            puertaPos.transform.localPosition = new Vector3(Mathf.Lerp(posicionCerrada.x, posicionAbierta.x, recorrido), transform.position.y,transform.position.z);
            if (puertaPos.localPosition.x == posicionAbierta.x)
            {
                abriendo = false;

            }
        }
        if (cerrando)
        {
            //particle.Clear();
            //particle.Pause();
            if (!sound2)
            {
                aSource.PlayOneShot(aClose);
                sound2 = true;
                sound = false;
            }

            recorrido = (Time.time - tiempoInicio) / tiempoApertura;
            puertaPos.transform.localPosition = new Vector3(Mathf.Lerp(posicionAbierta.x, posicionCerrada.x, recorrido),transform.position.y, transform.position.z);
            if (puertaPos.localPosition.x == posicionCerrada.x)
            {
                cerrando = false;

            }
        }
    }
}
