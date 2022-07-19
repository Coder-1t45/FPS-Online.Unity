using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    static List<SpawnPoint> points = new List<SpawnPoint>();

    public static SpawnPoint GetSpawnPoint()
    {
        foreach (var item in points)
        {
            if (!item.used) { 
                item.used = true;
                return item;
            }
        }
        return null;
    }
    public static SpawnPoint GetClearestPoint()
    {
        float minest = float.MaxValue;
        SpawnPoint presult = null;

        foreach (var p in points)
        {
            var players = GameObject.FindGameObjectsWithTag("Player");
            float minDistance = float.MaxValue;
            foreach (var pl in players)
            {
                minDistance = Mathf.Min(minDistance, Vector3.Distance(pl.transform.position, p.pos));
            }
            
            if (minest > minDistance)
            {
                minest = minDistance;
                presult = p;
            }
        }
        return presult;
    }
    static SpawnPoint()
    {
        points = new List<SpawnPoint>();
    }
    public bool used;
    private void Awake()
    {
        points.Add(this);
    }

    public Vector3 pos
    {
        get
        {
            return this.transform.position;
        }
    }
    public Quaternion rot
    {
        get
        {
            return this.transform.rotation;
        }
    }
}
