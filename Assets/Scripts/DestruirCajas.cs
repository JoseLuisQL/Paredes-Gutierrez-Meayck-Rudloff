using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestruirCajas : MonoBehaviour
{
    public int saludActual = 0;
    public void Daño(int cantidadDaño)
    {
        this.saludActual -= cantidadDaño;
        Debug.Log("Disparo");
        if (this.saludActual < 0)
        {
            gameObject.SetActive(false);
            Debug.Log("destruir" );
        }
    }
}
