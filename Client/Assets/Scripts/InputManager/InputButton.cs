using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputSelection
{
    public string prefab;
    public KeyCode currentSelected;
    public KeyCode beforeSelected;
    public string text => currentSelected.ToString();
    public InputButton main;
    public InputSelection(string p, KeyCode current, KeyCode before, InputButton main)
    {
        this.prefab = p;
        this.currentSelected = current;
        this.beforeSelected = before;
        this.main = main;
    }
    public IEnumerator Return()
    {
        if (currentSelected != KeyCode.None)
        { 
            this.currentSelected = this.beforeSelected;
            Debug.Log("IN");
            yield return new WaitUntil(() => main.isActiveAndEnabled);
            Debug.Log("FINISHED");
            main.text.text = this.beforeSelected.ToString();
        }
    }
}
public class InputButton : MonoBehaviour
{
    private Button button => GetComponent<Button>();
    public Text text => GetComponentInChildren<Text>();
    public string Key => InputManager.playerPrefs[InputManager.names.Index(transform.parent.GetComponent<Text>().text)];
    public InputManager manager => InputManager.Singleton;
    
    public KeyCode recentKey;

    public static Dictionary<ushort, InputSelection> list = new Dictionary<ushort, InputSelection>();
    static ushort NextID;
    public ushort id;
    private void Awake()
    {
        if (list.ContainsKey(id))
            text.text = list[id].text;
    }
    private IEnumerator Start()
    {
        yield return new WaitUntil(() => manager.updated);
        beforeSelected = InputManager.keys[InputManager.names.Index(transform.parent.GetComponent<Text>().text)];
        text.text = beforeSelected.ToString();

        id = NextID;
        NextID++;
        list.Add(id, new InputSelection(Key, KeyCode.None, beforeSelected, this));
    }
    public KeyCode currentSelected;
    public KeyCode beforeSelected;
    public IEnumerator Clicked()
    {
        text.text = "WAITING";
        button.interactable = false;
        yield return new WaitUntil(() => Input.anyKeyDown);
        yield return new WaitUntil(() => recentKey != KeyCode.None);
        currentSelected = recentKey;
        button.interactable = true;
        list[id].currentSelected = currentSelected;
        text.text = list[id].text;
    }
    void OnGUI()
    {
        //Event e = Event.current;
        //if (e.isKey)
        //{
        //    recentKey = e.keyCode;
        //}

        foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
                recentKey = kcode;
        }
    }
    public void Update()
    {
        button.onClick.AddListener(delegate {
            StartCoroutine(Clicked());
        });
    }
}
