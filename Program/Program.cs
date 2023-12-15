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

		public void Wissel(int row1, int col1, int row2, int col2) //twee waardes in cellen van de grid worden hier omgewisseld
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
		private const int iteraties = 100000;
		private const int HoeVaakRandomWalkJeDoet = 5; //stappen aantal!
		private const int zitvast = 15; //als de heuristiek niet beter wordt na ....  iteraties
		static void Main(string[] args)
		{
			Console.WriteLine("Hoi, geef input in een reeks aan getallen!"); //Debug string
			string input = "003020600900305001001806400008102900700000008006708200002609500800203009005010300"; //Moet nog ingelezen worden ipv dit

			// string lezen = Console.ReadLine(); //Ervan uitgaande dat er input is, met spaties
			// string input = string.Join("", lezen.Split(' '));

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

			int[] HeuristiekeLijst = new int[18]; //Er zijn in totaal 18 rijen + kolommen

			for (int rijindex = 0; rijindex < 9; rijindex++) //Loop elke rij langs en evalueer deze
			{
				HeuristiekeLijst[rijindex] = EvalueerRij(Grid, rijindex);
			}

			for (int kolomindex = 0; kolomindex < 9; kolomindex++) //Kolomindex + 9 = lijstindex. Hier lopen we alle kolomen langs
			{
				HeuristiekeLijst[kolomindex + 9] = EvalueerKolom(Grid, kolomindex); //+ 9, omdat de 2e helft vd lijst voor de kolommen is
			} 

			//EVALUATIE LIJST AANGEMAAKT!!

			//NU: pak blok, ga alle getallen swappen en check de ev waarde

			//string Yeehaw = Console.ReadLine(); //random line for fun en voor debuggging

			//NU willen we een heuristieke functie maken, die aan de hand van een totaal grid en rij en kolom aanduiding de heur waarde kan 
			//berekenen.
			int ZitVastTeller = 0;

			// Hier kiezen we (het aantal keer van de iteraties een random blok uit om local search mee te doen
			for (int iteration = 0; iteration < iteraties; iteration++) 
			{
				int randomBlockRow = random.Next(3); // genereert een random getal tussen 0, 1 en 2
				int randomBlockCol = random.Next(3);

				int SomVoorILS = HeuristiekeLijst.Sum();

				ILS(Grid, randomBlockRow, randomBlockCol, HeuristiekeLijst);

				int SomNaILS = HeuristiekeLijst.Sum();

				if (SomNaILS ==0) {
					break; //Dan is ie klaar!
				}

				else if (SomVoorILS == SomNaILS) {
					ZitVastTeller++;
					
					if (ZitVastTeller == zitvast) {
					RandomWalk(Grid,HoeVaakRandomWalkJeDoet, HeuristiekeLijst);
					ZitVastTeller = 0;
					}
				}

				else if (SomVoorILS != SomNaILS) {
					ZitVastTeller = 0;
				}

			}

			// Display or use the best solution obtained by ILS.
			Console.WriteLine("Best Solution:");
			PrintGrid(Grid);
			System.Console.WriteLine("Heuristiekewaarde: " + HeuristiekeLijst.Sum());
			//EN DISPLAY TIJD YWWHAW
		}

        static void RandomWalk(Blok[,] grid, int HoeVaakRandomWalkJeDoet,int [] HeuristiekeLijst)
        {
            for (int i = 0; i < HoeVaakRandomWalkJeDoet; i++) {
				int randomBlockRow = random.Next(3); // genereert een random getal tussen 0, 1 en 2
				int randomBlockCol = random.Next(3);

				//if (!grid[randomBlockRow, randomBlockCol].Cel[row1, col1].Locked)
				int randomrow1= random.Next(3);
				int randomrow2 = random.Next(3);
				int randomcol1 = random.Next(3);
				int randomcol2 = random.Next(3);

				while (grid[randomBlockRow, randomBlockCol].Cel[randomrow1, randomcol1].Locked) {
					randomrow1= random.Next(3);
					randomcol1 = random.Next(3);
				}

				while (grid[randomBlockRow, randomBlockCol].Cel[randomrow2, randomcol2].Locked) {
					randomrow2= random.Next(3);
					randomcol2 = random.Next(3);
				}

				//Nu hebben 2 losse cellen, laten we ze swappen!
				grid[randomBlockRow, randomBlockCol].Wissel(randomrow1, randomcol1, randomrow2, randomcol2);

				int Rijdindex1 = randomrow1 + 3*randomBlockRow;
				int Rijdindex2 = randomrow2 + 3* randomBlockRow;
				int ColomIndex1 = randomcol1 + 3*randomBlockCol;
				int ColomIndex2 = randomcol2 + 3*randomBlockCol;

				HeuristiekeLijst[Rijdindex1] = EvalueerRij(grid, Rijdindex1);
				HeuristiekeLijst[Rijdindex2] = EvalueerRij(grid, Rijdindex2);
				HeuristiekeLijst[ColomIndex1+9] = EvalueerKolom(grid, ColomIndex1);
				HeuristiekeLijst[ColomIndex2+9] = EvalueerKolom(grid, ColomIndex2);	
			}
        }

        static void ILS(Blok[,] grid, int randomBlockRow, int randomBlockCol, int[] HeuristiekeLijst)
		{
			int bestScore = HeuristiekeLijst.Sum(); // opslaan hoeveel getallen er voor alle rijen en kolommen missen na initialisatie in totaal

			List<Tuple<int, int, int, int, int>> bestSwaps = new List<Tuple<int, int, int, int, int>>(); // Lijst voor het bijhouden van de beste swaps

			// Met het aantal ingestelde stappen (S) gaan we alle niet vastgezetten waarden in het blok wisselen en evalueren
			
			for (int row1 = 0; row1 < 3; row1++)
			{
				for (int col1 = 0; col1 < 3; col1++)
				{
					if (!grid[randomBlockRow, randomBlockCol].Cel[row1, col1].Locked)
					{
						for (int row2 = 0; row2 < 3; row2++)
						{
							for (int col2 = 0; col2 < 3; col2++)
							{
								if (!grid[randomBlockRow, randomBlockCol].Cel[row2, col2].Locked &&
									!(row1 == row2 && col1 == col2))
								{
									// waardes omwisselen
									grid[randomBlockRow, randomBlockCol].Wissel(row1, col1, row2, col2);

									//maken copy lijst
									int[] CopyHeurLijst = new int[18];
									HeuristiekeLijst.CopyTo(CopyHeurLijst,0); //Copy lijst!

									//4 items in de lijst gaan aanpassen. Dit zijn indexen voor de lijst aanpassen
									int Rijdindex1 = row1 + 3*randomBlockRow;
									int Rijdindex2 = row2 + 3* randomBlockRow;
									int ColomIndex1 = col1 + 3*randomBlockCol;
									int ColomIndex2 = col2 + 3*randomBlockCol;

									CopyHeurLijst[Rijdindex1] = EvalueerRij(grid, Rijdindex1);
									CopyHeurLijst[Rijdindex2] = EvalueerRij(grid, Rijdindex2);
									CopyHeurLijst[ColomIndex1+9] = EvalueerKolom(grid, Rijdindex1);
									CopyHeurLijst[ColomIndex1+9] = EvalueerKolom(grid, Rijdindex2);

									int currentScore = CopyHeurLijst.Sum();

									// Als minder of evenveel getallen dan eerst missen is dit de nieuwe bestscore
									if (currentScore <= bestScore)
									{
										bestSwaps.Add(new Tuple<int, int, int, int, int>(row1, col1, row2, col2, currentScore));
									}

									// De waardes weer teruggewisseld
									grid[randomBlockRow, randomBlockCol].Wissel(row1, col1, row2, col2);
								}
							}
						}
					}
				}
			}
		

			// Kies de beste swap
			if (bestSwaps.Count > 0)
			{
				Tuple<int, int, int, int, int> bestSwap = bestSwaps.OrderBy(t => t.Item5).First();
				grid[randomBlockRow, randomBlockCol].Wissel(bestSwap.Item1, bestSwap.Item2, bestSwap.Item3, bestSwap.Item4);

				int Rijdindex1 = bestSwap.Item1 + 3*randomBlockRow;
				int Rijdindex2 = bestSwap.Item3 + 3* randomBlockRow;
				int ColomIndex1 = bestSwap.Item2 + 3*randomBlockCol;
				int ColomIndex2 = bestSwap.Item4 + 3*randomBlockCol;

				HeuristiekeLijst[Rijdindex1] = EvalueerRij(grid, Rijdindex1);
				HeuristiekeLijst[Rijdindex2] = EvalueerRij(grid, Rijdindex2);
				HeuristiekeLijst[ColomIndex1+9] = EvalueerKolom(grid, ColomIndex1);
				HeuristiekeLijst[ColomIndex1+9] = EvalueerKolom(grid, ColomIndex2);
			}
		}




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

		static int Evalueer(Blok[,] grid) //kijk naar het aantal totale getallen die missen in de hele grid voor alle rijen + kolommen
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
				totalMissing += EvalueerKolom(grid, colIndex); //Hoe dit toch wel + 9 zijn?
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

			//EERST STOND HIER OVERAL 3+9, 6+9, maar dit moet gwn 3,6,9 zijn enzo!
			if (Kolomindex < 3) { BlockColIndex = 0; }
			else if (Kolomindex < 6) { BlockColIndex = 1; }
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
