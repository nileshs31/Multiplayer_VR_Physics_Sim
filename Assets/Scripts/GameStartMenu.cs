using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
#if UNITY_EDITOR
using ParrelSync;
#endif
public class GameStartMenu : MonoBehaviour
{
    [Header("UI Pages")]
    public GameObject mainMenu;
    public GameObject about;

    [Header("Main Menu Buttons")]
    public Button startButton;
    public Button aboutButton;
    public Button quitButton;

    public TextMeshProUGUI userNameField;


    public List<Button> returnButtons;

    // Start is called before the first frame update
    void Start()
    {
        EnableMainMenu();
        GenerateUsernameAtStart();
        //Hook events
        startButton.onClick.AddListener(StartGame);
        aboutButton.onClick.AddListener(EnableAbout);
        quitButton.onClick.AddListener(QuitGame);

        foreach (var item in returnButtons)
        {
            item.onClick.AddListener(EnableMainMenu);
        }
    }
    public void GenerateUsernameAtStart()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        char[] word = new char[5];
        string usernameGenerated = "";

        for (int i = 0; i < 5; i++)
        {
            word[i] = chars[Random.Range(0,chars.Length)];
        }
        usernameGenerated = new string(word);
        userNameField.text = usernameGenerated;
        var parrelarg = "";
#if UNITY_EDITOR
        parrelarg = ClonesManager.GetArgument();
#endif

        PlayerPrefs.SetString(parrelarg+"username", usernameGenerated);
        UnityEngine.Debug.Log(PlayerPrefs.GetString(parrelarg + "username"));
    }


    public void QuitGame()
    {
        UnityEngine.Application.Quit();
    }

    public void StartGame()
    {
        HideAll();
        SceneTransitionManager.singleton.GoToSceneAsync(1);
    }

    public void HideAll()
    {
        mainMenu.SetActive(false);
        about.SetActive(false);
    }

    public void EnableMainMenu()
    {
        mainMenu.SetActive(true);
        about.SetActive(false);
    }
    public void EnableOption()
    {
        mainMenu.SetActive(false);
        about.SetActive(false);
    }
    public void EnableAbout()
    {
        mainMenu.SetActive(false);
        about.SetActive(true);
    }
}
