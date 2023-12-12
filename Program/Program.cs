using System;
using System.Runtime.CompilerServices;

//Evaluatie functie schrijven
//Klasse blok maken
//Klasse Nummer maken
//INput inlezen

namespace MyApp 
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hoi, geef input in een reeks aan getallen!");
            //Sudoko su = new Sudoko(string aan getallen)
            string input = "003020600900305001001806400008102900700000008006708200002609500800203009005010300";

            //string input = Console.ReadLine(); //Ervan uitgaande dat er input is
            Nummer[,] sudoku = new Nummer[9, 9];

            //Nu gaan we het hele grid invullen
            for (int i = 0; i < 9; i++) //ywaarden
            {
                for (int j = 0; j < 9; j++) //xwaarden
                {
                    int index = i * 9 + j; //Leipe methode G
                    int getal = int.Parse(input[index].ToString());
                    bool vergrendeld;
                    
                    if (getal != 0) { 
                        vergrendeld = true;
                        }
                    
                    else {
                        vergrendeld = false;
                    }
                    
                    sudoku[i, j] = new Nummer(getal, vergrendeld);
                }


                //Nu moeten we hiervan blokken maken, en per blok invullen. 
            }

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

    internal class Blok 
    {
        public Nummer[,] Cel = new Nummer[3, 3];
        // Implementeer methoden om met de cijfers in het blok te werken
    }

    internal class Sudoko 
    {
        public Blok[,] Cel = new Blok[3, 3];

        public Sudoko(string input) {



        }
        // Implementeer methoden om met de cijfers in het blok te werken
    }
}
}