using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Loader { get; private set; }
    private Animator animator;

    private void Start()
    {
        if (Loader!= null && Loader != this)
        {
            Destroy(this);
        }
        else
        {
            Loader = this;
        }
        animator = GetComponent<Animator>();
    }

    public void LoadScene(int buildIndex)
    {
        StartCoroutine(LoadLevel(buildIndex));
    }

    IEnumerator LoadLevel(int buildIndex)
    {
        animator.SetTrigger("Start");
        yield return new WaitForSeconds(.75f);
        SceneManager.LoadScene(buildIndex, LoadSceneMode.Single);
    }
}
