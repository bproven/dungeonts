using UnityEngine;
using UnityEngine.SceneManagement;

using Assets.Game.Scripts.Pickups;

public class MainMenuScript : MonoBehaviour
{

    // Use this for initialization
    //void Start () {
    //	
    //}
    //
    // Update is called once per frame
    //void Update () {
    //	
    //}

    public void PlayScene( string name )
    {
        Debug.LogFormat("Playing {0}", name);
        SceneManager.LoadScene(name);
        Looter?.Pickup(SelectedItem);
    }

    public void PlayGame()
    {
        PlayScene("HybridLooterShooter");
    }

    public void PlayMap1()
    {
        PlayScene("HybridLooterShooter");
    }

    public void PlayMap2()
    {
        PlayScene("HybridLooterShooter 1");
    }

    public void PlayMap3()
    {
        PlayScene("HybridLooterShooter");
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
