using System;
using UnityEngine;

namespace AppCenterEditor
{
    class LocalLogger: IEdExLogger
    {
        public void LogWithTimeStamp(string message)
        {
            Debug.Log(GetUniqueMessage(message));
        }

        public void LogWarning(string message)
        {
            Debug.LogWarning(GetUniqueMessage(message));
        }

        public void LogError(string message)
        {
            Debug.LogError(GetUniqueMessage(message));
        }

        public void Log(string message)
        {
            Debug.Log(message);
        }

        private string GetUniqueMessage(string message)
        {
            // Return unique message in order to distinguish similar messages.
            return string.Format("[App Center EdEx MSG{0}] {1}", DateTime.Now.Millisecond, message);
        }
    }
}
