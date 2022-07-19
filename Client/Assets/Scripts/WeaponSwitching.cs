using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{
    public int selectedWeapons = 0;
    public Player player;
    private void Awake()
    {
        SelectWeapon();
    }

    private void Update()
    {
        int previus = selectedWeapons;

        selectedWeapons = Input.GetKeyDown(KeyCode.Alpha1) ? 0 :
            Input.GetKeyDown(KeyCode.Alpha2) ? 1 : selectedWeapons;

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (selectedWeapons >= transform.childCount - 1)
                selectedWeapons = 0;
            else
                selectedWeapons++;
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (selectedWeapons <= 0)
                selectedWeapons = transform.childCount - 1;
            else
                selectedWeapons--;
        }

        if (previus != selectedWeapons)
        {
            SelectWeapon();
        }
    }
    void SelectWeapon()
    {
        int i = 0;
        foreach (Transform weapon in transform)
        {
            if (i == selectedWeapons) { weapon.gameObject.SetActive(true); weapon.GetComponent<Weapon>().Inisiate(); Weapon.Singleton = weapon.GetComponent<Weapon>(); }
            else weapon.gameObject.SetActive(false);
            
            i++;
        }
    }
}
