using System;
using System.Collections.Generic;
using LibYear.Lib;
using LibYear.Lib.FileTypes;
using LibYear.Lib.Tests;
using NSubstitute;
using NuGet.Versioning;
using Xunit;

namespace LibYear.App.Tests
{
    public class RunnerTests
    {
        [Fact]
        public void HelpFlagShowsHelp()
        {
            //arrange
            var checker = Substitute.For<IPackageVersionChecker>();
            var manager = Substitute.For<IProjectFileManager>();
            var runner = new Runner(checker, manager);

            //act
            var output = runner.Run(new List<string>(new[] { "-h" }));

            //assert
            Assert.Contains("Usage: ", output);
        }

        [Fact]
        public void UpdateFlagUpdates()
        {
            //arrange
            var checker = Substitute.For<IPackageVersionChecker>();

            var manager = Substitute.For<IProjectFileManager>();
            manager.GetAllProjects(Arg.Any<IReadOnlyList<string>>()).Returns(new IProjectFile[] { new TestProjectFile("test1") });
            manager.Update(Arg.Any<IDictionary<IProjectFile, IEnumerable<Result>>>()).Returns(new[] { "updated" });

            var runner = new Runner(checker, manager);

            //act
            var output = runner.Run(new List<string>(new[] { "-u" }));

            //assert
            Assert.Contains("updated", output);
        }

        [Fact]
        public void DisplaysPackagesByDefault()
        {
            //arrange
            var checker = Substitute.For<IPackageVersionChecker>();
            var projectFile = new TestProjectFile("test project");
            var results = new Dictionary<IProjectFile, IEnumerable<Result>>
            {
                { projectFile, new[] { new Result("test1", new Release(new PackageVersion(1, 2, 3), DateTime.Today), new Release(new PackageVersion(1,2,3), DateTime.Today) ) } }
            };
            checker.GetPackages(Arg.Any<IEnumerable<IProjectFile>>()).Returns(results);

            var manager = Substitute.For<IProjectFileManager>();
            manager.GetAllProjects(Arg.Any<IReadOnlyList<string>>()).Returns(new IProjectFile[] { projectFile });

            var runner = new Runner(checker, manager);

            //act
            var output = runner.Run(new List<string>(new[] { "" }));

            //assert
            Assert.Contains("test1", output);
        }

        [Fact]
        public void QuietModeIgnoresPackageAtNewestVersion()
        {
            //arrange
            var checker = Substitute.For<IPackageVersionChecker>();
            var projectFile = new TestProjectFile("test project");
            var results = new Dictionary<IProjectFile, IEnumerable<Result>>
            {
                { projectFile, new[] { new Result("test1", new Release(new PackageVersion(1, 2, 3), DateTime.Today), new Release(new PackageVersion(1,2,3), DateTime.Today) ) } }
            };
            checker.GetPackages(Arg.Any<IEnumerable<IProjectFile>>()).Returns(results);

            var manager = Substitute.For<IProjectFileManager>();
            manager.GetAllProjects(Arg.Any<IReadOnlyList<string>>()).Returns(new IProjectFile[] { projectFile });

            var runner = new Runner(checker, manager);

            //act
            var output = runner.Run(new List<string>(new[] { "-q" }));

            //assert
            Assert.DoesNotContain("test1", output);
        }

        [Fact]
        public void MultiplePackagesShowsGrandTotal()
        {
            //arrange
            var checker = Substitute.For<IPackageVersionChecker>();
            var projectFile1 = new TestProjectFile("test project 1");
            var projectFile2 = new TestProjectFile("test project 2");
            var results = new Dictionary<IProjectFile, IEnumerable<Result>>
            {
                { projectFile1, new[] { new Result("test1", new Release(new PackageVersion(1, 2, 3), DateTime.Today), new Release(new PackageVersion(1,2,3), DateTime.Today) ) } },
                { projectFile2, new[] { new Result("test1", new Release(new PackageVersion(1, 2, 3), DateTime.Today), new Release(new PackageVersion(1,2,3), DateTime.Today) ) } }
            };
            checker.GetPackages(Arg.Any<IEnumerable<IProjectFile>>()).Returns(results);

            var manager = Substitute.For<IProjectFileManager>();
            manager.GetAllProjects(Arg.Any<IReadOnlyList<string>>()).Returns(new IProjectFile[] { projectFile1, projectFile2 });

            var runner = new Runner(checker, manager);

            //act
            var output = runner.Run(new List<string>(new[] { "-q" }));

            //assert
            Assert.Contains("Total", output);
        }

        [Fact]
        public void EmptyResultsAreSkipped()
        {
            //arrange
            var checker = Substitute.For<IPackageVersionChecker>();
            var projectFile1 = new TestProjectFile("test project 1");
            var projectFile2 = new TestProjectFile("test project 2");
            var results = new Dictionary<IProjectFile, IEnumerable<Result>>
            {
                { projectFile1, new[] { new Result("test1", new Release(new PackageVersion(1, 2, 3), DateTime.Today), new Release(new PackageVersion(1,2,3), DateTime.Today) ) } },
                { projectFile2, new List<Result>() }
            };
            checker.GetPackages(Arg.Any<IEnumerable<IProjectFile>>()).Returns(results);

            var manager = Substitute.For<IProjectFileManager>();
            manager.GetAllProjects(Arg.Any<IReadOnlyList<string>>()).Returns(new IProjectFile[] { projectFile1, projectFile2 });

            var runner = new Runner(checker, manager);

            //act
            var output = runner.Run(new List<string>(new[] { "" }));

            //assert
            Assert.Contains("test project 1", output);
            Assert.DoesNotContain("test project 2", output);
        }

        [Fact]
        public void MissingProjectFilesShowsError()
        {
            //arrange
            var checker = Substitute.For<IPackageVersionChecker>();

            var manager = Substitute.For<IProjectFileManager>();
            manager.GetAllProjects(Arg.Any<IReadOnlyList<string>>()).Returns(new List<IProjectFile>());

            var runner = new Runner(checker, manager);

            //act
            var output = runner.Run(new List<string>(new[] { "" }));

            //assert
            Assert.Contains("No project files found", output);
        }
    }
}