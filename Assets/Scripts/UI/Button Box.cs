using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBox : MonoBehaviour
{
    [SerializeField] MouseCellPlacer placer;
    private void OnMouseEnter()
    {
        placer.disablePlacer++;
    }

    private void OnMouseExit()
    {
        placer.disablePlacer--;
    }
}
