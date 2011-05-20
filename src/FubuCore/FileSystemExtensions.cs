using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.CommandLine;

namespace FubuCore
{
    public static class FileSystemExtensions
    {
        public static void WriteToFlatFile(this IFileSystem system, string path, Action<IFlatFileWriter> configuration)
        {
            system.AlterFlatFile(path, list => configuration(new FlatFileWriter(list)));
        }

        public static void WriteProperty(this IFileSystem system, string path, string propertyText)
        {
            ConsoleWriter.Write("Writing {0} to {1}", path, propertyText);
            system.WriteToFlatFile(path, file =>
            {
                var parts = propertyText.Split('=');
                file.WriteProperty(parts.First(), parts.Last());

                Console.WriteLine("Contents of {0}", path);
                file.Sort();

                file.Describe();

                ConsoleWriter.PrintHorizontalLine();
            });
        }

        public static bool DirectoryExists(this IFileSystem fileSystem, params string[] pathParts)
        {
            return fileSystem.DirectoryExists(FileSystem.Combine(pathParts));
        }

        public static void LaunchEditor(this IFileSystem fileSystem, params string[] pathParts)
        {
            fileSystem.LaunchEditor(FileSystem.Combine(pathParts));
        }

        public static bool FileExists(this IFileSystem fileSystem, params string[] pathParts)
        {
            return fileSystem.FileExists(FileSystem.Combine(pathParts));
        }

        public static T LoadFromFile<T>(this IFileSystem fileSystem, params string[] pathParts) where T : new()
        {
            return fileSystem.LoadFromFile<T>(FileSystem.Combine(pathParts));
        }

        public static IEnumerable<string> ChildDirectoriesFor(this IFileSystem fileSystem, params string[] pathParts)
        {
            return fileSystem.ChildDirectoriesFor(FileSystem.Combine(pathParts));
        }

        public static IEnumerable<string> FileNamesFor(this IFileSystem fileSystem, FileSet set, params string[] pathParts)
        {
            return fileSystem.FindFiles(FileSystem.Combine(pathParts), set);
        }

        public static string ReadStringFromFile(this IFileSystem fileSystem, params string[] pathParts)
        {
            return fileSystem.ReadStringFromFile(FileSystem.Combine(pathParts));
        }

        public static void PersistToFile(this IFileSystem fileSystem, object target, params string[] pathParts)
        {
            fileSystem.WriteObjectToFile(FileSystem.Combine(pathParts), target);
        }

        public static void DeleteDirectory(this IFileSystem fileSystem, params string[] pathParts)
        {
            fileSystem.DeleteDirectory(FileSystem.Combine(pathParts));
        }

        public static void CreateDirectory(this IFileSystem fileSystem, params string[] pathParts)
        {
            fileSystem.CreateDirectory(FileSystem.Combine(pathParts));
        }
    }
}