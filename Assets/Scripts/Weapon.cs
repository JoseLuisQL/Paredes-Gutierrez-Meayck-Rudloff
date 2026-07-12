using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Camera miCam;
    public float range = 100f;
    public GameObject balaDecal;
    private Vector2 centroCamara;
    private Ray rayo;
    private RaycastHit hit;
    public float tiempoDisparo, tiempoUltimoDisp;
    private Quaternion rotDecal;
    private Vector3 posDecal;
    public int decalIndex;


    public float fuerzaDisparo = 150f;
    public int escopetaDanio = 1;
    public float distanciaDisparo;
    public LayerMask decalLayerMask;

    public GameObject[] decalsPrefabs; //Array de los prefabs 
    public GameObject[] createdDecals; //Array para crear los Decals

    void Awake()
    {
        miCam = gameObject.transform.GetChild(0).GetComponent<Camera>();
        centroCamara.x = Screen.width / 2;
        centroCamara.y = Screen.height / 2;

        this.tiempoUltimoDisp = Time.time;

        for (int decalNum = 0; decalNum < this.createdDecals.Length; decalNum++)
        {
            this.createdDecals[decalNum] = GameObject.Instantiate(this.decalsPrefabs[0], Vector3.zero, Quaternion.identity) as GameObject;
            this.createdDecals[decalNum].GetComponent<Renderer>().enabled = false;
        }
        this.decalIndex = 0;
    }
    void Update()
    {
        if (Input.GetButtonDown("Fire1")) 
        if ((Time.time - tiempoUltimoDisp) > tiempoDisparo)
            disparar();

    }
    private void disparar()
    {
        rayo = miCam.ScreenPointToRay(centroCamara);
        tiempoUltimoDisp = Time.time;

        //if (Physics.Raycast(rayo, out hit, range))
        if (Physics.Raycast(this.rayo, out this.hit, this.distanciaDisparo, this.decalLayerMask))
        {
            rotDecal = Quaternion.FromToRotation(Vector3.forward, hit.normal);
            posDecal = hit.point + hit.normal * 0.1f;

            this.rotDecal = Quaternion.FromToRotation(Vector3.forward, this.hit.normal);
            this.posDecal = this.hit.point + this.hit.normal * 0.01f;
            this.createdDecals[this.decalIndex].transform.position = this.posDecal;
            this.createdDecals[this.decalIndex].transform.rotation = this.rotDecal;
            this.createdDecals[this.decalIndex].transform.parent = null;
            this.createdDecals[this.decalIndex].GetComponent<Renderer>().enabled = true;

            if (hit.collider.tag == "Caja")
            {
                this.createdDecals[this.decalIndex].transform.parent = this.hit.collider.gameObject.transform;
                DestruirCajas salud = hit.collider.GetComponent<DestruirCajas>();
                if (salud != null) salud.Daño(escopetaDanio);
                if (hit.rigidbody != null) 
                {
                    hit.rigidbody.AddForce(-hit.normal *fuerzaDisparo);
                }

            }
            GameObject.Instantiate(balaDecal, posDecal, rotDecal);
        }
    }
}
