using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hand : MonoBehaviour
{
    private Vector3 posMouse;

    void Update()
    {
        posMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        posMouse.z = 0;
        transform.position = posMouse;
    }
}
