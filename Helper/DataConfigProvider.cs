using ScheduledCleanup.Model;

namespace ScheduledCleanup.Helper
{
    public class DataConfigProvider
    {
        private static DataConfig? _dataConfig;

        private static string ConfigFilePath => Path.Combine(AppContext.BaseDirectory, "data.json");

        /// <summary>
        /// 获取数据配置
        /// </summary>
        public static DataConfig? DataConfig
        {
            get
            {
                if (_dataConfig == null)
                {
                    var path = Path.Combine(AppContext.BaseDirectory, "data.json");
                    _dataConfig = DataConfig.LoadFromJson(path);
                }

                return _dataConfig;
            }
        }

        /// <summary>
        /// 路径是否存在于删除路径列表中
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool PathContains(string path)
        {
            return _dataConfig.DelPaths.Any(p => p.Path == path);
        }

        /// <summary>
        /// 设置路径的选中状态
        /// </summary>
        /// <param name="path"></param>
        /// <param name="selected"></param>
        public static void SetPathSelected(string path, bool selected)
        {
            var delPath = _dataConfig.DelPaths.FirstOrDefault(p => p.Path == path);
            if (delPath != null)
            {
                delPath.Selected = selected;
                JsonHelper.WriteJsonFile(ConfigFilePath, _dataConfig);
            }
        }

        /// <summary>
        /// 添加新的删除路径
        /// </summary>
        /// <param name="delPath"></param>
        public static void AddPath(DelPath delPath)
        {
            _dataConfig.DelPaths.Add(delPath);

            JsonHelper.WriteJsonFile(ConfigFilePath, _dataConfig);
        }

        /// <summary>
        /// 删除指定的删除路径
        /// </summary>
        /// <param name="path"></param>
        public static void RemovePath(string path)
        {
            var delPath = _dataConfig.DelPaths.FirstOrDefault(p => p.Path == path);
            if (delPath != null)
            {
                _dataConfig.DelPaths.Remove(delPath);
                JsonHelper.WriteJsonFile(ConfigFilePath, _dataConfig);
            }
        }

        /// <summary>
        /// 删除指定的删除路径
        /// </summary>
        /// <param name="path"></param>
        public static void RemovePath(int index)
        {
            var delPath = _dataConfig.DelPaths.ElementAt(index);
            if (delPath != null)
            {
                _dataConfig.DelPaths.Remove(delPath);
                JsonHelper.WriteJsonFile(ConfigFilePath, _dataConfig);
            }
        }

        /// <summary>
        /// 获取删除路径列表
        /// </summary>
        /// <param name="index">路径索引</param>
        /// <returns></returns>
        public static string GetPath(int index)
        {
            return _dataConfig.DelPaths.ElementAt(index)?.Path;
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="config"></param>
        public static void SaveConfig(Config config)
        {
            _dataConfig.Config = config;

            JsonHelper.WriteJsonFile(ConfigFilePath, _dataConfig);
        }

        /// <summary>
        /// 添加允许清除的文件扩展名
        /// </summary>
        /// <param name="path"></param>
        public static void AddAllowExtension(string extension)
        {
            if (!_dataConfig.Config.Allow.Select(x => x.Extension).Contains(extension))
            {
                _dataConfig.Config.Allow.Add(new Allow
                {
                    Extension = extension,
                    Selected = true,
                });
                JsonHelper.WriteJsonFile(ConfigFilePath, _dataConfig);
            }
        }

        /// <summary>
        /// 设置路径的选中状态
        /// </summary>
        /// <param name="extension"></param>
        /// <param name="selected"></param>
        public static void SetAllowSelected(string extension, bool selected)
        {
            var ext = _dataConfig.Config.Allow.FirstOrDefault(p => p.Extension == extension);
            if (extension != null)
            {
                ext.Selected = selected;
                JsonHelper.WriteJsonFile(ConfigFilePath, _dataConfig);
            }
        }
    }
}
