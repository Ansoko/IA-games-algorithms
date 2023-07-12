using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class positions : MonoBehaviour
{
    public Dictionary<int, Vector3> coordSomm = new Dictionary<int, Vector3>();
    void Awake()
    {
        coordSomm.Add(0, new Vector3(1, 4, 0));
        coordSomm.Add(1, new Vector3(1, 2, 0));
        coordSomm.Add(2, new Vector3(1, 0, 0));
        coordSomm.Add(3, new Vector3(1, -2, 0));
        coordSomm.Add(4, new Vector3(1, -4, 0));
        coordSomm.Add(5, new Vector3(4, 4, 0));
        coordSomm.Add(6, new Vector3(4, 2, 0));
        coordSomm.Add(7, new Vector3(4, 0, 0));
        coordSomm.Add(8, new Vector3(4, -2, 0));
        coordSomm.Add(9, new Vector3(4, -4, 0));
        coordSomm.Add(10, new Vector3(7, 4, 0));
        coordSomm.Add(11, new Vector3(7, 2, 0));
        coordSomm.Add(12, new Vector3(7, 0, 0));
        coordSomm.Add(13, new Vector3(7, -2, 0));
        coordSomm.Add(14, new Vector3(7, -4, 0));
        coordSomm.Add(15, new Vector3(10, 4, 0));
        coordSomm.Add(16, new Vector3(10, 2, 0));
        coordSomm.Add(17, new Vector3(10, 0, 0));
        coordSomm.Add(18, new Vector3(10, -2, 0));
        coordSomm.Add(19, new Vector3(10, -4, 0));
        coordSomm.Add(20, new Vector3(13, 4, 0));
        coordSomm.Add(21, new Vector3(13, 2, 0));
        coordSomm.Add(22, new Vector3(13, 0, 0));
        coordSomm.Add(23, new Vector3(13, -2, 0));
        coordSomm.Add(24, new Vector3(13, -4, 0));
        coordSomm.Add(25, new Vector3(16, 4, 0));
        coordSomm.Add(26, new Vector3(16, 2, 0));
        coordSomm.Add(27, new Vector3(16, 0, 0));
        coordSomm.Add(28, new Vector3(16, -2, 0));
        coordSomm.Add(29, new Vector3(16, -4, 0));
    }
}
