using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
// using UnityEditor.WindowsStandalone;
using UnityEngine;
using NUnit.Framework.Constraints;
using System.Linq;



#if UNITY_EDITOR_OSX
using UnityEditor.iOS.Xcode;
#endif

namespace QwertyGarden
{

    [InitializeOnLoad]
    public class BuildGame
    {
        [MenuItem("QwertyGarden/Build/Mac")]
        public static void BuildMac()
        {
            setMacBuildNumber();

            string dateTime = DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss");
            string outputPath = Application.dataPath + "/../../Build/QwertyGarden OSX " + dateTime + "/QwertyGarden.app";
            BuildStandaloneCommon(BuildTarget.StandaloneOSX, outputPath, "");
        }

        [MenuItem("QwertyGarden/Build/PC")]
        public static void BuildPC()
        {
            string dateTime = DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss");
            string outputPath = Application.dataPath + "/../../Build/QwertyGarden WIN " + dateTime + "/QwertyGarden.exe";
            BuildStandaloneCommon(BuildTarget.StandaloneWindows64, outputPath, "");
        }

        [MenuItem("QwertyGarden/Build/Steamdeck")]
        public static void BuildSteamdeck()
        {
            string dateTime = DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss");
            // string outputPath = Application.dataPath + "/../../Build/Cardwheel/Cardwheel.x86_64";
            string outputPath = Application.dataPath + "/../../Build/QwertyGarden STEAM " + dateTime + "/QwertyGarden.x86_64";
            BuildStandaloneCommon(BuildTarget.StandaloneLinux64, outputPath, "");
        }

        [MenuItem("QwertyGarden/Build/Mac DEMO")]
        public static void BuildMacDemo()
        {
            setMacBuildNumber();

            string dateTime = DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss");
            string outputPath = Application.dataPath + "/../../Build/QwertyGarden OSX DEMO " + dateTime + "/QwertyGarden_DEMO.app";
            BuildStandaloneCommon(BuildTarget.StandaloneOSX, outputPath, "DEMO");
        }

        [MenuItem("QwertyGarden/Build/PC DEMO")]
        public static void BuildPCDemo()
        {
            string dateTime = DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss");
            string outputPath = Application.dataPath + "/../../Build/QwertyGarden WIN DEMO " + dateTime + "/QwertyGarden_DEMO.exe";
            BuildStandaloneCommon(BuildTarget.StandaloneWindows64, outputPath, "DEMO");
        }

        [MenuItem("QwertyGarden/Build/Steamdeck DEMO")]
        public static void BuildSteamdeckDemo()
        {
            string dateTime = DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss");
            string outputPath = Application.dataPath + "/../../Build/QwertyGarden STEAM DEMO " + dateTime + "/QwertyGarden_DEMO.x86_64";
            BuildStandaloneCommon(BuildTarget.StandaloneLinux64, outputPath, "DEMO");
        }

        public static void BuildStandaloneCommon(BuildTarget buildTarget, string outputPath, string flag)
        {
            setMacBuildNumber();

            // Get current defines
            PlayerSettings.GetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.Standalone, out var definesArray);
            var originalDefines = definesArray.ToArray(); // copy to restore later

            // Add DEMO define (uppercase; case-sensitive)
            var definesList = definesArray.ToList();
            if (!definesList.Contains(flag))
                definesList.Add(flag);

            if (flag.Length == 0 && definesList.Contains("DEMO"))
                definesList.Remove("DEMO");

            try
            {
                PlayerSettings.SetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.Standalone, definesList.ToArray());

