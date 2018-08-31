using UnityEngine;
using System.IO;
using UnityEditor;
using System;
namespace UnityInterHigh
{
    public class InterHighWindow : EditorWindow
    {
        Texture logo;
        Texture windowsLogo;
        Texture macLogo;
        Texture project;
        Texture first_interview;
        Texture second_interview;
        Texture third_interview;
        Font font;
        GUISkin guiskin;

        GUIStyle miniLabelStyle;

        bool supportedWindows;
        bool supportedMac;
        bool supportedWebGL;

        [MenuItem("File/Unityインターハイ - ビルドエクスポーター", false, 100)]
        static void Open()
        {
            foreach (var w in Resources.FindObjectsOfTypeAll<InterHighWindow>())
            {
                w.Close();
            }

            var window = CreateInstance<InterHighWindow>();
            window.titleContent = new GUIContent("Unityインターハイ - ビルドエクスポーター");
            window.minSize = window.maxSize = new Vector2(450, 750);
            window.ShowUtility();

            EditorSettings.externalVersionControl = "Visible Meta Files";
            AssetDatabase.SaveAssets();
        }

        void OnEnable()
        {
            logo = AssetDatabase.LoadAssetAtPath<Texture>(Path.Combine(currentFolderPath, "logo.png"));
            windowsLogo = AssetDatabase.LoadAssetAtPath<Texture>(Path.Combine(currentFolderPath, "Windows.png"));
            macLogo = AssetDatabase.LoadAssetAtPath<Texture>(Path.Combine(currentFolderPath, "Mac.png"));
            project = AssetDatabase.LoadAssetAtPath<Texture>(Path.Combine(currentFolderPath, "project.png"));
            first_interview = AssetDatabase.LoadAssetAtPath<Texture>(Path.Combine(currentFolderPath, "first_interview.png"));
            second_interview = AssetDatabase.LoadAssetAtPath<Texture>(Path.Combine(currentFolderPath, "second_interview.png"));
            third_interview = AssetDatabase.LoadAssetAtPath<Texture>(Path.Combine(currentFolderPath, "third_interview.png"));

            font = AssetDatabase.LoadAssetAtPath<Font>(Path.Combine(currentFolderPath, "Kazesawa-Regular.tiff"));
            guiskin = AssetDatabase.LoadAssetAtPath<GUISkin>(Path.Combine(currentFolderPath, "GUISkin.guiskin"));
        }

        void OnGUI()
        {

            supportedWindows = false;
            supportedMac = false;
            supportedWebGL = false;

            if (guiskin != null)
                GUI.skin = guiskin;


            if (miniLabelStyle == null)
            {
                miniLabelStyle = new GUIStyle(EditorStyles.miniLabel);
                miniLabelStyle.wordWrap = true;
                miniLabelStyle.fontSize = 9;
                miniLabelStyle.font = font;
            }

            GUI.DrawTexture(new Rect(0, 0, position.width, position.height), EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill);

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Space(4);
                GUILayout.Label(logo, GUILayout.Width(position.width - 20));
            }


            // For 5.3 or later...
            var playbackEnginesPath = Path.Combine(Path.GetDirectoryName(EditorApplication.applicationPath), "PlaybackEngines");

            if (!Directory.Exists(playbackEnginesPath))
            {
                // For 5.2 or earlier...
                playbackEnginesPath = Path.Combine(EditorApplication.applicationContentsPath, "PlaybackEngines");
            }

            CheckModules(playbackEnginesPath);

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Space(16);
                var locationLabel = new GUIContent(string.Format("エクスポート先:\n{0}", Application.dataPath.Replace("Assets", "build")));

                GUILayout.Label(locationLabel, miniLabelStyle);
            }


            using (new GUILayout.VerticalScope())
            {
                DrawModule(supportedWindows, "Windows", windowsLogo, Builder.BuildWindows);
                DrawModule(supportedMac, "Mac", macLogo, Builder.BuildMac);
                DrawModule(supportedWebGL, "WebGL", EditorGUIUtility.IconContent("BuildSettings.WebGL").image, Builder.BuildWebGL);
                DrawModule(true, "プロジェクト", project, Builder.BuildProject);
                GUILayout.Space(12);
                DrawLine();
                GUILayout.Space(12);
                DrawModule(true, "1次審査用", first_interview, Builder.BuildFirstInterview);
                DrawModule(true, "2次審査用", second_interview, Builder.BuildSecondInterview);
                GUILayout.Space(12);
                DrawModule(true, "本戦出場用", third_interview, Builder.BuildThirdInterview);
                GUILayout.Space(12);
            }

