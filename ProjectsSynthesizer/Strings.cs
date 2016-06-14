using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsSynthesizer
{
    internal class Strings
    {
        private static ResourceManager rm = Resource1.ResourceManager;

        public static string ProjectString = rm.GetString(nameof(ProjectString));

        public static string FileString = rm.GetString(nameof(FileString));

        public static string AssemblyInfoString = rm.GetString(nameof(AssemblyInfoString));

        public static string SolutionStringPrefix = rm.GetString(nameof(SolutionStringPrefix));
        public static string SolutionStringProjectPart = rm.GetString(nameof(SolutionStringProjectPart));
        public static string SolutionStringGlobalPart = rm.GetString(nameof(SolutionStringGlobalPart));

        public static string AppConfigFileString = rm.GetString(nameof(AppConfigFileString));
    }
}
