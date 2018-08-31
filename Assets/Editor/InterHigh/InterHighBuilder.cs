using UnityEditor;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using ICSharpCode.SharpZipLib.Core;

namespace UnityInterHigh
{
    public class Builder
    {

        public static void BuildWindows(string date)
        {
            try
            {
                Build(new WindowsPlayer(), date);
            }
            catch
            {
                EditorUtility.ClearProgressBar();
            }
        }

        public static void BuildMac(string date)
        {
            try
            {
                Build(new MacPlayer(), date);
            }
            catch
            {
                EditorUtility.ClearProgressBar();
            }
        }
        public static void BuildWebGL(string date)
        {
            try
            {
                Build(new WebGLPlayer(), date);
            }
            catch
            {
                EditorUtility.ClearProgressBar();
            }
        }

        public static void BuildFirstInterview(string date)
        {
            try
            {
                BuildWindows(date);
                BuildMac(date);
            }
            catch
            {
                EditorUtility.ClearProgressBar();
            }
        }

        public static void BuildSecondInterview(string date)
        {
            try
            {
                BuildWindows(date);
                BuildMac(date);
                BuildWebGL(date);
            }
            catch
            {
                EditorUtility.ClearProgressBar();
            }
        }

        public static void BuildThirdInterview(string date)
        {
            try
            {
                BuildWindows(date);
                BuildMac(date);
                BuildWebGL(date);
                BuildProject(date);
            }
            catch
            {
                EditorUtility.ClearProgressBar();
            }
        }

        public static void BuildProject(string date)
        {
            var title = "Unityインターハイ - ビルドエクスポーター";
            var info = "プロジェクト エクスポート中...";
            Directory.CreateDirectory("build");
            EditorUtility.DisplayCancelableProgressBar(title, info, 0);
            using (var memoryStreamOut = new MemoryStream())
            {
                using (ZipOutputStream zipOutStream = new ZipOutputStream(memoryStreamOut))
                {
                    CompressFolder("Assets", zipOutStream, "");
                    EditorUtility.DisplayCancelableProgressBar(title, info, 0.5f);
                    CompressFolder("ProjectSettings", zipOutStream, "");
                    EditorUtility.DisplayCancelableProgressBar(title, info, 0.75f);
                    zipOutStream.Finish();
                    zipOutStream.Close();
                }

                File.WriteAllBytes(string.Format("build/{0}_project.zip", date), memoryStreamOut.ToArray());
            }
            EditorUtility.ClearProgressBar();
        }

        static void Build(Player player, string date)
        {
            EditorUtility.DisplayCancelableProgressBar("Unityインターハイ - ビルドエクスポーター", string.Format("{0} エクスポート中...", player.GetType().Name), 0);
            if (player != null)
            {
                player.Build();
                EditorUtility.DisplayCancelableProgressBar("Unityインターハイ - ビルドエクスポーター", string.Format("{0} エクスポート中...", player.GetType().Name), 0.8f);
                player.Archive(date);
            }
            EditorUtility.ClearProgressBar();
        }

        internal static void CompressFolder(string path, ZipOutputStream zipStream, string rootPath)
        {
            string[] files = Directory.GetFiles(path);

            foreach (string filename in files)
            {
                var fi = new FileInfo(filename);

                var entryName = ZipEntry.CleanName(filename.Substring(rootPath.Length));
                var newEntry = new ZipEntry(entryName);
                newEntry.DateTime = fi.LastWriteTime;
                newEntry.Size = fi.Length;

                zipStream.PutNextEntry(newEntry);

                byte[] buffer = new byte[4096];
                using (FileStream streamReader = File.OpenRead(filename))
                {
                    StreamUtils.Copy(streamReader, zipStream, buffer);
                }
                zipStream.CloseEntry();
            }
            string[] folders = Directory.GetDirectories(path);
            foreach (string folder in folders)
            {
                CompressFolder(folder, zipStream, rootPath);
            }
        }
    }

    public abstract class Player
    {
        internal string location = "build";

        internal abstract string filename { get; }

        internal abstract string locationPath { get; }

        public abstract Player Build();

        public virtual void Archive(string date)
        {
            using (var memoryStreamOut = new MemoryStream())
            {
                using (ZipOutputStream zipOutStream = new ZipOutputStream(memoryStreamOut))
                {
                    Builder.CompressFolder(locationPath, zipOutStream, location);
                    zipOutStream.Finish();
                    zipOutStream.Close();
                }
                File.WriteAllBytes(string.Format("{0}/{1}_{2}.zip", location, date, filename), memoryStreamOut.ToArray());
                Directory.Delete(locationPath, true);
            }
        }

        internal string[] GetEnabledScenes()
        {
            return EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
        }
    }

    public class WebGLPlayer : Player
    {
        public override Player Build()
        {
            BuildPipeline.BuildPlayer(GetEnabledScenes(),
                locationPath,
                BuildTarget.WebGL,
                BuildOptions.None);
            return this;
        }

        internal override string filename
        {
            get { return "webgl"; }
        }

        internal override string locationPath
        {
            get
            {
                return string.Format("{0}/{1}", location, filename);
            }
        }
    }

    public class MacPlayer : Player
    {
        internal override string filename
        {
            get { return "mac"; }
        }

        internal override string locationPath
        {
            get
            {
                return string.Format("{0}/{1}.app", location, filename);
            }
        }

        public override Player Build()
        {
            BuildPipeline.BuildPlayer(GetEnabledScenes(),
                locationPath,
                BuildTarget.StandaloneOSX,
                BuildOptions.None);
            return this;
        }
    }

    public class WindowsPlayer : Player
    {
        internal override string filename
        {
            get { return "windows"; }
        }

        internal override string locationPath
        {
            get
            {
                return string.Format("{0}/windows/{1}.exe", location, filename);
            }
        }

        public override Player Build()
        {
            Directory.CreateDirectory(string.Format("{0}/windows", location));
            BuildPipeline.BuildPlayer(GetEnabledScenes(),
                locationPath,
                BuildTarget.StandaloneWindows64,
                BuildOptions.None);
            return this;
        }

        public override void Archive(string date)
        {
            var rootPath = Path.Combine(location, filename);
            foreach (var file in Directory.GetFiles(rootPath, "*.pdb"))
            {
                File.Delete(file);
            }
            using (var memoryStreamOut = new MemoryStream())
            {
                using (ZipOutputStream zipOutStream = new ZipOutputStream(memoryStreamOut))
                {
                    Builder.CompressFolder(rootPath, zipOutStream, rootPath);
                    zipOutStream.Finish();
                    zipOutStream.Close();
                }
                File.WriteAllBytes(string.Format("{0}/{1}_{2}.zip", location, date, filename), memoryStreamOut.ToArray());
                Directory.Delete(rootPath, true);
            }
        }
    }
}