using Mochj._PackageManager.Models;
using Mochj._PackageManager.Models.Constants;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
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

                Log($"Downloading remote file...");
                WebClient client = new WebClient();
                client.DownloadFile(file.RemoteUrl, $"{Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location)}/{file.Local}");
                Log("Finished");
            }
        }

        public static void Validate(string manifestPath)
        {
            var packages = ListPackages(manifestPath);
            Log("Validation process started, this may take some time.\n");
            packages.ForEach(pkg =>
            {
                pkg.Versions.ForEach(version =>
                {
                    Log($"\nVerifying package {pkg.Name}@{version.VersionNumber}");

                    string stageDestination = $"{PathConstants.StagePath}{pkg.Name}@{version.VersionNumber}.zip";
                    
                    if (!File.Exists(stageDestination)) {
                        Log($"unable to verify package integrity; package not staged");
                    } else
                    {
                        if (string.IsNullOrEmpty(version.Hash))
                        {
                            Log($"unable to verify package integrity; package has no defined hash");
                        } else
                        {
                            string hash = GenerateMD5Hash(stageDestination);
                            Log($"Package hash: {hash}");
                            if (hash == version.Hash)
                            {
                                Log($"Package {pkg.Name}@{version.VersionNumber} verification SUCCESS.");
                            } else
                            {
                                Log($"Package {pkg.Name}@{version.VersionNumber} verification FAIL. Hashes differ. Please redownload and reinstall the package.");
                            }
                        }
                    }

                });

            });
            Log("\nValidation completed.");
        }

        public static void Use(string moduleName, string version, string manifestPath, _Storage.Environment environment)
        {
            RemotePackage pkg = FindPackage(moduleName, version, manifestPath);
            string pkgDir = $"{PathConstants.PackagePath}{moduleName}@{pkg.VersionNumber}/";
            if (!Directory.Exists(pkgDir))
            {
                Log($"Package {moduleName}@{pkg.VersionNumber} is not installed. Attempting to fetch...");
                Fetch(moduleName, version, manifestPath);
            } else
            {
                Log($"Package found at {pkgDir}");
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

            if (!Directory.Exists(PathConstants.StagePath))
            {
                Log($"Directory {PathConstants.StagePath} not found. Creating...");
                Directory.CreateDirectory(PathConstants.StagePath);
            }

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
                    Log($"Package already downloaded to {downloadDestination}. Attempting to unzip...");
                    ZipFile.ExtractToDirectory(downloadDestination, unzipDestination);
                    return;
                }

                // Otherwise, see if it is available locally 
                // and try to unzip it from there
                if (File.Exists(remotePackage.LocalPath))
                {
                    Log($"Package found at local path: {remotePackage.LocalPath}. Attempting to unzip...");
                    ZipFile.ExtractToDirectory(remotePackage.LocalPath, unzipDestination);
                    return;
                }
            }
            // If we get here it means we need to download and install
            Log($"Downloading from remote url: {remotePackage.RemoteUrl} to {downloadDestination}...");
            WebClient client = new WebClient();
            client.DownloadFile(remotePackage.RemoteUrl, downloadDestination);
            ZipFile.ExtractToDirectory(downloadDestination, unzipDestination);
        }

        public static void Uninstall(string moduleName, string version, string manifestPath, bool keepCached = false)
        {
            RemotePackage remotePackage = FindPackage(moduleName, version, manifestPath);

            string downloadDestination = $"{PathConstants.StagePath}{moduleName}@{version}.zip";
            string unzipDestination = $"{PathConstants.PackagePath}{moduleName}@{version}";

            if (Directory.Exists(unzipDestination))
            {
                Log($"Deleting {moduleName}@{version} from {unzipDestination}...");
                Directory.Delete(unzipDestination, true);
            } else
            {
                Log($"Package {moduleName}@{version} not installed, skipping...");
            }
            
            if (!keepCached)
            {
                Log($"Attempting to delete {moduleName}@{version} from cache at {downloadDestination}");
                if (File.Exists(downloadDestination)) File.Delete(downloadDestination);
            }
        }

        public static void UninstallAll(string manifestPath)
        {
            List<VersionedPackage> packages = new List<VersionedPackage>();
            using (StreamReader r = new StreamReader(manifestPath))
            {
                string json = r.ReadToEnd();
                List<VersionedPackage> items = JsonConvert.DeserializeObject<List<VersionedPackage>>(json);

                items.ForEach(vpkg =>
                {
                    vpkg.Versions.ForEach(pkg =>
                    {
                        string installPath = $"{PathConstants.PackagePath}{vpkg.Name}@{pkg.VersionNumber}";
                        if (Directory.Exists(installPath))
                        {
                            Log($"Deleting {vpkg.Name}@{pkg.VersionNumber} from {installPath}...");
                            Directory.Delete(installPath, true);
                            return;
                        }
                        Log($"Package {vpkg.Name}@{pkg.VersionNumber} not installed, skipping...");
                    });
                });
            }        
        }

        public static void CleanCache()
        {
            Log($"Cleaning cache ({PathConstants.StagePath})");
            foreach(string path in Directory.EnumerateFileSystemEntries(PathConstants.StagePath))
            {
                File.Delete(path);
            }
            Log("Success");
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
                List<VersionedPackage> items = ListPackages(manifestPath);
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
                items.Sort((a, b) => a.Name.CompareTo(b.Name));

                items.ForEach(vPkg =>
                {
                    vPkg.Versions.Sort((a, b) => b.VersionNumber.CompareTo(a.VersionNumber));
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

        public static void Package(string settingsPath, bool publishLocally = false)
        {
            List<VersionedPackage> packages = new List<VersionedPackage>();
            using (StreamReader r = new StreamReader(settingsPath))
            {
                string json = r.ReadToEnd();
                PackageSettings settings = JsonConvert.DeserializeObject<PackageSettings>(json);

                settings.PackageInfo.ForEach(pkg =>
                {
                    Log("==============================================");
                    var packageToAdd = CreatePackage(pkg, settings.OutputDirectory, settings.PublishToLocal || publishLocally);
                    if (packageToAdd != null)
                    {
                        packages.Add(packageToAdd);
                    }
                });

                List<VersionedPackage> finalPackages = new List<VersionedPackage>();

                if (File.Exists(settings.ManifestPath))
                {
                    finalPackages = ListPackages(settings.ManifestPath);
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
                        if (settings.UsePreviousLoadFiles)
                        {
                            if (existingPkg.Versions.Any())
                            {
                                var loadFiles = existingPkg.Versions.ElementAt(existingPkg.Versions.Count() - 1).LoadFiles;
                                vPkg.Versions.ForEach(rpkg => rpkg.LoadFiles = loadFiles);
                            }
                        }
                        
                        existingPkg.Versions.AddRange(vPkg.Versions);
                        existingPkg.Versions.Sort((a, b) => b.VersionNumber.CompareTo(a.VersionNumber));
                    }
                });
                if (settings.ManifestPath != string.Empty)
                {
                    File.WriteAllText(settings.ManifestPath, JsonConvert.SerializeObject(finalPackages));
                }
            }
        }

        private static string GenerateMD5Hash(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
        private static VersionedPackage CreatePackage(PackageInfo settings, string outputDirectory, bool publishLocally = false)
        {
            string pkgDir = Path.Combine(outputDirectory, settings.PackageName);
            Directory.CreateDirectory(pkgDir);
            if (!File.Exists(settings.VersionRecordPath))
            {
                File.Create(settings.VersionRecordPath).Close();
                File.WriteAllText(settings.VersionRecordPath, _startVersion.ToString());
            }

            double versionNo = double.Parse(File.ReadAllText(settings.VersionRecordPath));
            double bumpedVersionNo = settings.BumpMajorVersion ? versionNo + 1 : versionNo + 0.01;
            string versionDir = $"{Path.Combine(pkgDir, bumpedVersionNo.ToString())}/";
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
            string zipDestination = $"{Path.Combine(pkgDir, bumpedVersionNo.ToString())}.zip";
            ZipFile.CreateFromDirectory(versionDir, zipDestination);
            Directory.Delete(versionDir, true);

            string hash = GenerateMD5Hash(zipDestination);
            Log($"Package created at {zipDestination}. Hash: {hash}");

            string previousZipDestination = $"{Path.Combine(pkgDir, versionNo.ToString())}.zip";

            if (File.Exists(previousZipDestination))
            {
                Log($"Found previous version at: {previousZipDestination}. Comparing hashes...");
                if (GenerateMD5Hash(previousZipDestination) == hash)
                {
                    Log($"Hash of previous version equaled that of new version, rolling back...");
                    File.Delete(zipDestination);
                    return null;
                } else
                {
                    Log($"Hashes not equal, continuing with new package creation");
                }
            }

            File.WriteAllText(settings.VersionRecordPath, bumpedVersionNo.ToString());
            Log($"Bumped version to {bumpedVersionNo}");
            return new VersionedPackage()
            {
                Name = settings.PackageName,
                Versions = new List<RemotePackage>
                {
                    new RemotePackage
                    {
                        VersionNumber = bumpedVersionNo.ToString(),
                        RemoteUrl = "",
                        Hash = hash,
                        LocalPath = publishLocally? $"{Path.Combine(pkgDir, bumpedVersionNo.ToString())}.zip" : "",
                    }
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
