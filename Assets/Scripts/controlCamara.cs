using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controlCamara : MonoBehaviour
{
    public GameObject player;
    public GameObject referencia;
    Vector3 distancia;
    public float velRotacion = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        distancia = transform.position - player.transform.position;    
    }

    // Update is called once per frame
    void Update()
    {
        distancia = Quaternion.AngleAxis(Input.GetAxis("Mouse X")*velRotacion,Vector3.up)*distancia;
        transform.position = player.transform.position + distancia;
        transform.LookAt(player.transform.position);
        Vector3 copiaRotacion = new Vector3(0, transform.eulerAngles.y, 0);
        referencia.transform.eulerAngles = copiaRotacion;
    }
}
