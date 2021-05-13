using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


namespace prashantMultiPlayer
{
    public class GameUIManager : MonoBehaviour
    {
        public static GameUIManager instance;

        public delegate void OnScoreIncrese(bool Success);
        public static OnScoreIncrese ScoreIncrement;

        [Header("GameUIManger")]
        public Text randomNUmber;
        public Text randomNumberSum;
        public Button randomNumberButtonGenerator;

        public int sum;
        int random;


        [Header("winerPanel")]
        public GameObject winnerPanel;
        public Button okButton;


        [Header("topPanel")]
        public Text myName;
        public Text myScore;
        public Sprite mySprite;

        [Space]

        public Text oppName;
        public Text oppScore;
        public Sprite oppSprite;

        [Header("WaitingForOtherPanel")]

        public GameObject WaitingForOther;

        [Header("leave game ui")]
        public Button pause;
        public GameObject PausePNL;
        public Button HomeBTNPauseMenu;
        public Button CancelMultiPause;
        public GameObject OtherPlayerLeft;

        private void Awake()
        {
            Debug.Log("GameUiManager Awake calling");
            if (instance == null)
            {

                instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }

        }
        private void OnEnable()
        {
            StaticData.MyStaticTiming = 30;
            PhotonNetwork.AutomaticallySyncScene = false;
            //UIManager.uiManagerInstance.opponentScearchPanel.SetActive(false);
            Debug.Log($"timer started");
            CounterTimer.instace
            .SetDuration(StaticData.MyStaticTiming)
            .Begin();

            randomNumberSum.text = "0";
            randomNUmber.text = "0";
            sum = 0;
            AddListnerInButton();
        }

        private void AddListnerInButton()
        {
            randomNumberButtonGenerator.onClick.AddListener(() => OnRandomButtonClick());
            okButton.onClick.AddListener(OnClickOkButton);

            // pause game buttons
            pause.onClick.AddListener(Pause);
            HomeBTNPauseMenu.onClick.AddListener(LeaveRoomFun);
            CancelMultiPause.onClick.AddListener(OnCancleMultipayer);
        }



        private void OnClickOkButton()
        {
            MainSceneLoad();
        }

        void MainSceneLoad()
        {

            SceneManager.LoadScene(0);

        }

        private void OnRandomButtonClick()
        {
            random = UnityEngine.Random.Range(0, 11);

            RandomText();
            RandomSum();

            ScoreIncrement?.Invoke(true);
        }

        private void RandomSum()
        {
            sum += random;
            randomNumberSum.text = sum.ToString();
            myScore.text = sum.ToString();
        }

        public void RandomText()
        {
            randomNUmber.text = random.ToString();
        }

        void LeaveRoomFun()
        {
            PhotonManager.instance.LeaveGameCond();
        }


        private void OnCancleMultipayer()
        {
            PausePNL.SetActive(false);
        }
        public void Pause()
        {
            if (PhotonManager.instance.IsMultiPlayer)
            {
                PausePNL.SetActive(true);
            }

        }

    }
}