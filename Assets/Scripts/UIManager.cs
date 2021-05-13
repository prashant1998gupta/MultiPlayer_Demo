using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace prashantMultiPlayer
{
    public enum PVPType { None, Join, Host, PublicGame };
    public enum PlayerState { Lobby, WaitingForOpponent, WaitingRematch, Game, GameEnd };

    public enum PlayerDisconnectBWGame { ConnectionLost, PauseGame, RemoveAppFromBackGround }
    public class UIManager : MonoBehaviour
    {

        public static UIManager uiManagerInstance;
        public PVPType pvpModestate;
        public PlayerDisconnectBWGame playerDisconnectBWGame;

        [Header("UImanagerCanvas")]
        public GameObject UIManagerCanvas;

        [Header("PvP")]
        public GameObject PvPPanel;
        public Button PVP;

        [Header("ModeSelection")]
        public GameObject modeSeletionPanel;
        public Button publicButton;
        public Button privatebutton;
        public Button BackButton;

        [Header("searchingPanelUI")]
        public GameObject opponentScearchPanel;
        public Text myName;
        public Text waitingLoadingOtherPlayerName;
        public Button Back;

        [Header("Host/join")]
        public GameObject host_join;
        public Button host;
        public Button join;
        public Button back_Host_join;


        [Header("ShareCode")]
        public GameObject ShareCode;
        public Text ShareCodeText;
        public Button sharebutton;
        public Button backShareCode;

        [Header("MatchCode")]
        public GameObject matchCode;
        public InputField EnterJoinCode;
        public Button Go;
        public Button backMatchCode;
        public Text infoText;


        [Header("Opponent not found")]
        public GameObject OpponentNotFoundPanel;
        public Button backToPVP;


        [Header("ResultBorad Ui")]
        public GameObject ResultPannel;
        public GameObject ResultMyWinnerLabel;
        public GameObject ResultOponentWinnerLabel;
        public Image ResultMyProfileImage;
        public Image ResultOponentProfileImage;
        public Text ResultMyName;
        public Text ResultOponentName;
        public Text ResultMyScore;
        public Text ResultOponentScore;
        public Button ResultBackToLobby;
        public Button ResultRematch;
        public Text ResultWarningBox;


        [Header("Contribution POP")]
        public GameObject ContributionPannel;
        public Button ContributionBack;
        public Image ContributionMyImage;
        public Text ContributionMyProfileName;
        public Text ContributionOponentProfileName;


        [Header("Rematch")]

        public GameObject RematchPop;
        public Text RematchText;
        public Button RematchOkk;
        public Button RematchCancel;

        private void Awake()
        {

            Debug.Log("UIManager is calling Awake");
            if (uiManagerInstance == null)
            {
                uiManagerInstance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }
            DontDestroyOnLoad(this.gameObject);
        }
        private void Start()
        {
            AddListners();
            PhotonManager.OnPhotonServerConnected += AfterPhotonServerConnectionCall;
            PhotonManager.OnJoinedLobbyCall += AfterJoinLobby;
        }

        private void OnEnable()
        {
            pvpModestate = PVPType.None;
        }
        void AddListners()
        {
            PVP.onClick.AddListener(OnPVPButtonClick);
            BackButton.onClick.AddListener(OnBackButtonClick);

            publicButton.onClick.AddListener(OnPublicButtonClick);
            Back.onClick.AddListener(OnBackClick);


            privatebutton.onClick.AddListener(() => OnPrivateButtonClick());

            back_Host_join.onClick.AddListener(OnBackHostJoinButtonClick);
            host.onClick.AddListener(OnHostButtonClick);
            sharebutton.onClick.AddListener(OnShareButtonClick);
            backShareCode.onClick.AddListener(OnBackShareCode);
            join.onClick.AddListener(OnJoinButtonClick);
            backMatchCode.onClick.AddListener(OnBackMatchButtonClick);
            Go.onClick.AddListener(EnterGameUsingCode);
            backToPVP.onClick.AddListener(OnClickBackPVPButton);

            // rematch
            RematchOkk.onClick.AddListener(LetsRematch);
            RematchCancel.onClick.AddListener(RematchRequestCancel);
        }



        public void CloseAllPanel()
        {
            UIManagerCanvas.SetActive(false);
            PvPPanel.SetActive(false);
            modeSeletionPanel.SetActive(false);
            opponentScearchPanel.SetActive(false);
            host_join.gameObject.SetActive(false);
            ShareCode.SetActive(false);
            matchCode.SetActive(false);
            OpponentNotFoundPanel.SetActive(false);
        }


        public void ClosePrivatePanals()
        {
            host_join.SetActive(false);
            ShareCode.SetActive(false);
            matchCode.SetActive(false);
        }
        private void OnClickBackPVPButton()
        {
            PvPPanel.SetActive(true);
            OpponentNotFoundPanel.SetActive(false);
            opponentScearchPanel.SetActive(false);
        }



        private void OnBackMatchButtonClick()
        {
            infoText.text = "";
            matchCode.SetActive(false);
            host_join.SetActive(true);
        }

        private void OnJoinButtonClick()
        {
            EnterJoinCode.text = "";
            matchCode.SetActive(true);
            host_join.SetActive(false);
        }

        private void OnBackShareCode()
        {
            host_join.SetActive(true);
            ShareCode.SetActive(false);
        }

        private string shareMassagep;

        private void OnShareButtonClick()
        {
            shareMassagep = $"join the room with this code {StaticData.PrivateRoomRandomCode} show we can play Game";
            Debug.LogError("share Button Work");
            StartCoroutine(TakeScreenshotAndShare());
        }


        private IEnumerator TakeScreenshotAndShare()
        {
            Debug.LogError("share Button Work");
            yield return new WaitForEndOfFrame();

            /* Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
             ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
             ss.Apply();

             string filePath = Path.Combine(Application.temporaryCachePath, "shared img.png");
             File.WriteAllBytes(filePath, ss.EncodeToPNG());

             // To avoid memory leaks
             Destroy(ss);*/

            //.AddFile(filePath)

            new NativeShare()
                .SetSubject("Random Number sum winer ").SetText(shareMassagep).SetUrl("https://github.com/prashant1998gupta")
                .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
                .Share();

            // Share on WhatsApp only, if installed (Android only)
            //if( NativeShare.TargetExists( "com.whatsapp" ) )
            //	new NativeShare().AddFile( filePath ).AddTarget( "com.whatsapp" ).Share();
        }


        private void OnBackHostJoinButtonClick()
        {
            host_join.SetActive(false);
            PvPPanel.SetActive(true);
        }

        void OnHostButtonClick()
        {
            //ShareCode.SetActive(true);
            //host_join.SetActive(false);
            pvpModestate = PVPType.Host;
            PhotonManager.instance.CustomConnectToMasterPhoton();
        }

        private void OnPrivateButtonClick()
        {
            host_join.SetActive(true);
            PvPPanel.SetActive(false);
        }

        void OnPVPButtonClick()
        {
            PvPPanel.SetActive(false);
            modeSeletionPanel.SetActive(true);
        }

        void OnPublicButtonClick()
        {

            opponentScearchPanel.SetActive(true);
            pvpModestate = PVPType.PublicGame;
            PhotonManager.instance.CustomConnectToMasterPhoton();
            myName.text = StaticData.MyProfileName;


            Timer.instace.
            SetDuration(20).OnEnd(() =>
            {
                OpponentNotFoundPanel.SetActive(true);
                opponentScearchPanel.SetActive(false);
            }).Begin();


        }

        void OnBackButtonClick()
        {
            PvPPanel.SetActive(true);
            modeSeletionPanel.SetActive(false);
        }

        void OnBackClick()
        {
            opponentScearchPanel.SetActive(false);
            modeSeletionPanel.SetActive(true);
        }



        void AfterPhotonServerConnectionCall(bool state)
        {
            if (state)
            {
                Debug.Log(pvpModestate);
                PhotonManager.instance.JoinCustomLobby();
            }
        }


        void AfterJoinLobby(bool state)
        {
            if (state == true)
            {
                if (pvpModestate == PVPType.PublicGame)
                {
                    Debug.Log("Call Back State" + state);
                    PhotonManager.instance.JoinRandomRoom();
                }
                else if (pvpModestate == PVPType.Host)
                {
                    Debug.Log("Call Back After join Lobby");
                    CreatorOfPrivateRoom();
                }
                else if (pvpModestate == PVPType.Join)
                {
                    Debug.Log("Call Back After join Lobby" + pvpModestate);
                    PhotonManager.instance.IsPrivateRoom = true;
                    //JoinnerOfPrivateRoom();
                }
            }
        }


        void CreatorOfPrivateRoom()
        {
            PhotonManager.instance.CreatePrivateRoom(StaticData.Room_Amount);
        }


        void JoinnerOfPrivateRoom()
        {
            PhotonManager.instance.JoinerRoomPropoerties();
            PhotonManager.instance.IsPrivateRoom = true;

            PhotonManager.instance.JoinCustomRoom(EnterJoinCode.text);


        }

        public void ShowShareCodePNL(bool state)
        {
            if (state)
            {

                ShareCode.SetActive(true);
                host_join.SetActive(false);
                ShareCodeText.text = "";
                ShareCodeText.text = StaticData.PrivateRoomRandomCode;
            }
        }

        void EnterGameUsingCode()
        {
            if (!string.IsNullOrEmpty(EnterJoinCode.text))
            {
                pvpModestate = PVPType.Join;
                PhotonManager.instance.CustomConnectToMasterPhoton();

            }
        }
        public void ShowContributionPanel(bool state)
        {
            if (state)
            {
                Debug.Log(state);
                opponentScearchPanel.SetActive(false);
                ContributionPannel.SetActive(true);
                ContributionMyImage.sprite = StaticData.MyGlobalSprite;
                ContributionMyProfileName.text = StaticData.MyProfileName;
                ContributionOponentProfileName.text = StaticData.OtherUserProfileName;
                //yield return new WaitForSeconds(10);
            }
            if (!state)
            {
                ContributionPannel.SetActive(false);
            }
        }


        public void ShowResultMenu(bool state, bool Draw)
        {
            ResultPannel.SetActive(true);
            ResultMyWinnerLabel.SetActive(false);
            ResultOponentWinnerLabel.SetActive(false);
            ResultBackToLobby.onClick.AddListener(ResultBackLobby);
            ResultRematch.onClick.AddListener(RematchRequest);
            PhotonManager.PlayerStateMode = PlayerState.GameEnd;
            ResultRematch.interactable = true;
            if (state && !Draw)
            {
                Debug.Log($"you are winner with {state} {Draw} game state {PhotonManager.PlayerStateMode}");

                ResultMyWinnerLabel.SetActive(true);
                ResultWarningBox.text = "";
                ResultMyProfileImage.sprite = StaticData.MyGlobalSprite;
                ResultOponentProfileImage.sprite = StaticData.OponentGlobalSprite;
                ResultMyName.text = StaticData.MyProfileName;
                ResultOponentName.text = StaticData.OtherUserProfileName;
                ResultMyScore.text = StaticData.My_Current_Score.ToString();
                ResultOponentScore.text = StaticData.Other_Current_Score.ToString();
            }
            else if (!state && !Draw)
            {
                Debug.Log($"you are looser with {state} {Draw} game state {PhotonManager.PlayerStateMode}");

                ResultOponentWinnerLabel.SetActive(true);
                ResultWarningBox.text = "";
                ResultMyProfileImage.sprite = StaticData.MyGlobalSprite;
                ResultOponentProfileImage.sprite = StaticData.OponentGlobalSprite;
                ResultMyName.text = StaticData.MyProfileName;
                ResultOponentName.text = StaticData.OtherUserProfileName;
                ResultMyScore.text = StaticData.My_Current_Score.ToString();
                ResultOponentScore.text = StaticData.Other_Current_Score.ToString();
            }
            else if (!state && Draw)
            {
                Debug.Log($"Draw  {state} {Draw} game state {PhotonManager.PlayerStateMode}");


                ResultWarningBox.text = "";
                ResultMyProfileImage.sprite = StaticData.MyGlobalSprite;
                ResultOponentProfileImage.sprite = StaticData.OponentGlobalSprite;
                ResultMyName.text = StaticData.MyProfileName;
                ResultOponentName.text = StaticData.OtherUserProfileName;
                ResultMyScore.text = StaticData.My_Current_Score.ToString();
                ResultOponentScore.text = StaticData.Other_Current_Score.ToString();
            }
            SceneManager.LoadScene(0);

        }

        void RematchRequest()
        {
            ResultWarningBox.text = "Rematch Request Send to Other Player";
            RaiseEventManager.instance.RaiseEVT(StaticData.REMATCH_REQUEST, true);
        }

        private void LetsRematch()
        {
            ResultPannel.SetActive(false);
            RematchPop.SetActive(false);


            RaiseEventManager.instance.RaiseEVT(StaticData.ACCEPT_REMATCH, "Accepted");
        }

        void RematchRequestCancel()
        {

            RaiseEventManager.instance.RaiseEVT(StaticData.REJECT_REMATCH, true);
            RematchPop.SetActive(false);
        }

        private void ResultBackLobby()
        {

            //RaiseEventManager.instance.RaiseEVT(StaticData.LEAVE_ROOM_WHILE_WAITING_OPPONENT, true);

            PhotonManager.instance.LeaveLobbyCond();
            PhotonManager.PlayerStateMode = PlayerState.Lobby;
            ResultPannel.SetActive(false);
        }


        public void RematchTexter()
        {
            RematchText.text = StaticData.OtherUserProfileName + "\n" + "want to challenge" + "\n" + "you for Play again" +
            "\n" + "click 'Yes' to accept";

        }
    }


}
