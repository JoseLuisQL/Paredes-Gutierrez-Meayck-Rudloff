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

    // Shooting VFX
    private ParticleSystem muzzleFlash;
    private AudioSource audioSource;
    public AudioClip shotSound;
    public GameObject bulletHolePrefab;

    private void Awake()
    {
        miCam = gameObject.transform.GetChild(0).GetComponent<Camera>();
        centroCamara.x = Screen.width / 2;
        centroCamara.y = Screen.height / 2;
        tiempoUltimoDisp = Time.time;

        if (createdDecals != null && createdDecals.Length > 0 && decalsPrefabs != null && decalsPrefabs.Length > 0 && decalsPrefabs[0] != null) {
            GameObject pool = new GameObject("DecalPool");
            for (int decalnum = 0; decalnum < createdDecals.Length; decalnum++) {
                createdDecals[decalnum] = GameObject.Instantiate(decalsPrefabs[0], Vector3.zero, Quaternion.identity) as GameObject;
                createdDecals[decalnum].transform.parent = pool.transform;
                createdDecals[decalnum].GetComponent<Renderer>().enabled = false;
            }
        }
        decalIndex = 0;

        // Find muzzle flash on the active weapon (Shotgun)
        var weaponsParent = miCam.transform.Find("Weapons");
        if (weaponsParent != null) {
            foreach (Transform weapon in weaponsParent) {
                if (weapon.gameObject.activeSelf) {
                    var mf = weapon.Find("Muzzle Flash - Orange");
                    if (mf != null) {
                        muzzleFlash = mf.GetComponent<ParticleSystem>();
                        if (muzzleFlash != null) muzzleFlash.Stop();
                    }
                    break;
                }
            }
        }

        // Audio
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
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

        // Muzzle flash VFX
        if (muzzleFlash != null) muzzleFlash.Play();

        // Shot audio
        if (AudioManager.Instance != null) {
            AudioManager.Instance.PlayShootSFX();
        } else if (audioSource != null && shotSound != null) {
            audioSource.PlayOneShot(shotSound);
        }

        if (Physics.Raycast(rayo, out hit, distanciaDisparo, decalLayerMask))
        {
            rotDecal = Quaternion.FromToRotation(Vector3.forward, hit.normal);
            posDecal = hit.point + hit.normal*0.1f;

            // Decal pool
            if (createdDecals != null && createdDecals.Length > 0 && createdDecals[decalIndex] != null) {
                createdDecals[decalIndex].transform.position = posDecal;
                createdDecals[decalIndex].transform.rotation = rotDecal;
                createdDecals[decalIndex].transform.parent = null;
                var r = createdDecals[decalIndex].GetComponent<Renderer>();
                if (r != null) r.enabled = true;
            }

            DestruirCajas salud = hit.collider.GetComponent<DestruirCajas>();
            if (salud != null)
            {
                if (createdDecals != null && createdDecals.Length > 0 && createdDecals[decalIndex] != null)
                    createdDecals[decalIndex].transform.parent = hit.collider.gameObject.transform;
                salud.Daño(escopetaDanio);
                if (hit.rigidbody != null) hit.rigidbody.AddForce(-hit.normal * fuerzaDisparo);
            }
            
            // Zombie / Enemy damage
            EnemyAI enemy = hit.collider.GetComponent<EnemyAI>();
            if (enemy == null) enemy = hit.collider.GetComponentInParent<EnemyAI>();
            if (enemy != null) {
                enemy.perderVida(escopetaDanio);
                if (hit.rigidbody != null) hit.rigidbody.AddForce(-hit.normal * fuerzaDisparo);
            }

            // Bullet hole at impact point
            if (bulletHolePrefab != null) {
                var hole = GameObject.Instantiate(bulletHolePrefab, hit.point + hit.normal * 0.01f, rotDecal);
                Destroy(hole, 5f);
            } else if (balaDecal != null) {
                var hole = GameObject.Instantiate(balaDecal, hit.point, rotDecal);
                Destroy(hole, 5f);
            }

            if (createdDecals != null && createdDecals.Length > 0)
                decalIndex = (decalIndex + 1) % createdDecals.Length;
        }
    }

    void OnGUI()
    {
        // Simple crosshair
        float size = 20f;
        float x = Screen.width / 2 - size / 2;
        float y = Screen.height / 2 - size / 2;
        GUI.Label(new Rect(x, y, size, size), "+",
            new GUIStyle { fontSize = 24, normal = { textColor = Color.white }, alignment = TextAnchor.MiddleCenter });
    }
}