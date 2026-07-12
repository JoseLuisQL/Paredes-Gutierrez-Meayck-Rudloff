using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Camera miCam;
    public float range = 100f;
    public GameObject balaDecal;
    public Vector2 centroCamara;
    private Ray rayo;
    private RaycastHit hit;

    public float tiempoDisparo, tiempoUltimoDisp;

    private Quaternion rotDecal;
    private Vector3 posDecal;

    public float fuerzaDisparo = 150f;
    public int escopetaDanio = 1;
    public float distanciaDisparo;
    public int decalIndex;
    public LayerMask decalLayerMask;

    public GameObject[] decalsPrefabs; //Array de los prefabs
    public GameObject[] createdDecals; //arrays para crear decals

    private void Awake()
    {
        miCam = gameObject.transform.GetChild(0).GetComponent<Camera>();
        centroCamara.x = Screen.width / 2;
        centroCamara.y = Screen.height / 2;
        tiempoUltimoDisp = Time.time;

        for (int decalnum = 0; decalnum < createdDecals.Length; decalnum++) {
            createdDecals[decalnum] = GameObject.Instantiate(decalsPrefabs[0], Vector3.zero, Quaternion.identity) as GameObject;
            createdDecals[decalnum].GetComponent<Renderer>().enabled = false;
        }
        decalIndex = 0;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1")) 
        {
            if ((Time.time - tiempoUltimoDisp)> tiempoDisparo) disparar();
        }
    }

    private void disparar()
    {
        rayo = miCam.ScreenPointToRay(centroCamara);
        tiempoUltimoDisp = Time.time;
        if (Physics.Raycast(rayo, out hit, distanciaDisparo, decalLayerMask))
        {
            rotDecal = Quaternion.FromToRotation(Vector3.forward, hit.normal);
            posDecal = hit.point + hit.normal*0.1f;
            createdDecals[decalIndex].transform.position = posDecal;
            createdDecals[decalIndex].transform.rotation = rotDecal;
            createdDecals[decalIndex].transform.parent = null;
            createdDecals[decalIndex].GetComponent<Renderer>().enabled =true;
            if (hit.collider.tag == "Caja")
            {
                createdDecals[decalIndex].transform.parent = hit.collider.gameObject.transform;
                DestruirCajas salud=hit.collider.GetComponent<DestruirCajas>();

                if (salud != null) salud.Daño(escopetaDanio);
                if (hit.rigidbody != null) { hit.rigidbody.AddForce(-hit.normal * fuerzaDisparo); }
            
            }
            GameObject.Instantiate(balaDecal, hit.point, rotDecal);

        }
    }
}
