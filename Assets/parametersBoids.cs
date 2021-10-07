using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parametersBoids : MonoBehaviour
{
    //paramètres
    public float distanceVoisin = 5;
    public int maxVelocity = 10;
    public float distanceRepousse = 20.0f;
    public float velociteRapprocher = 40f;
    public float velociteVersVoisins = 100f;
    public float velociteRepousse = 10f;

    //obstacles
    public List<GameObject> obstacles;
}
