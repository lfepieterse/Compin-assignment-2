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
            Student Robin = new Student(5, true);
            Console.WriteLine(Robin.nummer.ToString());
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
    }
}