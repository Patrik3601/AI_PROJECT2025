using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalGameManager : MonoBehaviour
{
    public static GlobalGameManager _instance;
    public static GlobalGameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log("NO GAME MANAGER");
            }
            return _instance;
        }
    }



    public void PlayerDied()
    {
        StartCoroutine(StartNew());
    }
    IEnumerator StartNew()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Game");
    }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
}
