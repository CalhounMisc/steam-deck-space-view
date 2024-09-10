using System;

public class InstalledGame
{
    public string DataFolder { get; }
    public string CompatFolderPath { get; set; }
    public string ShaderCacheFolderPath { get; set; }
    public long Id { get; }
    public string Name { get; private set; }

    public string Installed { get; private set; }
    public long CompatDataSizeLong { get; set; } = -1;

    public string CompatDataSize
    {
        get
        {
            return CompatDataSizeLong == -1 ? "-" : ToSize(CompatDataSizeLong);
        }
    }

    public long ShaderCacheSizeLong { get; set; } = -1;

    public string ShaderCacheSize
    {
        get
        {
            return ShaderCacheSizeLong == -1 ? "-" : ToSize(ShaderCacheSizeLong);
        }
    }

    public string TotalSize
    {
        get
        {
            return ToSize(TotalSizeLong);
        }
    }

    public long TotalSizeLong
    {
        get
        {
            if (CompatDataSizeLong == -1)
            {
                return ShaderCacheSizeLong;
            }
            else if (ShaderCacheSizeLong == -1)
            {
                return CompatDataSizeLong;
            }
            else
            {
                return CompatDataSizeLong + ShaderCacheSizeLong;
            }
        }
    }

    public InstalledGame(long id, string dataFolder)
    {
        Id = id;
        DataFolder = dataFolder;
        ResolveName();
    }

    private void ResolveName()
    {

        Installed = "✓";

        string result = NameResolver.ResolveByManifest(Id);

        if (result != null)
        {
            Name = result;
            return;
        }

        result = NameResolver.ResolveByShortcuts(Id);

        if (result != null)
        {
            Name = result;
            return;
        }

        Installed = "✕";
        result = NameResolver.ResolveBySteamAPI(Id);

        if (result != null)
        {
            Name = result;
            return;
        }

        Name = "[UNKNOWN]";
    }



    public static string ToSize(long bytes)
    {
        return (Math.Pow(10, -6) * bytes).ToString("N0");
    }
}