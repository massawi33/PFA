using System;
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
				StreamWriter file = new StreamWriter("/home/massawi33/svg/HF/HF_nb_fusi.txt",true);
				file.WriteLine (nb_de_fusiller_atteints [i]);
				file.Close ();
			}

			StreamWriter fichier = new StreamWriter("/home/massawi33//svg/HF/rules.txt",true);

			printToFile(best_square,best_rules,fichier);

			for (int i = 2; i <= nb_square; i++) {
				
				best_square = this.evol(best_rules, i);
				Console.WriteLine(i + " : " + best_square);
				this.exportSVG(i, 2 * i - 2, "/home/massawi33/svg/HF/" + i + ".svg");
			}


		}






		//**************************************************
		// Hill Climber First 
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

		//**************************************************
		// Iterated Local Search
		//**************************************************

		public void ILS (int sizeMax, int nbeval_hill , int nbeval_ils , int nb_perturbation)
		{
			int[] bestsolution = new int[216];
			int best_fitness = 0;
			int rdm_int = 0;
			Random rdm = new Random ();
			int[] solution = null;
			int fitnes = 0;
			int[] regles = new int[2];
			Initialization initTest = new Initialization();

			Console.WriteLine ("begining");
			initTest.init(bestsolution);



			bestsolution = this.Hill_Climber (bestsolution, sizeMax, nbeval_hill);
			ArrayList<int[]>  liste_all_neigbors = Possible_neighbors (bestsolution);

			for (int i = 0; i < nbeval_ils; i++) {

				for (int j = 0; j < nb_perturbation; j++) { // perturbation 20 fois
					int c = liste_all_neigbors.Count;
					rdm_int = rdm.Next(c);
					regles = liste_all_neigbors [rdm_int];
					solution = (int[])bestsolution.Clone ();
					solution =  (int[])Neighbors_Rules (solution, regles [0], regles [1]).Clone();
				
				}


				best_fitness = this.f (bestsolution, sizeMax);
				fitnes = this.f (solution, fitnes);

				if (fitnes > best_fitness) {

					bestsolution = (int[])solution.Clone ();
					best_fitness = fitnes;
					StreamWriter file = new StreamWriter("/home/massawi33/svg/EVO/EVO_nb_fusi.txt",true);
					file.Write (best_fitness+",");
					file.Close ();


				}

			
			
			}
			StreamWriter fichier = new StreamWriter("/home/massawi33/svg/ILS/rules.txt",true);

			printToFile(best_fitness,bestsolution,fichier);

			for (int i = 2; i <= sizeMax; i++) {

				best_fitness = this.evol(bestsolution, i);
				Console.WriteLine(i + " : " + best_fitness);
				this.exportSVG(i, 2 * i - 2, "/home/massawi33/svg/ILS/" + i + ".svg");
			}



			

		}

		//**************************************************
		// Hill Climber Best 
		//**************************************************

		public int[] Hill_Climber_Best (int[] rules, int sizeMax, int nbeval)
		{
			ArrayList<int[]>  liste_all_neigbors = Possible_neighbors (rules);
			int[] bestSolutions = rules;
			//Random rdm = new Random ();
			//int curentFitness = this.f (rules, sizeMax);
			//int bestFitness = curentFitness;
			//int rdm_int = -1;
			int[] regles = new int[2];
			//int[] regles ;
			int[] new_rules;
			int best_fit = 0;
			int new_fit = 0;
			//int[] exclu = new int[liste_all_neigbors.Count];
			ArrayList<int>  exclu = new ArrayList<int>();
			//int k = 0;

			for (int i = 0; i < nbeval; i++) {
				if (liste_all_neigbors.Count == 0) {
					break;
				}

				int c = liste_all_neigbors.Count;
				int position = -1;
				best_fit = this.f (bestSolutions, sizeMax);// recalculage 
				//rdm_int = rdm.Next (liste_all_neigbors.Count);
				for(int j = 0 ; j < c ; j++){
					if (exclu.Contains (j)) {
						continue;
					}
					regles = liste_all_neigbors [j];
					new_rules = Neighbors_Rules (bestSolutions, regles [0], regles [1]);

					new_fit = this.f (new_rules, sizeMax);
					if (new_fit > best_fit) {

						position = j;
						best_fit = new_fit;
					}

				}
				if (position != -1) {
					exclu.Add (position);

					regles = liste_all_neigbors [position];
					//liste_all_neigbors.RemoveAt (position);

					new_rules =  Neighbors_Rules (bestSolutions, regles [0], regles [1]);
					bestSolutions = (int[])new_rules.Clone();

					liste_all_neigbors.Clear();
					liste_all_neigbors = Possible_neighbors (bestSolutions);
				}

				//rdm_int = rdm.Next (c);

				//regles = liste_all_neigbors [rdm_int];
				//liste_all_neigbors.RemoveAt (rdm_int);// couteux

				//new_rules = Neighbors_Rules (bestSolutions, regles [0], regles [1]);

				//best_fit = this.f (bestSolutions, sizeMax);// recalculage 
				//new_fit = this.f (new_rules, sizeMax);


				//if (new_fit >= best_fit) {

				//	bestSolutions = new_rules;
					/*for (int j = 0; j < bestSolutions.Length; j++) {
						bestSolutions[j] = new_rules[j];
					}*/
				//	liste_all_neigbors.Clear();
				//	liste_all_neigbors = Possible_neighbors (bestSolutions);
				//}


			}
			return bestSolutions;

		}
		//************************************************
		// Test du hill climber Best
		//************************************************
		public void Test_Hill_Climber_best(int taille_square , int nb , int nbeval ){

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

				resolved_rules = this.Hill_Climber_Best(reglesTest, taille_square , nbeval);
				nb_de_fusiller_atteints[i] = this.f(resolved_rules, taille_square);

				if (nb_de_fusiller_atteints[i] > best_square) {
					for (int j = 0; j < resolved_rules.Length; j++) {
						best_rules[j] = resolved_rules[j];
					}
					best_square = nb_de_fusiller_atteints[i];
					nb_square = best_square;
				}
				Console.WriteLine( nb_de_fusiller_atteints[i] + " square resolved");

				StreamWriter file = new StreamWriter("/home/massawi33/svg/HB/HB_nb_fusi.txt",true);
				file.WriteLine (nb_de_fusiller_atteints [i]);
				file.Close ();



			}

			StreamWriter fichier = new StreamWriter("/home/massawi33/svg/HB/rules.txt",true);

			printToFile(best_square,best_rules,fichier);


			for (int i = 2; i <= nb_square; i++) {

				best_square = this.evol(best_rules, i);
				Console.WriteLine(i + " : " + best_square);
				this.exportSVG(i, 2 * i - 2, "/home/massawi33/svg/HB/" + i + ".svg");
			}


		}

		//************************************************
		// Algorithme Evolutionaire_Modifier
		//************************************************

		public void Evolutionaire_modifier(int taille_square, int nbeval ,int nbGeneration, int nbPopulation , int nb_population_voulu ){

			//int[] Population = new int[nbPopulation];
			//int[] P2 = null;
			//int[] P3 = null;
			Initialization initTest = new Initialization();
			int[] reglesTest = new int[216];
			ArrayList<int[]> Population = new ArrayList<int[]> ();
			int best_fitnesse = -1;
			int position_best_fitensse = -1;
			ArrayList<int> best_population = new ArrayList<int> ();
			//int nb_population_voulu = 4; // doit etre pair
			int nb_croisement = 50;

			Console.WriteLine("begin initialisation");
			for (int i = 0; i < nbPopulation; i++) { //inisialisation des population voulu
				
				initTest.init(reglesTest);
				Population.Add (this.Hill_Climber (reglesTest, taille_square, nbeval));
			}
			//initTest.init(reglesTest);
			//P1 = this.Hill_Climber_Best(reglesTest, taille_square , nbeval);
			//initTest.init(reglesTest);
			//P2 = this.Hill_Climber_Best(reglesTest, taille_square , nbeval);
			//initTest.init(reglesTest);
			//P3 = this.Hill_Climber_Best(reglesTest, taille_square , nbeval);
			//int f1 = this.f(P1, taille_square);
			//int f2 = this.f(P2, taille_square);
			//int f3 = this.f(P3, taille_square);
			//Console.WriteLine( f1 +" "+ f2 + " "+f3);

			/*for(int i = 0 ; i < nbPopulation ; i++){ // affichage des fitnesse 

				int f1 = this.f(Population[i], taille_square);
				Console.WriteLine(f1);


			}*/

			for (int genration = 0; genration < nbGeneration; genration++) {

				for (int j = 0; j < nb_population_voulu; j++) {
					for (int i = 0; i < nbPopulation; i++) {

						int f1 = this.f (Population [i], taille_square);
						if (f1 > best_fitnesse && !best_population.Contains (i)) {

							best_fitnesse = f1;
							position_best_fitensse = i;

						}

					}
					if (best_fitnesse != -1) {

						best_population.Add (position_best_fitensse);

					}

				}

				for (int i = 0; i < nb_croisement; i++) { // croisement

					Random rdm = new Random ();
					int rdm_int = -1;
					int c = 216;
					rdm_int = rdm.Next (c);
					for (int j = 0; j < best_population.Count; j += 2) {

						int[] pop = Population [best_population [j]];
						int[] pop2 = Population [best_population [j + 1]];
						//int k = pop.Count();
						int provisoire = pop [rdm_int];  
						pop [rdm_int] = pop2 [rdm_int];
						pop2 [rdm_int] = provisoire;
						Population [best_population [j]] = pop;
						Population [best_population [j + 1]] = pop2;
					}
					//int zizo;
				}
				for (int i = 0; i < nbPopulation; i++) { // mutation des population non selectionner 

					if (!best_population.Contains (i)) {


						Population.Add (this.Hill_Climber (Population [i], taille_square, nbeval));
						Population.RemoveAt (i);
					}




				}

			}

			best_fitnesse = -1;
			position_best_fitensse = -1;

			for (int i = 0; i < nbPopulation; i++) {

				int f1 = this.f (Population [i], taille_square);
				//Console.WriteLine (f1);
				if (f1 > best_fitnesse) {

					best_fitnesse = f1;
					position_best_fitensse = i;
					StreamWriter file = new StreamWriter("/home/massawi33/svg/EVO/EVO_nb_fusi.txt",true);
					file.WriteLine (best_fitnesse);
					file.Close ();


				}



			}

			StreamWriter fichier = new StreamWriter("/home/massawi33/svg/EVO/rules.txt",true);

			printToFile(best_fitnesse,Population [position_best_fitensse],fichier);

			for (int i = 2; i <= taille_square; i++) {

				best_fitnesse = this.evol(Population [position_best_fitensse], i);
				Console.WriteLine(i + " : " + best_fitnesse);
				this.exportSVG(i, 2 * i - 2, "/home/massawi33/svg/EVO/" + i + ".svg");
			}

		}


		//************************************************
		// Algorithme Evolutionaire_Simple
		//************************************************

		public void Evolutionaire_Simple(int taille_square, int nbeval ,int nbGeneration, int nbPopulation , int nb_population_voulu , int nb_mutation ){

			//int[] Population = new int[nbPopulation];
			//int[] P2 = null;
			//int[] P3 = null;
			Initialization initTest = new Initialization();
			int[] reglesTest = new int[216];
			ArrayList<int[]> Population = new ArrayList<int[]> ();
			int best_fitnesse = -1;
			int position_best_fitensse = -1;
			ArrayList<int> best_population = new ArrayList<int> ();
			//int nb_population_voulu = 4; // doit etre pair
			int nb_croisement = 50;

			Console.WriteLine("begin initialisation");
			for (int i = 0; i < nbPopulation; i++) { //inisialisation des population voulu

				initTest.init(reglesTest);
				Population.Add (this.Hill_Climber (reglesTest, taille_square, nbeval));
			}
			//initTest.init(reglesTest);
			//P1 = this.Hill_Climber_Best(reglesTest, taille_square , nbeval);
			//initTest.init(reglesTest);
			//P2 = this.Hill_Climber_Best(reglesTest, taille_square , nbeval);
			//initTest.init(reglesTest);
			//P3 = this.Hill_Climber_Best(reglesTest, taille_square , nbeval);
			//int f1 = this.f(P1, taille_square);
			//int f2 = this.f(P2, taille_square);
			//int f3 = this.f(P3, taille_square);
			//Console.WriteLine( f1 +" "+ f2 + " "+f3);

			/*for(int i = 0 ; i < nbPopulation ; i++){ // affichage des fitnesse 

				int f1 = this.f(Population[i], taille_square);
				Console.WriteLine(f1);


			}*/
			Random rdm = new Random ();
			int rdm_int_1 = 0;
			int rdm_int_2 = 0;
			int fitness_provisoire_1 = -1;
			int fitness_provisoire_2 = -1;

			for (int genration = 0; genration < nbGeneration; genration++) {

				for (int j = 0; j < nb_population_voulu; j++) { // nb_population_voulu , c'est le nombre de tournoi est il doit etre obligatoirement pair , pour les croisement

					rdm_int_1 = rdm.Next (nbPopulation);
					rdm_int_2 = rdm.Next (nbPopulation);

					fitness_provisoire_1 = this.f (Population [rdm_int_1], taille_square);
					fitness_provisoire_2 = this.f (Population [rdm_int_2], taille_square);

					if (fitness_provisoire_1 > fitness_provisoire_2) {
					
						best_population.Add (rdm_int_1);
					
					} else {
					
						best_population.Add (rdm_int_2);
					
					}



				}

				for (int i = 0; i < nb_croisement; i++) { // croisement

					//Random rdm = new Random ();
					int rdm_int = -1;
					int c = 216;
					rdm_int = rdm.Next (c);
					for (int j = 0; j < best_population.Count; j += 2) {

						int[] pop = Population [best_population [j]];
						int[] pop2 = Population [best_population [j + 1]];
						//int k = pop.Count();
						int provisoire = pop [rdm_int];  
						pop [rdm_int] = pop2 [rdm_int];
						pop2 [rdm_int] = provisoire;
						Population [best_population [j]] = pop;
						Population [best_population [j + 1]] = pop2;
					}
					//int zizo;
				}



				for (int i = 0; i < nbPopulation; i++) {

					if (best_population.Contains (i)) {

						for (int j = 0; j < nb_mutation; j++) {

							int c = 216;
							int rdm_int = rdm.Next(c);
							Population[i] = this.Neighbors_Rules(Population[i],rdm_int,rdm.Next(4));



						
						}

						Population.Add (this.Hill_Climber (Population [i], taille_square, nbeval));
						//Population.RemoveAt (i);
					}




				}

			}

			best_fitnesse = -1;
			position_best_fitensse = -1;

			for (int i = 0; i < nbPopulation; i++) {

				int f1 = this.f (Population [i], taille_square);
				//Console.WriteLine (f1);
				if (f1 > best_fitnesse) {

					best_fitnesse = f1;
					position_best_fitensse = i;
					StreamWriter file = new StreamWriter("/home/massawi33/svg/EVO/EVO_nb_fusi.txt",true);
					file.WriteLine (best_fitnesse);
					file.Close ();


				}



			}

			StreamWriter fichier = new StreamWriter("/home/massawi33/svg/EVO/rules.txt",true);

			printToFile(best_fitnesse,Population [position_best_fitensse],fichier);

			for (int i = 2; i <= taille_square; i++) {

				best_fitnesse = this.evol(Population [position_best_fitensse], i);
				Console.WriteLine(i + " : " + best_fitnesse);
				this.exportSVG(i, 2 * i - 2, "/home/massawi33/svg/EVO/" + i + ".svg");
			}

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


