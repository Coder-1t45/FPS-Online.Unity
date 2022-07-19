using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyEvent : MonoBehaviour
{
    private static KeyEvent _singleton;
    public static KeyEvent Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(KeyEvent)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }
    void Start()
    {
        Singleton = this;
        DontDestroyOnLoad(Singleton);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Home))
        {

            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 0)
                NetworkManager.Singleton.Client.Disconnect();
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
}
