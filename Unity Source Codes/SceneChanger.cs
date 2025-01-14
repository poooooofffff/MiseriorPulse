using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public Animator animator;
    private int levelToLoad;

    public void FadeToNextLevel ()
    {
        Execute(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void FadeToPreviousLevel()
    {
        Execute(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void Execute(int level)
    {
        levelToLoad = level;
        animator.SetTrigger("ChangeScene");
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(levelToLoad);
    }
}
