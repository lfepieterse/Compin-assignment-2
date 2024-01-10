using System;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.ComponentModel;


namespace MyApp
{

	internal class Blok //De klasse Blok: een blok is een 2D array van nummers!
	{
		public Nummer[,] Cel = new Nummer[3, 3];

	}
	internal class Nummer //De klasse Nummer: een nummer heeft een Getalwaarde, en is Locked (indien ingevuld in de initiele sudoko of niet)
	{
		public int Getalwaarde { get; set; }
		public bool Locked { get; set; }
		public Nummer(int Getal, bool JaOfNee)
		{
			Getalwaarde = Getal;
			Locked = JaOfNee;
		}
	}
	internal class Program
	{


		static void Main(string[] args)
		{
			Console.WriteLine("Hoi, geef input in een reeks aan getallen!"); //Debug string

			string lezen = Console.ReadLine(); //Ervan uitgaande dat er input is, met spaties
			string input = string.Join("", lezen.Split(' ')); //Zet het om naar een leesbare string!
			//Rijenlijst nodig
			//Kolommenlijst nodig
			//Blokkenlijst nodig
			//Al deze lijsten zijn lijsten van lijsten met nummers erin van 1t/m9. Default null.
			
			//Eerst input in de 2D array zetten
			Nummer[,] Sudoku = new Nummer[9,9];
			int index = 0;
			for (int x=0;x<9;x++) {
				for (int y=0;y<9;y++) {
					Sudoku[x,y].Getalwaarde = int.Parse(input[index].ToString());
					if (Sudoku[x,y].Getalwaarde!=0) {
						Sudoku[x,y].Locked = true;
					}
					else {
						Sudoku[x,y].Locked = false;
					}
					
					index++;

				}
			}
			
			//Nu lijsten aanmaken van rijen, van kollomen en van blokken.

			
			Stopwatch stopwatch = new Stopwatch(); //We gebruiken een stopwatch om de tijd te meten!
			

			stopwatch.Restart();

			//-----BLOKKEN AANMAKEN en INVULLEN---
			Blok[,] Grid = new Blok[3, 3]; //Het grid is een 2D array van blokken

			//Let bij het volgende stukje goed opde i*3, j*3. Dit is nodig om de juiste input uit de string in te lezen (zie Maakblok)
			for (int x = 0; x < 3; x++)
			{
				for (int y = 0; y < 3; y++)
				{
					Grid[x, y] = MaakBlok(input, x * 3, y * 3); //Hiermee maken we de blokken aan.
					//BlokInVuller(Grid[x, y]); //En zo vullen we het blok meteen in!
				}
			}
			// KLAAR MET BLOKKEN AANMAKEN EN INVULLEN

			stopwatch.Stop(); //STOP DE TIJD!
			System.Console.WriteLine("----------------------------------");
			Console.WriteLine("Best Solution:");
			PrintGrid(Grid);
			//Console.WriteLine("Heuristiekewaarde: " + HeuristiekeLijst.Sum());
			System.Console.WriteLine(stopwatch.Elapsed.ToString()); //Print de tijd!
			//System.Console.WriteLine("Zitvast: " + zitvast);
			//System.Console.WriteLine("Hoeveel stappen je random walkt: " + HoeVaakRandomWalkJeDoet);
				}
		// 	}
		// }




		static void PrintGrid(Blok[,] grid) // we gaan alle rijen en kolommen af als de sudoku is opgelost, om de oplossing te printen in een goede vorm
		{
			for (int i = 0; i < 3; i++)
			{
				for (int row = 0; row < 3; row++)
				{
					for (int j = 0; j < 3; j++)
					{
						for (int col = 0; col < 3; col++)
						{
							Console.Write(grid[i, j].Cel[row, col].Getalwaarde + " ");
						}
						Console.Write("  ");
					}
					Console.WriteLine();
				}
				Console.WriteLine();
			}
			Console.WriteLine();
		}


		static int EvalueerRij(Blok[,] grid, int rijIndex) //aan te passen
		{
			List<int> Waarden = new List<int>();
			int nietvoorkomend;
			int BlockRowIndex;
			//Nu willen we een lijst maken van alle getallen in de verschillende blokken, in dezelfde kolommen

			//BlockRowIndex bepalen: op welke rij (vd blokken, dus 1e, 2e of 3e) moeten wij gaan zoeken?
			if (rijIndex < 3) { BlockRowIndex = 0; }
			else if (rijIndex < 6) { BlockRowIndex = 1; }
			else { BlockRowIndex = 2; }

			int rowIndexInBlok = rijIndex % 3; //Welke rij in het blok we moeten zoeken is onze eerdere index % 3.

			for (int blokCol = 0; blokCol < 3; blokCol++)
			{
				for (int celCol = 0; celCol < 3; celCol++)
				{
					int WaardeUItHetVakje = grid[BlockRowIndex, blokCol].Cel[rowIndexInBlok, celCol].Getalwaarde;
					Waarden.Add(WaardeUItHetVakje);
				}
			}
			
			// Bepaal het aantal duplicaten in de lijst
			nietvoorkomend = 9 - Waarden.Distinct().Count(); // kijken naar hoeveel van de 9 waarden niet in de rij voorkomen (ipv duplicaten)
			return nietvoorkomend;
		}
		static int EvalueerKolom(Blok[,] grid, int Kolomindex) //aan te passen
		{
			List<int> Waarden = new List<int>();
			int nietvoorkomend;
			int BlockColIndex;
			//Nu willen we een lijst maken van alle getallen in de verschillende blokken, in dezelfde kolommen

			//BlockColIndex bepalen: op welke kolom (vd blokken, dus 1e, 2e of 3e) moeten wij gaan zoeken?

			if (Kolomindex < 3) { BlockColIndex = 0; }
			else if (Kolomindex < 6) { BlockColIndex = 1; }
			else { BlockColIndex = 2; }

			int ColIndexInBlok = Kolomindex % 3; //Welke rij in het blok we moeten zoeken is onze eerdere index % 3.

			for (int blokRow = 0; blokRow < 3; blokRow++)
			{
				for (int celRow = 0; celRow < 3; celRow++)
				{
					int WaardeUItHetVakje = grid[blokRow, BlockColIndex].Cel[celRow, ColIndexInBlok].Getalwaarde;
					Waarden.Add(WaardeUItHetVakje);
				}
			}

			// Bepaal het aantal duplicaten in de lijst
			nietvoorkomend = 9 - Waarden.Distinct().Count();
			return nietvoorkomend;
		}


		static Blok MaakBlok(string input, int startRow, int startCol) //Deze code vult adhv van een input string de blokken in!
		{

			Blok blok = new Blok();

			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					int index = (startRow + i) * 9 + (startCol + j);
					int getal = int.Parse(input[index].ToString());
					bool vergrendeld = getal != 0; //Geeft bool aan of hij vergendeld is of niet

					blok.Cel[i, j] = new Nummer(getal, vergrendeld);
				}
			}

			return blok;
		}
	}
}
