using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parametersBoids : MonoBehaviour
{
    //paramètres
    public float maxVelocity = 10;
    public float distanceVoisin = 5f;
    public float distanceMain = 12f;
    public float distanceRepousse = 2f;
    public float velociteVersVoisins = 100f;
    public float velociteRapprocher = 40f;
    //public float velociteRepousse = 10f;

    //obstacles
    public List<GameObject> obstacles;
}
