#addin "Cake.Xamarin"
#addin "Cake.FileHelpers"
#tool nunit.consolerunner
#tool gitlink

var target = Argument("target", Argument("t", "package"));

Setup(x =>
{
    DeleteFiles("./*.nupkg");
    DeleteFiles("./output/*.*");

	if (!DirectoryExists("./output"))
		CreateDirectory("./output");
});

Task("build")
	.Does(() =>
{
	NuGetRestore("./src/lib.sln");
	DotNetBuild("./src/lib.sln", x => x
        .SetConfiguration("Release")
        .SetVerbosity(Verbosity.Minimal)
        .WithProperty("TreatWarningsAsErrors", "false")
    );
});

Task("package")
	.IsDependentOn("build")
	.Does(() =>
{
    GitLink("./", new GitLinkSettings
    {
         RepositoryUrl = "https://github.com/dw2kim/slidingpanel",
         Branch = "master"
    });
	NuGetPack(new FilePath("./nuspec/DK.SlidingPanel.nuspec"), new NuGetPackSettings());
	MoveFiles("./*.nupkg", "./output");
});

Task("publish")
    .IsDependentOn("package")
    .Does(() =>
{
    NuGetPush("./output/*.nupkg", new NuGetPushSettings
    {
        Source = "http://www.nuget.org/api/v2/package",
        Verbosity = NuGetVerbosity.Detailed
    });
    CopyFiles("./ouput/*.nupkg", "c:\\users\\daewon.kim\\dropbox\\nuget");
});

RunTarget(target);