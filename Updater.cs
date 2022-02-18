using Octokit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedWindowsAppearence
{
    public class Updater
    {
        private IReleasesClient _releaseClient;
        private GitHubClient Github;
        private readonly string RepositoryOwner = "ArkadySK";
        private readonly string RepositoryName = "AdvancedWindowsAppearance";
        private readonly string PackageName = "AWA"; //the downloading package
        Version CurrentVersion;
        Version LatestVersion;

        public Updater()
        {           
            CurrentVersion = GetCurrentVersion();

            Github = new GitHubClient(new ProductHeaderValue(RepositoryName + @"-UpdateCheck"));
            _releaseClient = Github.Repository.Release;
        }

        private Version GetCurrentVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }
        public async Task<bool> IsUpToDate()
        {
            Version newVersion = await GetNewVersionInfo();

            return (newVersion == CurrentVersion);
        }

        #region get new version name
        private Version StringToVersion(string versionString)
        {
            versionString = versionString.Replace("v", string.Empty);
            try
            {
                return new Version(versionString);

            }
            catch {
                versionString = versionString.Replace("-beta", string.Empty);
                versionString = versionString.Replace("-alpha", string.Empty);
                return new Version(versionString);
            }
        }

        public async Task<Version> GetNewVersionInfo()
        {
            if (String.IsNullOrWhiteSpace(RepositoryName) || String.IsNullOrWhiteSpace(RepositoryOwner)) return null;

            var allReleases = await _releaseClient.GetAll(RepositoryOwner, RepositoryName);
            var latestRelease = allReleases.FirstOrDefault(release => StringToVersion(release.TagName) > CurrentVersion);
            if (latestRelease != null)
                LatestVersion = StringToVersion(latestRelease.TagName);
            else
                LatestVersion = CurrentVersion;
            return LatestVersion;
        }
        #endregion


        #region Download
        public void DownloadUpdate()
        {
            const string urlTemplate = "https://github.com/{0}/{1}/releases/download/{2}/{3}";
            var url = string.Format(urlTemplate, RepositoryOwner, RepositoryName, "v" + LatestVersion, PackageName+".zip");

            url = url.Replace("&", "^&");
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }
        #endregion
    }
}
