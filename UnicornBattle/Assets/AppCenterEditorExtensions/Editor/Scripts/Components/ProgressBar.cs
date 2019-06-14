using UnityEditor;
using UnityEngine;

namespace AppCenterEditor
{
    public class ProgressBar
    {
        public enum ProgressBarStates { off = 0, on = 1, spin = 2, error = 3, warning = 4, success = 5 }
        public static ProgressBarStates currentProgressBarState = ProgressBarStates.off;

        public static float progress = 0;
        private static GUIStyle pbarStyle = AppCenterEditorHelper.uiStyle.GetStyle("progressBarFg");
        private static GUIStyle pbarBgStyle = AppCenterEditorHelper.uiStyle.GetStyle("progressBarBg");

        private static float progressWidth = 0;
        private static float animationSpeed = 1f;
        private static float tickRate = .15f;
        private static float stTime;
        private static float endTime;
        private static float lastUpdateTime;
        private static bool isReveresed;

        public static void UpdateState(ProgressBarStates state)
        {
            if (currentProgressBarState == ProgressBarStates.off && state != ProgressBarStates.off)
            {
                stTime = (float)EditorApplication.timeSinceStartup;
                endTime = stTime + animationSpeed;
            }

            currentProgressBarState = state;
        }

        //not a good way to do this right now.
        public static void UpdateProgress(float p)
        {
            progress = p;
        }

        public static void Draw()
        {
            var progressMaxWidth = AppCenterEditor.InnerContainerWidth;

            pbarBgStyle = AppCenterEditorHelper.uiStyle.GetStyle("progressBarBg");
            if (currentProgressBarState == ProgressBarStates.off)
            {
                stTime = 0;
                endTime = 0;
                progressWidth = 0;
                lastUpdateTime = 0;
                isReveresed = false;

                progressWidth = progressMaxWidth;
                pbarStyle = AppCenterEditorHelper.uiStyle.GetStyle("progressBarClear");
                pbarBgStyle = AppCenterEditorHelper.uiStyle.GetStyle("progressBarClear");
                //return;
            }
            else if (EditorWindow.focusedWindow != AppCenterEditor.window)
            {
                // pause draw while we are in the bg
                return;
            }
            else if (currentProgressBarState == ProgressBarStates.success)
            {
                if ((float)EditorApplication.timeSinceStartup - stTime < animationSpeed)
                {
                    progressWidth = progressMaxWidth;
                    pbarStyle = AppCenterEditorHelper.uiStyle.GetStyle("progressBarSuccess");
                }
                else if (AppCenterEditor.blockingRequests.Count > 0)
                {
                    UpdateState(ProgressBarStates.spin);
                }
                else
                {
                    UpdateState(ProgressBarStates.off);
                }
            }
            else if (currentProgressBarState == ProgressBarStates.warning)
            {
                if ((float)EditorApplication.timeSinceStartup - stTime < animationSpeed)
                {
                    progressWidth = progressMaxWidth;
                    pbarStyle = AppCenterEditorHelper.uiStyle.GetStyle("progressBarWarn");
                }
                else if (AppCenterEditor.blockingRequests.Count > 0)
                {
                    UpdateState(ProgressBarStates.spin);
                }
                else
                {
                    UpdateState(ProgressBarStates.off);
                }
            }
            else if (currentProgressBarState == ProgressBarStates.error)
            {
                if ((float)EditorApplication.timeSinceStartup - stTime < animationSpeed)
                {
                    progressWidth = progressMaxWidth;
                    pbarStyle = AppCenterEditorHelper.uiStyle.GetStyle("progressBarError");
                }
                else if (AppCenterEditor.blockingRequests.Count > 0)
                {
                    UpdateState(ProgressBarStates.spin);
                }
                else
                {
                    UpdateState(ProgressBarStates.off);
                }
            }
            else
            {

                if ((float)EditorApplication.timeSinceStartup - lastUpdateTime > tickRate)
                {
                    lastUpdateTime = (float)EditorApplication.timeSinceStartup;
                    pbarStyle = AppCenterEditorHelper.uiStyle.GetStyle("progressBarFg");

                    if (currentProgressBarState == ProgressBarStates.on)
                    {
                        progressWidth = progressMaxWidth * progress;
                    }
                    else if (currentProgressBarState == ProgressBarStates.spin)
                    {
                        var currentTime = (float)EditorApplication.timeSinceStartup;
                        if (currentTime < endTime && !isReveresed)
                        {
                            UpdateProgress((currentTime - stTime) / animationSpeed);
                            progressWidth = progressMaxWidth * progress;
                        }
                        else if (currentTime < endTime && isReveresed)
                        {
                            UpdateProgress((currentTime - stTime) / animationSpeed);
                            progressWidth = progressMaxWidth - progressMaxWidth * progress;
                        }
                        else
                        {
                            isReveresed = !isReveresed;
                            stTime = (float)EditorApplication.timeSinceStartup;
                            endTime = stTime + animationSpeed;
                        }
                    }
                }

            }

            using (new AppCenterGuiFieldHelper.UnityHorizontal(pbarBgStyle))
            {
                if (isReveresed)
                {
                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.LabelField("", pbarStyle, GUILayout.Width(progressWidth));
            }
        }
    }
}
