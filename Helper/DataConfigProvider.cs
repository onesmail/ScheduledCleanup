using ScheduledCleanup.Helper;
using ScheduledCleanup.Model;

public class DataConfigProvider
{
    private static readonly Lazy<DataConfig> _dataConfig = new Lazy<DataConfig>(() =>
        DataConfig.LoadFromJson(ConfigFilePath), LazyThreadSafetyMode.ExecutionAndPublication);

    private static string ConfigFilePath => Path.Combine(AppContext.BaseDirectory, "config.json");

    public static DataConfig DataConfig => _dataConfig.Value;

    public static bool PathExists(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return false;
        return DataConfig.DelPaths.Any(p => p.Path == path);
    }

    public static void SetPathSelected(string path, bool selected)
    {
        if (string.IsNullOrWhiteSpace(path)) return;

        var delPath = DataConfig.DelPaths.FirstOrDefault(p => p.Path == path);
        if (delPath != null)
        {
            delPath.Selected = selected;
            SaveToJson();
        }
    }

    public static void AddPath(DelPath delPath)
    {
        if (delPath?.Path == null) return;

        if (!DataConfig.DelPaths.Any(p => p.Path == delPath.Path))
        {
            DataConfig.DelPaths.Add(delPath);
            SaveToJson();
        }
    }

    public static void RemovePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return;

        var delPath = DataConfig.DelPaths.FirstOrDefault(p => p.Path == path);
        if (delPath != null)
        {
            DataConfig.DelPaths.Remove(delPath);
            SaveToJson();
        }
    }

    public static void RemovePath(int index)
    {
        if (index >= 0 && index < DataConfig.DelPaths.Count)
        {
            DataConfig.DelPaths.RemoveAt(index);
            SaveToJson();
        }
    }

    public static string GetPath(int index)
    {
        return (index >= 0 && index < DataConfig.DelPaths.Count)
            ? DataConfig.DelPaths.ElementAt(index).Path
            : null;
    }

    public static void SaveConfig(Config config)
    {
        if (config == null) return;

        DataConfig.Config = config;
        SaveToJson();
    }

    public static void AddAllowExtension(string extension)
    {
        if (string.IsNullOrWhiteSpace(extension)) return;

        extension = extension.Trim().ToLowerInvariant();
        if (!DataConfig.Config.Allow.Any(a => a.Extension.Equals(extension, StringComparison.OrdinalIgnoreCase)))
        {
            DataConfig.Config.Allow.Add(new Allow
            {
                Extension = extension,
                Selected = true
            });
            SaveToJson();
        }
    }

    public static void SetAllowSelected(string extension, bool selected)
    {
        if (string.IsNullOrWhiteSpace(extension)) return;

        var allow = DataConfig.Config.Allow.FirstOrDefault(p =>
            p.Extension.Equals(extension.Trim(), StringComparison.OrdinalIgnoreCase));
        if (allow != null)
        {
            allow.Selected = selected;
            SaveToJson();
        }
    }

    private static void SaveToJson()
    {
        JsonHelper.WriteJsonFile(ConfigFilePath, DataConfig);
    }
}