using System.Collections.Generic;
using CCD.File.Doubles.Common.Interfaces;

namespace CCD.File.Doubles.Common.Implementation
{
    public class DublettenKandidat
    {
        public string Name { get; set; }
        public long Size { get; set; }
        public List<string> Folders { get; set; }

        public DublettenKandidat()
        {
            Folders = new List<string>();
        }
    }
}