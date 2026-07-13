using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ctrlPuerta : MonoBehaviour
{
    bool abriendo = false;
    bool cerrando = false;
    bool isAbierta = false;
    public Transform puertaPos;
    public ParticleSystem particle;
    public Vector3 posicionAbierta;
    public Vector3 posicionCerrada;
    public Vector3 posicionFinal;
    public float recorrido, tiempoInicio, tiempoApertura;
    public AudioClip aOpen;
    public AudioClip aClose;
    AudioSource aSource;
    bool sound=false;
    bool sound2 = false;
    // Start is called before the first frame update
    void Start()
    {
        puertaPos = gameObject.transform;
        posicionCerrada = puertaPos.transform.localPosition;
        posicionFinal = new Vector3(0f, 0f, 4f);
        posicionAbierta = posicionCerrada - posicionFinal;
        aSource = gameObject.GetComponent<AudioSource>();
        if (particle != null) particle.Pause();
    }
    public bool requiresItems = false;
    
    private void OnTriggerEnter(Collider collision)
    {
        if (requiresItems && GameManager.Instance != null && GameManager.Instance.itemsCollected < GameManager.Instance.itemsRequired) {
            return; // No tienes los items
        }
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
            if (!sound)
            {
                if (particle != null) particle.Play();
                aSource.PlayOneShot(aOpen);
                sound = true;
                sound2 = false;
            }
            
            recorrido = (Time.time - tiempoInicio) / tiempoApertura;
            puertaPos.transform.localPosition = new Vector3(transform.position.x, transform.position.y, Mathf.Lerp(posicionCerrada.z, posicionAbierta.z, recorrido));
            if (puertaPos.localPosition.z == posicionAbierta.z)
            {
                abriendo = false;

            }
        }
        if (cerrando)
        {
            if (!sound2)
            {
                if (particle != null) {
                    particle.Clear();
                    particle.Pause();
                }
                aSource.PlayOneShot(aClose);
                sound2 = true;
                sound = false;
            }
            
            recorrido = (Time.time - tiempoInicio) / tiempoApertura;
            puertaPos.transform.localPosition = new Vector3(transform.position.x, transform.position.y, Mathf.Lerp(posicionAbierta.z, posicionCerrada.z, recorrido));
            if (puertaPos.localPosition.z == posicionCerrada.z)
            {
                cerrando = false;

            }
        }
    }
}
