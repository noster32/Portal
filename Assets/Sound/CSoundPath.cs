using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundPathInfo
{
    public string path;
}

[System.Serializable]
public class SoundList
{
    public SoundPathInfo[] turret_active;
}

public class CSoundPath : CComponent
{
    TextAsset soundJsonFile;

    public override void Start()
    {
        base.Start();

        soundJsonFile = Resources.Load<TextAsset>("SoundPathJson");

        string soundJsonText = soundJsonFile.text;
        SoundList soundList = JsonUtility.FromJson<SoundList>(soundJsonText);

        //int randomIndex = Random.Range(0, soundList.turret_active.Length);
        //string randomPath = soundList.turret_active[randomIndex].path;
        //
        //Debug.Log($"Randomly selected path : {randomPath}");

    }

    public override void Update()
    {
        base.Update();

    }

}
