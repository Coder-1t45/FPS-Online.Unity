using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinManager : MonoBehaviour
{
    private static WinManager _singleton;
    public static WinManager Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(WinManager)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }

    static bool good2Go;
    private static Dictionary<int, string> players = null;

    private void Awake()
    {
        good2Go = false;
        Singleton = this;
        Singleton.StartCoroutine(Singleton.winSake());
    }
    public static void Inisiate(Dictionary<int, string> players)
    {
        WinManager.players = players;
    }
    private void Update()
    {
        if (good2Go)
            return;
        if (players != null)
        {
            StartCoroutine(WinName.Inisiate(players));
            good2Go = true;
        }
    }
    public IEnumerator winSake()
    {
        yield return new WaitForSeconds(14f + 33f/60f);
        SceneManager.LoadScene(1); //Goto Home
    }

}
