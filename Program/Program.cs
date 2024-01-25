using System;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.ComponentModel;
using System.Net;
using System.Xml.Linq;
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
			Console.WriteLine("Hoi, geef eens input in een reeks aan getallen!");

			string lezen = Console.ReadLine(); //Ervan uitgaande dat er input is, met spaties
			Console.WriteLine("Alright! Welk algoritme wil je gebruiken? CBT (1), CBT met Forward Checking (2) of dat alles met MCV (3)?");
			string lezen2 = Console.ReadLine();
			int Algortimegetal = int.Parse(lezen2);
			string input = string.Join("", lezen.Split(' ')); //Zet het om naar een leesbare string!

			//We gaan hem 10 keer runnen, om daar in de analyse het gemiddelde van te pakken!
			for (int i=0;i<10;i++) {
			

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
				KiesAlgoritme(Algortimegetal, Sudoku);
			}

			
			
		}

		static void KiesAlgoritme(int nummer, Nummer[,] Sudoku)
		{
			if (nummer == 1)
			{
				Stopwatch stopwatch = new Stopwatch(); //We gebruiken een stopwatch om de tijd te meten!
				stopwatch.Restart();
				
				//Nu lijsten aanmaken van rijen, van kollomen, en de mogelijkheden per cel!
				List<List<int>> rijenLijst = MaakRijenLijst(Sudoku);
				List<List<int>> kolommenLijst = MaakKolommenLijst(Sudoku);
				List<Stack<int>> MogelijkHedenPerCel = MaakMogelijkHedenPerCel(Sudoku);

				int Cursor = 0; //We beginnen met de cursor op puntje bij de 1e item uit de sudoku.
				bool ZittenWeInDeAchteruit = false; //Bool om bij te houden of we terug aan het gaan zijn!

				while (Cursor < 81) //Zodra de cursor voorbij cel 80 is, ben je klaar met de sudoku!
				{
					//Als de stack waarvan we de mogelijkheid willen checken leeg is, moet deze waarde van dit item op 0 worden gezet, 
					//de stack worden aangevuld en de cursor een stap terug. We moeten wel voorkomen dat we terug gaan, bij een locked aankomen, en daardoor weer vooruitgaan!
					//Daarom hebben we een bool anagemaakt die checkt of we 'naar achter' bewegen. Zo ja, en we komen bij een locked cel aan, gaan we nog een stapje achteruit!

					int yCoordinaat = Cursor / 9; //Van Cursor naar coodinaat. Rekentrucje om van een getal tussen 0 en 80 naar de coordinaat in de 2D array te gaan
					int xCoordinaat = Cursor % 9;

					//Op moment dat de mogelijkheden lijst empty is, hervullen we de stack, zetten we de waarde op 0 en gaat de cursor eentje achteruit!
					if (MogelijkHedenPerCel[Cursor].Count == 0)
					{
						MogelijkHedenPerCel[Cursor] = HerVulStack(); //We hervullen de stack
						Sudoku[yCoordinaat, xCoordinaat].Getalwaarde = 0; //Zetten de waarde weer op 0
						UpdateRijen(rijenLijst, Sudoku, yCoordinaat); //Updaten de rijen en kolommen
						UpdateKolommen(kolommenLijst, Sudoku, xCoordinaat);
						Cursor--; //En zetten de cursor een tikkie achteruit
						ZittenWeInDeAchteruit = true; //We gaan nu terug, dus we zitten in de achteruit!
						continue; //Continue betekent: ga naar de volgende iteratie van de while loop en skip wat hieronder staat.
					}

					if (Sudoku[yCoordinaat, xCoordinaat].Locked) //Als hij locked is...
					{
						if (ZittenWeInDeAchteruit) { Cursor--; } //Als we in de achteruitzitten en we komen aan bij een locked cel, moeten we nog eentje naar achteren!
						else { Cursor++; } //Anders gaan we gewoon weer door naar de volgende cel
						continue;

					}

					ZittenWeInDeAchteruit = false; //We zetten em weer op default waarde
					Sudoku[yCoordinaat, xCoordinaat].Getalwaarde = MogelijkHedenPerCel[Cursor].Pop(); //Pak de bovenste van de stack!

					//Als de waarde al in de Rij, Blok, Of kolom staat, moeten we naar de volgende!
					if (StaatMijnWaardeAlInDeRij(rijenLijst, yCoordinaat, Sudoku[yCoordinaat, xCoordinaat].Getalwaarde) ||
					StaatMijnWaardeAlInHetBlok(Sudoku, yCoordinaat, xCoordinaat) ||
					StaatMijnWaardeAlInDeKolom(kolommenLijst, xCoordinaat, Sudoku[yCoordinaat, xCoordinaat].Getalwaarde)) // || betekent 'or'. Als een vd gevallen waar is, kijken we naar het volgende item op de stack.
					{
						continue; //En nu gaan we eigenlijk het hele proces opnieuw doen, alleen dat met een verminderde stack!	

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

				stopwatch.Stop(); //STOP DE TIJD!
				System.Console.WriteLine("----------------------------------");
				Console.WriteLine("Best Solution:");
				PrintSudoku(Sudoku);
				System.Console.WriteLine(stopwatch.Elapsed.ToString()); //Print de tijd!

			}
			if (nummer == 2)
			{
				Stopwatch stopwatch = new Stopwatch(); //We gebruiken een stopwatch om de tijd te meten!
				stopwatch.Restart();

				//Nu lijsten aanmaken van rijen, van kollomen, en de mogelijkheden per cel!
				List<List<int>> rijenLijst = MaakRijenLijst(Sudoku);
				List<List<int>> kolommenLijst = MaakKolommenLijst(Sudoku);
				List<List<Stack<int>>> Geschiedenis = new List<List<Stack<int>>>();
				List<Stack<int>> OrgineleMogelijkHedenPerCel = MaakMogelijkHedenPerCel2(Sudoku, rijenLijst, kolommenLijst);


				Geschiedenis.Add(OrgineleMogelijkHedenPerCel); //We maken een volledige geschiedenis aan!

				int Cursor = 0; //We beginnen met de cursor op puntje bij de 1e item uit de sudoku.
				bool ZittenWeInDeAchteruit = false; //Bool om bij te houden of we terug aan het gaan zijn!

				while (Cursor < 81) //Zodra de cursor voorbij cel 80 is, ben je klaar met de sudoku!
				{
					List<Stack<int>> UpToDateMogelijkHedenPerCel = new List<Stack<int>>();

					int Vorige = Geschiedenis.Count - 1;

					//Het laatste geschiedenis item is het laatste item in de geschiedenislijst!
					List<Stack<int>> laatsteGeschiedenisItem = Geschiedenis[Vorige]
						.Select(stack => new Stack<int>(stack.Reverse()))
						.ToList();

					// Voeg de nieuwe lijst van stacks toe aan de lijst
					UpToDateMogelijkHedenPerCel.AddRange(laatsteGeschiedenisItem);

					int yCoordinaat = Cursor / 9; //Van Cursor naar coodinaat. Rekentrucje om van een getal tussen 0 en 80 naar de coordinaat in de 2D array te gaan
					int xCoordinaat = Cursor % 9;

					//Op moment dat de mogelijkheden lijst empty is, hervullen we de stack, zetten we de waarde op 0 en gaat de cursor eentje achteruit!
					if (UpToDateMogelijkHedenPerCel[Cursor].Count() == 0)
					{
						Sudoku[yCoordinaat, xCoordinaat].Getalwaarde = 0;
						Geschiedenis.RemoveAt(Geschiedenis.Count() - 1);
						Cursor--; //En zetten de cursor een tikkie achteruit

						yCoordinaat = Cursor / 9; //Van Cursor naar coodinaat. Rekentrucje om van een getal tussen 0 en 80 naar de coordinaat in de 2D array te gaan
						xCoordinaat = Cursor % 9;

						while (Sudoku[yCoordinaat, xCoordinaat].Locked == true)
						{
							Cursor--;
							yCoordinaat = Cursor / 9; //Van Cursor naar coodinaat. Rekentrucje om van een getal tussen 0 en 80 naar de coordinaat in de 2D array te gaan
							xCoordinaat = Cursor % 9;

						}

						Geschiedenis[Geschiedenis.Count - 1][Cursor].Pop(); //En pop het bovenste item!
						ZittenWeInDeAchteruit = true; //We gaan nu terug, dus we zitten in de achteruit!

						continue; //Continue betekent: ga naar de volgende iteratie van de while loop en skip wat hieronder staat.
					}

					if (Sudoku[yCoordinaat, xCoordinaat].Locked) //Als hij locked is...
					{
						if (ZittenWeInDeAchteruit) { Cursor--; } //Als we in de achteruitzitten en we komen aan bij een locked cel, moeten we nog eentje naar achteren!
						else { Cursor++; } //Anders gaan we gewoon weer door naar de volgende cel
						continue;

					}

					ZittenWeInDeAchteruit = false; //We zetten em weer op default waarde
					Sudoku[yCoordinaat, xCoordinaat].Getalwaarde = UpToDateMogelijkHedenPerCel[Cursor].Pop(); //Pak de bovenste van de stack!
					
					//Wordt nu een van de domeinen leeg? Dan terug, anders door.
					//Dus: ga elk vakje in rij, kolom, en blok na en pas hun domeinen aan. Is een van deze nu leeg? Reset alle domeinen en continue.
					bool IkBenLeegHelpHelp = false;

					// Update de domeinen van vakjes na het huidige vakje in de rij
					for (int x = xCoordinaat + 1; x < 9; x++)
					{
						UpdateDomeinenVoorCel(Sudoku, UpToDateMogelijkHedenPerCel, yCoordinaat, x, Sudoku[yCoordinaat, xCoordinaat].Getalwaarde);

						if (UpToDateMogelijkHedenPerCel[yCoordinaat * 9 + x].Count() == 0)
						{
							IkBenLeegHelpHelp = true; //De stack is leeg, dus we breken de loop
							break;
						}
					}

					// Update de domeinen van vakjes na het huidige vakje in de kolom
					for (int y = yCoordinaat + 1; y < 9; y++)
					{
						if (!IkBenLeegHelpHelp)
						{
							UpdateDomeinenVoorCel(Sudoku, UpToDateMogelijkHedenPerCel, y, xCoordinaat, Sudoku[yCoordinaat, xCoordinaat].Getalwaarde);
							if (UpToDateMogelijkHedenPerCel[y * 9 + xCoordinaat].Count() == 0)
							{
								IkBenLeegHelpHelp = true; //De stack is leeg, dus we breken de loop
								break;
							}

						}

					}

					// Update de domeinen van vakjes na het huidige vakje in het blok
					int blokYStart = yCoordinaat - (yCoordinaat % 3);
					int blokXStart = xCoordinaat - (xCoordinaat % 3);


					for (int y = blokYStart; y < blokYStart + 3; y++)
					{
						for (int x = blokXStart; x < blokXStart + 3; x++)
						{
							if (y == yCoordinaat)
							{
								if (x > xCoordinaat & !IkBenLeegHelpHelp)
								{
									UpdateDomeinenVoorCel(Sudoku, UpToDateMogelijkHedenPerCel, y, x, Sudoku[yCoordinaat, xCoordinaat].Getalwaarde);
									if (UpToDateMogelijkHedenPerCel[y * 9 + x].Count == 0)
									{
										IkBenLeegHelpHelp = true; //De stack is leeg, dus we breken de loop
										break;
									}
								}
							}
							if (y > yCoordinaat & !IkBenLeegHelpHelp)
							{
								UpdateDomeinenVoorCel(Sudoku, UpToDateMogelijkHedenPerCel, y, x, Sudoku[yCoordinaat, xCoordinaat].Getalwaarde);
								if (UpToDateMogelijkHedenPerCel[y * 9 + x].Count == 0)
								{
									IkBenLeegHelpHelp = true; //De stack is leeg, dus we breken de loop
									break;
								}
							}
						}
					}

					//Is geen een domein leeg geraakt? Fantastisch! Voeg dan alle domeinen toe aan de geschiedenis en zet de cursor een stap vooruit!
					if (!IkBenLeegHelpHelp)
					{
						Geschiedenis.Add(UpToDateMogelijkHedenPerCel);
						Cursor++;
					}

					//Als er maar één ding leeg was, moeten we alle domeinen weer hervullen en een stap terug.
					if (IkBenLeegHelpHelp)
					{
						//Draai de wijziging in domeinen terug, maar het maak het domein waar je mee bezig bent wel een stapje kleiner.
						Stack<int> DomeinVanAlleenDezeCel = UpToDateMogelijkHedenPerCel[Cursor];
						Geschiedenis[Geschiedenis.Count() - 1][Cursor] = DomeinVanAlleenDezeCel;
						continue;

					}
				}

				//Als hij hier uitkomt is hij klaar!

				stopwatch.Stop(); //STOP DE TIJD!
				System.Console.WriteLine("----------------------------------");
				Console.WriteLine("Best Solution:");
				PrintSudoku(Sudoku);
				System.Console.WriteLine(stopwatch.Elapsed.ToString()); //Print de tijd!

			}
			if (nummer == 3)
			{
				Stopwatch stopwatch = new Stopwatch(); //We gebruiken een stopwatch om de tijd te meten!
				stopwatch.Restart();
				
				//Nu lijsten aanmaken van rijen, van kollomen, en de mogelijkheden per cel!
				List<List<int>> rijenLijst = MaakRijenLijst(Sudoku);
				List<List<int>> kolommenLijst = MaakKolommenLijst(Sudoku);
				List<Dictionary<int, (Stack<int>, int)>> Geschiedenis = new List<Dictionary<int, (Stack<int>, int)>>(); //Lijst van dict met <gridnummer, (stack, aantal in domein)>
				Dictionary<int, (Stack<int>, int)> OrgineleMogelijkHedenPerCel = MaakMogelijkHedenPerCel3(Sudoku, rijenLijst, kolommenLijst);
				Dictionary<int, (Stack<int>, int)> OrgineleMogelijkHedenPerCeltemp = SorteerDictionary(OrgineleMogelijkHedenPerCel); //We zetten de domeinen op volgorde
				Geschiedenis.Add(OrgineleMogelijkHedenPerCeltemp); //We maken een volledige geschiedenis aan!

				int Cursor = OrgineleMogelijkHedenPerCel.Keys.First();//We beginnen bij het element met de minste elementen.

				while (BevatEenNul(Sudoku))
				{
					//--Pak laatste item uit geschiedenis, vanuit daar gaan we werken---
					Dictionary<int, (Stack<int>, int)> UpToDateMogelijkHedenPerCel = new Dictionary<int, (Stack<int>, int)>();
					int Vorige = Geschiedenis.Count - 1;

					// Sorteer de nieuwe lijst van stacks op basis van de values (de domeinen)
					Dictionary<int, (Stack<int>, int)> laatsteGeschiedenisItem = SorteerDictionary(Geschiedenis[Vorige]);
					UpToDateMogelijkHedenPerCel = laatsteGeschiedenisItem;

					Cursor = UpToDateMogelijkHedenPerCel.Keys.First();//We beginnen bij het element met de minste elementen.

					int yCoordinaat = Cursor / 9; //Van Cursor naar coodinaat. Rekentrucje om van een getal tussen 0 en 80 naar de coordinaat in de 2D array te gaan
					int xCoordinaat = Cursor % 9;

					if (UpToDateMogelijkHedenPerCel[Cursor].Item1.Count == 0) // als domein leeg is...
					{
						Sudoku[yCoordinaat, xCoordinaat].Getalwaarde = 0; //Zet de getalwaarde terug op 0
						Geschiedenis.RemoveAt(Geschiedenis.Count - 1); //Remove het laatste geschiedenis item
						Cursor = Geschiedenis[Geschiedenis.Count - 1].Keys.First();//En zetten de cursor een tikkie achteruit naar waar hij in het vorige geschiedenispunt was

						yCoordinaat = Cursor / 9; //Van Cursor naar coodinaat. Rekentrucje om van een getal tussen 0 en 80 naar de coordinaat in de 2D array te gaan
						xCoordinaat = Cursor % 9;

						Geschiedenis[Geschiedenis.Count - 1][Cursor].Item1.Pop(); //En pop het bovenste item!
						continue;
					}

					Sudoku[yCoordinaat, xCoordinaat].Getalwaarde = UpToDateMogelijkHedenPerCel[Cursor].Item1.Pop(); //Pak de bovenste van de stack!
					UpToDateMogelijkHedenPerCel[Cursor] = (UpToDateMogelijkHedenPerCel[Cursor].Item1, 50); //50 is een random, hoge waarde zodat als je een vakje hebt aangepast deze naar onder gaat in de aangepaste lijst!
					bool IkBenLeegHelpHelp = false;

					for (int x = 0; x < 9; x++)
					{
						if (x != xCoordinaat)
						{

							UpdateDomeinenVoorCel2(Sudoku, UpToDateMogelijkHedenPerCel, yCoordinaat, x, Sudoku[yCoordinaat, xCoordinaat].Getalwaarde);
							if (UpToDateMogelijkHedenPerCel[yCoordinaat * 9 + x].Item2 == 0)
							{
								IkBenLeegHelpHelp = true; //De stack is leeg, dus we breken de loop
								break;
							}
						}
					}

					// Update de domeinen van vakjes na het huidige vakje in de kolom
					for (int y = 0; y < 9; y++)
					{
						if (y != yCoordinaat)
						{
							if (!IkBenLeegHelpHelp)
							{
								UpdateDomeinenVoorCel2(Sudoku, UpToDateMogelijkHedenPerCel, y, xCoordinaat, Sudoku[yCoordinaat, xCoordinaat].Getalwaarde);
								if (UpToDateMogelijkHedenPerCel[y * 9 + xCoordinaat].Item2 == 0)
								{
									IkBenLeegHelpHelp = true; //De stack is leeg, dus we breken de loop
									break;
								}

							}
						}
					}

					// Update de domeinen van vakjes na het huidige vakje in het blok
					int blokYStart = yCoordinaat - (yCoordinaat % 3);
					int blokXStart = xCoordinaat - (xCoordinaat % 3);

					for (int y = blokYStart; y < blokYStart + 3; y++)
					{
						for (int x = blokXStart; x < blokXStart + 3; x++)
						{
							if (!IkBenLeegHelpHelp && !(x == xCoordinaat && y == yCoordinaat))
							{
								UpdateDomeinenVoorCel2(Sudoku, UpToDateMogelijkHedenPerCel, y, x, Sudoku[yCoordinaat, xCoordinaat].Getalwaarde);
								if (UpToDateMogelijkHedenPerCel[y * 9 + x].Item2 == 0)
								{
									IkBenLeegHelpHelp = true; //De stack is leeg, dus we breken de loop
									break;
								}
							}
						}
					}

					//Als er maar één ding leeg was, moeten we alle domeinen weer hervullen en een stap terug.
					if (!IkBenLeegHelpHelp)
					{
						Dictionary<int, (Stack<int>, int)> DeGeschiedenisEntry = SorteerDictionary(UpToDateMogelijkHedenPerCel);
						Geschiedenis.Add(DeGeschiedenisEntry);
						continue;
					}

					if (IkBenLeegHelpHelp)
					{
						Stack<int> DomeinVanAlleenDezeCel = UpToDateMogelijkHedenPerCel[Cursor].Item1;
						int countVanAlleenDezeCel = DomeinVanAlleenDezeCel.Count;//hier moet je wel de count nemen! niet het getal 100
						Geschiedenis[Geschiedenis.Count() - 1][Cursor] = (DomeinVanAlleenDezeCel, countVanAlleenDezeCel);
						continue;
					}
				}

				//Als hij hier uitkomt is hij klaar!

				stopwatch.Stop(); //STOP DE TIJD!
				System.Console.WriteLine("----------------------------------");
				Console.WriteLine("Best Solution:");
				PrintSudoku(Sudoku);
				System.Console.WriteLine(stopwatch.Elapsed.ToString()); //Print de tijd!
			}
		}

		public static Dictionary<TKey, (Stack<TValue>, int)> SorteerDictionary<TKey, TValue>(Dictionary<TKey, (Stack<TValue>, int)> dictionary)
		{
			//Functie om een dictionary te sorteren!
			return dictionary.ToDictionary(kvp => kvp.Key, kvp => (new Stack<TValue>(kvp.Value.Item1.OrderByDescending(x => x)), kvp.Value.Item2))
				.OrderBy(x => x.Value.Item2).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
		}

		static bool BevatEenNul(Nummer[,] sudoku)
		{
			foreach (Nummer hokje in sudoku) //Check of er ergens nog een 0 staat in de Sudoku!
			{
				if (hokje.Getalwaarde == 0)
				{
					return true;
				}
			}
			return false;
		}

		static void UpdateDomeinenVoorCel(Nummer[,] sudoku, List<Stack<int>> mogelijkhedenLijst, int y, int x, int getalwaarde)
		{
			if (!sudoku[y, x].Locked)
			{
				// Verwijder het recent ingevulde getal uit het domein van het huidige vakje
				int ingevuldGetal = getalwaarde;
				List<int> temp = mogelijkhedenLijst[y * 9 + x].ToList();
				temp.Remove(ingevuldGetal);
				temp.Sort();
				temp.Reverse();
				mogelijkhedenLijst[y * 9 + x] = new Stack<int>(temp);
			}
		}

		static void UpdateDomeinenVoorCel2(Nummer[,] sudoku, Dictionary<int, (Stack<int>, int)> mogelijkhedenLijst, int y, int x, int getalwaarde)
		{
			if (!sudoku[y, x].Locked && sudoku[y, x].Getalwaarde == 0)
			{
				int ingevuldGetal = getalwaarde;
				(Stack<int>, int) value = mogelijkhedenLijst[y * 9 + x];
				List<int> temp = value.Item1.ToList();
				temp.Remove(ingevuldGetal);
				temp.Sort();
				temp.Reverse();

				//Pas de mogelijkhedenlijst weer aan!
				mogelijkhedenLijst[y * 9 + x] = (new Stack<int>(temp), temp.Count); //Een niewe stack, én een nieu getal!
																					

				//En sorteer de lijst!
				Dictionary<int, (Stack<int>, int)> mogelijkhedenLijsttemp = SorteerDictionary(mogelijkhedenLijst);
				mogelijkhedenLijst=mogelijkhedenLijsttemp;
			}
		}

		static void PrintSudoku(Nummer[,] sudoku) //Handig om uit te printen!
		{
			for (int i = 0; i < 9; i++)
			{
				if (i % 3 == 0 && i > 0)
				{
					Console.WriteLine("------+-------+------");
				}

				for (int j = 0; j < 9; j++)
				{
					if (j % 3 == 0 && j > 0)
					{
						Console.Write("| ");
					}

					Console.Write(sudoku[i, j].Getalwaarde == 0 ? " " : sudoku[i, j].Getalwaarde.ToString());
					Console.Write(" ");
				}
				Console.WriteLine();
			}
		}
		static void UpdateRijen(List<List<int>> rijenLijst, Nummer[,] sudoku, int rijIndex) //Functie om de rijenlijst te updaten
		{
			// Lees de rij uit de sudoku in
			List<int> nieuweRij = new List<int>(); //Maak een nieuwe lijst
			for (int j = 0; j < 9; j++)
			{
				nieuweRij.Add(sudoku[rijIndex, j].Getalwaarde); //Vul em met alle relevante waarden (dus op één rij in de sudoku)
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

		static void UpdateKolommen(List<List<int>> kolommenLijst, Nummer[,] sudoku, int kolomIndex) //Gelijk aan UpdateRijen
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
		static bool StaatMijnWaardeAlInDeRij(List<List<int>> rijenLijst, int rijIndex, int waarde) //Checkker of de waarde al in de rij staat!
		{
			List<int> rij = rijenLijst[rijIndex];
			return rij.Contains(waarde);
		}

		static bool StaatMijnWaardeAlInDeKolom(List<List<int>> kolommenlijst, int kolomindex, int waarde) //Idem aan De Rij variant
		{
			List<int> kolom = kolommenlijst[kolomindex];
			return kolom.Contains(waarde);
		}

		static List<Stack<int>> MaakMogelijkHedenPerCel(Nummer[,] sudoku) //Functie die stacks aan maakt met de mogelijkheden per cel.
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

		static List<Stack<int>> MaakMogelijkHedenPerCel2(Nummer[,] sudoku, List<List<int>> rijenLijst, List<List<int>> kolommenLijst) //Functie die stacks aan maakt met de mogelijkheden per cel.
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

						List<int> alleGetallen = Enumerable.Range(1, 9).ToList();

						// Haal getallen uit het blok
						List<int> blokGetallen = MaakBlokGetallenLijst(sudoku, i, j);

						// Bereken het verschil tussen alleGetallen en de gecombineerde lijst van rij, kolom en blok
						List<int> mogelijkheden = alleGetallen.Except(rijenLijst[i].Concat(kolommenLijst[j]).Concat(blokGetallen)).ToList();
						mogelijkheden.Sort(); //Sorteer de lijst...
						mogelijkheden.Reverse(); //En draai em om zodat hij goed op de stack komt!
												 // Voeg de mogelijkheden toe aan de stack
						foreach (var mogelijkheid in mogelijkheden)
						{
							mogelijkhedenStack.Push(mogelijkheid);
						}
					}

					mogelijkhedenStacks.Add(mogelijkhedenStack);

				}
			}

			return mogelijkhedenStacks;
		}

		static Dictionary<int, (Stack<int>, int)> MaakMogelijkHedenPerCel3(Nummer[,] sudoku, List<List<int>> rijenLijst, List<List<int>> kolommenLijst)
		{
			Dictionary<int, (Stack<int>, int)> mogelijkhedenStacks = new Dictionary<int, (Stack<int>, int)>(); // We gaan een lijst aan stacks maken

			for (int i = 0; i < 9; i++) // Loop over alle cellen
			{
				for (int j = 0; j < 9; j++)
				{
					int cursor = 9 * i + j;
					Nummer cel = sudoku[i, j];

					int count;
					Stack<int> stack;


					if (cel.Locked)
					{
						// Als de cel Locked is, wil je alleen dat nummer op de stack.
						count = 100; //100 zodat het helemaal achteraan de dictionary komt
						stack = new Stack<int>();
						stack.Push(cel.Getalwaarde);
					}
					else
					{
						List<int> alleGetallen = Enumerable.Range(1, 9).ToList();

						// Haal getallen uit het blok
						List<int> blokGetallen = MaakBlokGetallenLijst(sudoku, i, j);

						// Bereken het verschil tussen alleGetallen en de gecombineerde lijst van rij, kolom en blok
						List<int> mogelijkheden = alleGetallen.Except(rijenLijst[i].Concat(kolommenLijst[j]).Concat(blokGetallen)).ToList();
						mogelijkheden.Sort(); // Sorteer de lijst...
						mogelijkheden.Reverse(); // En draai hem om zodat hij goed op de stack komt!

						stack = new Stack<int>(mogelijkheden);
						count = stack.Count;
					}

					mogelijkhedenStacks[cursor] = (stack, count);
				}
			}

			return mogelijkhedenStacks;
		}

		static bool StaatMijnWaardeAlInHetBlok(Nummer[,] sudoku, int rij, int kolom) //Checker of de waarde al in het blok staat
		{
			int blokYCoordinaat = rij / 3;  // Bereken het blokcoordinaat voor de rij (door de int hoef je niet te flooren)
			int blokXCoordinaat = kolom / 3;  // Bereken het blokcoordinaat voor de kolom

			for (int y = blokYCoordinaat * 3; y < 3 + blokYCoordinaat * 3; y++) //We gaan alle cellen in het blok doorlopen
			{
				for (int x = blokXCoordinaat * 3; x < 3 + blokXCoordinaat * 3; x++)
				{
					// Controleer of de waarde al in het blok zit, behalve de cel waarin we net hebben veranderd
					if (y != rij || x != kolom)
					{
						if (sudoku[y, x].Getalwaarde == sudoku[rij, kolom].Getalwaarde) //Het getal dat je invult kan nooit een nul zijn, dus daar hoef je geen eexception voor te schrijven
						{
							return true;  // De waarde zit al in het blok
						}
					}
				}
			}

			return false;  // De waarde zit nog niet in het blok
		}

		static List<List<int>> MaakRijenLijst(Nummer[,] sudoku) //Functie om adhv een gehele sudoku een rijenlijst aan te maken.
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

		static List<List<int>> MaakKolommenLijst(Nummer[,] sudoku) //Idem aan de Rij variant
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

		static List<int> MaakBlokGetallenLijst(Nummer[,] sudoku, int rij, int kolom)
		{
			int blokYCoordinaat = rij / 3;
			int blokXCoordinaat = kolom / 3;

			List<int> blokGetallenLijst = new List<int>();

			for (int y = blokYCoordinaat * 3; y < 3 + blokYCoordinaat * 3; y++)
			{
				for (int x = blokXCoordinaat * 3; x < 3 + blokXCoordinaat * 3; x++)
				{
					blokGetallenLijst.Add(sudoku[y, x].Getalwaarde);
				}
			}

			return blokGetallenLijst;
		}

	}
}
