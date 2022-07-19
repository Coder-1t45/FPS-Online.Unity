using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionTab : MonoBehaviour
{
    private static ActionTab _singleton;
    public static ActionTab Singleton
    {
        get => _singleton;
        private set
        {
            if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Debug.Log($"{nameof(ActionTab)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }
    public GameObject dupKill;
    public GameObject dupFall;
    public Transform folder;
    public AnimationCurve disappearing;
    // Start is called before the first frame update
    void Awake()
    {
        Singleton = this;
    }

    public static void PlayerKill(ushort alive, ushort death)
    {
        Singleton.playerKill(alive, death);
    }
    public static void PlayerFall(ushort death)
    {
        Singleton.playerFall( death);
    }
    public void playerKill(ushort alive, ushort death)
    {
        var i = Instantiate(dupKill, folder);
        i.transform.GetChild(0).GetText().text = Player.list[alive].username;
        i.transform.GetChild(2).GetText().text = Player.list[death].username;
        i.SetActive(true);
        i.GetComponent<DestroyAfterSeconds>().curveobj = gameObject;
    }


    public void playerFall(ushort death)
    {
        var i = Instantiate(dupFall, folder);
        i.transform.GetChild(0).GetText().text = Player.list[death].username;
        i.SetActive(true);
        i.GetComponent<DestroyAfterSeconds>().curveobj = gameObject;
    }
}
