#if UNITY_STANDALONE_WIN

using System.Diagnostics;
using UnityEngine;

namespace FirewallUtils
{
    public class WindowsPlatform : BasePlatform
    {
        private string ruleName => "Unity " + Application.unityVersion + " Editor";

        private string disableEnable => "advfirewall firewall set rule name=\"" + ruleName + "\" profile=public new enable=";

        private string status => "advfirewall firewall show rule name=\"" + ruleName + "\" profile=public status=disabled";

        public override bool HasAuthorization()
        {
            var respond = ExecuteCommand(status);
            return respond.Replace(" ", "").Contains("Enabled:No");
        }

        public override void GrantAuthorization()
        {
            ExecuteCommandAdmin(disableEnable + "no");
        }

        public override void RemoveAuthorization()
        {
            ExecuteCommandAdmin(disableEnable + "yes");
        }

        public static string ExecuteCommand(string cmd)
        {
            var processInfo = new ProcessStartInfo("netsh.exe", cmd);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardOutput = true;
            var process = Process.Start(processInfo);
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process.Close();
            return output;
        }

        public static void ExecuteCommandAdmin(string cmd)
        {
            var processInfo = new ProcessStartInfo("netsh.exe", cmd);
            processInfo.Verb = "runas";
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = true;
            var process = Process.Start(processInfo);
            process.WaitForExit();
            process.Close();
        }
    }
}

#endif