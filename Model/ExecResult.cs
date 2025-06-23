namespace ScheduledCleanup.Model
{
    public class ExecResult
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public object Data { get; set; }


        public ExecResult(bool success, string message = null, object data = null)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public static ExecResult SuccessResult(object data = null, string message = "操作成功")
        {
            return new ExecResult(true, message, data);
        }

        public static ExecResult ErrorResult(string message = "操作失败", object data = null)
        {
            return new ExecResult(false, message, data);
        }
    }
}
