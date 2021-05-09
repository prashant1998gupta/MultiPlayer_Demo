using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager instance;

    [Header("GameUIManger")]
    public Text randomNUmber;
    public Text randomNumberSum;
    public Button randomNumberButtonGenerator;

    public int sum;
    int random;


    [Header("winerPanel")]
    public GameObject winnerPanel;
    public Button okButton;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        UIManager.uiManagerInstance.opponentScearchPanel.SetActive(false);
        Debug.Log($"timer started");
        Timer.instace
        .SetDuration(20).OnEnd(() => winnerPanel.SetActive(true))
        .Begin();

        randomNumberSum.text = "0";
        randomNUmber.text = "0";
        sum = 0;
        AddListnerInButton();
    }

    private void AddListnerInButton()
    {
        randomNumberButtonGenerator.onClick.AddListener(() =>OnRandomButtonClick());
        okButton.onClick.AddListener(OnClickOkButton);
    }

    private void OnClickOkButton()
    {
        //StartCoroutine(MainSceneLoad());
        MainSceneLoad();
    }


    //IEnumerator MainSceneLoad()
    void MainSceneLoad()
    {

        SceneManager.LoadScene(0);
        /*AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(0);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
            Debug.Log("Scene Done");
        }*/
    }

    private void OnRandomButtonClick()
    {
        random = UnityEngine.Random.Range(0, 11);

        RandomText();
        RandomSum();
    }

    private void RandomSum()
    {
        sum += random;
        randomNumberSum.text = sum.ToString();
    }

    public void RandomText()
    {
        randomNUmber.text = random.ToString();
    }
}
