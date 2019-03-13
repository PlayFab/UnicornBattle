namespace AppCenterEditor
{
    interface IEdExLogger
    {
        void LogWithTimeStamp(string message);

        void LogWarning(string message);

        void LogError(string message);

        void Log(string message);
    }
}
