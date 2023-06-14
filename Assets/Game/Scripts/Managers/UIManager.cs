using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    private void Awake()
    {
        instance = (instance == null) ? this : instance;
    }

    public GameObject mainMenu;
    public GameObject completeMenu;
    public GameObject failMenu;

    public bool isTapToplay;

    private void Start()
    {
        mainMenu.SetActive(true);
        completeMenu.SetActive(false);
        failMenu.SetActive(false);
    }

    public void OnTapToPlay()
    {
        mainMenu.SetActive(false);
        completeMenu.SetActive(false);
        failMenu.SetActive(false);

        Invoke("Delay", 0.5f);
    }

    private void Delay()
    {
        isTapToplay = true;
    }

    public void OnCotinue()
    {
        GameManager.Instance.GenerateLevel();

        mainMenu.SetActive(false);
        completeMenu.SetActive(false);
        failMenu.SetActive(false);
    }

    public void ShowMenu_Complete()
    {
        mainMenu.SetActive(false);
        completeMenu.SetActive(true);
        failMenu.SetActive(false);
    }

    

    public void ShowMenu_Fail()
    {
        mainMenu.SetActive(false);
        completeMenu.SetActive(false);
        failMenu.SetActive(true);
    }
}
