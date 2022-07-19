using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardManager : MonoBehaviour
{
    public bool active;
    public GameObject panel;
    public GameObject content;
    public GameObject example;

    // Update is called once per frame
    void Inputs()
    {
        if (Input.GetKeyDown(InputManager.scoreboard))
        {
            Active();
        }

        if (Input.GetKeyUp(InputManager.scoreboard))
        {
            Deactivate();
        }
    }
    void Update()
    {
        Inputs();
        if (!active)
            return;

        var players = SortPlayer(ArrayToList(FindObjectsOfType<Player>()));
        Clear();

        foreach (var p in players)
        {
            var ex = Instantiate(example, content.transform);
            var children = ex.GetChildren();

            children[0].GetText().text = p.username;
            children[1].GetText().text = p.stats.kills.ToString();
            children[2].GetText().text = p.stats.deaths.ToString();
            children[3].GetText().text = p.stats.kOd.ToString2();

            ex.SetActive(true);
        }
    }

    private List<Player> SortPlayer(List<Player> players)
    {
        List<Player> l = new List<Player>();
        while (players.Count > 0)
        {
            int index = MaxValueIndex(players);
            Player p = players[index];
            players.RemoveAt(index);
            l.Add(p);
        }
        return l;
    }

    private int MaxValueIndex(List<Player> players)
    {
        int index = 0;
        int value = int.MinValue;

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].stats.kills > value)
            {
                index = i;
                value = players[i].stats.kills;
            }
        }
        return index;

    }

    private void Clear()
    {
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private List<T> ArrayToList<T>(T[] a)
    {
        List <T> l = new List<T>();
        foreach (T item in a)
            l.Add(item);
        return l;
    }
    
    public void Active()
    {
        active = true;
        MouseRules.scoreboard = true;
        panel.SetActive(true);
    }
    public void Deactivate()
    {
        MouseRules.scoreboard = false;
        active = false;
        panel.SetActive(false);
    }
}

