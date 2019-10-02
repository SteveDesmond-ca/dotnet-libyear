﻿using System;
using System.Collections.Generic;
using System.Linq;
using LibYear.Lib.FileTypes;
using NuGet.Versioning;
using Xunit;

namespace LibYear.Lib.Tests.FileTypes
{
    public class CsProjTests
    {
        [Fact]
        public void CanLoadCsProjFile()
        {
            //arrange
            const string filename = "FileTypes\\project.csproj";

            //act
            var file = new CsProjFile(filename);

            //assert
            Assert.Equal("test1", file.Packages.First().Key);
            Assert.Equal("test2", file.Packages.Skip(1).First().Key);
            Assert.Equal("test3", file.Packages.Skip(2).First().Key);
            Assert.Equal("test4", file.Packages.Skip(3).First().Key);
            Assert.Equal("test5", file.Packages.Skip(4).First().Key);
        }

        [Fact]
        public void CanLoadPropsAsProjFile()
        {
            //arrange
            const string filename = "FileTypes\\Directory.Build.props";

            //act
            var file = new CsProjFile(filename);

            //assert
            Assert.Equal("test1", file.Packages.First().Key);
            Assert.Equal("test2", file.Packages.Skip(1).First().Key);
            Assert.Equal("test3", file.Packages.Skip(2).First().Key);
        }

        [Fact]
        public void CanLoadTargetsAsProjFile()
        {
            //arrange
            const string filename = "FileTypes\\Directory.Build.targets";

            //act
            var file = new CsProjFile(filename);

            //assert
            Assert.Equal("test1", file.Packages.First().Key);
            Assert.Equal("test2", file.Packages.Skip(1).First().Key);
            Assert.Equal("test3", file.Packages.Skip(2).First().Key);
        }

        [Fact]
        public void CanUpdateCsProjFile()
        {
            //arrange
            const string filename = "FileTypes\\project.csproj";
            var file = new CsProjFile(filename);
            var results = new List<Result>
            {
                new Result("test1", new VersionInfo(new SemanticVersion(0, 1, 0), DateTime.Today), new VersionInfo(new SemanticVersion(1, 2, 3), DateTime.Today)),
                new Result("test2", new VersionInfo(new SemanticVersion(0, 2, 0), DateTime.Today), new VersionInfo(new SemanticVersion(2, 3, 4), DateTime.Today)),
                new Result("test3", new VersionInfo(new SemanticVersion(0, 3, 0), DateTime.Today), new VersionInfo(new SemanticVersion(3, 4, 5), DateTime.Today)),
                new Result("test4", new VersionInfo(new SemanticVersion(0, 4, 0), DateTime.Today), new VersionInfo(new SemanticVersion(4, 5, 6), DateTime.Today)),
                new Result("test5", new VersionInfo(new SemanticVersion(0, 5, 0), DateTime.Today), new VersionInfo(new SemanticVersion(5, 6, 7), DateTime.Today))
            };

            //act
            file.Update(results);

            //assert
            var newFile = new CsProjFile(filename);
            Assert.Equal("1.2.3", newFile.Packages.First().Value.ToString());
            Assert.Equal("2.3.4", newFile.Packages.Skip(1).First().Value.ToString());
            Assert.Equal("3.4.5", newFile.Packages.Skip(2).First().Value.ToString());
            Assert.Equal("4.5.6", newFile.Packages.Skip(3).First().Value.ToString());
            Assert.Equal("5.6.7", newFile.Packages.Skip(4).First().Value.ToString());
        }
        [Fact]
        public void CanUpdatePropsFile()
        {
            //arrange
            const string filename = "FileTypes\\Directory.Build.props";
            var file = new CsProjFile(filename);
            var results = new List<Result>
                {
                    new Result("test1", new VersionInfo(new SemanticVersion(0, 1, 0), DateTime.Today), new VersionInfo(new SemanticVersion(1, 2, 3), DateTime.Today)),
                    new Result("test2", new VersionInfo(new SemanticVersion(0, 2, 0), DateTime.Today), new VersionInfo(new SemanticVersion(2, 3, 4), DateTime.Today)),
                    new Result("test3", new VersionInfo(new SemanticVersion(0, 3, 0), DateTime.Today), new VersionInfo(new SemanticVersion(3, 4, 5), DateTime.Today))
                };

            //act
            file.Update(results);

            //assert
            var newFile = new CsProjFile(filename);
            Assert.Equal("1.2.3", newFile.Packages.First().Value.ToString());
            Assert.Equal("2.3.4", newFile.Packages.Skip(1).First().Value.ToString());
            Assert.Equal("3.4.5", newFile.Packages.Skip(2).First().Value.ToString());
        }

        [Fact]
        public void CanUpdateTargetsFile()
        {
            //arrange
            const string filename = "FileTypes\\Directory.Build.targets";
            var file = new CsProjFile(filename);
            var results = new List<Result>
                {
                    new Result("test1", new VersionInfo(new SemanticVersion(0, 1, 0), DateTime.Today), new VersionInfo(new SemanticVersion(1, 2, 3), DateTime.Today)),
                    new Result("test2", new VersionInfo(new SemanticVersion(0, 2, 0), DateTime.Today), new VersionInfo(new SemanticVersion(2, 3, 4), DateTime.Today)),
                    new Result("test3", new VersionInfo(new SemanticVersion(0, 3, 0), DateTime.Today), new VersionInfo(new SemanticVersion(3, 4, 5), DateTime.Today))
                };

            //act
            file.Update(results);
                   
            //assert
            var newFile = new CsProjFile(filename);
            Assert.Equal("1.2.3", newFile.Packages.First().Value.ToString());
            Assert.Equal("2.3.4", newFile.Packages.Skip(1).First().Value.ToString());
            Assert.Equal("3.4.5", newFile.Packages.Skip(2).First().Value.ToString());
        }
    }
}