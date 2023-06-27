using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using CCD.File.Doubles.Common.Enums;
using CCD.File.Doubles.Common.Interfaces;

namespace CCD.File.Doubles.Common.Implementation
{

    public class DublettenPruefung : IDublettenprüfung
    {
        private readonly IFileSystem _fileSystem;
        private readonly List<string> _notAccessibleFilenames = new List<string>();

        public DublettenPruefung(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        // If nothing passed to the constructor, use the default System.IO filesystem
        public DublettenPruefung()
            : this(new FileSystem())
        {

        }

        public List<System.IO.Abstractions.IFileInfo> GetFiles(IDirectoryInfo directoryInfo, string searchPattern, SearchOption searchOption)
        {
            var result = new List<System.IO.Abstractions.IFileInfo>();
            result.AddRange(directoryInfo.GetFiles(searchPattern, SearchOption.TopDirectoryOnly));
            if (searchOption == SearchOption.AllDirectories)
            {
                foreach (var subDirectory in directoryInfo.GetDirectories())
                {
                    try
                    {
                        result.AddRange(GetFiles(subDirectory, searchPattern, searchOption));
                    }
                    catch (System.UnauthorizedAccessException uae)
                    {
                        Console.WriteLine($"Access denied on folder: {subDirectory.FullName}");
                    }
                    catch (System.Exception ex)
                    {
                        Console.WriteLine($"Generic Exception on folder: {subDirectory.FullName}");
                    }
                }
            }
            return result;
        }

        public IEnumerable<IDublette> Sammle_Kandidaten(string pfad)
        {
            return Sammle_Kandidaten(pfad, Vergleichsmodi.Größe_und_Name);
        }

        public IEnumerable<IDublette> Sammle_Kandidaten(string pfad, Vergleichsmodi modus)
        {
            var candidate = new List<DublettenKandidat>();

            var directoryInfo = _fileSystem.DirectoryInfo.New(pfad);
            var files = GetFiles(directoryInfo, "*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var dublette = modus == Vergleichsmodi.Größe
                    ? candidate.SingleOrDefault(a => a.Size == file.Length)
                    : candidate.SingleOrDefault(a => a.Name == file.Name && a.Size == file.Length);

                if (dublette == null)
                {
                    dublette = new DublettenKandidat()
                    {
                        Name = file.Name,
                        Size = file.Length
                    };
                    candidate.Add(dublette);
                }

                dublette.Folders.Add(file.FullName);
            }

            var result = candidate.Where(a => a.Folders.Count > 1).Select(a => new Dublette(a.Folders));
            return result;
        }

        public IEnumerable<IDublette> Prüfe_Kandidaten(IEnumerable<IDublette> kandidaten)
        {
            var files = kandidaten.ToList();
            var result = new List<IDublette>();
            foreach (var file in files)
            {
                var fileInfos = file.Dateipfade.Select(a => _fileSystem.FileInfo.FromFileName(a)).ToList();
                var hashes = fileInfos.Select(a => CalculateMd5(a.FullName)).Distinct().ToList();

                if (hashes.Count == 1)
                {
                    result.Add(new Dublette(fileInfos.Select(a => a.FullName).ToList()));
                }
                else
                {
                    foreach (var hash in hashes)
                    {
                        if (fileInfos.Count(a => CalculateMd5(a.FullName) == hash) > 1)
                        {
                            result.Add(new Dublette(fileInfos.Where(a => CalculateMd5(a.FullName) == hash)
                                .Select(a => a.FullName).ToList()));
                        }
                    }
                }
            }

            return result;
        }

        private string CalculateMd5(string filename)
        {
            if (_notAccessibleFilenames.Any(a => a == filename)) return null;
            try
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = _fileSystem.File.OpenRead(filename))
                    {
                        var hash = md5.ComputeHash(stream);
                        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                    }
                }
            }
            catch (System.IO.IOException ioe)
            {
                _notAccessibleFilenames.Add(filename);
                Console.WriteLine($"File is not accessible: {filename}");
            }
            catch (System.UnauthorizedAccessException uae)
            {
                _notAccessibleFilenames.Add(filename);
                Console.WriteLine($"Access denied on file: {filename}");
            }
            catch (System.Exception ex)
            {
                _notAccessibleFilenames.Add(filename);
                Console.WriteLine($"Generic Exception on file: {filename}");
            }
            return null;
        }
    }
}
