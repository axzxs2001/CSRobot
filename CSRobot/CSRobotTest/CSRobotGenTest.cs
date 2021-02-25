using System;
using System.Diagnostics;
using System.IO;
using Xunit;

namespace CSRobotTest
{
    public class CSRobotGenTest
    {
        string _exePath = @"C:\MyFile\Source\Repos\CSRobot\CSRobot\CSRobot\bin\Debug\net5.0\CSRobot.exe";
        [Fact]
        public void CSRobotGenHelpTest()
        {
            var path = @$"{Directory.GetCurrentDirectory()}\stealthdb";
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            var process = Process.Start(_exePath, "gen --dbtype=mssql --host=127.0.0.1 --db=stealthdb --user=sa --pwd=sa");
            process.WaitForExit();
            Assert.True(Directory.Exists(path));
        }
    }
}
