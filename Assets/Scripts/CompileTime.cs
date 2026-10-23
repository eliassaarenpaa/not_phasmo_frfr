using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
static class CompileTime
{
    const string StartTimeKey = "CompileTime.StartTime";

    static bool isTrackingTime;
    static double startTime;

    static CompileTime()
    {
        EditorApplication.update += Update;

        // Restore tracking after the domain reload that follows compilation
        startTime = SessionState.GetFloat(StartTimeKey, 0f);
        if (startTime > 0)
        {
            isTrackingTime = true;
        }
    }

    static void Update()
    {
        if (EditorApplication.isCompiling && !isTrackingTime)
        {
            startTime = EditorApplication.timeSinceStartup;
            SessionState.SetFloat(StartTimeKey, (float)startTime);
            isTrackingTime = true;
        }
        else if (!EditorApplication.isCompiling && isTrackingTime)
        {
            double finishTime = EditorApplication.timeSinceStartup;
            isTrackingTime = false;
            double compileTime = finishTime - startTime;
            SessionState.EraseFloat(StartTimeKey);
            Debug.Log($"Script compilation time: {compileTime:F2}s");
        }
    }
}