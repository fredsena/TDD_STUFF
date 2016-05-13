using System;
using System.IO;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Integration_Test
{
    public class Logger_Tests : IDisposable
    {
        private readonly ITestOutputHelper output;
        string folder;
        Logger log;
     
        public Logger_Tests(ITestOutputHelper output)
        {
            this.output = output;
            log = new Logger();
            folder = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())), "LogTests");
        }

        [Fact]
        public void Shoud_Create_Folder()
        {
            output.WriteLine("Log test");
            Assert.True(Directory.Exists(folder));
        }

        [Fact]
        public void Shoud_WriteToFile()
        {
            output.WriteLine("Log test");
            log.WriteToFile("this is a test text", "TEST_WRITE_FILE");
            Assert.True(Directory.GetFiles(folder, "*TEST_WRITE_FILE*.sql").Length > 0);
        }

        [Fact]
        public void AppendDashes()
        {
            output.WriteLine("Log test");
            string fileName = "Filename";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("--" + DateTime.Now.ToString() + "_" + fileName);
            sb.Append("--");
            sb.AppendLine("TESTE append");
            log.WriteToFile(sb.ToString(), fileName);
        }

        public void Dispose()
        {
            folder = null;
            log = null;
        }

    }
}
