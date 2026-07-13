using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ctrlCoin : MonoBehaviour
{
    public AudioClip aCoin;
    AudioSource aSource;
    // Start is called before the first frame update
    void Start()
    {
        aSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") || other.name == "Player")
        {
            if (AudioManager.Instance != null) {
                AudioManager.Instance.PlayItemSFX();
            } else if (aSource != null && aCoin != null) {
                aSource.PlayOneShot(aCoin);
            } else if (aCoin != null) {
                AudioSource.PlayClipAtPoint(aCoin, transform.position);
            }
            
            other.SendMessage("ganarCoin", 1, SendMessageOptions.DontRequireReceiver);
            
            if (GameManager.Instance != null) {
                GameManager.Instance.AddItem();
            }
            
            var mr = GetComponent<MeshRenderer>();
            if (mr != null) mr.enabled = false;
            var col = GetComponent<Collider>();
            if (col != null) col.enabled = false;
            
            Destroy(gameObject, 1.0f);
        }
    }
}
