using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace XUnitTestProject1
{
    public class MergeFileScriptsTest
    {
        [Fact]
        public void MergeFileScriptTemplate_ASAP()
        {
            string deployPath = @"C:\path-to-files\Deploy";
            string deployFileName = "Deploy-DBScript.sql";

            string validationPath = @"C:\path-to-files\Validation";
            string validationFileName = "Validation-DBScript.sql";

            string rollbackPath = @"C:\path-to-files\Rollback";
            string rollbackFileName = "RollBack-ReversedDBScript.sql";

            MergeFileScriptTemplate(deployPath, deployFileName, "DEPLOY");
            MergeFileScriptTemplate(validationPath, validationFileName, "VALIDATION");
            GenerateReversedScriptTemplate(rollbackPath, rollbackFileName, "ROLLBACK");
        }

        public void MergeFileScriptTemplate(string path, string filename, string scriptType)
        {
            StringBuilder builder = new StringBuilder();
            string[] subdirectoryEntries = Directory.GetDirectories(path);

            foreach (var folder in subdirectoryEntries)
            {
                builder.AppendLine(GetData(folder, scriptType));
            }

            string filePath = GetSecondToLastDirectoryInPath(path) + "\\" + filename;

            File.WriteAllText(filePath, builder.ToString().Substring(0, builder.Length));
        }

        public string GetData(string folder, string scriptType)
        {
            StringBuilder builderInsert = new StringBuilder();

            DirectoryInfo di = new DirectoryInfo(folder);
            FileSystemInfo[] files = di.GetFileSystemInfos();
            var orderedFiles = files.OrderBy(f => f.CreationTime);

            foreach (var file in orderedFiles)
            {
                var dataFile = GetDataFile(file.FullName);

                builderInsert.AppendLine();
                builderInsert.AppendLine("-- ****************************************************************************************************************************");
                builderInsert.AppendLine("-- " + scriptType + "\\" + Path.GetFileName(Path.GetDirectoryName(file.FullName)) + "\\" + file.Name);
                builderInsert.AppendLine("-- ****************************************************************************************************************************");
                builderInsert.AppendLine();
                builderInsert.AppendLine(dataFile);
            }

            return builderInsert.ToString();
        }

        public void GenerateReversedScriptTemplate(string path, string filename, string scriptType)
        {
            StringBuilder builder = new StringBuilder();
            string[] subdirectoryEntries = Directory.GetDirectories(path);

            foreach (var folder in subdirectoryEntries)
            {
                builder.AppendLine(GetReversedData(folder, scriptType));
            }

            string filePath = GetSecondToLastDirectoryInPath(path) + "\\" + filename;

            File.WriteAllText(filePath, builder.ToString().Substring(0, builder.Length));
        }

        public string GetReversedData(string folder, string scriptType)
        {
            StringBuilder builderInsert = new StringBuilder();

            DirectoryInfo di = new DirectoryInfo(folder);
            FileSystemInfo[] files = di.GetFileSystemInfos();
            var reverseOrderedFiles = files.OrderByDescending(f => f.CreationTime);

            foreach (var file in reverseOrderedFiles)
            {
                var dataFile = GetDataFile(file.FullName);

                builderInsert.AppendLine();
                builderInsert.AppendLine("-- ****************************************************************************************************************************");
                builderInsert.AppendLine("-- " + scriptType + "\\" + Path.GetFileName(Path.GetDirectoryName(file.FullName)) + "\\" + file.Name);
                builderInsert.AppendLine("-- ****************************************************************************************************************************");
                builderInsert.AppendLine();
                builderInsert.AppendLine(dataFile);
            }

            return builderInsert.ToString();
        }

        private string GetDataFile(string filePath)
        {
            return File.ReadAllText(filePath, Encoding.GetEncoding("ISO-8859-1"));
        }

        public string GetSecondToLastDirectoryInPath(string path)
        {
            DirectoryInfo parentDir = Directory.GetParent(path);
            return parentDir.FullName;
        }
    }
}
