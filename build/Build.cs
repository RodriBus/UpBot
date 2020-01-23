using System;
using System.Collections.Generic;
using System.Linq;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

namespace RodriBus.UpBot.Build
{
    [CheckBuildProjectConfigurations]
    [UnsetVisualStudioEnvironmentVariables]
    internal class Build : NukeBuild
    {
        /// <summary>
        /// Support plugins are available for:
        ///   - JetBrains ReSharper        https://nuke.build/resharper
        ///   - JetBrains Rider            https://nuke.build/rider
        ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
        ///   - Microsoft VSCode           https://nuke.build/vscode
        /// </summary>

        public static int Main() => Execute<Build>(x => x.Compile);

        [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
        private readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

        private const string MAIN_PROJECT = "RodriBus.UpBot.Worker";
        private readonly IEnumerable<string> Runtimes = new[] { "win-x64", "linux-x64" };

        [Solution] private readonly Solution Solution;
        [GitRepository] private readonly GitRepository GitRepository;
        [GitVersion] private readonly GitVersion GitVersion;

        private AbsolutePath SourceDirectory => RootDirectory / "src";
        private AbsolutePath TestsDirectory => RootDirectory / "tests";
        private AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

        private Target Clean => _ => _
             .Before(Restore)
             .Executes(() =>
             {
                 SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
                 TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
                 EnsureCleanDirectory(ArtifactsDirectory);
             });

        private Target Restore => _ => _
             .Executes(() =>
             {
                 DotNetRestore(_ => _
                     .SetProjectFile(Solution));
             });

        private Target Compile => _ => _
             .DependsOn(Restore)
             .Executes(() =>
             {
                 DotNetBuild(_ => _
                     .SetProjectFile(Solution)
                     .SetConfiguration(Configuration)
                     .SetAssemblyVersion(GitVersion.AssemblySemVer)
                     .SetFileVersion(GitVersion.AssemblySemFileVer)
                     .SetInformationalVersion(GitVersion.InformationalVersion)
                     .EnableNoRestore());
             });

        private Target Publish => _ => _
             .After(Clean)
             .Executes(() =>
             {
                 EnsureCleanDirectory(ArtifactsDirectory);
                 DotNetPublish(_ => _
                     .SetAssemblyVersion(GitVersion.AssemblySemVer)
                     .SetFileVersion(GitVersion.AssemblySemFileVer)
                     .SetInformationalVersion(GitVersion.InformationalVersion)
                     .SetOutput(ArtifactsDirectory)
                     .SetProject(Solution.GetProject(MAIN_PROJECT))
                     .EnableSelfContained()
                     .SetConfiguration(Configuration)
                     .CombineWith(Runtimes, (o, r) => o
                        .SetRuntime(r)
                        .SetOutput(ArtifactsDirectory / r)
                        .SetArgumentConfigurator(a => a
                            .Add("/p:PublishSingleFile=true")
                            .Add("/p:PublishTrimmed=true")))
                 );
             });
    }
}