using System;
using System.Reflection;

/*
 * Wrapper class for PBXProject that invokes methods via reflection. Needed
 * because there are cases when conditional compilation symbols are not
 * defined soon enough to use the class directly. Using the class directly
 * can cause problems on Windows machines that don't have the iOS build
 * tool installed.
 */
public class PBXProjectWrapper
{
    private static readonly Type PBXProjectType;
    private object _pbxProject;
    private string _projectPath;

    static PBXProjectWrapper()
    {
        var xcExtensionsAssembly = Assembly.Load("UnityEditor.iOS.Extensions.Xcode");
        if (xcExtensionsAssembly != null)
        {
            PBXProjectType = xcExtensionsAssembly.GetType("UnityEditor.iOS.Xcode.PBXProject");
        }
    }

    public static string GetUnityTargetName()
    {
        var flags = BindingFlags.Public | BindingFlags.Static;
        return PBXProjectType.GetMethod("GetUnityTargetName", flags)
                             .Invoke(PBXProjectType, null) as string;
    }

    public static bool PBXProjectIsAvailable
    {
        get
        {
            return PBXProjectType != null;
        }
    }

    public string ProjectPath
    {
        get
        {
            return _projectPath;
        }
    }

    public PBXProjectWrapper(string pathToBuiltProject)
    {
        var flags = BindingFlags.Public | BindingFlags.Static;
        var arguments = new object[] { pathToBuiltProject };
        _projectPath = PBXProjectType.GetMethod("GetPBXProjectPath", flags)
                                     .Invoke(PBXProjectType, arguments) as string;
        _pbxProject = PBXProjectType.GetConstructor(Type.EmptyTypes).Invoke(null);
        PBXProjectType.GetMethod("ReadFromFile").Invoke(_pbxProject, new[] { _projectPath });
    }

    public void WriteToFile()
    {
        PBXProjectType.GetMethod("WriteToFile").Invoke(_pbxProject, new[] { _projectPath });
    }

    public void AddBuildProperty(string name, string value)
    {
        var targetName = GetUnityTargetName();
        var targetGuid = PBXProjectType.GetMethod("TargetGuidByName")
                                       .Invoke(_pbxProject, new object[] { targetName });
        PBXProjectType.GetMethod("AddBuildProperty",
                                 new[] { typeof(string), typeof(string), typeof(string) })
                      .Invoke(_pbxProject,
                              new[] { targetGuid, name, value });
    }
}