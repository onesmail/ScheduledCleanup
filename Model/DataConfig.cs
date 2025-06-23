using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScheduledCleanup.Model
{
    public class DataConfig
    {
        /// <summary>
        /// 配置信息
        /// </summary>
        [JsonPropertyName("cnofig")]
        public Config? Config { get; set; }

        /// <summary>
        /// 删除结合
        /// </summary>
        [JsonPropertyName("delpaths")]
        public IList<DelPath> DelPaths { get; set; } = new List<DelPath>();

        public static DataConfig? LoadFromJson(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return null;
                }
                string jsonString = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<DataConfig>(jsonString);
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"JSON解析错误: {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"读取JSON文件失败: {ex.Message}");
            }
            return null;
        }
    }


    public class Config
    {
        /// <summary>
        /// 文件创建开始时间
        /// </summary>
        [JsonPropertyName("start")]
        public string? Start { get; set; }

        /// <summary>
        /// 文件创建结束时间
        /// </summary>
        [JsonPropertyName("end")]
        public string? End { get; set; }

        /// <summary>
        /// 删除间隔时间
        /// </summary>
        [JsonPropertyName("interval")]
        public int Interval { get; set; } = 1;

        /// <summary>
        /// 删除间隔时间
        /// </summary>
        [JsonIgnore]
        public int Seconds => Interval * 1000;

        [JsonPropertyName("allow")]
        public IList<Allow> Allow { get; set; } = new List<Allow>();
    }

    public class Allow
    {
        /// <summary>
        /// 扩展名
        /// </summary>
        [JsonPropertyName("extension")]
        public string? Extension { get; set; }

        /// <summary>
        /// 是否选中允许
        /// </summary>
        [JsonPropertyName("selected")]
        public bool Selected { get; set; }
    }

    public class DelPath
    {
        /// <summary>
        /// 文件夹路径
        /// </summary>
        [JsonPropertyName("path")]
        public string? Path { get; set; }

        /// <summary>
        /// 是否选中删除
        /// </summary>
        [JsonPropertyName("selected")]
        public bool Selected { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [JsonPropertyName("start")]
        public string? Start { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [JsonPropertyName("end")]
        public string? End { get; set; }

        public override string ToString()
        {
            return $"[{Start} - {End}] -> {Path}";
        }
    }
}
