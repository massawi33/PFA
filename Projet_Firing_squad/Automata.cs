﻿using System;
using System.IO;
using System.Collections;
using C5;
using System.Linq;

namespace Projet_Firing_squad
{
	public class Automata
	{

		// diagramme espace-temps;
		int[,] configs;

		// nombre d'états
		int nbStates;

		// taille de l'automate , , nb fusiller 
		int maxSize;

		// nombre maximum d'iterations nb maximum qu'on doit atteindre
		int maxIteration;

		// notation pour certains états
		int BORD;
		int FIRE;
		int GEN;
		int REPOS;


		/*********************************************
		 * constructor
		 *
		 * input : N : maximum size of the automata
		 *
		 *********************************************/
		public Automata (int _maxSize)
		{
			nbStates = 5;	
			maxSize = _maxSize; 
			maxIteration = 2 * _maxSize - 2;
			BORD = nbStates;	
			FIRE = nbStates - 1;
			GEN = 1;
			REPOS = 0;

			if ((maxIteration > 1) && (maxSize > 1))
				configs = new int [maxIteration + 1, maxSize + 2];
		}

		/*********************************************
		 * configuration initiale de l'automate
		 *
		 * input : N : size of the automata
		 *
		 *********************************************/
		public void initialConfiguration (int N)
		{
			// bord gauche
			for (int t = 0; t <= maxIteration; t++)
				configs [t, 0] = BORD;

			// bord droit
			for (int t = 0; t <= maxIteration; t++)
				configs [t, N + 1] = BORD;

			// repos
			for (int i = 2; i <= N; i++)
				configs [0, i] = REPOS;

			// général
			configs [0, 1] = GEN;
		}

		/*********************************************
		 * evolution of the automate from initial configuration
		 * to the first time of firing
		 *
		 * input : regles : rules of the automata
		 *         N : size of the automata
		 *         nbIter : maximum number of iterations
		 *	
		 * output : number of the firing after 2N-2 iteration
		 *          0 else
		 *********************************************/
		public int evol (int[] regles, int N)
		{
			int nbIter = 2 * N - 2;

			if (nbIter > maxIteration)
				nbIter = maxIteration;

			// initialise l'automate
			initialConfiguration (N);

			// nombre d'états feu
			int nbFire = 0;

			// valeur de la regle locale
			int r;

			int i, t;

			t = 1;
			while (t <= nbIter && nbFire == 0) {
				for (i = 1; i <= N; i++) {
					r = regles [configs [t - 1, i - 1] * 36 + configs [t - 1, i] * 6 + configs [t - 1, i + 1]];

					if (r == FIRE)
						nbFire++;

					configs [t, i] = r;
				}

				t++;
			}

			// nombre de fusiliers après 2N-2 it屍ations
			if (t == 2 * N - 2 + 1)
				return nbFire;
			else
				return 0;
		}

		/*********************************************
		 * compute objective function
		 *
		 * input : regles : rules of the automata
		 *         n : maximum of automata size
		 *
		 * output : the maximum size solved
		 *
		 *********************************************/
		public int f (int[] regles, int n)
		{
			int nbFireTot = evol (regles, 2);

			int k = 2;

			while (nbFireTot == k && k <= n) {
				k++;
				nbFireTot = evol (regles, k);
			}

			if (k == 2)
				return 0;
			else
				return k - 1;
		}

		//************************************************
		// return true si la regle peut etre modifier sinon la regle est fixe
		//************************************************

