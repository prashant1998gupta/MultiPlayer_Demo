using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public static float Room_Name;

    public static PhotonManager instance;
    public static PlayerState PlayerStateMode;

    public bool IsMultiPlayer;
    public bool IsPrivateRoom;
    public bool IsPrivateRoomJoin = false;
    public bool IsPrivateRoomCreater = false;

    [SerializeField]
    private int roomSize;
    private bool connected;
    private bool starting;


    internal TypedLobby privateLobby = new TypedLobby("privateLobby", LobbyType.Default);
    internal TypedLobby publicLobby = new TypedLobby("publicLobby", LobbyType.Default);

    internal OpJoinRandomRoomParams ExpectedRandomRoomProperties = new OpJoinRandomRoomParams();

    public delegate void PhotonConnect(bool Success);
    public static PhotonConnect OnPhotonServerConnected;

    public delegate void JoinedLobbyCall(bool Success);
    public static JoinedLobbyCall OnJoinedLobbyCall;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

    }


    private void Start()
    {
        //PhotonNetwork.ConnectUsingSettings();
        int i = SupportClass.ThreadSafeRandom.Next() % 99;
        Debug.Log(i);
    }



    public void CustomConnectToMasterPhoton()
    {
        Debug.Log($"Cureent State at masteer {UIManager.uiManagerInstance.pvpModestate} mathod name CustomConnectToMasterPhoton");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connection made to " + PhotonNetwork.CloudRegion + " server.");

        PhotonNetwork.AutomaticallySyncScene = true;
        //connected = true;
        //buttonText.text = "Begin Game";

        OnPhotonServerConnected(true);
    }

    public void JoinCustomLobby()
    {
        Debug.Log("Joining Custom Lobby" + PhotonNetwork.CurrentLobby + "Current state" + UIManager.uiManagerInstance.pvpModestate);
        if (UIManager.uiManagerInstance.pvpModestate == PVPType.PublicGame)
        {
            Debug.Log("Joined Public Lobby");
            PhotonNetwork.JoinLobby(publicLobby);
        }
        else if (UIManager.uiManagerInstance.pvpModestate == PVPType.Host || UIManager.uiManagerInstance.pvpModestate == PVPType.Join)
        {
            PhotonNetwork.JoinLobby(privateLobby);
            Debug.Log("joined Private Lobby Check If Connected=" + PhotonNetwork.IsConnected);
        }
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby.....");
        PlayerStateMode = PlayerState.Lobby;
        OnJoinedLobbyCall(true);


        base.OnJoinedLobby();
    }


    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("join random room");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        if (IsPrivateRoomJoin)
        {
            print(message);

        }
        Debug.Log($"onjoinRandomFailed is call...");
        CreateandJoinRoom();
        base.OnJoinRandomFailed(returnCode, message);
    }

     
    public string GenerateRandomRoomName()
    {
        int length = 5;
        string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        System.Random random = new System.Random();
       

        char[] chars = new char[length];
        for (int i = 0; i < length; i++)
        {
            chars[i] = validChars[random.Next(0, validChars.Length)];
        }
        return new string(chars);
    }

    public void CreateandJoinRoom()
    {
        if (UIManager.uiManagerInstance.pvpModestate == PVPType.PublicGame)
        {

            string randomRoomName =  GenerateRandomRoomName() + Random.Range(10000, 100000).ToString();
            Debug.Log("Room name..." + randomRoomName);
            //Room_Name = float.Parse(randomRoomName);
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.IsOpen = true;
            roomOptions.IsVisible = true;
            roomOptions.MaxPlayers = 2;
            ExitGames.Client.Photon.Hashtable RoomCustomProps = new ExitGames.Client.Photon.Hashtable();
            roomOptions.CustomRoomProperties = RoomCustomProps;
            PhotonNetwork.CreateRoom(randomRoomName, roomOptions);
        }
    }

    public override void OnJoinedRoom()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log($"this is not MasterClint {!PhotonNetwork.IsMasterClient}");
        }
        else
        {
            Debug.Log($"this is MasterClint {PhotonNetwork.IsMasterClient}");
        }
        if (IsPrivateRoomCreater)
        {
            if (UIManager.uiManagerInstance.pvpModestate == PVPType.Host)
            {
                //UIController.Instance.ShowShareCodePNL(true);
                //UIController.Instance.ShowLoading(false);
            }
        }
        PlayerStateMode = PlayerState.WaitingForOpponent;
        IsMultiPlayer = true;
        print($"Joined Room.... {PhotonNetwork.CurrentRoom.Name } {IsPrivateRoomCreater}  = Is multiplayer =  { IsMultiPlayer}");
        base.OnJoinedRoom();
    }


    /*public override void OnJoinRoomFailed(short returnCode,string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
    }*/

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.IsOpen == true)
        {
            //StartGameConditon();
            //StartGame();
            RaiseEventManager.instance.RaiseEVT(StaticData.PUBLIC_GAME_MODE, "StartGame");
            newPlayer.NickName = "prasahnt";
            Debug.Log(newPlayer.ToStringFull());
            Debug.Log($"Now game is started");
            Debug.Log(newPlayer.NickName);
        }

        //base.OnPlayerEnteredRoom(newPlayer);
    }



    public void StartGameCondition()
    {
        //PhotonNetwork.AutomaticallySyncScene = true;

        if (PhotonNetwork.CurrentRoom.IsOpen && PhotonNetwork.CurrentRoom.IsVisible)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }
        
        IsMultiPlayer = true;
        StaticData.MyGameFinish = false;
        StaticData.OtherGameFinish = false;
        StartCoroutine(LoadGameModeMulti());
    }


    IEnumerator LoadGameModeMulti()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
            Debug.Log("Scene Done");
        }
    }

   
}
