using Mochj._PackageManager.Models;
using Mochj._PackageManager.Models.Constants;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Mochj._PackageManager
{
    static class PackageManager
    {
        private static double _startVersion = 1.00; 
        private static bool _showOutput = false;
        private static void Log(string message)
        {
            if (_showOutput)
            {
                Console.WriteLine(message);
            }
        }

        public static void ShowOutput(bool showFlag = true)
        {
            _showOutput = showFlag;
        }

        public static void Update(string remoteInfoPath)
        {
            using (StreamReader r = new StreamReader(remoteInfoPath))
            {
                string json = r.ReadToEnd();
                RemoteFile file = JsonConvert.DeserializeObject<RemoteFile>(json);

                WebClient client = new WebClient();
                client.DownloadFile(file.RemoteUrl, $"{Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location)}/{file.Local}");
            }
        }

        public static void Use(string moduleName, string version, string manifestPath, _Storage.Environment environment)
        {
            RemotePackage pkg = FindPackage(moduleName, version, manifestPath);
            string pkgDir = $"{PathConstants.PackagePath}{moduleName}@{pkg.VersionNumber}/";
            if (!Directory.Exists(pkgDir))
            {
                Log($"Package {moduleName}@{pkg.VersionNumber} is not installed. Attempting to fetch...");
                Fetch(moduleName, version, manifestPath);
            }
            foreach (string file in pkg.LoadFiles)
            {
                _Interpreter.Helpers.LoadFileHelper.LoadFile(environment, Path.Combine(pkgDir, file));
            }
        }

        public static void Fetch(string moduleName, string version, string manifestPath, bool force = false)
        {
            RemotePackage remotePackage = FindPackage(moduleName, version, manifestPath);

            string downloadDestination = $"{PathConstants.StagePath}{moduleName}@{remotePackage.VersionNumber}.zip";
            string unzipDestination = $"{PathConstants.PackagePath}{moduleName}@{remotePackage.VersionNumber}";

            if (!force)
            {
                // Check to see if it's already been installed
                if (Directory.Exists(unzipDestination))
                {
                    Log($"Package already exists at {unzipDestination}. Skipping...");
                    return;
                }

                // Otherwise, check to see if it's been downloaded already
                // and try to unzip it if it has
                if (File.Exists(downloadDestination))
                {
                    Log($"Package already downloaded to {downloadDestination}, attempting to unzip...");
                    ZipFile.ExtractToDirectory(downloadDestination, unzipDestination);
                    return;
                }
            }
            // If we get here it means we need to download and install
            WebClient client = new WebClient();
            client.DownloadFile(remotePackage.RemoteUrl, downloadDestination);
            ZipFile.ExtractToDirectory(downloadDestination, unzipDestination);
        }

        public static void Remove(string moduleName, string version, string manifestPath, bool keepCached = false)
        {
            RemotePackage remotePackage = FindPackage(moduleName, version, manifestPath);

            string downloadDestination = $"{PathConstants.StagePath}{moduleName}@{version}.zip";
            string unzipDestination = $"{PathConstants.PackagePath}{moduleName}@{version}";

            if (!keepCached) File.Delete(downloadDestination);
            Directory.Delete(unzipDestination, true);
        }


        public static void FetchLatest(string moduleName, string manifestPath, bool force = false)
        {
            string latestVersion = FindLatestPackageVersion(moduleName, manifestPath).VersionNumber;

            Fetch(moduleName, latestVersion, manifestPath, force);
        }

        public static void FetchAllLatest(string manifestPath, bool force = false)
        {
            using (StreamReader r = new StreamReader(manifestPath))
            {
                string json = r.ReadToEnd();
                List<VersionedPackage> items = JsonConvert.DeserializeObject<List<VersionedPackage>>(json);

                items.ForEach(x => FetchLatest(x.Name, manifestPath, force));
            }
        }

        public static List<string> GetAllPackageNamesAndVersions(string manifestPath)
        {
            List<string> pkgNamesAndVersions = new List<string>();
            using (StreamReader r = new StreamReader(manifestPath))
            {
                string json = r.ReadToEnd();
                List<VersionedPackage> items = JsonConvert.DeserializeObject<List<VersionedPackage>>(json);

                items.ForEach(vPkg =>
                {
                    vPkg.Versions.Sort((a, b) => a.VersionNumber.CompareTo(b.VersionNumber));
                    vPkg.Versions.Reverse();
                });
                items.ForEach(vPkg => vPkg.Versions.ForEach(pkg => pkgNamesAndVersions.Add($"{vPkg.Name}@{pkg.VersionNumber}")));
                return pkgNamesAndVersions;
            }
        }

        public static List<VersionedPackage> ListPackages(string manifestPath)
        {
            List<VersionedPackage> packages = new List<VersionedPackage>();
            using (StreamReader r = new StreamReader(manifestPath))
            {
                string json = r.ReadToEnd();
                List<VersionedPackage> items = JsonConvert.DeserializeObject<List<VersionedPackage>>(json);
                items.ForEach(vPkg =>
                {
                    vPkg.Versions.Sort((a, b) => a.VersionNumber.CompareTo(b.VersionNumber));
                    vPkg.Versions.Reverse();
                });
                return items;
            }
        }

        public static RemotePackage FindPackage(string moduleName, string version, string manifestPath)
        {
            if (version == VersionConstants.LatestVersion) return FindLatestPackageVersion(moduleName, manifestPath);
            try
            {
                using (StreamReader r = new StreamReader(manifestPath))
                {
                    string json = r.ReadToEnd();
                    List<VersionedPackage> items = JsonConvert.DeserializeObject<List<VersionedPackage>>(json);

                    VersionedPackage versionedPackage = items.Find(p => p.Name == moduleName);
                    return versionedPackage.Versions.Find(p => p.VersionNumber == version);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"unable to find package {moduleName} with version {version}", e);
            }
        }

        public static void Package(string settingsPath, string outputDirectory, string manifestOutput = "")
        {
            List<VersionedPackage> packages = new List<VersionedPackage>();
            using (StreamReader r = new StreamReader(settingsPath))
            {
                string json = r.ReadToEnd();
                List<PackageSettings> items = JsonConvert.DeserializeObject<List<PackageSettings>>(json);

                items.ForEach(pkg =>
                {
                    packages.Add(CreatePackage(pkg, outputDirectory));
                });
            }
            List<VersionedPackage> finalPackages = new List<VersionedPackage>();

            if (File.Exists(manifestOutput))
            {
                finalPackages = ListPackages(manifestOutput);
            }

            packages.ForEach(vPkg =>
            {
                var existingPkg = finalPackages.Find(x => x.Name == vPkg.Name);
                if (existingPkg == null)
                {
                    finalPackages.Add(vPkg);
                }
                else
                {
                    existingPkg.Versions.AddRange(vPkg.Versions);
                }
            });
            if (manifestOutput != string.Empty)
            {
                File.WriteAllText(manifestOutput, JsonConvert.SerializeObject(finalPackages));
            }
        }


        private static VersionedPackage CreatePackage(PackageSettings settings, string outputDirectory, bool bumpMajorVersion = false)
        {
            string pkgDir = Path.Combine(outputDirectory, settings.PackageName);
            Directory.CreateDirectory(pkgDir);
            if (!File.Exists(settings.VersionRecordPath))
            {
                File.Create(settings.VersionRecordPath).Close();
                File.WriteAllText(settings.VersionRecordPath, _startVersion.ToString());
            }

            double versionNo = double.Parse(File.ReadAllText(settings.VersionRecordPath));
            versionNo = bumpMajorVersion ? versionNo + 1 : versionNo + 0.01;
            string versionDir = $"{Path.Combine(pkgDir, versionNo.ToString())}/";
            Directory.CreateDirectory(versionDir);
            foreach(string file in settings.IncludedFiles)
            {
                if (Directory.Exists(file))
                {
                    CopyFilesRecursively(file, versionDir);
                } else
                {
                    File.Copy(file, Path.Combine(versionDir, Path.GetFileName(file)));
                }
            }
            ZipFile.CreateFromDirectory(versionDir, $"{Path.Combine(pkgDir, versionNo.ToString())}.zip");
            Directory.Delete(versionDir, true);
            File.WriteAllText(settings.VersionRecordPath, versionNo.ToString());
            return new VersionedPackage()
            {
                Name = settings.PackageName,
                Versions = new List<RemotePackage>
                {
                    new RemotePackage{VersionNumber = versionNo.ToString(), RemoteUrl = ""}
                }
            };
        }

        private static void CopyFilesRecursively(string sourcePath, string targetPath)
        {
            // Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            // Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
            }
        }


        private static RemotePackage FindLatestPackageVersion(string moduleName, string manifestPath)
        {
            try
            {
                using (StreamReader r = new StreamReader(manifestPath))
                {
                    string json = r.ReadToEnd();
                    List<VersionedPackage> items = JsonConvert.DeserializeObject<List<VersionedPackage>>(json);

                    VersionedPackage versionedPackage = items.Find(p => p.Name == moduleName);
                    versionedPackage.Versions.Sort((a, b) => a.VersionNumber.CompareTo(b.VersionNumber));
                    versionedPackage.Versions.Reverse();
                    return versionedPackage.Versions.First();
                }
            }
            catch (Exception e)
            {
                throw new Exception($"unable to find latest version of package {moduleName}", e);
            }
        }


    }
}
