using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject gM;
    [SerializeField] private GameObject cM;

    private Playerhealth playerH;

    //Player health
    public float playerMaxHealth;
    public float playerCurrentHealth;

    //Player ammo
    public int playerMaxAmmo;
    public int playerCurrentAmmo;
    public int playerMagazineSize;
    public int playerCurrentMagazineAmmo;

    public GameObject canvas;

    Scene currentScene;

    private void Start()
    {
        DontDestroyOnLoad(gM);
        DontDestroyOnLoad(cM);
        DontDestroyOnLoad(canvas);

        playerH = FindObjectOfType<Playerhealth>();
        currentScene = SceneManager.GetActiveScene();


    }

    private void Update()
    {
        Debug.Log(currentScene.buildIndex);

        if (playerCurrentHealth <= 0)
        {
            playerH.kill();
        }

        if (playerCurrentHealth > playerMaxHealth)
        {
            playerCurrentHealth = playerMaxHealth;
        }
        if (playerCurrentMagazineAmmo > playerMagazineSize)
        {
            playerCurrentMagazineAmmo = playerMagazineSize;
        }
        if (playerCurrentAmmo > playerMaxAmmo)
        {
            playerCurrentAmmo = playerMaxAmmo;
        }
        if (playerCurrentAmmo <= 0)
        {
            playerCurrentAmmo = 0;
        }

        int buildIndex = currentScene.buildIndex;

        switch (buildIndex)
        {
            case 0:
                canvas.SetActive(false);
                break;
            case 1:
                canvas.SetActive(true);
                break;
        }
    }

    public void ValuesSetStart()
    {
        playerCurrentHealth = 1;
        playerCurrentMagazineAmmo = 0;
        playerCurrentAmmo = 60;
    }

    public void DamagePlayer(float dmg)
    {
        playerCurrentHealth -= dmg;
    }
}
