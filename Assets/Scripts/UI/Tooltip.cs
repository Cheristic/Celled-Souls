using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tooltip : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float distance = 0.5f;
    Animator animator;
    [SerializeField] CanvasGroup group;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        float y = Mathf.Sin(Time.time * speed);
        transform.position = new Vector2(transform.position.x, transform.position.y + (y * distance));
    }

    public void ShowTooltip()
    {
        animator.SetBool("In", true);
    }

    public void HideTooltip()
    {
        animator.SetBool("In", false);
    }
}
