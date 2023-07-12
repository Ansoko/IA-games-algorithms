using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateCows : MonoBehaviour
{
    public Rigidbody2D meuh;
    public positions pos;
    public GameObject car;

    //vaisseau
    public GameObject ufoobj;
    public moveDijkstra ufo;

    //score
    public int scoreufo;
    public int scoreplayer;
    public Text countufo;
    public Text countplayer;

    // Start is called before the first frame update
    void Start()
    {
        ufo = ufoobj.GetComponent<moveDijkstra>();

        GameObject thegrid = GameObject.Find("Sommets");
        pos = thegrid.GetComponent<positions>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("+1 pour la voiture ! vroom vroom");
        scoreplayer++;
        if (scoreplayer == 5)
        {
            countplayer.text = "YOU WIN !";
        }
        else
        {
            countplayer.text = scoreplayer.ToString();
            newCowRand();
        }
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        //start the party
        if (Input.GetButtonDown("Fire1")){
            newCowRand();
            scoreufo = 0;
            scoreplayer = 0;
            countufo.text = scoreufo.ToString();
            countplayer.text = scoreplayer.ToString();
        }

        if (ufoobj.transform.position == transform.position)
        {
            Debug.Log("+1 pour l'alien !!!!");
            scoreufo++;
            if (scoreufo == 5)
            {
                countufo.text = "UFO WIN !";
            }
            else
            {
                countufo.text = scoreufo.ToString();
                newCowRand();
            }
            Destroy(gameObject);

        }
        
    }

    void newCowRand()
    {
        int posrand = Random.Range(0, 30);
        Instantiate(meuh, pos.coordSomm[posrand], transform.rotation);
        ufo.goToPoint(posrand);
    }
}
