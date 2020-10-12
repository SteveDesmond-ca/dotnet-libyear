using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibYear.Lib.FileTypes;
using NuGet.Common;
using NuGet.Protocol.Core.Types;

namespace LibYear.Lib
{
    public class PackageVersionChecker : IPackageVersionChecker
    {
        private readonly PackageMetadataResource _metadataResource;
        private readonly IDictionary<string, IList<Release>> _versionCache;

        public PackageVersionChecker(PackageMetadataResource metadataResource, IDictionary<string, IList<Release>> versionCache)
        {
            _metadataResource = metadataResource;
            _versionCache = versionCache;
        }

        public IDictionary<IProjectFile, IEnumerable<Result>> GetPackages(IEnumerable<IProjectFile> projectFiles)
            => projectFiles.ToDictionary(proj => proj, proj => AwaitResults(proj.Packages.Select(p => GetResultTask(p.Key, p.Value))));

        public static IEnumerable<Result> AwaitResults(IEnumerable<Task<Result>> resultsTasks)
        {
            var tasks = resultsTasks.ToArray();
            Task.WaitAll(tasks);
            return tasks.Select(t => t.Result);
        }

        public async Task<Result> GetResultTask(string packageName, PackageVersion installed)
        {
            var versions = _versionCache.ContainsKey(packageName) ? _versionCache[packageName] : _versionCache[packageName] = await GetVersions(packageName);
            var current = versions.FirstOrDefault(v => v.Version == installed);
            var latest = versions.FirstOrDefault(v => v.Version == versions.Where(m => !m.Version.IsPrerelease && m.IsPublished).Max(m => m.Version));
            if (installed?.IsWildcard ?? false)
            {
                current = latest;
            }
            return new Result(packageName, current, latest);
        }

        public async Task<IList<Release>> GetVersions(string packageName)
        {
            var metadata = _metadataResource.GetMetadataAsync(packageName, true, true, NullSourceCacheContext.Instance, NullLogger.Instance, CancellationToken.None);
            return (await metadata).Select(m => new Release(m)).ToList();
        }
    }
}