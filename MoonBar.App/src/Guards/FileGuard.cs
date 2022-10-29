using Ardalis.GuardClauses;

namespace MoonBar.App.Guards;

internal static class FileGuard
{
    public static void FileExists(this IGuardClause guardClause, string filePath, string parameterName)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found: {filePath}", parameterName);
        }
    }
}