using UnityEngine;

public class CheatManager : MonoBehaviour
{
    private bool infiniteHealthActivated = false;
    private bool infiniteAmmoActivated = false;

    private GameManager gm;

    private void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        InputCheck();
        EditValues();
    }

    private void EditValues()
    {
        if (infiniteHealthActivated)
        {
            //player health is set to 2,147,483,647
            gm.playerCurrentHealth = 214748367;
            gm.playerMaxHealth = 214748367;
        }
        if (infiniteAmmoActivated)
        {
            gm.playerCurrentAmmo = 214748367;
            gm.playerMaxAmmo = 214748367;
            //the ammo amount is set to 2,147,483,647
        }
    }


    private void InputCheck()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            infiniteHealthActivated = true;
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            infiniteAmmoActivated = true;
        }
    }
}
