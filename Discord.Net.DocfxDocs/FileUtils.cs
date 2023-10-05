namespace Discord.Net.DocfxDocs;

public class FileUtils
{
    public static void CopyFiles(string from, string to)
    {
        if (!Directory.Exists(to))
            Directory.CreateDirectory(to);

        foreach (var file in Directory.GetFiles(from))
        {
            File.Copy(file, Path.Combine(to, Path.GetFileName(file)), true);
        }

        foreach (var dir in Directory.GetDirectories(from))
        {
            var dirName = Path.GetFileName(dir);
            CopyFiles(Path.Combine(from, dirName), Path.Combine(to, dirName));
        }
    }
}