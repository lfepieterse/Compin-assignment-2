using System;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.ComponentModel;
using System.Net;


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

			Stopwatch stopwatch = new Stopwatch(); //We gebruiken een stopwatch om de tijd te meten!
			stopwatch.Restart();

			//Eerst input in de 2D array zetten
			Nummer[,] Sudoku = new Nummer[9, 9];
			int index = 0;
			for (int y = 0; y < 9; y++)
			{
				for (int x = 0; x < 9; x++)
				{
					Sudoku[y, x] = new Nummer(0, false);
					Sudoku[y, x].Getalwaarde = int.Parse(input[index].ToString());
					if (Sudoku[y, x].Getalwaarde != 0)
					{
						Sudoku[y, x].Locked = true;
					}
					else
					{
						Sudoku[y, x].Locked = false;
					}

					index++;

				}
			}

			//Nu lijsten aanmaken van rijen, van kollomen en van blokken.
			List<List<int>> rijenLijst = MaakRijenLijst(Sudoku);
			List<List<int>> kolommenLijst = MaakKolommenLijst(Sudoku);
			List<Stack<int>> MogelijkHedenPerCel = MaakMogelijkHedenPerCel(Sudoku);

			int Cursor = 0; //We beginnen met de cursor op puntje bij de 1e item uit de sudoku.
			bool ZittenWeInDeAchteruit = false; //Bool om bij te houden of we terug aan het gaan zijn!

			while (Cursor < 81) //Zodra de cursor voorbij cel 80 is, ben je klaar met de sudoku!
			{
				//Als de stack waarvan we de mogelijkheid willen checken leeg is, moet deze waarde van dit item op 9 worden gezet, 
				//de stack worden aangevuld en de cursor een stap terug. We moeten wel voorkomen dat we terug gaan, bij een locked aankomen, en daardoor weer vooruitgaan!


				int yCoordinaat = Cursor / 9; //Van Cursor naar coodinaat.
				int xCoordinaat = Cursor % 9;

				//Op moment dat de mogelijkheden lijst empty is, hervullen we de stack, zetten we de waarde op 0 en gaat de cursor eentje achteruit!
				if (MogelijkHedenPerCel[Cursor].Count == 0) //TODO EY DE ERROR DIE IK KRIJG IS ALS DE CURSOR -1 WORDT
				{
					MogelijkHedenPerCel[Cursor] = HerVulStack();
					Sudoku[yCoordinaat, xCoordinaat].Getalwaarde = 0;
					Cursor--;
					ZittenWeInDeAchteruit = true; //We gaan nu terug, dus we zitten in de achteruit!
					continue;
				}

				if (Sudoku[yCoordinaat, xCoordinaat].Locked)
				{
					if (ZittenWeInDeAchteruit) { Cursor--; } //Als we in de achteruitzitten en we komen aan bij een locked cel, moeten we nog eentje naar achteren!
					else { Cursor++; } //Anders gaan we gewoon weer door naar de volgende cel

					continue;

				}

				ZittenWeInDeAchteruit = false; //We zetten em weer op default waarde

				//We gaan eerst het geval schrijven dat alles klopt!

				Sudoku[yCoordinaat, xCoordinaat].Getalwaarde = MogelijkHedenPerCel[Cursor].Peek(); //Pak de bovenste van de stack!

				//Als de waarde al in de Rij, Blok, Of kolom staat, moeten we naar de volgende!
				if (StaatMijnWaardeAlInDeRij(rijenLijst, yCoordinaat, Sudoku[yCoordinaat, xCoordinaat].Getalwaarde) ||
				StaatMijnWaardeAlInHetBlok(Sudoku, yCoordinaat, xCoordinaat) ||
				StaatMijnWaardeAlInDeKolom(kolommenLijst, xCoordinaat, Sudoku[yCoordinaat, xCoordinaat].Getalwaarde))
				{
					MogelijkHedenPerCel[Cursor].Pop();
					continue;
					//En nu gaan we eigenlijk het hele proces opnieuw doen, alleen dat met een verminderde stack!				
				}

				else //Op moment dat het wél klopt, aldus het getal correct is ingevuld en met niks stramt... 
				{
					//Update de rijen en kolommen lijst. (blokken is geen lijst van en hoeft niet upgedate te worden)
					UpdateRijen(rijenLijst, Sudoku, yCoordinaat);
					UpdateKolommen(kolommenLijst, Sudoku, xCoordinaat);

					//En doe Cursor naar de volgende: 
					Cursor++;
					ZittenWeInDeAchteruit = false;
				}
			}

			//Als hij hier uitkomt is hij klaar!



			// //-----BLOKKEN AANMAKEN en INVULLEN---
			// Blok[,] Grid = new Blok[3, 3]; //Het grid is een 2D array van blokken

			// //Let bij het volgende stukje goed opde i*3, j*3. Dit is nodig om de juiste input uit de string in te lezen (zie Maakblok)
			// for (int x = 0; x < 3; x++)
			// {
			// 	for (int y = 0; y < 3; y++)
			// 	{
			// 		Grid[x, y] = MaakBlok(input, x * 3, y * 3); //Hiermee maken we de blokken aan.
			// 		//BlokInVuller(Grid[x, y]); //En zo vullen we het blok meteen in!
			// 	}
			// }
			// // KLAAR MET BLOKKEN AANMAKEN EN INVULLEN

			stopwatch.Stop(); //STOP DE TIJD!
			System.Console.WriteLine("----------------------------------");
			Console.WriteLine("Best Solution:");
			//PrintGrid(Grid);
			//Console.WriteLine("Heuristiekewaarde: " + HeuristiekeLijst.Sum());
			System.Console.WriteLine(stopwatch.Elapsed.ToString()); //Print de tijd!
																	//System.Console.WriteLine("Zitvast: " + zitvast);
																	//System.Console.WriteLine("Hoeveel stappen je random walkt: " + HoeVaakRandomWalkJeDoet);
		}
		// 	}
		// }
		static void UpdateRijen(List<List<int>> rijenLijst, Nummer[,] sudoku, int rijIndex)
		{
			// Lees de rij uit de sudoku in
			List<int> nieuweRij = new List<int>();
			for (int j = 0; j < 9; j++)
			{
				nieuweRij.Add(sudoku[rijIndex, j].Getalwaarde);
			}

			// Update de rijenLijst met de nieuwe rij
			rijenLijst[rijIndex] = nieuweRij;
		}
		static Stack<int> HerVulStack() //Functie die een stack van 1 t/m 9 oplevert
		{
			Stack<int> nieuweStack = new Stack<int>();

			for (int waarde = 9; waarde >= 1; waarde--)
			{
				nieuweStack.Push(waarde);
			}

			return nieuweStack;
		}

		static void UpdateKolommen(List<List<int>> kolommenLijst, Nummer[,] sudoku, int kolomIndex)
		{
			// Lees de kolom uit de sudoku in
			List<int> nieuweKolom = new List<int>();
			for (int i = 0; i < 9; i++)
			{
				nieuweKolom.Add(sudoku[i, kolomIndex].Getalwaarde);
			}

			// Update de kolommenLijst met de nieuwe kolom
			kolommenLijst[kolomIndex] = nieuweKolom;
		}
		static bool StaatMijnWaardeAlInDeRij(List<List<int>> rijenLijst, int rijIndex, int waarde)
		{
			List<int> rij = rijenLijst[rijIndex];
			return rij.Contains(waarde);
		}

		static bool StaatMijnWaardeAlInDeKolom(List<List<int>> kolommenlijst, int kolomindex, int waarde)
		{
			List<int> kolom = kolommenlijst[kolomindex];
			return kolom.Contains(waarde);
		}

		static List<Stack<int>> MaakMogelijkHedenPerCel(Nummer[,] sudoku)
		{
			List<Stack<int>> mogelijkhedenStacks = new List<Stack<int>>(); //We gaan een lijst aan stacks maken

			for (int i = 0; i < 9; i++) //Loop over alle cellen
			{
				for (int j = 0; j < 9; j++)
				{
					Nummer cel = sudoku[i, j];

					Stack<int> mogelijkhedenStack = new Stack<int>();

					if (cel.Locked)
					{
						// Als de cel Locked is, wil je alleen dát nummer op de stack.
						mogelijkhedenStack.Push(cel.Getalwaarde);
					}
					else
					{
						//Niet Lockeed? Dat zijn de mogelijkheden initeel 1 t/m 9
						mogelijkhedenStack = HerVulStack();
					}

					mogelijkhedenStacks.Add(mogelijkhedenStack);
				}
			}

			return mogelijkhedenStacks;
		}

		static bool StaatMijnWaardeAlInHetBlok(Nummer[,] sudoku, int row, int col)
		{
			int blokYCoordinaat = row / 3;  // Bereken het blokcoordinaat voor de rij (door de int hoef je niet te flooren)
			int blokXCoordinaat = col / 3;  // Bereken het blokcoordinaat voor de kolom

			for (int y = blokYCoordinaat * 3; y < 3 + blokYCoordinaat * 3; y++) //We gaan alle cellen in het blok doorlopen
			{
				for (int x = blokXCoordinaat * 3; x < 3 + blokXCoordinaat * 3; x++)
				{
					// Controleer of de waarde al in het blok zit, behalve de cel waarin we net hebben veranderd
					if (y != row || x != col)
					{
						if (sudoku[y, x].Getalwaarde == sudoku[row, col].Getalwaarde) //Het getal dat je invult kan nooit een nul zijn, dus daar hoef je geen eexception voor te schrijven
						{
							return true;  // De waarde zit al in het blok
						}
					}
				}
			}

			return false;  // De waarde zit nog niet in het blok
		}

		static bool StaatMijnWaardeAlInDeRij(List<int> rij, int waarde)
		{
			return rij.Contains(waarde);
		}
		static List<List<int>> MaakRijenLijst(Nummer[,] sudoku)
		{
			List<List<int>> rijenLijst = new List<List<int>>(); //Lijst aanmaken

			for (int i = 0; i < 9; i++) //Loop alle rijen vd sudoku langs
			{
				List<int> rij = new List<int>();
				for (int j = 0; j < 9; j++) //En pak elk item uit die rij!
				{
					rij.Add(sudoku[i, j].Getalwaarde);
				}
				rijenLijst.Add(rij);
			}

			return rijenLijst;
		}

		static List<List<int>> MaakKolommenLijst(Nummer[,] sudoku)
		{
			List<List<int>> kolommenLijst = new List<List<int>>(); //Lijst aanmaken

			for (int j = 0; j < 9; j++) //Loop alle kolommen langs uit de sudoku...
			{
				List<int> kolom = new List<int>();
				for (int i = 0; i < 9; i++) //En pak elk item uit die kolom!
				{
					kolom.Add(sudoku[i, j].Getalwaarde);
				}
				kolommenLijst.Add(kolom);
			}

			return kolommenLijst;
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
