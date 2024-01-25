using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Loader { get; private set; }
    private Animator animator;
    public GameObject Splash;

    private void Awake()
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

    [RuntimeInitializeOnLoadMethod]
    private static void SplashScreens()
    {
        if (Loader.Splash == null) return;
        Loader.animator.speed = 0;
        Loader.StartCoroutine(Loader.StartSplash());
    }

    IEnumerator StartSplash()
    {
        Loader.Splash.SetActive(true);
        yield return new WaitForSeconds(5f);
        Loader.animator.speed = 1;
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
