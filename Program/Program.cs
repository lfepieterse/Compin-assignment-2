using System;
using System.Runtime.CompilerServices;
using System.Diagnostics;


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
		private const int iteraties = 100000; //Een aantal iteraties voor ILS
		private const int HoeVaakRandomWalkJeDoet = 3; //Hoeveel je randomwalkt!
		private const int zitvast = 15; //Na zoveel iteraties zonder verbetering, ga je randomwalken

		//static List<int> LijstZitVastWaarden = new List<int>() { 1000, 100, 50, 15 }; //Lijsten voor onderzoek doen!
		//static List<int> LijstAantalStappen = new List<int>() {50,15,3 ,1};

		
		

		static void Main(string[] args)
		{
			Console.WriteLine("Hoi, geef input in een reeks aan getallen!"); //Debug string

			string lezen = Console.ReadLine(); //Ervan uitgaande dat er input is, met spaties
			string input = string.Join("", lezen.Split(' ')); //Zet het om naar een leesbare string!
			
			Stopwatch stopwatch = new Stopwatch(); //We gebruiken een stopwatch om de tijd te meten!
			
			//for (int i = 0; i < LijstZitVastWaarden.Count(); i++) //2 forloops om onderzoek te doen! 
			//{
				//for (int j = 0; j < LijstAantalStappen.Count;j++) 
				// {
				// 	int zitvast = LijstZitVastWaarden[i];
				// 	int HoeVaakRandomWalkJeDoet = LijstAantalStappen[j];
				
			

					stopwatch.Restart();

					//-----BLOKKEN AANMAKEN en INVULLEN---
					Blok[,] Grid = new Blok[3, 3]; //Het grid is een 2D array van blokken

					//Let bij het volgende stukje goed opde i*3, j*3. Dit is nodig om de juiste input uit de string in te lezen (zie Maakblok)
					for (int x = 0; x < 3; x++)
					{
						for (int y = 0; y < 3; y++)
						{
							Grid[x, y] = MaakBlok(input, x * 3, y * 3); //Hiermee maken we de blokken aan.
							BlokInVuller(Grid[x, y]); //En zo vullen we het blok meteen in!
						}
					}
					// KLAAR MET BLOKKEN AANMAKEN EN INVULLEN

					//EVALUATIE LIJST AANMAKEN

					//De Heuristiekelijst is een lijst van 18 entries met (9rijen en 9 kolommen) waarin wordt bijgehouden per rij en kolom hoeveel items dubbel staan.
					//In plaats van het hele grid te evalueren, hoeven we vaak slechts 2 rijen en kolommen te evalueren, en dit aan te passen in de lijst!

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

					int ZitVastTeller = 0;

					// Hier kiezen we (het aantal keer van de iteraties een random blok uit om local search mee te doen
					for (int iteration = 0; iteration < iteraties; iteration++)
					{
						int randomBlockRow = random.Next(3); //Pak een random blok van je sudoku
						int randomBlockCol = random.Next(3);

						int SomVoorILS = HeuristiekeLijst.Sum(); //Bereken de totale heurisiteke waarde van het Grid vóór ILS

						ILS(Grid, randomBlockRow, randomBlockCol, HeuristiekeLijst); //zie ILS definitie

						int SomNaILS = HeuristiekeLijst.Sum(); //En bereken de waarde ook erna.


						if (SomNaILS ==0) {
							break; //Dan is ie klaar!
						}

						else if (SomVoorILS == SomNaILS) {
							ZitVastTeller++; //Als er geen verbetering is, laat je de 'ik zit vast'-teller oplopen
							
							if (ZitVastTeller == zitvast) //indien de teller een threshold heeft bereikt, ga je randomwalken!
							{
							RandomWalk(Grid,HoeVaakRandomWalkJeDoet, HeuristiekeLijst); //Randomwalk
							ZitVastTeller = 0; //En reset de teller weer
							}
						}

						else if (SomVoorILS != SomNaILS) //Indien er wel een aanpassing is gemaakt, reset de teller weer.
						{
							ZitVastTeller = 0;
						}

					}
					stopwatch.Stop(); //STOP DE TIJD!
					System.Console.WriteLine("----------------------------------");
					//Console.WriteLine("Best Solution:");
					//PrintGrid(Grid);
					Console.WriteLine("Heuristiekewaarde: " + HeuristiekeLijst.Sum());
					System.Console.WriteLine("Time elapsed: " + stopwatch.Elapsed); //Print de tijd!
					System.Console.WriteLine("Zitvast: " + zitvast);
					System.Console.WriteLine("Hoeveel stappen je random walkt: " + HoeVaakRandomWalkJeDoet);
					//EN DISPLAY TIJD YWWHAW
				}
		//	}
		//}

        static void RandomWalk(Blok[,] grid, int HoeVaakRandomWalkJeDoet, int [] HeuristiekeLijst) //De randomwalk functie!
        {
            for (int i = 0; i < HoeVaakRandomWalkJeDoet; i++) {
				int randomBlockRow = random.Next(3); //Randomblok
				int randomBlockCol = random.Next(3);

				int randomrow1= random.Next(3); //2 random cellen
				int randomrow2 = random.Next(3);
				int randomcol1 = random.Next(3);
				int randomcol2 = random.Next(3);

				//Deze cellen mogen natuurlijk niet locked zijn, dus dat checken we even
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

				//En nu gaan we opnieuw de heuristiek berkenen en dit aanpassen.
				int Rijdindex1 = randomrow1 + 3*randomBlockRow; //Dit is de rekenkundige manier om de random row om te zetten naar een getal tussen de 0 en 18.
				int Rijdindex2 = randomrow2 + 3* randomBlockRow;
				int ColomIndex1 = randomcol1 + 3*randomBlockCol;
				int ColomIndex2 = randomcol2 + 3*randomBlockCol;

				HeuristiekeLijst[Rijdindex1] = EvalueerRij(grid, Rijdindex1); //Evalueer de Rij! En kolom ofcourse.
				HeuristiekeLijst[Rijdindex2] = EvalueerRij(grid, Rijdindex2);
				HeuristiekeLijst[ColomIndex1+9] = EvalueerKolom(grid, ColomIndex1);
				HeuristiekeLijst[ColomIndex2+9] = EvalueerKolom(grid, ColomIndex2);	
			}
        }

        static void ILS(Blok[,] grid, int randomBlockRow, int randomBlockCol, int[] HeuristiekeLijst) //Het HART van dit programma
		{
			int bestScore = HeuristiekeLijst.Sum(); // opslaan hoeveel getallen er voor alle rijen en kolommen missen na initialisatie in totaal

			List<Tuple<int, int, int, int, int>> bestSwaps = new List<Tuple<int, int, int, int, int>>(); // Lijst voor het bijhouden van de beste swaps

			
			for (int row1 = 0; row1 < 3; row1++) //Ga alle mogelijke swaps langs
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

									CopyHeurLijst[Rijdindex1] = EvalueerRij(grid, Rijdindex1); //Evalueer de aangepaste rijen en kolommen!
									CopyHeurLijst[Rijdindex2] = EvalueerRij(grid, Rijdindex2);
									CopyHeurLijst[ColomIndex1+9] = EvalueerKolom(grid, ColomIndex1);
									CopyHeurLijst[ColomIndex2+9] = EvalueerKolom(grid, ColomIndex2);

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
				HeuristiekeLijst[ColomIndex2+9] = EvalueerKolom(grid, ColomIndex2);
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
					int WaardeUItHetVakje = grid[BlockRowIndex, blokCol].Cel[rowIndexInBlok, celCol].Getalwaarde;
					Waarden.Add(WaardeUItHetVakje);
				}
			}
			
			// Bepaal het aantal duplicaten in de lijst
			nietvoorkomend = 9 - Waarden.Distinct().Count(); // kijken naar hoeveel van de 9 waarden niet in de rij voorkomen (ipv duplicaten)
			return nietvoorkomend;
		}
		static int EvalueerKolom(Blok[,] grid, int Kolomindex)
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
