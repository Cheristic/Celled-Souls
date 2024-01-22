using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressTracker : MonoBehaviour
{
    public static ProgressTracker Main { get; private set; }
    public Progress progress;

    private void Awake()
    {
        if (Main != null && Main != this)
        {
            Destroy(this);
        } else
        {
            Main = this;
        }
        DontDestroyOnLoad(this);
        progress = Progress.None;
    }

    public enum Progress // 
    {
        None,
        Level1,
        Level2,
        Level3,
        Level4,
        Level5,
        Level6,
        Level7,
        Level8
    }

    public void LevelWin(int level)
    {
        if (level <= ((int)progress)) return;
        progress++;
    }
}
