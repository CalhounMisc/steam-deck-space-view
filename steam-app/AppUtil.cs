using System.IO;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

public class AppUtil
{
    private static AppUtil lazy = null;


    public bool IsLinux { get; private set; } = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

    public static AppUtil Instance
    {
        get
        {
            if (lazy == null)
            {
                lazy = new AppUtil();
            }
            return lazy;
        }
    }

    private AppUtil()
    {

        string steamPath = null;

        if (!Directory.Exists("/home/deck/.local/share/Steam/steamapps/"))
        {

            throw new Exception("path not found");


        }
        else
        {
            steamPath = "/home/deck/.local/share/Steam";

        }

        string userdataPath = $"{steamPath}/userdata/";
        var userId = FindUserId(userdataPath);

        ShortcutVdfPath = Path.Combine(userId, "config/shortcuts.vdf");
        LibraryFoldersVdfPath = $"{steamPath}/steamapps/libraryfolders.vdf";

        var content = File.ReadAllText(LibraryFoldersVdfPath);
        var matches = Regex.Matches(content, "\"path\"\\s*\"([^\"]+)\"");

        SteamAppsPaths = matches.OfType<Match>().Select(m => Path.Combine(m.Groups[1].Value, "steamapps")).ToArray();

        CompatdataPaths = SteamAppsPaths.Select(p => Path.Combine(p, "compatdata")).Where(d => Directory.Exists(d)).ToArray();
        ShadercachePaths = SteamAppsPaths.Select(p => Path.Combine(p, "shadercache")).Where(d => Directory.Exists(d)).ToArray();
    }

    public string[] CompatdataPaths { get; private set; }

    public string LibraryFoldersVdfPath { get; private set; }
    public string[] ShadercachePaths { get; private set; }
    public string[] SteamAppsPaths { get; private set; }
    public string ShortcutVdfPath { get; private set; }

    private static string FindUserId(string userdataPath)
    {
        string[] userIdDirs = Directory.GetDirectories(userdataPath);

        return userIdDirs.First();
    }

    public string ExecuteCommand(string command)
    {
        if (!AppUtil.Instance.IsLinux)
        {
            return "1627984895     /sdaas 22";
        }
        string result = "";
        command = command.Replace("\"", "\\\"");
        using (System.Diagnostics.Process proc = new System.Diagnostics.Process())
        {
            proc.StartInfo.FileName = "/bin/bash";
            proc.StartInfo.Arguments = "-c \" " + command + " \"";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.Start();

            result += proc.StandardOutput.ReadToEnd();
            result += proc.StandardError.ReadToEnd();

            proc.WaitForExit();
        }

        return result;
    }

    /*
     appid = get_appid_from_shortcut(
                    target=shortcut_data["exe"], name=shortcut_data["appname"]
                )
    */

    // public ulong get_appid_from_shortcut(string exe, string appName)
    // {
    //     // https://github.com/Matoking/protontricks/blob/master/src/protontricks/steam.py#L1084


    //      Crc32Base c = new Crc32Base(0x04C11DB7, 0xFFFFFFFF, 0xFFFFFFFF, true, true);

    //      byte[] nameTargetBytesa = System.Text.Encoding.UTF8.GetBytes(exe + appName + "");
    //      var hash = c.ComputeHash(nameTargetBytesa);
    //      var x = BitConverter.ToUInt32(hash, 0);

    //      var result = x | 0x80000000;
    //      return = result << 32 | 0x02000000;
    // }
}