                // BuildOptions.Development | BuildOptions.AllowDebugging
                Build(buildTarget, outputPath, BuildOptions.None, "Assets/Scenes/MainGameScene.unity");
            }
            finally
            {
                // Restore original defines no matter what
                PlayerSettings.SetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.Standalone, originalDefines);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        static void setMacBuildNumber()
        {
            string buildNumText = File.ReadAllText("Assets/Resources/MacBuildNum.txt");
            UnityEngine.Debug.LogFormat("Build num " + buildNumText);
            int buildNum;
            if (int.TryParse(buildNumText, out buildNum))
            {
                buildNum++;
                PlayerSettings.macOS.buildNumber = buildNum.ToString();
                File.WriteAllText("Assets/Resources/MacBuildNum.txt", buildNum.ToString());
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        static void Build(BuildTarget buildTarget, string path, BuildOptions options, string scene)
        {
            BalanceParser.ParseLocal();

            float time = Time.realtimeSinceStartup;

            DateTime theTime = DateTime.Now;
            string dateTime = theTime.ToString("yyyy-MM-dd HH.mm.ss");
            File.WriteAllText("Assets/Resources/Version.txt", dateTime);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            string text = File.ReadAllText("Assets/Resources/Version.txt");
            UnityEngine.Debug.LogFormat("version saved as " + text);

            UnityEngine.Debug.LogFormat("Build {0} to {1}", buildTarget, path);

            CreateAssetBundles.BuildAllAssetBundles(buildTarget);

            UnityEngine.Debug.LogFormat("BuildAllAssetBundles elapsed time {0}", Time.realtimeSinceStartup - time);

            // Get filename.
            string[] levels = new string[] { scene };

            // Build player.
            BuildPipeline.BuildPlayer(levels, path, buildTarget, options);

            UnityEngine.Debug.LogFormat("Build elapsed time {0}", Time.realtimeSinceStartup - time);
        }

        [PostProcessBuild(999)]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
        {
            if (buildTarget == BuildTarget.StandaloneLinux64)
            {
                OnPostprocessLinux(buildTarget, path);
            }
        }

        private static void modifyXcodeFrameworks(string path)
        {
            UnityEngine.Debug.Log("modifyXcodeFrameworks(" + path + ")");
#if UNITY_EDITOR_OSX
            string projPath = PBXProject.GetPBXProjectPath(path);

            var project = new PBXProject();
            project.ReadFromFile(projPath);

            string mainTargetGuid = project.GetUnityMainTargetGuid();

            foreach (var targetGuid in new[] { mainTargetGuid, project.GetUnityFrameworkTargetGuid() })
            {
                project.SetBuildProperty(targetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");
                project.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");
                project.SetTeamId(targetGuid, "CLKC34YN6T");

                UnityEngine.Debug.Log("project.GetUnityMainTargetGuid()" + project.GetUnityMainTargetGuid());
                UnityEngine.Debug.Log("project.GetUnityFrameworkTargetGuid() " + project.GetUnityFrameworkTargetGuid());
            }

            project.SetBuildProperty(mainTargetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
            project.SetBuildProperty(mainTargetGuid, "ENABLE_BITCODE", "NO");

            project.WriteToFile(projPath);
#endif
        }

        private static void modifyXCodePList(string path)
        {
#if UNITY_EDITOR_OSX
            string plistPath = path + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            PlistElementDict rootDict = plist.root;

            rootDict.SetBoolean("ITSAppUsesNonExemptEncryption", false);
            rootDict.SetString("GADApplicationIdentifier", "ca-app-pub-2715576489475489~7867603494");

            File.WriteAllText(plistPath, plist.WriteToString());
#endif
        }

        private static void copyPodfile(string pathToBuiltProject)
        {
#if UNITY_EDITOR_OSX
            var destPodfilePath = pathToBuiltProject + "/Podfile";

            UnityEngine.Debug.Log(String.Format("Append text to {0}", destPodfilePath));
            using (StreamWriter sw = File.AppendText(destPodfilePath))
            {
                sw.WriteLine("");
                sw.WriteLine("post_install do |installer|");
                sw.WriteLine("  installer.generated_projects.each do |project|");
                sw.WriteLine("    project.targets.each do |target|");
                sw.WriteLine("        target.build_configurations.each do |config|");
                sw.WriteLine("            config.build_settings[\"DEVELOPMENT_TEAM\"] = \"CLKC34YN6T\"");
                sw.WriteLine("         end");
                sw.WriteLine("    end");
                sw.WriteLine("  end");
                sw.WriteLine("end");
            }
#endif
        }


        //////// LINUX STEAMDECK ///////
        public static void OnPostprocessLinux(BuildTarget target, string buildPath)
        {
            // buildPath is either the .x86_64 file (if you built "single file")
            // or the folder containing <Name>.x86_64 and <Name>_Data/.
            string exePath = buildPath;
            string dir = buildPath;

            if (Directory.Exists(buildPath))
            {
                // Find the .x86_64 in the folder
                var candidates = Directory.GetFiles(buildPath, "*.x86_64", SearchOption.TopDirectoryOnly);
                if (candidates.Length == 0)
                {
                    UnityEngine.Debug.LogWarning($"PostBuild: No .x86_64 found in {buildPath}");
                    return;
                }
                exePath = candidates[0];
                dir = buildPath;
            }

            // Mark the main binary executable
            RunChmod($@"""{exePath}""");

            // (Optional) Mark native plugins executable
            // Useful if you bundle shared libs that need the exec bit.
            var dataDir = Path.Combine(dir, Path.GetFileNameWithoutExtension(exePath) + "_Data");
            var pluginsDir = Path.Combine(dataDir, "Plugins");
            if (Directory.Exists(pluginsDir))
            {
                RunFindChmod(pluginsDir, "*.so");
            }

            UnityEngine.Debug.Log($"PostBuild chmod done:\n  exe: {exePath}\n  plugins: {pluginsDir}");
        }

        static void RunChmod(string path)
        {
            // Works on macOS/Linux editor machines
            var psi = new ProcessStartInfo
            {
                FileName = "/bin/chmod",
                Arguments = $@"+x {path}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            using var p = Process.Start(psi);
            p.WaitForExit();
            if (p.ExitCode != 0)
                UnityEngine.Debug.LogError($"chmod failed: {p.StandardError.ReadToEnd()}");
        }

        static void RunFindChmod(string root, string pattern)
        {
            // Use /bin/bash -lc so we can run a simple find -exec
            var cmd = $"find {Escape(root)} -type f -name '{pattern}' -exec chmod +x {{}} \\;";
            var psi = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-lc \"{cmd}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            using var p = Process.Start(psi);
            p.WaitForExit();
            if (p.ExitCode != 0)
                UnityEngine.Debug.LogError($"find+chmod failed: {p.StandardError.ReadToEnd()}");
        }

        static string Escape(string path) => path.Replace("\"", "\\\"");
    }
}