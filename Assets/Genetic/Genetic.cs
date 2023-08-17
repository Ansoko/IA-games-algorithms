using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Genetic : MonoBehaviour
{
	public class Person
	{
		public int daysSurvived;
		public string choices;
		public int meadow;
		public Person()
		{
			daysSurvived = 0;
			choices = "";
			meadow = 0;
		}
	}

	static readonly System.Random aleatoire = new();
	static string getChromosome(int size)
	{
		string chromosome = "";
		for (int i = 0; i < size; i++)
		{
			chromosome += aleatoire.Next(2);
		}
		return chromosome;
	}

	//parameters
	public int timeReproduceBoar = 115;
	public int startWithBoars = 25;
	public int daysBeforeDie = 7;
	public int nbrIndividus = 500;
	public int nbrGenerations = 1000;
	public int daysToSurvive = 500;
	public int probability = 100;// 1/100 chance for mutation
	public int litter = 6; 

	[SerializeField] private TMPro.TMP_InputField timereproduceInput;
	[SerializeField] private TMPro.TMP_InputField startingnumberInput;
	[SerializeField] private TMPro.TMP_InputField lifeInput;
	[SerializeField] private TMPro.TMP_InputField litterInput;
	[SerializeField] private TMPro.TMP_InputField indiviualsInput;
	[SerializeField] private TMPro.TMP_InputField nbrGenerationsInput;
	[SerializeField] private TMPro.TMP_InputField daysToSurviveInput;
	[SerializeField] private TMPro.TMP_InputField mutationInput;

	[SerializeField] private TMPro.TMP_Text textresult;


	public void Seeresult()
	{
		StartCoroutine(Algorithm());
	}

	private IEnumerator Algorithm(){

		timeReproduceBoar = int.Parse(timereproduceInput.text);
		startWithBoars = int.Parse(startingnumberInput.text);
		daysBeforeDie = int.Parse(lifeInput.text);
		nbrIndividus = int.Parse(indiviualsInput.text);
		nbrGenerations = int.Parse(nbrGenerationsInput.text);
		daysToSurvive = int.Parse(daysToSurviveInput.text);
		probability = int.Parse(mutationInput.text);
		litter = int.Parse(litterInput.text) / 2; // litter of the boars (nbr boars/2 * litter)
		Debug.Log("parameters : " + timeReproduceBoar + " // "+ nbrIndividus);

		//choice for the individues : kill a boar (1) or wait next day (0)
		Person[] pop = new Person[nbrIndividus];
		Person best = new Person();

		//definition
		for (int i = 0; i < nbrIndividus; i++)
		{
			pop[i] = new Person();
			pop[i].choices = getChromosome(daysToSurvive);
			//Debug.LogWarning(pop[i].choices); //show all the starting choices
		}

		for (int g = 0; g < nbrGenerations; g++)
		{
			//Debug.Log("____________ Generation " + g + " ____________");

			//evaluation (fitness = nombre de jours survécus)
			for (int i = 0; i < nbrIndividus; i++)
			{
				int daysWithoutEat = 0;
				int meadow = startWithBoars;
				pop[i].daysSurvived = 0;
				foreach (var choice in pop[i].choices)
				{
					if ((pop[i].daysSurvived + 1) % timeReproduceBoar == 0)
					{
						if (meadow == 2147483647) continue;
						meadow += (int)Math.Floor((double)meadow * litter);
						if (meadow >= 2147483647 || meadow <= -10000) meadow = 2147483647;
						//Debug.Log("The day "+ pop[i].daysSurvived + " had "+meadow+" boars.");
					}
					if (choice.Equals('0') || meadow == 0)
					{
						daysWithoutEat++;
						if (daysWithoutEat == daysBeforeDie) break;
					}
					else
					{
						daysWithoutEat = 0;
						meadow--;
					}
					pop[i].daysSurvived++;
					pop[i].meadow = meadow;
				}
				//Debug.Log("Number " + i + " survive " + pop[i].daysSurvived + " days with " + meadow + " boars.");
			}

			//séléction
			List<Person> newGeneration = new List<Person>();

			Array.Sort(pop, delegate (Person user1, Person user2)
			{ //sort of the population, Elitism Selection
				return user2.daysSurvived.CompareTo(user1.daysSurvived);
			});
			//Debug.Log("Best humans survived " + pop[0].daysSurvived + " days with "+ pop[0].meadow +" boars left.");

			textresult.text = "<align=center>Generation " + g + "</align>\r\nBest human strategy (survived " + pop[0].daysSurvived + " days with " + pop[0].meadow + " boars left) :\r\n" + pop[0].choices.Substring(0, pop[0].daysSurvived+1) + "\r\n<size=80%>0 = is do nothing, 1 = kill one boar</size>";
			if (g == nbrGenerations - 1) best = pop[0];

			//we keep the best 15
			for (int i = 0; i < 15; i++)
			{
				newGeneration.Add(pop[i]);
			}


			//croisement
			Person[] parents = (Person[])pop.Clone();
			while (newGeneration.Count < nbrIndividus)
			{
				int individu1 = aleatoire.Next(pop.Length);
				int individu2 = aleatoire.Next(pop.Length);
				Person newPerson1 = new Person();
				Person newPerson2 = new Person();
				int ptSwitch = aleatoire.Next(daysToSurvive);
				for (int j = 0; j < ptSwitch; j++)
				{
					newPerson1.choices += pop[individu1].choices[j];
					newPerson2.choices += pop[individu2].choices[j];
				}
				for (int j = ptSwitch; j < daysToSurvive; j++)
				{
					newPerson2.choices += pop[individu1].choices[j];
					newPerson1.choices += pop[individu2].choices[j];
				}
				newGeneration.Add(newPerson1);
				if (newGeneration.Count != nbrIndividus) newGeneration.Add(newPerson2);
			}

			//mutation
			int pos = 0;
			foreach (var chromosome in newGeneration)
			{
				if (aleatoire.Next(probability + 1) == 1) //you get the mutation !!
				{
					//Debug.Log("mutation !!");
					int pos1 = aleatoire.Next(chromosome.choices.Length); //swap between two random position
					int pos2 = aleatoire.Next(chromosome.choices.Length);
					char temp = chromosome.choices[pos1];
					StringBuilder sb = new StringBuilder(chromosome.choices);
					sb[pos1] = chromosome.choices[pos2];
					sb[pos2] = temp;
					chromosome.choices = sb.ToString();
				}
				pop[pos] = chromosome;
				pos++;
			}

			yield return null;
		}
	}
}
