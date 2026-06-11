namespace TradingBot.Core.Configuration;

public static class DotEnvLoader
{
    public static void Load(string fileName = ".env")
    {
        var path = FindFile(fileName);

        if (path is null)
        {
            return;
        }

        foreach (var line in File.ReadAllLines(path))
        {
            var trimmed = line.Trim();

            if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith('#'))
            {
                continue;
            }

            var separatorIndex = trimmed.IndexOf('=');

            if (separatorIndex <= 0)
            {
                continue;
            }

            var key = trimmed[..separatorIndex].Trim();
            var value = trimmed[(separatorIndex + 1)..].Trim().Trim('"', '\'');

            if (string.IsNullOrWhiteSpace(key) || Environment.GetEnvironmentVariable(key) is not null)
            {
                continue;
            }

            Environment.SetEnvironmentVariable(key, value);
        }
    }

    private static string? FindFile(string fileName)
    {
        var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());

        while (currentDirectory is not null)
        {
            var candidate = Path.Combine(currentDirectory.FullName, fileName);

            if (File.Exists(candidate))
            {
                return candidate;
            }

            currentDirectory = currentDirectory.Parent;
        }

        var baseDirectory = new DirectoryInfo(AppContext.BaseDirectory);

        while (baseDirectory is not null)
        {
            var candidate = Path.Combine(baseDirectory.FullName, fileName);

            if (File.Exists(candidate))
            {
                return candidate;
            }

            baseDirectory = baseDirectory.Parent;
        }

        return null;
    }
}
