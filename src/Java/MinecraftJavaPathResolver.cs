using CmlLib.Core.Rules;

namespace CmlLib.Core.Java;

public class MinecraftJavaPathResolver : IJavaPathResolver
{
    public static readonly JavaVersion JreLegacyVersion = new JavaVersion("jre-legacy");
    public static readonly JavaVersion CmlLegacyVersion = new JavaVersion("m-legacy");

    private readonly MinecraftPath _path;

    public MinecraftJavaPathResolver(MinecraftPath path)
    {
        this._path = path;
    }

    public string[] GetInstalledJavaVersions()
    {
        var dir = new DirectoryInfo(_path.Runtime);
        if (!dir.Exists)
            return [];

        var topLevelDirectories = dir.GetDirectories();

        var secondLevelDirectories = topLevelDirectories
            .SelectMany(d => d.GetDirectories())
            .ToArray();

        var allDirectories = topLevelDirectories.Concat(secondLevelDirectories);

        return allDirectories
            .Select(x => x.Name)
            .Distinct()
            .ToArray();
    }


    public string? GetDefaultJavaBinaryPath(RulesEvaluatorContext rules)
    {
        var javaVersions = GetInstalledJavaVersions();
        string? javaPath = null;

        if (string.IsNullOrEmpty(javaPath) &&
            javaVersions.Contains(MinecraftJavaPathResolver.JreLegacyVersion.Component))
            javaPath = GetJavaBinaryPath(MinecraftJavaPathResolver.JreLegacyVersion, rules);

        if (string.IsNullOrEmpty(javaPath) &&
            javaVersions.Contains(MinecraftJavaPathResolver.CmlLegacyVersion.Component))
            javaPath = GetJavaBinaryPath(MinecraftJavaPathResolver.CmlLegacyVersion, rules);

        if (string.IsNullOrEmpty(javaPath) &&
            javaVersions.Length > 0)
            javaPath = GetJavaBinaryPath(new JavaVersion(javaVersions[0]), rules);

        return javaPath;
    }

    public string GetJavaBinaryPath(JavaVersion javaVersionName, RulesEvaluatorContext rules)
    {
        var runtime = GetJavaDirPath(javaVersionName, rules);
        return (rules.OS.Name) switch
        {
            // bin/javaw.exe
            LauncherOSRule.Windows => Path.Combine(runtime, "bin", "javaw.exe"),

            // jre.bundle/Contents/Home/bin/java
            LauncherOSRule.OSX => Path.Combine(runtime, "jre.bundle", "Contents", "Home", "bin", "java"),

            // bin/java
            _ => Path.Combine(runtime, "bin", "java")
        };
    }

    public string GetJavaDirPath(JavaVersion javaVersionName, RulesEvaluatorContext rules)
        => Path.Combine(
            _path.Runtime,
            MinecraftJavaManifestResolver.GetOSNameForJava(rules.OS),
            javaVersionName.Component);
}
