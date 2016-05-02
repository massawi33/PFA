using System;
using System.IO;

namespace Projet_Firing_squad
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			
			/*Automata automate = new Automata(20);

			// path to change
			String solFileName = "solution_19a";
			String path = "/home/massawi33/Documents/prj1/code/code/";

			String bestName = path + solFileName + ".dat";
			int [] rules = initRulesFromFile(bestName);

			// Nombre maximale de fusiliers (taille maximale du réseau)
			int sizeMax = 20;

			int nFire;

			int fit = automate.f(rules, sizeMax);
			Console.WriteLine (fit);

			for(int i = 2; i <= fit; i++) { // j'ai changer sizeMax par fit pour ne pas etre hors limites
				// évolution de l'automate avec la règle rule sur un réseau de longueur i
				nFire = automate.evol(rules, i);

				// affichage du nombre de fusiliers ayant tiré
				Console.WriteLine("longueur " + i + " : " + nFire);

				// affiche la dynamique dans un fichier au format svg
				automate.exportSVG(i, 2 * i - 2, path + "svg/" + solFileName + "_" + i + ".svg");
			} 

			String outName = path + "out.dat";

			Initialization init = new Initialization();

			StreamWriter ecrivain;
			try {
				ecrivain =  new StreamWriter(outName);

				for(int i = 0; i < 1000; i++) {
					init.init(rules);

					fit = automate.f(rules, 19);

					printToFile(fit, rules, ecrivain);
				}

				ecrivain.Close();
			}
			catch (Exception e){
				Console.WriteLine(e.ToString());
			}

			Console.WriteLine("The End.");*/
			int taille = 20;
			Automata at = new Automata (taille);
			//at.Test_Hill_Climber (taille,10,50000);
			at.Test_Hill_Climber_best(taille,100,1000);



		}

		public static void printToFile(int fitness, int [] rules, StreamWriter ecrivain) {
			ecrivain.Write(fitness);
			for(int i = 0; i < 216; i++) {
				ecrivain.Write(" ");
				ecrivain.Write(rules[i]);
			}
			ecrivain.WriteLine();
		}

		public static int [] initRulesFromFile(String fileName) {
			// 5 états + l'état "bord"
			int n = 5 + 1;

			int [] rules = new int[n * n * n];

			try {
				StreamReader fichier = new StreamReader(fileName);

				StreamTokenizer entree = new StreamTokenizer(fichier);

				int i = 0;
				while(entree.NextToken() == StreamTokenizer.TT_NUMBER)
				{		
					rules[i] = (int) entree.NumberValue;
					i++;
				} 
				fichier.Close();
			}		
			catch (Exception e){
				Console.WriteLine(e.ToString());
			}

			return rules;
		}
	}
}
