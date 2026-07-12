using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ctrlPlayer : MonoBehaviour
{
    int coins = 0;
    int points = 0;
    public TextMeshProUGUI txtCoin;
    public TextMeshProUGUI txtPoints;
    public GameObject continuar;
    // Start is called before the first frame update
    void Start()
    {
        //continuar = GameObject.Find("ContinueMenu").GetComponent<ContinueMenu>();
    }

    // Update is called once per frame
    void Update()
    {
        txtCoin.text = coins.ToString();
        txtPoints.text = points.ToString();
    }
    void ganarCoin(int cant)
    {
        coins += cant;
    }
    void ganarPoints(int point)
    {
        points += point; 
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Fin")
        {
            continuar.GetComponent<ContinueMenu>().setActive();
        }
    }
}
