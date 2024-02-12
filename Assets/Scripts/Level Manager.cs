using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] TextAsset gridBuildPath;

    void Start()
    {
        GridManager.Main.cellGridA.Populate(gridBuildPath);
    }
}
