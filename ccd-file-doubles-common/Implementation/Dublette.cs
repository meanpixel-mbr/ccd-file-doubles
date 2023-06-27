using System.Collections.Generic;
using CCD.File.Doubles.Common.Interfaces;

namespace CCD.File.Doubles.Common.Implementation
{
    public class Dublette : IDublette
    {
        private readonly List<string> _dateipfade = new List<string>();

        public Dublette(string pfad)
        {
            _dateipfade.Add(pfad);
        }

        public Dublette(List<string> pfade)
        {
            _dateipfade = pfade;
        }

        public IEnumerable<string> Dateipfade => _dateipfade;
    }
}
