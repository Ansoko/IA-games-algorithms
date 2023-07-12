using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class moveDijkstra : MonoBehaviour
{
	int posAct; //position actuelle de l'ufo
	int goTo;
	List<int> route = new List<int>(); //plan de route
	Graph graph; //graph du jeu
	public positions pos; //positions des sommets du graph


	void Start()
    {
		GameObject thegrid = GameObject.Find("Sommets");
		if (thegrid.TryGetComponent<positions>(out pos)){

			const int inf = int.MaxValue; //infini
			const int nbrSommets = 30;
			posAct = 2;

			graph = new Graph(nbrSommets, new int[nbrSommets, nbrSommets]);
			for (int i = 0; i < nbrSommets; i++)
			{
				for (int j = 0; j < nbrSommets; j++)
				{
					graph.ponderation[i, j] = inf;
				}

			}

			string[] readText = File.ReadAllLines("Assets\\Dijktra\\distances.txt");
			//Console.WriteLine(readText.Length);
			//int nbrChemins = readText.Length;
			foreach (string s in readText)
			{
				string[] subs = s.Split(' ');
				graph.ponderation[int.Parse(subs[0]), int.Parse(subs[1])] = int.Parse(subs[2]);
				graph.ponderation[int.Parse(subs[1]), int.Parse(subs[0])] = int.Parse(subs[2]);
			}

			/*
			//liste des points du chemin
			route = graph.dijkstra(posAct, goTo);
			foreach (var item in route)
			{
				Debug.Log(item);
			}
			*/
		}
	}

	void Update()
	{
		if (route.Count > 0)
		{
			float step = 2f * Time.deltaTime;
			transform.position = Vector3.MoveTowards(transform.position, pos.coordSomm[route[0]], step);
			if (transform.position == pos.coordSomm[route[0]])
			{
				posAct = route[0];
				route.RemoveAt(0);
			}
		}
		
	}


	public void goToPoint(int arrivee)
	{
		route.Clear();
		route = graph.dijkstra(posAct, arrivee);		
	}

	class Graph
	{
		const int inf = int.MaxValue;

		public int[,] ponderation;
		//private Dictionary<int, string> nomSommets { get; set; } //unused
		private int nbrSommets;

		public Graph(int nbrSommets, int[,] ponderation)
		{
			this.ponderation = ponderation;
			this.nbrSommets = nbrSommets;
		}

		public List<int> dijkstra(int depart, int arrivee)
		{
			List<int> listSommets = new List<int>();
			int[] distances = new int[nbrSommets]; //distance entre le sommet de départ et un autre sommet i
			int[] predecesseur = new int[nbrSommets]; //predessesseur du sommet i

			for (int i = 0; i < nbrSommets; i++)
			{
				distances[i] = inf;
				listSommets.Add(i);
			}

			distances[depart] = 0;
			int som = depart;

			while (listSommets.Count > 0)
			{
				//Console.WriteLine(som);
				listSommets.Remove(som);

				foreach (var b in listSommets)
				{
					if (ponderation[som, b] == inf) { continue; } //si ce n'est pas un voisin de som, continuer la boucle
																  //Console.WriteLine("le point "+b + " est à une distance de "+depart+" de " + distances[b] + " ou " + (distances[som] + ponderation[som, b]));
					if (distances[b] > (distances[som] + ponderation[som, b]))
					{
						distances[b] = distances[som] + ponderation[som, b];
						predecesseur[b] = som;
					}

				}
				int min = inf;
				int previoussom = som;
				//quel est le sommet avec le plus court chemin du départ :
				//Console.WriteLine("nombre de sommets non testés : "+listSommets.Count);
				foreach (var b in listSommets)
				{
					if (distances[b] < min)
					{
						som = b;
						min = distances[b];
					}
				}
				if (som == previoussom) { break; } //si le nouveau sommet n'a pas changé, c'est que certains points ne sont pas connectés au sommet de départ, l'algo risque alors de boucler à l'infini
												   //Console.WriteLine("-------prochain point-------");
			}

			//list des sommets du chemin le plus court
			List<int> shortestPath = new List<int>();
			int current = arrivee;
			while (current != depart)
			{
				shortestPath.Add(current);
				current = predecesseur[current];
			}

			shortestPath.Reverse();

			return shortestPath;
		}
	}
}
