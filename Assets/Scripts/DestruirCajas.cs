using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestruirCajas : MonoBehaviour
{
    public int saludActual = 0;
    public void Daño(int cantidadDaño) {
        this.saludActual -= cantidadDaño;
        if (this.saludActual <= 0) {
            var enemy = GetComponent<EnemyAI>();
            if (enemy != null) {
                enemy.Die();
            } else {
                gameObject.SetActive(false);
            }
        }
    }
}