		public bool editable_rule (int index)
		{

			int[] tableauReglesNonModifiable = new int[11];
			tableauReglesNonModifiable [0] = 0 * 36 + 0 * 6 + 0;
			tableauReglesNonModifiable [1] = 5 * 36 + 0 * 6 + 0;
			tableauReglesNonModifiable [2] = 0 * 36 + 0 * 6 + 5;
			tableauReglesNonModifiable [3] = 1 * 36 + 1 * 6 + 1;
			tableauReglesNonModifiable [4] = 5 * 36 + 1 * 6 + 1;
			tableauReglesNonModifiable [5] = 1 * 36 + 1 * 6 + 5;
			tableauReglesNonModifiable [6] = 1 * 36 + 0 * 6 + 0;
			tableauReglesNonModifiable [7] = 2 * 36 + 0 * 6 + 0;
			tableauReglesNonModifiable [8] = 1 * 36 + 0 * 6 + 5;
			tableauReglesNonModifiable [9] = 2 * 36 + 0 * 6 + 5;
			tableauReglesNonModifiable [10] = 5 * 36 + 1 * 6 + 0;
			// les regles impossible
			int d = index % 6;
			int c = (index / 6) % 6;
			int g = index / 36;
			if (d == 4 || c == 5 || g == 4 || c == 4) {
				return false;
			} else {
				for (int i = 0; i < tableauReglesNonModifiable.Length; i++) {
					if (index == tableauReglesNonModifiable [i]) {
						return false;
					}
				}
			}
			return true;
		}


		//************************************************
		// Genrer une liste de tout les regles possible
		//************************************************

		public ArrayList<int[]> Possible_neighbors (int[] rules)
		{

			//IList liste = new IList ();
			//int nb = rules.Length * 5;
			ArrayList<int[]> liste = new ArrayList<int[]> ();
			int[] nouvelle_regle;

			for (int i = 0; i < rules.Length; i++) {
				
				if (editable_rule (i)) {
					for (int j = 0; j < nbStates - 1; j++) {
						if (rules [i] == j) {
							continue;
						} else {
							nouvelle_regle = new int[2];
							nouvelle_regle [0] = i;
							nouvelle_regle [1] = j;
							liste.Add (nouvelle_regle);// modifier l'algorithme
						}
					}
				}
			}
			//int c = liste.Count;

			//Console.WriteLine(c);

			return liste;
		}

		//************************************************
		// modifier une liste de regle par une autre et retourne la liste des nouvelles regles
		//************************************************

		public int[] Neighbors_Rules (int[] rules, int index, int etat)
		{

			int[] neighbors_R = new int[rules.Length];
			neighbors_R = (int[])rules.Clone(); // problème voisinage
			neighbors_R [index] = etat;
			return neighbors_R;

		}

		/*public int[] Neighbors_Rules(int[] regles, int indice, int resultat) {
			int[] nouveauTableauRegles = new int[regles.Length];
			for (int i = 0; i < regles.Length; i++) {
				nouveauTableauRegles[i] = regles[i];
			}
			nouveauTableauRegles[indice] = resultat;
			return nouveauTableauRegles;
		}*/

		//************************************************
		// Test du hill climber
		//************************************************

		public void Test_Hill_Climber(int taille_square , int nb , int nbeval ){
			
			int best_square = 0;
			int[] best_rules = new int[216];
			int[] resolved_rules = null;
			int[] nb_de_fusiller_atteints = new int[nb];
			//Automata automateTest = new Automata(taille);

			int nb_square = 0;
			Console.WriteLine("beginning of the algorithm");

			for (int i = 0; i < nb; i++) {

				Initialization initTest = new Initialization();
				int[] reglesTest = new int[216];

				initTest.init(reglesTest);

				resolved_rules = this.Hill_Climber(reglesTest, taille_square , nbeval);
				nb_de_fusiller_atteints[i] = this.f(resolved_rules, taille_square);

				if (nb_de_fusiller_atteints[i] > best_square) {
					for (int j = 0; j < resolved_rules.Length; j++) {
						best_rules[j] = resolved_rules[j];
					}
					best_square = nb_de_fusiller_atteints[i];
					nb_square = best_square;
				}
				Console.WriteLine( nb_de_fusiller_atteints[i] + " square resolved");
			}

			StreamWriter fichier = new StreamWriter("/home/massawi33/svg/rules.txt",true);

			printToFile(best_square,best_rules,fichier);

			for (int i = 2; i <= nb_square; i++) {
				
				best_square = this.evol(best_rules, i);
				Console.WriteLine(i + " : " + best_square);
				this.exportSVG(i, 2 * i - 2, "/home/massawi33/svg/" + i + ".svg");
			}


		}






		//**************************************************
		// Hill Climber
		//**************************************************

