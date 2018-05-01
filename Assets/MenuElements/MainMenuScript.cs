using System;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

using Assets.Game.Scripts.Pickups;

public class MainMenuScript : MonoBehaviour
{

    public int[] mapIndex = new int[3];
    public string[] maps = new string[3];

    private IEnumerator WaitForSceneToLoad( string name, Action loaded )
    {

        AsyncOperation asyncSceneLoader = SceneManager.LoadSceneAsync(name, LoadSceneMode.Single);
        while (!asyncSceneLoader.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
        loaded();
    }

    public void PlayScene(string name)
    {
        Debug.LogFormat("Playing {0}", name);
        StartCoroutine(WaitForSceneToLoad(name, () => PickupSelectedItem()));
    }

    public void PickupSelectedItem()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.MoveGameObjectToScene(SelectedItem.gameObject, currentScene);
        if ( Looter == null )
        {
            Debug.LogFormat("LooterAgent not found in Scene {0}", name);
        }
        Looter?.Pickup(SelectedItem);
    }

    public void PlayGame()
    {
        PlayScene(maps[0]);
    }

    public void PlayMap1()
    {
        PlayScene(maps[0]);
    }

    public void PlayMap2()
    {
        PlayScene(maps[1]);
    }

    public void PlayMap3()
    {
        PlayScene(maps[2]);
    }

    public LooterAgent Looter
    {
        get
        {
            return GameObject.FindObjectOfType<LooterAgent>();
        }
    }

    public Item SelectedItem { get; set; }

    private Item GetNewItem( string tag )
    {
        Item item = GameObject.FindGameObjectWithTag(tag).GetComponent<Item>();
        if (item == null)
        {
            Debug.LogErrorFormat( "No {0} Object defined in Scene", tag );
        }
        return item;
    }

    public void SelectItem( string tag )
    {
        Debug.LogFormat("Selecting {0}", tag);
        Item item = GetNewItem(tag);
        if ( item != null )
        {
            SelectedItem = item;
            DontDestroyOnLoad(SelectedItem);
        }
    }

    public void selectSword()
    {
        SelectItem("Sword");
    }

    public void selectArmor()
    {
        SelectItem("Armor");
    }

    public void selectBoots()
    {
        SelectItem("Boots");
    }

}
