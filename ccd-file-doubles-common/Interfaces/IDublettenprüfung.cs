using System.Collections.Generic;
using CCD.File.Doubles.Common.Enums;

namespace CCD.File.Doubles.Common.Interfaces
{
    public interface IDublettenprüfung
    {
        IEnumerable<IDublette> Sammle_Kandidaten(string pfad);
        IEnumerable<IDublette> Sammle_Kandidaten(string pfad, Vergleichsmodi modus);

        IEnumerable<IDublette> Prüfe_Kandidaten(IEnumerable<IDublette> kandidaten);
    }
}
