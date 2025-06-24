using System.Diagnostics;
using ScheduledCleanup.Model;

namespace ScheduledCleanup.Helper
{
    public class FileHelper
    {
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static ExecResult DelFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return ExecResult.ErrorResult("文件路径不能为空");
            }

            if (!File.Exists(filePath))
            {
                return ExecResult.ErrorResult("文件不存在");
            }

            try
            {
                File.Delete(filePath);
                return ExecResult.SuccessResult("文件删除成功");
            }
            catch (FileNotFoundException)
            {
                return ExecResult.ErrorResult("文件不存在");
            }
            catch (DirectoryNotFoundException)
            {
                return ExecResult.ErrorResult("目录不存在");
            }
            catch (UnauthorizedAccessException)
            {
                return ExecResult.ErrorResult("没有删除权限");
            }
            catch (IOException ex)
            {
                return ExecResult.ErrorResult($"文件正在使用中: {ex.Message}");
            }
            catch (Exception ex)
            {
                return ExecResult.ErrorResult($"未知错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 打开文件夹或文件
        /// </summary>
        /// <param name="path"></param>
        public static void OpenPath(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    // 是文件，用默认程序打开
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = path,
                        UseShellExecute = true
                    });
                }
                else if (Directory.Exists(path))
                {
                    // 是文件夹，在资源管理器中打开
                    Process.Start("explorer.exe", path);
                }
                else if (Uri.TryCreate(path, UriKind.Absolute, out Uri uri) &&
                        (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                {
                    // 是URL，用默认浏览器打开
                    Process.Start(path);
                }
                else
                {
                    MessageBox.Show("指定的路径不存在或格式不正确");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打开路径时出错: {ex.Message}");
            }
        }

        /// <summary>
        /// 删除指定路径下的所有空目录
        /// </summary>
        /// <param name="rootPath"></param>
        public static void DeleteEmptyDirectories(string rootPath)
        {
            if (string.IsNullOrWhiteSpace(rootPath))
                return;

            if (!Directory.Exists(rootPath))
                return;

            // 获取所有子目录（包括嵌套目录）
            var allDirectories = Directory.GetDirectories(rootPath, "*", SearchOption.AllDirectories);

            // 按目录深度降序排序（先处理最深层的目录）
            var orderedDirectories = allDirectories
                .OrderByDescending(d => d.Split(Path.DirectorySeparatorChar).Length);

            foreach (var directory in orderedDirectories)
            {
                if (IsDirectoryEmpty(directory))
                {
                    try
                    {
                        Directory.Delete(directory);
                        Logger.WriteLog($"已删除空目录: {directory}");
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog($"删除目录 {directory} 失败: {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// 检查目录是否为空（没有文件和子目录）
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static bool IsDirectoryEmpty(string path)
        {
            return Directory.GetFiles(path).Length == 0 &&
                   Directory.GetDirectories(path).Length == 0;
        }
    }
}
