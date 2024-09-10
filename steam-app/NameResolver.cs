using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Net.Http;

internal static class NameResolver
{
    static HttpClient client = new HttpClient();

    private static ShortcutEntry[] shortcuts = null;

    public static string ResolveByManifest(long id)
    {
        var manifestPath = AppUtil.Instance.SteamAppsPaths
            .Select(p => Path.Combine(p, $"appmanifest_{id}.acf"))
            .FirstOrDefault(p => File.Exists(p));

        if (manifestPath != null)
        {
            string content = File.ReadAllText(manifestPath);
            var match = Regex.Match(content, "\"name\"\\s*(.*)");
            string title;
            try
            {

                title = match.Groups[1].Captures[0].Value;
                return title.Trim('"').Trim();
            }
            catch (System.Exception)
            {

            }
        }

        return null;
    }

    public static string ResolveBySteamAPI(long appId)
    {
        var response = client.GetAsync("https://store.steampowered.com/api/appdetails?appids=" + appId).Result;

        if (response.IsSuccessStatusCode)
        {
            var content = response.Content.ReadAsStringAsync().Result;
            var dynamicObject = JsonSerializer.Deserialize<JsonElement>(content);

            JsonElement appObject;

            if (dynamicObject.TryGetProperty(appId.ToString(), out appObject))
            {
                JsonElement dataObject;

                if (appObject.TryGetProperty("data", out dataObject))
                {
                    JsonElement nameObject;

                    if (dataObject.TryGetProperty("name", out nameObject))
                    {
                        return nameObject.GetString();
                    }
                }
            }
        }

        return null;
    }

    public static string ResolveByShortcuts(long appId)
    {
        if (shortcuts == null)
        {
            shortcuts = new VdfReader().Read(AppUtil.Instance.ShortcutVdfPath);
        }
        var shortcut = shortcuts.FirstOrDefault(s => s.AppId == appId);
        return shortcut?.AppName;
    }
}

