using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalIcon : MonoBehaviour
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