            var unityVersion = new GUIContent(string.Format("使用しているUnityバージョン: {0}", Application.unityVersion));
            var unityVersionRect = GUILayoutUtility.GetRect(unityVersion, EditorStyles.miniLabel);
            unityVersionRect.x = position.width - 200;

            GUI.Label(unityVersionRect, unityVersion, miniLabelStyle);

            var readme = new GUIContent("<size=12>ビルドエクスポーターの使い方</size>");
            var readmeRect = GUILayoutUtility.GetRect(150, 36);
            readmeRect.x = position.width - 190;
            readmeRect.y -= 18;
            readmeRect.width = 190;

            DrawLink(readmeRect, readme.text, "https://developer-inter-high.unity3d.jp");


            var linkRect = new Rect(8, position.height - 48, 200, 36);
            DrawLink(linkRect, "作品提出アップローダを開く", "https://developer-inter-high.unity3d.jp");

            var allBuildRect = new Rect(position.width - 208, position.height - 40, 200, 36);
            if (GUI.Button(allBuildRect, "すべてエクスポート"))
            {
                EditorApplication.delayCall += () =>
                            {
                                var date = DateTime.Now.ToString("MMdd_HHmm");
                                if (supportedWindows)
                                    Builder.BuildWindows(date);
                                if (supportedMac)
                                    Builder.BuildMac(date);
                                if (supportedWebGL)
                                    Builder.BuildWebGL(date);
                                Builder.BuildProject(date);
                                System.Diagnostics.Process.Start("build");
                            };
            }
        }

        void DrawLink(Rect rect, string label, string url)
        {
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);

            if (GUI.Button(rect, label, GUI.skin.FindStyle("link")))
            {
                Application.OpenURL(url);
            }
        }

        void DrawLine()
        {
            var lineRect = GUILayoutUtility.GetRect(position.width, 2);

            GUI.Box(lineRect, "");

        }

        void DrawModule(bool installedModule, string label, Texture logo, Action<string> export)
        {
            using (new GUILayout.HorizontalScope("box"))
            {
                GUILayout.Label(logo);
                GUILayout.Label(string.Format("{0}\n{1}ファイル一式", label, label == "プロジェクト" ? "" : "実行"), GUILayout.Width(100));
                using (new GUILayout.VerticalScope(GUILayout.Height(36)))
                {
                    if (installedModule)
                    {
                        if (GUILayout.Button("エクスポート", GUILayout.ExpandHeight(true), GUILayout.Width(230)))
                        {
                            EditorApplication.delayCall += () =>
                            {
                                export(DateTime.Now.ToString("MMdd_HHmm"));
                                System.Diagnostics.Process.Start("build");

                            };
                        }
                    }
                    else
                    {
                        if (GUILayout.Button(string.Format("{0}モジュールをダウンロード", label), GUILayout.ExpandHeight(true), GUILayout.Width(230)))
                        {
                            Application.OpenURL("http://unity3d.com/jp/get-unity/download/archive");
                        };
                    }
                }
            }
        }

        void CheckModules(string playbackEnginesPath)
        {
            var windowsModulePath = Path.Combine(playbackEnginesPath, "WindowsStandaloneSupport");
            var macModulePath = Path.Combine(playbackEnginesPath, "MacStandaloneSupport");
            var webglModulePath = Path.Combine(playbackEnginesPath, "WebGLSupport");
            supportedWindows = Directory.Exists(windowsModulePath);

#if UNITY_EDITOR_WIN
            supportedWindows = true;
#endif


            supportedMac = Directory.Exists(macModulePath);

#if UNITY_EDITOR_OSX
            supportedMac = true;
#endif

            supportedWebGL = Directory.Exists(webglModulePath);
        }

        static string currentFolderPath
        {
            get
            {
                var currentFilePath = new System.Diagnostics.StackTrace(true).GetFrame(0).GetFileName();
                return "Assets" + currentFilePath.Substring(0, currentFilePath.LastIndexOf(Path.DirectorySeparatorChar) + 1).Replace(Application.dataPath.Replace("/", Path.DirectorySeparatorChar.ToString()), string.Empty).Replace("\\", "/");
            }
        }
    }
}