using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDropItem : MonoBehaviour
{
    Vector3 mousePosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Vector3 GetMousePos(){
        return Camera.main.WorldToScreenPoint(transform.position);
    }

    private void OnMouseDown(){
        mousePosition = Input.mousePosition - GetMousePos();
    }

    private void OnMouseDrag(){
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition);
    }
}
