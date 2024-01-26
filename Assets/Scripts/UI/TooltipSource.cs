using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipSource : MonoBehaviour
{
    [SerializeField] Tooltip tooltip;
    private void OnMouseEnter()
    {
        tooltip.ShowTooltip();
    }

    private void OnMouseExit()
    {
        tooltip.HideTooltip();
    }
}
