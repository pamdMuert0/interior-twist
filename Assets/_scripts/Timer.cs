using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Image radialImage;  // La imagen radial
    public float totalTime = 60f;  // Tiempo total para que la imagen se vacíe
    private float currentTime;  // Tiempo actual del contador

    void Start()
    {
        currentTime = totalTime;  // Inicializa el tiempo actual con el tiempo total
    }

    void Update()
    {
        // Reducir el tiempo actual basado en deltaTime
        currentTime -= Time.deltaTime;

        // Asegurarse de que el tiempo no sea negativo
        if (currentTime < 0f)
        {
            currentTime = 0f;
        }

        // Actualizar el fillAmount basado en el tiempo restante
        radialImage.fillAmount = currentTime / totalTime;

        // Opción: Si quieres que ocurra algo cuando el tiempo llega a 0
        if (currentTime == 0f)
        {
            // Acción cuando el tiempo se acaba
            Debug.Log("¡El tiempo se ha acabado!");
        }
    }

    public void reiniciar(){
        currentTime = totalTime;
    }
}
