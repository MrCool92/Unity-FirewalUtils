using UnityEditor;
using UnityEngine;

namespace FirewallUtils.Editor
{
    public class FirewallEditor : EditorWindow
    {
        private const string Prefs_CheckAuthorizationOnPlay = "FirewallUtils.CheckAuthorizationOnPlay";

        private const string MenuItem_CheckAuthorizationOnPlay = "Tools/Firewall/Check Authorization On Play";

        private const string MenuItem_OpenUtils = "Tools/Firewall/Open Utils";

        private static int windowWidth = 500;

        private static int windowHeight = 120;

        private static class Styles
        {
            public static GUIStyle label = new GUIStyle("label")
            {
                wordWrap = true,
                alignment = TextAnchor.MiddleCenter,
            };
        }

        private static BasePlatform currentPlatform;

        private static bool hasAuthorization;

        public static bool CheckAuthorizationOnPlay
        {
            get
            {
                return EditorPrefs.GetBool(Prefs_CheckAuthorizationOnPlay, true);
            }
            set
            {
                EditorPrefs.SetBool(Prefs_CheckAuthorizationOnPlay, value);
            }
        }

        private static BasePlatform GetPlatform()
        {
#if UNITY_STANDALONE_WIN
            return new WindowsPlatform();
#else
            return null;
#endif
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void InitializeOnLoad()
        {
            if (CheckAuthorizationOnPlay && Application.isPlaying)
            {
                currentPlatform = GetPlatform();
                hasAuthorization = currentPlatform.HasAuthorization();

                if (!hasAuthorization)
                {
                    OpenUtils();
                }
            }
        }

        [MenuItem(MenuItem_OpenUtils)]
        private static void OpenUtils()
        {
            var w = GetWindow<FirewallEditor>();
            var midx = Screen.currentResolution.width / 2;
            var midy = Screen.currentResolution.height / 2;
            w.position = new Rect(midx - windowWidth / 2, midy - windowHeight / 2, windowWidth, windowHeight);
            w.titleContent = new GUIContent("Firewall Utils");
            w.maxSize = new Vector2(windowWidth, windowHeight);
            w.minSize = w.maxSize;
            w.ShowModal();
        }

        [MenuItem(MenuItem_CheckAuthorizationOnPlay)]
        private static void ToggleCheckAuthorizationOnPlay()
        {
            CheckAuthorizationOnPlay = !CheckAuthorizationOnPlay;
        }

        [MenuItem(MenuItem_CheckAuthorizationOnPlay, validate = true)]
        private static bool ToggleCheckAuthorizationOnPlayValidate()
        {
            Menu.SetChecked(MenuItem_CheckAuthorizationOnPlay, CheckAuthorizationOnPlay);
            return true;
        }

        private void OnEnable()
        {
            if (currentPlatform == null)
            {
                currentPlatform = GetPlatform();
                hasAuthorization = currentPlatform.HasAuthorization();
            }
        }

        private void OnGUI()
        {
            if (currentPlatform == null) return;

            if (!hasAuthorization)
            {
                EditorGUILayout.LabelField("To allow incoming network data you should disable firewall blocking rule!", Styles.label, null);

                GUILayout.FlexibleSpace();

                GUI.color = new Color(0, .8f, 0);
                if (GUILayout.Button("Disable rule", GUILayout.Height(42)))
                {
                    currentPlatform.GrantAuthorization();
                    hasAuthorization = currentPlatform.HasAuthorization();
                }
            }
            else
            {
                EditorGUILayout.LabelField("Networking should now work!", Styles.label, null);

                GUILayout.FlexibleSpace();

                GUI.color = new Color(.8f, 0, 0);
                if (GUILayout.Button("Enable rule", GUILayout.Height(42)))
                {
                    currentPlatform.RemoveAuthorization();
                    hasAuthorization = currentPlatform.HasAuthorization();
                }
            }

            GUILayout.FlexibleSpace();
            GUI.color = Color.white;

            if (GUILayout.Button("Default"))
            {
                currentPlatform.RemoveAuthorization();
                hasAuthorization = currentPlatform.HasAuthorization();
            }

            if (GUILayout.Button("Close"))
            {
                Close();
            }
        }
    }
}