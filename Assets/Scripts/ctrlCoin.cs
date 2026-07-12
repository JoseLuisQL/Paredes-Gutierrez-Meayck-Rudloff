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
        if(other.name == "FPSController")
        {
            aSource.PlayOneShot(aCoin);
            other.SendMessage("ganarCoin", 1, SendMessageOptions.DontRequireReceiver);
            
            Destroy(gameObject,0.5f);

        }
    }
}