		public int[] Hill_Climber (int[] rules, int sizeMax, int nbeval)
		{
			ArrayList<int[]>  liste_all_neigbors = Possible_neighbors (rules);
			int[] bestSolutions = rules;
			Random rdm = new Random ();
			//int curentFitness = this.f (rules, sizeMax);
			//int bestFitness = curentFitness;
			int rdm_int = -1;
			int[] regles = new int[2];
			//int[] regles ;
			int[] new_rules;
			int best_fit = 0;
			int new_fit = 0;


			for (int i = 0; i < nbeval; i++) {
				if (liste_all_neigbors.Count == 0) {
					break;
				}

				int c = liste_all_neigbors.Count;
				//rdm_int = rdm.Next (liste_all_neigbors.Count);
				rdm_int = rdm.Next (c);

				regles = liste_all_neigbors [rdm_int];
				liste_all_neigbors.RemoveAt (rdm_int);// couteux

				new_rules = Neighbors_Rules (bestSolutions, regles [0], regles [1]);

				best_fit = this.f (bestSolutions, sizeMax);// recalculage 
				new_fit = this.f (new_rules, sizeMax);


				if (new_fit >= best_fit) {
					
					bestSolutions = new_rules;
					/*for (int j = 0; j < bestSolutions.Length; j++) {
						bestSolutions[j] = new_rules[j];
					}*/
					liste_all_neigbors.Clear();
					liste_all_neigbors = Possible_neighbors (bestSolutions);
				}


			}
			return bestSolutions;
			
		}



		/*********************************************
	     * Export the space-time diagramme of the automa
	     *   into SVG format
	     *
	     * input : N - size of the automata
	     *         nbIter - number of iterations
	     *         
	     *
	     * output : fileName
	     *
	     *********************************************/
		public void exportSVG (int N, int nbIter, String fileName)
		{
			if (nbIter > maxIteration)
				nbIter = maxIteration;

			int width = 10;
			int height = 10;

			StreamWriter file;
			try {
				file = new StreamWriter (fileName);

				// head of the file
				file.WriteLine ("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
				file.WriteLine ("<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.0//EN\" \"http://www.w3.org/TR/2001/REC-SVG-20010904/DTD/svg10.dtd\">");
				file.WriteLine ("<svg");
				file.WriteLine ("xmlns=\"http://www.w3.org/2000/svg\"");
				file.WriteLine ("xmlns:xlink=\"http://www.w3.org/1999/xlink\"");
				file.WriteLine ("xmlns:ev=\"http://www.w3.org/2001/xml-events\"");
				file.WriteLine ("version=\"1.1\"");
				file.WriteLine ("baseProfile=\"full\">");
				file.WriteLine ("<g stroke-width=\"1px\" stroke=\"black\" fill=\"white\">");

				bool fire = false;
				int i, j; 
				for (i = 0; i <= nbIter && !fire; i++) {
					for (j = 1; j <= N; j++) {
						file.Write ("<rect width=\"" + width + "\" "
						+ "height=\"" + height + "\" "
						+ "x=\"" + ((j - 1) * width) + "\" "
						+ "y=\"" + (i * height) + "\" "
						+ "fill=\"");

						if (configs [i, j] == REPOS)
							file.Write ("white");
						else if (configs [i, j] == FIRE)
							file.Write ("red");
						else if (configs [i, j] == GEN)
							file.Write ("blue");
						else if (configs [i, j] == 2)
							file.Write ("yellow");
						else if (configs [i, j] == 3)
							file.Write ("green");
						else
							file.Write ("grey"); 

						file.WriteLine ("\"/>");

						fire = fire || (configs [i, j] == FIRE);
					}
				}

				file.WriteLine ("</g> </svg>");

				file.Close ();
			} catch (Exception e) {
				Console.WriteLine (e.ToString ());
			}
		}

		/*********************************************
	     Print to file
	     *********************************************/

		public static void printToFile(int fitness, int [] rules, StreamWriter ecrivain) {
			ecrivain.Write(fitness);
			for(int i = 0; i < 216; i++) {
				ecrivain.Write(" ");
				ecrivain.Write(rules[i]);
			}
			ecrivain.WriteLine();
			ecrivain.Close ();
		}
	}
}

