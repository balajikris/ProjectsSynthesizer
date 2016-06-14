using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsSynthesizer
{
    class Program
    {
        const int ProjectCount = 1000;
        static bool manyFilesPerProject = true;
        const int FilesPerProject = 10;
        static string WorkingDir = @"D:\LargeProject1";
        static string SolutionName = @"D:\LargeProject1\Solution.sln";
        static string SubFolderName = @"D:\LargeProject1\ConsoleApplication{0}";
        static string PropertiesFolderName = Path.Combine(SubFolderName, "Properties");
        static string ProjectFileName = Path.Combine(SubFolderName, @"ConsoleApplication{0}.csproj");
        static string CodeFileName = Path.Combine(SubFolderName, @"Program.cs");
        static string CustomCodeFileName = Path.Combine(SubFolderName, @"Program{1}.cs");
        static string AssemblyInfoFileName = Path.Combine(SubFolderName, @"Properties\AssemblyInfo.cs");
        static string AppConfigFileName = Path.Combine(SubFolderName, @"App.config");

        static Guid[] guidArray = new Guid[ProjectCount];

        static void Main(string[] args)
        {
            Console.WriteLine("Generating projects at: " + WorkingDir);
            CreateProjects();
            Console.WriteLine("Done.. Press any key to quit.");
            Console.ReadKey();
        }

        static void CreateProjects()
        {
            // 1. create root working folder for all projects
            Directory.Delete(WorkingDir, recursive: true);
            Directory.CreateDirectory(WorkingDir);

            // 2. create each project
            for (int i = 1; i <= ProjectCount; i++)
            {
                CreateProject(i, manyFilesPerProject);
            }

            // 3. create sln file
            var solutionFileText = GetSolutionFile();

            File.WriteAllText(SolutionName, solutionFileText);
        }

        static void CreateProject(int number, bool manyFiles = false)
        {
            // 1. create root folder for project - {projectX}
            Directory.CreateDirectory(string.Format(SubFolderName, number));
            Directory.CreateDirectory(string.Format(PropertiesFolderName, number));

            // 2. write ConsoleApplicationX.csproj, Program.cs and Properties\AssemblyInfo.cs
            var guid = Guid.NewGuid();
            guidArray[number - 1] = guid;

            string projectFileText;
            if (manyFiles)
            {
                //< Compile Include = "Program.cs" />
                projectFileText = GetProjectFile(guid, number);
                var patternToFind = @"<Compile Include=""Program.cs"" />";
                var patternToReplace = @"<Compile Include = ""Program{0}.cs"" />";

                StringBuilder replacementString = new StringBuilder();
                for (int i = 1; i <= FilesPerProject; i++)
                {
                    replacementString.AppendLine(string.Format(patternToReplace, i));
                }

                projectFileText = projectFileText.Replace(patternToFind, replacementString.ToString());
            }
            else
            {
                projectFileText = GetProjectFile(guid, number);
            }

            File.WriteAllText(string.Format(ProjectFileName, number), projectFileText);

            if (manyFiles)
            {
                for (int i = 1; i <= FilesPerProject; i++)
                {
                    var codefileText = GetCustomFile(number, i);
                    File.WriteAllText(string.Format(CustomCodeFileName, number, i), codefileText);
                }
            }
            else
            {
                var codefileText = GetFile(number);
                File.WriteAllText(string.Format(CodeFileName, number), codefileText);
            }

            var assemblyInfoText = GetAssemblyInfoFile(guid, number);
            File.WriteAllText(string.Format(AssemblyInfoFileName, number), assemblyInfoText);

            var appConfigText = Strings.AppConfigFileString;
            File.WriteAllText(string.Format(AppConfigFileName, number), appConfigText);
        }

        static string GetProjectFile(Guid guid, int number)
        {
            var rawString = Strings.ProjectString;
            return string.Format(rawString, guid.ToString(), number);
        }

        static string GetFile(int number)
        {
            var rawString = Strings.FileString;
            return rawString.Replace("ConsoleApplication", "ConsoleApplication" + number);
        }

        static string GetCustomFile(int number, int number2)
        {
            var rawString = Strings.FileString;
            rawString = rawString.Replace("ConsoleApplication", "ConsoleApplication" + number);
            rawString = rawString.Replace("Program", "Program" + number2);
            rawString = rawString.Replace("Main", "Method" + number2);

            return rawString;
        }

        static string GetAssemblyInfoFile(Guid guid, int number)
        {
            var rawString = Strings.AssemblyInfoString;
            var guidString = "\"" + guid.ToString() + "\"";

            return string.Format(rawString, number, guidString);
        }

        static string GetSolutionFile()
        {
            var solutionPrefix = Strings.SolutionStringPrefix;
            var projectPart = Strings.SolutionStringProjectPart;
            var globalPart = Strings.SolutionStringGlobalPart;

            StringBuilder solutionString = new StringBuilder();
            solutionString.AppendLine(solutionPrefix);

            for (int i = 1; i <= ProjectCount; i++)
            {
                string formattedGuid = string.Concat("{", guidArray[i - 1].ToString(), "}");
                solutionString.AppendLine(string.Format(projectPart, i, formattedGuid));
                solutionString.AppendLine(string.Format(globalPart, formattedGuid));
            }

            return solutionString.ToString();
        }
    }
}
