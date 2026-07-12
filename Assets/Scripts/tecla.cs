using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tecla : MonoBehaviour
{
    Transform tamano;
    public Vector3 crecimiento = new Vector3(0f,0.5f,0f);
    Vector3 tamOriginal = new Vector3(0f, 0f, 0f);
    AudioSource aSource;
    public bool tipoPiano =true;
    float timer;
    float espera=0.1f;
    public float tiempoEntreSonidos = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        tamano = this.GetComponent<Transform>();
        tamOriginal = tamano.localScale;
        aSource = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnMouseDown()
    {
        tocar();
            
    }
    private void OnMouseOver()
    {
        //tocar();
    }
    private void OnMouseUp()
    {
        tamano.localScale = tamOriginal;
        //aSource.Stop();
    }
    public void tocar()
    {
        if (tipoPiano)
        {
            aSource.PlayOneShot(this.GetComponent<AudioSource>().clip);

            tamano.localScale = tamOriginal - crecimiento;
            Invoke("volverTamano", espera);
            //tamano.localScale = tamOriginal;
        }
        //else
        //{
        //    timer = Time.deltaTime;
        //    if (timer > tiempoEntreSonidos)
        //    {
        //        aSource.Play();
        //        timer = 0;
        //    }
        //    tamano.localScale = tamOriginal - crecimiento;
        //}
    }
    void volverTamano()
    {
        tamano.localScale = tamOriginal;
        
    }
}
