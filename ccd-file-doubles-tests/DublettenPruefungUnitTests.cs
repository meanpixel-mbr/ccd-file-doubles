using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using CCD.File.Doubles.Common.Enums;
using CCD.File.Doubles.Common.Interfaces;
using CCD.File.Doubles.Common.Implementation;
using Xunit;

namespace CCD.File.Doubles.Tests
{
    public class DublettenPruefungUnitTests
    {
        IDublettenprüfung _pruefer = new DublettenPruefung();

        private MockFileSystem GenerateMockFileSystem()
        {
            return new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {@"c:\temp\folder.01\file.01.txt", new MockFileData("Hellu World Dooble")},
                {@"c:\temp\folder.01\file.02.txt", new MockFileData("We are one")},
                {@"c:\temp\folder.01\file.03.txt", new MockFileData("Lorem Ipsum")},
                {@"c:\temp\folder.01\file.04.txt", new MockFileData("This is a unique one")},
                {@"c:\temp\folder.01\anotherFile.txt", new MockFileData("Library Kata 'Dateidtbletuen aufspüren'")},

                {@"c:\temp\folder.02\file.01.txt", new MockFileData("Hello Everyone")},
                {@"c:\temp\folder.02\file.02.txt", new MockFileData("We are one")},
                {@"c:\temp\folder.02\andAnotherFile.txt", new MockFileData("Library Kata 'Dateidubletten aufspüren'")},

                {@"c:\temp\folder.03\file.01.txt", new MockFileData("Hlleo World Double")},
                {@"c:\temp\folder.03\file.03.txt", new MockFileData("Lorem Ipsom")},
                {@"c:\temp\folder.03\dummy.txt", new MockFileData("Library Kata 'Dateidubletten aufspüren'")},

                {@"c:\temp\folder.04\file.01.txt", new MockFileData("Hello dlorw Double")},
                {@"c:\temp\folder.04\file.02.txt", new MockFileData("We are one")},
                {@"c:\temp\folder.04\file.03.txt", new MockFileData("Lorem Ipsum")},
            });
        }

        [Fact]
        public void Implements_IDublettenpruefung()
        {
            // arrange
            // act
            // assert
            Assert.IsAssignableFrom<IDublettenprüfung>(_pruefer);
        }

        [Theory]
        [InlineData(@"C:\temp\")]
        public void Returns_Enumerable_IDublette(string path)
        {
            // arrange
            var fileSystem = GenerateMockFileSystem();
            var pruefer = new DublettenPruefung(fileSystem);

            // act
            var result = pruefer.Sammle_Kandidaten(path);
            // assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEnumerable<IDublette>>(result);
        }

        [Theory]
        [InlineData(@"C:\temp\")]
        public void Returns_Three_Enumerable_IDublette_Based_On_Filesystem_Mock(string path)
        {
            // arrange
            var fileSystem = GenerateMockFileSystem();
            var pruefer = new DublettenPruefung(fileSystem);

            // act
            var result = pruefer.Sammle_Kandidaten(path);
            // assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEnumerable<IDublette>>(result);
            Assert.True(result?.Count() == 3);
        }

        [Theory]
        [InlineData(@"C:\temp\")]
        public void Returns_Four_Enumerable_IDublette_Based_On_Filesystem_Mock_Searching_Only_For_Size(string path)
        {
            // arrange
            var fileSystem = GenerateMockFileSystem();
            var pruefer = new DublettenPruefung(fileSystem);

            // act
            var result = pruefer.Sammle_Kandidaten(path, Vergleichsmodi.Größe);
            // assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEnumerable<IDublette>>(result);
            Assert.True(result?.Count() == 4);
        }

        [Theory]
        [InlineData(@"C:\temp\")]
        public void Returns_IDublette_Second_Pass(string path)
        {
            // arrange
            var fileSystem = GenerateMockFileSystem();
            var pruefer = new DublettenPruefung(fileSystem);

            // act
            var result = pruefer.Sammle_Kandidaten(path);
            var secondPass = pruefer.Prüfe_Kandidaten(result);

            // assert
            Assert.NotNull(secondPass);
            Assert.IsAssignableFrom<IEnumerable<IDublette>>(secondPass);
        }

        [Theory]
        [InlineData(@"C:\temp\")]
        public void Returns_Lesser_IDublette_After_Second_Pass(string path)
        {
            // arrange
            var fileSystem = GenerateMockFileSystem();
            var pruefer = new DublettenPruefung(fileSystem);

            // act
            var result = pruefer.Sammle_Kandidaten(path, Vergleichsmodi.Größe_und_Name);
            var kandidaten = result.ToList();
            var secondPass = pruefer.Prüfe_Kandidaten(kandidaten);

            // assert
            Assert.NotNull(secondPass);
            Assert.IsAssignableFrom<IEnumerable<IDublette>>(secondPass);
            Assert.True(kandidaten.Count() > secondPass.Count());
        }
    }
}
