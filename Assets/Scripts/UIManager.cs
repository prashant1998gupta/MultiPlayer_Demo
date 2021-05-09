using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum PVPType { None, Join, Host, PublicGame};
public enum PlayerState { Lobby, WaitingForOpponent, WaitingRematch, Game, GameEnd };
public class UIManager : MonoBehaviour
{

    public static UIManager uiManagerInstance;
    public PVPType pvpModestate;

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
    public Text waitingLoadingOtherPlayerName;
    public Button Back;

    [Header("Host/join")]
    public GameObject host_join;
    public Button host;
    public Button join;
    public Button back_Host_join;


    [Header("ShareCode")]
    public GameObject ShareCode;
    public Button sharebutton;
    public Button backShareCode;

    [Header("MatchCode")]
    public GameObject matchCode;
    public Button Go;
    public Button backMatchCode;
    public Text infoText;


    [Header("Opponent not found")]
    public GameObject OpponentNotFoundPanel;
    public Button backToPVP;

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
        publicButton.onClick.AddListener(OnPublicButtonClick);
        BackButton.onClick.AddListener(OnBackButtonClick);
        Back.onClick.AddListener(OnBackClick);
        privatebutton.onClick.AddListener(()=>OnPrivateButtonClick());
        back_Host_join.onClick.AddListener(OnBackHostJoinButtonClick);
        host.onClick.AddListener(OnHostButtonClick);
        sharebutton.onClick.AddListener(OnShareButtonClick);
        backShareCode.onClick.AddListener(OnBackShareCode);
        join.onClick.AddListener(OnJoinButtonClick);
        backMatchCode.onClick.AddListener(OnBackMatchButtonClick);
        Go.onClick.AddListener(OnGobuttonClick);
        backToPVP.onClick.AddListener(OnClickBackPVPButton);
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

    private void OnClickBackPVPButton()
    {
        PvPPanel.SetActive(true);
        OpponentNotFoundPanel.SetActive(false);
        opponentScearchPanel.SetActive(false);
    }

    private void OnGobuttonClick()
    {
       
    }

    private void OnBackMatchButtonClick()
    {
        matchCode.SetActive(false);
        host_join.SetActive(true);
    }

    private void OnJoinButtonClick()
    {
        matchCode.SetActive(true);
        host_join.SetActive(false);
    }

    private void OnBackShareCode()
    {
        host_join.SetActive(true);
        ShareCode.SetActive(false);
    }

    private void OnShareButtonClick()
    {
        
    }

    private void OnBackHostJoinButtonClick()
    {
        host_join.SetActive(false);
        PvPPanel.SetActive(true);
    }

    private void OnHostButtonClick()
    {
        ShareCode.SetActive(true);
        host_join.SetActive(false);
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



        Timer.instace.
        SetDuration(20).OnEnd(() => 
        { OpponentNotFoundPanel.SetActive(true);
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
            Debug.Log(pvpModestate );
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
                //CreatorOfPrivateRoom();
            }
            else if (pvpModestate == PVPType.Join)
            {
                Debug.Log("Call Back After join Lobby" + pvpModestate);
                PhotonManager.instance.IsPrivateRoom = true;
                // JoinnerOfPrivateRoom();
            }
        }
    }

}
