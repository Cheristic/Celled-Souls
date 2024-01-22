using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] string gridBuildPath;

    void Start()
    {
        GridManager.Main.cellGridA.Populate(gridBuildPath);
    }
}
