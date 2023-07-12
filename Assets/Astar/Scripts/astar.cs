using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class astar : MonoBehaviour
{
    public GameObject joueur;
    public List<GameObject> murs;
    public GameObject test;
    public Text gameover;

    //a*
    private float[,,] grilleBase;
    private float[,,] cout;
    List<Vector3> route = new List<Vector3>(); //plan de route

    public deplacement jr; //script joueur

	void Start()
    {
        jr = joueur.GetComponent<deplacement>();
        grilleBase = new float[50, 50, 1]; //distance depuis méchant
        Array.Clear(grilleBase, 0, grilleBase.Length);

		for (int i = 0; i < 50; i++)
		{
			for (int j = 0; j < 50; j++)
			{
				foreach (var obj in murs)
				{
                    if (obj.GetComponent<Collider>().bounds.Contains(new Vector3(i-25, 0 , j-25)))
                    {
                        grilleBase[i, j, 0] = int.MaxValue;
                        //Instantiate(test, new Vector3(i - 25,0 ,j - 25 ), Quaternion.identity);
                    }
                }
			}
		}

        InvokeRepeating("updateRoute", 1f, 2f);
    }

    // Update is called once per frame
    void Update()
    {
		
		if (route.Count > 0)
		{
			transform.position = Vector3.MoveTowards(transform.position, new Vector3(route[0].x - 25, 0, route[0].y - 25), Time.deltaTime*2);
			//transform.position = new Vector3(route[0].x - 25, 0.05f, route[0].y - 25);
			if (Vector3.Distance(transform.position, new Vector3(route[0].x - 25, 0.05f, route[0].y - 25)) <= 0.1f )
			{
				route.RemoveAt(0);
			}
		}

		if (jr.nbrobjet == 5)
        {
            CancelInvoke();
            gameover.text = "You win !";
            Debug.Log("Gagné");
            jr.nbrobjet++;
        }
    }
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CancelInvoke();
            gameover.text = "Game Over";
            Debug.Log("Perdu");
        }
    }

    void updateRoute() {
        route = algoastar(new Vector3(transform.position.x + 25, transform.position.z + 25, 0), new Vector3(joueur.transform.position.x + 25, joueur.transform.position.z + 25, 0));
    }


    void OnDrawGizmos()
    {
		foreach (var item in route)
		{
            Handles.Label(new Vector3(item.x - 25, 1, item.y - 25), cout[(int)item.x, (int)item.y, 0 ]+"");
        }
    }



    public List<Vector3> algoastar (Vector3 d, Vector3 a)
    {

        List<Vector3> listeouverte = new List<Vector3>(); //contient les positions encore à analyser
        List<Vector3> listefermee = new List<Vector3>(); // contient les voisins vérifiés, qui n'ont plus à être analysé
        float[,,] grille = (float[,,])grilleBase.Clone();
        cout = new float[50, 50, 1];

        Vector3[,,] predecesseur = new Vector3[50,50,1];

        listeouverte.Add(d);
        while (listeouverte.Count>0)
		{
            if (listeouverte.Count <= 0)
            {
                Debug.Log(listeouverte.Count);
            }

            //on prend la plus petite valeur dans la liste ouverte
            Vector3 lowest = lowestPrice(listeouverte, cout);
            //on déplace cette valeur dans la liste fermée
            listefermee.Add(lowest);
            listeouverte.Remove(lowest);
            //si cette valeur est la valeur d'arrivée, l'algo est terminé
            if (lowest == a)
            {
                break;
            }
            //pour toutes les positions adjacentes :
            for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
                    if (lowest.x + i >= grille.GetLength(0)) { continue; }
                    if (lowest.y + j >= grille.GetLength(1)) { continue; }
                    if (lowest.x - i < 0) { continue; }
                    if (lowest.y - j < 0) { continue; }
                    Vector3 temp = new Vector3(lowest.x + i, lowest.y + j, lowest.z);
                    if (!listeouverte.Contains(temp) && !listefermee.Contains(temp) && (grille[(int)lowest.x + i, (int)lowest.y + j, (int)lowest.z] != int.MaxValue))
                    {
                        listeouverte.Add(temp);
                        grille[(int)lowest.x + i, (int)lowest.y + j, (int)lowest.z] = grille[(int)lowest.x, (int)lowest.y, (int)lowest.z] + 1; //nombre de déplacement 
                        cout[(int)lowest.x + i, (int)lowest.y + j, (int)lowest.z] = grille[(int)lowest.x + i, (int)lowest.y + j, (int)lowest.z] + distancevoloiseau(temp, a); //cout jusquà la position
                        predecesseur[(int)lowest.x + i, (int)lowest.y + j, (int)lowest.z] = lowest;
                    }
                }
			}
        }

        //list des sommets du chemin le plus court
        List<Vector3> shortestPath = new List<Vector3>();
        Vector3 current = a;
        int count = 0;
        while (current != d && count < 100)
        {
            shortestPath.Add(current);
            current = predecesseur[(int)current.x, (int)current.y, (int)current.z];
            count++;
        }
        shortestPath.Reverse();
        return shortestPath;
    }

    private Vector3 lowestPrice(List<Vector3> list, float [,,] grille) //point de la liste avec le poids le plus faible
    {
		try
		{
            Vector3 minvct = list[0];
            float min = cout[(int)list[0].x, (int)list[0].y, (int)list[0].z];
            List<Vector3> listtest = new List<Vector3>(list);
            listtest.RemoveAt(0);

            foreach (var item in listtest)
            {
                if (min > cout[(int)item.x, (int)item.y, (int)item.z])
                {
                    min = cout[(int)item.x, (int)item.y, (int)item.z];
                    minvct = item;
                }
            }
            return minvct;
        }
		catch (System.Exception)
		{
            Debug.Log("liste ouverte vide ou pas de cout");
			throw;
		}
        
    }

    private float distancevoloiseau(Vector3 v2, Vector3 v1) {
        return (float)Math.Sqrt(((v2.x - v1.x) * (v2.x - v1.x)) + ((v2.y - v1.y) * (v2.y - v1.y)) + ((v2.z - v1.z) * (v2.z - v1.z)));
    }
}
