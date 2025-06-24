using System.Text.Json;

namespace ScheduledCleanup.Helper
{
    public class JsonHelper
    {
        /// <summary>
        /// 写入json文件
        /// </summary>
        /// <param name="filePath"></param>
        public static void WriteJsonFile<T>(string filePath, T data)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true, // 美化输出（格式化）
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase // 驼峰命名
                };

                string jsonString = JsonSerializer.Serialize(data, options);
                File.WriteAllText(filePath, jsonString);
            }
            catch (Exception ex)
            {
                Logger.WriteLog($"写入JSON文件失败: {ex.Message}", LogLevel.ERROR);
            }
        }

        /// <summary>
        /// 读取json文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static T? ReadJsonFile<T>(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Logger.WriteLog($"文件不存在: {filePath}", LogLevel.ERROR);
                    return default;
                }

                string jsonString = File.ReadAllText(filePath);
                var data = JsonSerializer.Deserialize<T>(jsonString);

                return data;
            }
            catch (JsonException jsonEx)
            {
                Logger.WriteLog($"JSON解析错误: {jsonEx.Message}", LogLevel.ERROR);
            }
            catch (Exception ex)
            {
                Logger.WriteLog($"读取JSON文件失败: {ex.Message}", LogLevel.ERROR);
            }

            return default;
        }
    }
}
