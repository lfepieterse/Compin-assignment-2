using System;
using System.Runtime.CompilerServices;

//Evaluatie functie schrijven
//Klasse blok maken DONE
//Klasse Nummer maken DONE
//INput inlezen DONE

namespace MyApp
{

	internal class Blok //De klasse Blok: een blok is een 2D array van nummers!
	{
		public Nummer[,] Cel = new Nummer[3, 3];

		public void SwapDigits(int row1, int col1, int row2, int col2) //twee waardes in cellen van de grid worden hier omgewisseld
		{
			
			Nummer temp = Cel[row1, col1];
			Cel[row1, col1] = Cel[row2, col2];
			Cel[row2, col2] = temp;
		}
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
		private static Random random = new Random(); //Maak een random object aan, handig voor later
		private const int MAX_ITERATIONS = 3000;
		private const int S = 100;
		static void Main(string[] args)
		{
			Console.WriteLine("Hoi, geef input in een reeks aan getallen!"); //Debug string
			string input = "003020600900305001001806400008102900700000008006708200002609500800203009005010300"; //Moet nog ingelezen worden ipv dit

			//string input = Console.ReadLine(); //Ervan uitgaande dat er input is

			//-----BLOKKEN AANMAKEN en INVULLEN---
			Blok[,] Grid = new Blok[3, 3]; //Het grid is een 2D array van blokken

			//Let bij het volgende stukje goed opde i*3, j*3. Dit is nodig om de juiste input uit de string in te lezen (zie Maakblok)
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					Grid[i, j] = MaakBlok(input, i * 3, j * 3); //Hiermee maken we de blokken aan.
					BlokInVuller(Grid[i, j]); //En zo vullen we het blok meteen in!
				}
			}
			// KLAAR MET BLOKKEN AANMAKEN EN INVULLEN

			//EVALUATIE LIJST AANMAKEN

			/*int[] Totaleduplicatenwaardenlijst = new int[18]; //Er zijn in totaal 18 rijen + kolommen

			for (int rijindex = 0; rijindex < 9; rijindex++) //Loop elke rij langs en evalueer deze
			{
				Totaleduplicatenwaardenlijst[rijindex] = EvalueerRij(Grid, rijindex);
			}

			for (int kolomindex = 0; kolomindex < 9; kolomindex++) //Kolomindex + 9 = lijstindex. Hier lopen we alle kolomen langs
			{
				Totaleduplicatenwaardenlijst[kolomindex + 9] = EvalueerKolom(Grid, kolomindex); //+ 9, omdat de 2e helft vd lijst voor de kolommen is
			} */

			//EVALUATIE LIJST AANGEMAAKT!!

			//NU: pak blok, ga alle getallen swappen en check de ev waarde

			//string Yeehaw = Console.ReadLine(); //random line for fun en voor debuggging

			//NU willen we een heuristieke functie maken, die aan de hand van een totaal grid en rij en kolom aanduiding de heur waarde kan 
			//berekenen.



			for (int iteration = 0; iteration < MAX_ITERATIONS; iteration++)
			{
				int randomBlockRow = random.Next(3); // genereert een random getal tussen 0, 1 en 2
				int randomBlockCol = random.Next(3);

				ApplyILS(Grid, randomBlockRow, randomBlockCol);

			}

