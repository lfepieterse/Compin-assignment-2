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

            int[] Totaleduplicatenwaardenlijst = new int[18]; //Er zijn in totaal 18 rijen + kolommen

            for (int rijindex = 0; rijindex<9;rijindex++) //Loop elke rij langs en evalueer deze
            {
                Totaleduplicatenwaardenlijst[rijindex] = EvalueerRij(Grid,rijindex);
            }

            for (int kolomindex = 0; kolomindex<9;kolomindex++) //Kolomindex + 9 = lijstindex. Hier lopen we alle kolomen langs
            {
                Totaleduplicatenwaardenlijst[kolomindex + 9] = EvalueerKolom(Grid,kolomindex); //+ 9, omdat de 2e helft vd lijst voor de kolommen is
            }

            //EVALUATIE LIJST AANGEMAAKT!!

            //NU: pak blok, ga alle getallen swappen en check de ev waarde
            
            string Yeehaw = Console.ReadLine(); //random line for fun en voor debuggging

            //NU willen we een heuristieke functie maken, die aan de hand van een totaal grid en rij en kolom aanduiding de heur waarde kan 
            //berekenen.


        }
        static int EvalueerRij(Blok[,] grid, int rijIndex)
        {
            List<int> Waarden = new List<int>();
            int duplicaten;
            int BlockRowIndex;
            //Nu willen we een lijst maken van alle getallen in de verschillende blokken, in dezelfde kolommen
            
            //BlockRowIndex bepalen: op welke rij (vd blokken, dus 1e, 2e of 3e) moeten wij gaan zoeken?
            if (rijIndex < 3) {BlockRowIndex = 0; }
            else if (rijIndex < 6) {BlockRowIndex = 1;        }
            else { BlockRowIndex = 2;}

            int rowIndexInBlok = rijIndex % 3; //Welke rij in het blok we moeten zoeken is onze eerdere index % 3.

            for (int blokCol = 0; blokCol < 3; blokCol++)
            {
                for (int celRow = 0; celRow < 3; celRow++)
                {
                    int WaardeUItHetVakje = grid[blokCol, BlockRowIndex].Cel[celRow, rowIndexInBlok].Getalwaarde;
                    Waarden.Add(WaardeUItHetVakje);
                }
             }
             System.Console.WriteLine(Waarden.Count());
            
            // Bepaal het aantal duplicaten in de lijst
            duplicaten = Waarden.Count - Waarden.Distinct().Count();
            return duplicaten;
        }
        static int EvalueerKolom(Blok[,] grid, int Kolomindex)
        {
            List<int> Waarden = new List<int>();
            int duplicaten = 0;
            int BlockColIndex;
            //Nu willen we een lijst maken van alle getallen in de verschillende blokken, in dezelfde kolommen
            
            //BlockColIndex bepalen: op welke kolom (vd blokken, dus 1e, 2e of 3e) moeten wij gaan zoeken?
            if (Kolomindex < 3 + 9) {BlockColIndex = 0; }
            else if (Kolomindex < 6 + 9) {BlockColIndex = 1;}
            else { BlockColIndex = 2;}

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
            duplicaten = Waarden.Count - Waarden.Distinct().Count();
            return duplicaten;
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
                    if (blok.Cel[i,j].Locked == true) {
                        verbodencijfers.Add(blok.Cel[i,j].Getalwaarde); //hiermee maken we een lijst aan getallen die verboden zijn en dus niet mogen worden ingevuld!
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