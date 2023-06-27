using System;
using System.Linq;
using CCD.File.Doubles.Common.Enums;
using CCD.File.Doubles.Common.Implementation;

namespace CCD.File.Doubles
{
    class Program
    {
        static void Main(string[] args)
        {
            var pruefer = new DublettenPruefung();
            var kandidaten = pruefer.Sammle_Kandidaten(@"C:\Users", Vergleichsmodi.Größe);
            var dubletten = pruefer.Prüfe_Kandidaten(kandidaten);

            foreach (var dubl in dubletten)
            {
                Console.WriteLine("Dublette gefunden:");
                foreach (var pfad in dubl.Dateipfade)
                {
                    Console.WriteLine(pfad);
                }

                Console.WriteLine("_______________________");
                Console.WriteLine("");
            }

            Console.WriteLine("Fertig");
            Console.ReadLine();
        }
    }
}