			// Display or use the best solution obtained by ILS.
			Console.WriteLine("Best Solution:");
			PrintGrid(Grid);


		}

		static void ApplyILS(Blok[,] grid, int randomBlockRow, int randomBlockCol)
		{
			int bestScore = EvaluateSolution(grid); //kijken hoeveel waardes er missen in het geheel van de 18 rijen en kolommen in het begin

			for (int step = 0; step < S; step++)
			{
				int row1 = random.Next(3);
				int col1 = random.Next(3);
				int row2 = random.Next(3);
				int col2 = random.Next(3);

				while (grid[randomBlockRow, randomBlockCol].Cel[row1, col1].Locked) //zorgen dat er geen vastgezet nummer geswapt wordt
				{
					row1 = random.Next(3);
					col1 = random.Next(3);
				}
				while (grid[randomBlockRow, randomBlockCol].Cel[row2, col2].Locked) 
				{
					row2 = random.Next(3);
					col2 = random.Next(3);
				}

				grid[randomBlockRow, randomBlockCol].SwapDigits(row1, col1, row2, col2);
				
				// we berekenen of er minder (of evenveel) waardes missen in het gehele grid, anders switchen we de waardes weer terug
				int currentScore = EvaluateSolution(grid); 
				
				if (currentScore <= bestScore)
				{
					bestScore = currentScore;
				}
				else
				{
					grid[randomBlockRow, randomBlockCol].SwapDigits(row1, col1, row2, col2);
				}
			}
		}

		static void PrintGrid(Blok[,] grid) // we gaan alle rijen en kolommen af als de sudoku is opgelost, om de oplossing te printen
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

		static int EvaluateSolution(Blok[,] grid)
		{
			int totalMissing = 0; //initialiseer waarde voor het aantal getallen dat mist in de grid

			// we gaan voor alle rijen optellen hoeveel waardes er missen
			for (int rowIndex = 0; rowIndex < 9; rowIndex++)
			{
				totalMissing += EvalueerRij(grid, rowIndex);
			}

			// dit doen we ook in de kolommen
			for (int colIndex = 0; colIndex < 9; colIndex++)
			{
				totalMissing += EvalueerKolom(grid, colIndex);
			}

			return totalMissing;
		}

		static int EvalueerRij(Blok[,] grid, int rijIndex)
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
					int WaardeUItHetVakje = grid[blokCol, BlockRowIndex].Cel[celCol, rowIndexInBlok].Getalwaarde;
					Waarden.Add(WaardeUItHetVakje);
				}
			}
			/* System.Console.WriteLine(Waarden.Count()); */

			// Bepaal het aantal duplicaten in de lijst
			nietvoorkomend = 9 - Waarden.Distinct().Count(); // kijken naar hoeveel van de 9 waarden niet in de rij voorkomen (ipv duplicaten)
			return nietvoorkomend;
		}
		static int EvalueerKolom(Blok[,] grid, int Kolomindex)
		{
			List<int> Waarden = new List<int>();
			int nietvoorkomend = 0;
			int BlockColIndex;
			//Nu willen we een lijst maken van alle getallen in de verschillende blokken, in dezelfde kolommen

			//BlockColIndex bepalen: op welke kolom (vd blokken, dus 1e, 2e of 3e) moeten wij gaan zoeken?
			if (Kolomindex < 3 + 9) { BlockColIndex = 0; }
			else if (Kolomindex < 6 + 9) { BlockColIndex = 1; }
			else { BlockColIndex = 2; }

			int ColIndexInBlok = Kolomindex % 3; //Welke rij in het blok we moeten zoeken is onze eerdere index % 3.

			for (int blokRow = 0; blokRow < 3; blokRow++)
			{
				for (int celRow = 0; celRow < 3; celRow++)
				{
					int WaardeUItHetVakje = grid[BlockColIndex, blokRow].Cel[ColIndexInBlok, celRow].Getalwaarde;
					Waarden.Add(WaardeUItHetVakje);
				}
			}

			// Bepaal het aantal duplicaten in de lijst
			nietvoorkomend = 9 - Waarden.Distinct().Count(); 
			return nietvoorkomend;
		}


		static void BlokInVuller(Blok blok)
		{
			List<int> beschikbareGetallen = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 }; //alle getallen van 0 t/m 9!
																						 //Eerst gaan we de getallen eruit halen die al locked zijn. 
			List<int> verbodencijfers = new List<int>();

			//Eerst zorgen we dat de getallen van de locked cellen niet gebruikt gaan worden. 
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					if (blok.Cel[i, j].Locked == true)
					{
						verbodencijfers.Add(blok.Cel[i, j].Getalwaarde); //hiermee maken we een lijst aan getallen die verboden zijn en dus niet mogen worden ingevuld!
					}
				}
			}
			List<int> GoedeLijstMeTMinderGetallen = beschikbareGetallen.Except(verbodencijfers).ToList();

			//Nu gaan we de getallen toevoegen!
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					if (!blok.Cel[i, j].Locked == true)
					{
						int randomIndex = random.Next(GoedeLijstMeTMinderGetallen.Count);
						int willekeurigGetal = GoedeLijstMeTMinderGetallen[randomIndex];

						blok.Cel[i, j].Getalwaarde = willekeurigGetal; //Vul de cel in het blok in!

						GoedeLijstMeTMinderGetallen.RemoveAt(randomIndex);
					}
				}
			}
		}
		static Blok MaakBlok(string input, int startRow, int startCol)
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
