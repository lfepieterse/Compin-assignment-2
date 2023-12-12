using System;
using System.Runtime.CompilerServices;
using random;


//Evaluatie functie schrijven
//Klasse blok maken
//Klasse Nummer maken
//INput inlezen

namespace MyApp 
{

    internal class Blok 
    {
        public Nummer[,] Cel = new Nummer[3, 3];
        // Implementeer methoden om met de cijfers in het blok te werken
    }

    internal class Program
    {
        private static Random random = new Random();
        static void Main(string[] args)
        {
            Console.WriteLine("Hoi, geef input in een reeks aan getallen!");
            //Sudoko su = new Sudoko(string aan getallen)
            string input = "003020600900305001001806400008102900700000008006708200002609500800203009005010300";

            //string input = Console.ReadLine(); //Ervan uitgaande dat er input is

            //-----BLOKKEN AANMAKEN---
            Blok[,] Grid = new Blok[3, 3];

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Grid[i, j] = MaakBlok(input, i * 3, j * 3); //Hiermee maken we de blokken aan.
                    BlokInVuller(Grid[i, j]); //moet andere naam krijgen
                }
            }

            //---BLOKKEN INVULLEN---


               // Nu kun je individuele blokken invullen en bewerken
        // blokken[0, 0] is het eerste blok bijvoorbeeld.


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

                    blok.Cel[i, j].Getalwaarde = willekeurigGetal;

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
    

   internal class Nummer
    {
        public int Getalwaarde { get; set; }
        public bool Locked { get; set; }
        public Nummer(int Getal, bool JaOfNee) {

            Getalwaarde = Getal;
            Locked = JaOfNee;

    }

    
    // internal class Sudoko 
    // {
    //     public Blok[,] Cel = new Blok[3, 3];

    //     public Sudoko(string input) {



    //     }
    //     // Implementeer methoden om met de cijfers in het blok te werken
    // }
}
}