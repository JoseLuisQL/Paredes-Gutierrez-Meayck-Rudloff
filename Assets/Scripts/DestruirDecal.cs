using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestruirDecal : MonoBehaviour
{
    public float tiempoDecal = 3f;
        void Start()
    {
        Destroy(gameObject, tiempoDecal);
    }

    
}
