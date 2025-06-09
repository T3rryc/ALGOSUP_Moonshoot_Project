using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class EnvLoader
{
    private static Dictionary<string, string> envVars = new Dictionary<string, string>();
    private static bool loaded = false;

    public static string Get(string key)
    {
        if (!loaded)
        {
            LoadEnv();
            loaded = true;
        }

        if (envVars.ContainsKey(key))
            return envVars[key];

        Debug.LogWarning($"EnvLoader: key '{key}' not found.");
        return null;
    }

    private static void LoadEnv()
    {
        string envPath = Path.Combine(Application.dataPath, "../.env");

        if (!File.Exists(envPath))
        {
            Debug.LogWarning(".env file not found at: " + envPath);
            return;
        }

        var lines = File.ReadAllLines(envPath, Encoding.UTF8);
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#")) continue;

            var split = line.Split('=', 2);
            if (split.Length == 2)
            {
                string key = split[0].Trim();
                string value = split[1].Trim().Trim('"').Replace("\uFEFF", ""); // remove BOM

                if (!string.IsNullOrEmpty(value))
                {
                    envVars[key] = value;
                    Debug.Log($"Loaded env key: {key} = [REDACTED] ({value.Length} chars)");
                }
                else
                {
                    Debug.LogWarning($"⚠️ Key '{key}' has empty value");
                }
            }
        }
    }
}
