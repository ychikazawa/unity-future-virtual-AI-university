using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class EnvManager
{
    public const string filename = ".env";

    public static Dictionary<string, string> variables = Load();

    public static Dictionary<string, string> Load()
    {
        var env = new Dictionary<string, string>();
        var lines = File.ReadAllLines(filename);
        foreach (var line in lines)
        {
            var parts = line.Split('=');
            env[parts[0]] = parts[1];
        }
        return env;
    }

    public static string Get(string key)
    {
        return variables[key];
    }
}