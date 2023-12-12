using System;

//Evaluatie functie schrijven
//Klasse blok maken
//Klasse Nummer maken
namespace MyApp 
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Nummer[,] Sudoku = new Nummer[9, 9];

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

    internal class Grid 
    {
        public Blok[,] Cel = new Blok[3, 3];
        // Implementeer methoden om met de cijfers in het blok te werken
    }
}
}