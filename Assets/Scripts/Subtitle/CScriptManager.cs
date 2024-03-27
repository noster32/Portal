using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class CScriptManager : CSingleton<CScriptManager>
{
    private Dictionary<string, Dictionary<string, string>> labs = new Dictionary<string, Dictionary<string, string>>();

    private Dictionary<string, string> lab00 = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, string> lab02 = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, string> lab11 = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, string> lab13 = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, string> lab16 = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, string> lab19 = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    public string resourceFile = "Assets/Subtitle/portal_subtitles.txt";
    
    public override void Awake()
    {
        base.Awake();

        if (m_oInstance != null)
        {
            Debug.Log("There can only be one ScriptManager");
        }
        else
        {
            m_oInstance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        TextAsset textAsset = Resources.Load<TextAsset>("portal_subtitles");

        var voText = JsonUtility.FromJson<CVoiceOverText>(textAsset.text);
        
        foreach (var t in voText.lab00)
        {
            lab00[t.key] = t.line;
        }
        foreach (var t in voText.lab02)
        {
            lab02[t.key] = t.line;
        }
        foreach (var t in voText.lab11)
        {
            lab11[t.key] = t.line;
        }
        foreach (var t in voText.lab13)
        {
            lab13[t.key] = t.line;
        }
        foreach (var t in voText.lab16)
        {
            lab16[t.key] = t.line;
        }
        foreach (var t in voText.lab19)
        {
            lab19[t.key] = t.line;
        }

        labs.Add("lab00", lab00);
        labs.Add("lab02", lab02);
        labs.Add("lab11", lab11);
        labs.Add("lab13", lab13);
        labs.Add("lab16", lab16);
        labs.Add("lab19", lab19);

    }

    public string GetText(string labName, string textKey)
    {
        string temp = "";
        Dictionary<string, string> dict;

        if (labs.TryGetValue(labName, out dict))
            if(dict.TryGetValue(textKey, out temp))
                return temp;

        return string.Empty;
    }

}
