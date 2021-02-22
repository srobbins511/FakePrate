using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour {
    public GameObject mainMenu;
    public GameObject multiplayerMenu;
    public GameObject hostMenu;
    public GameObject joinMenu;
    public Text HostIPText;
    public InputField JoinIP;

    public void Start() {
        mainMenu.SetActive(true);
        multiplayerMenu.SetActive(false);
        joinMenu.SetActive(false);
        hostMenu.SetActive(false);
    }

    public void SinglePlayer() {
        SceneManager.LoadScene(1);
    }

    public static void LoadScene(int scene) {
        SceneManager.LoadScene(scene);
    }

    public void ShowMultiplayer() {
        mainMenu.SetActive(false);
        multiplayerMenu.SetActive(true);
        joinMenu.SetActive(false);
        hostMenu.SetActive(false);
    }

    public void HideMultiplayer() {
        mainMenu.SetActive(true);
        multiplayerMenu.SetActive(false);
        joinMenu.SetActive(false);
        hostMenu.SetActive(false);
    }

    public void HostGame() {
        mainMenu.SetActive(false);
        multiplayerMenu.SetActive(false);
        hostMenu.SetActive(true);
        joinMenu.SetActive(false);
        HostIPText.text = "Your IP is " + Dns.GetHostEntry("127.0.0.1").AddressList[0].ToString();
        NetworkManager.Instance.StartServer();
        NetworkManager.Instance.SendString("TEST MESSAGE 2");
    }

    public void JoinGameMenu() {
        mainMenu.SetActive(false);
        multiplayerMenu.SetActive(false);
        joinMenu.SetActive(true);
        hostMenu.SetActive(false);
    }

    public void JoinGame() {
        Debug.Log("Joining: "+ JoinIP.text);
        NetworkManager.Instance.JoinServer(JoinIP.text);
        string testString = NetworkManager.Instance.ReceiveString();
        Debug.Log(testString);
    }
}
