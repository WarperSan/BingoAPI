using System;
using System.Reflection;

namespace BingoAPI.Helpers;

/// <summary>
/// Helper that loads embedded DLL from this mod
/// </summary>s
internal static class AssemblyLoader
{
    private const string RESOURCE_NAME_TEMPLATE = MyPluginInfo.PLUGIN_GUID + ".Resources.{0}.dll";

    public static void LoadEmbeddedDLL()
    {
        AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
        {
            var resourceName = string.Format(RESOURCE_NAME_TEMPLATE, new AssemblyName(args.Name).Name);

            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            
            if (stream == null)
                return null;
            
            var assemblyData = new byte[stream.Length];

            _ = stream.Read(assemblyData, 0, assemblyData.Length);

            return Assembly.Load(assemblyData);
        };
    }
}