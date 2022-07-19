using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MainMenuUI : MonoBehaviour
{

    //Dictionary<int, CursorUI> cursors = new Dictionary<int, CursorUI>();
    public Slider masterVolume;
    public Slider musicVolume;
    public Slider sfxVolume;
    public Slider fov;
    public Slider ySens;
    public Slider xSens;
    public BoolUI fullscreen;

    public void PlayButton()
    {
        MouseRules.main = false;
        SceneManager.LoadScene(2);

    }
    public void QuitButton()
    {
        Application.Quit();
    }

    private void Awake()
    {
        LoadData();
        MouseRules.main = true;
    }
    public void SaveButton()
    {
        SaveData();
    }
    public void LoadButton()
    {
        LoadData();
    }
    private void Update()
    {
        MouseRules.Update();
    }
    private int SaveData()
    {
        //Cursur
        PlayerPrefs.SetString("cursorSettings", JsonUtility.ToJson(CursorMnagement.cursorSettings));
        //Input
        foreach (var item in InputButton.list.Values)
        {
            PlayerPrefs.SetString(item.prefab, item.currentSelected.ToString());
        }
        //Options - TODO: USE THEM IN THE GAME - i dont have music in game still
        PlayerPrefs.SetFloat("masterSound", masterVolume.value);
        PlayerPrefs.SetFloat("musicSound", musicVolume.value);
        PlayerPrefs.SetFloat("sfxSound", sfxVolume.value);
        
        //Camera Controller configuration
        PlayerPrefs.SetFloat("fov", fov.value);
        PlayerPrefs.SetFloat("ySens", ySens.value);
        PlayerPrefs.SetFloat("xSens", xSens.value);

        PlayerPrefs.SetInt("fullscreen", fullscreen.Value ? 1 : 0);
        Screen.fullScreen = fullscreen.Value? true : false;


        return 0;
    }
    IEnumerator tellCursor(CursorUI c)
    {
        CursorMnagement.cursorSettings = c;
        yield return new WaitUntil(() => CursorMnagement.Singleton != null);
        yield return new WaitUntil(() => CursorMnagement.Singleton.isActiveAndEnabled);
        CursorMnagement.GetData(c);
    }
    private int LoadData()
    {
        //Cursur
        CursorUI c = JsonUtility.FromJson<CursorUI>(PlayerPrefs.GetString("cursorSettings", JsonUtility.ToJson(CursorMnagement.normalSettings)));
        StartCoroutine(tellCursor(c)); 
        //Input
        foreach (var item in InputButton.list.Values)
        {
            StartCoroutine(item.Return());
            PlayerPrefs.SetString(item.prefab, item.beforeSelected.ToString());
        }
        //Options
        masterVolume.value = PlayerPrefs.GetFloat("masterSound", 100);
        musicVolume.value = PlayerPrefs.GetFloat("musicSound", 100);
        sfxVolume.value = PlayerPrefs.GetFloat("sfxSound", 100);
        fov.value = PlayerPrefs.GetFloat("fov", 70); //used
        ySens.value = PlayerPrefs.GetFloat("ySens", 25); //used
        xSens.value = PlayerPrefs.GetFloat("xSens", 25); //used
        fullscreen.SetValue(PlayerPrefs.GetInt("fullscreen", 1) == 1); //used
        Screen.fullScreen = fullscreen.Value;
        return 0;
    }
}
