using System;
using System.IO;
using System.Diagnostics;

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
			int taille = 25;
			Automata at = new Automata (taille);
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			at.Test_Hill_Climber (taille,1,10000000);
			//at.Test_Hill_Climber_best(taille,100,10000);
			//at.Evolutionaire_modifier(20,1000,5,10,4);
			at.Test_ILS(taille,100000000,1000000,21,1);
			//at.Evolutionaire_Simple(20,100000000,5,100,6,20);
			//at.Evolutionaire_ILS(20,1000000,100,3,10,2,50,21);
			stopwatch.Stop();
			Console.Error.WriteLine("Parallel loop time in milliseconds: {0}",
				stopwatch.ElapsedMilliseconds);

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
