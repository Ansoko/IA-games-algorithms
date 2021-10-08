using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hand : MonoBehaviour
{
    private Vector3 posMouse;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        posMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        posMouse.z = 0;
        transform.position = posMouse;
    }
}
