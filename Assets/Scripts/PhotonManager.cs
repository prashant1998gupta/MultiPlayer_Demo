using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;


namespace prashantMultiPlayer
{
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

        internal Dictionary<string, Hashtable> cachedRoomList = new Dictionary<string, Hashtable>();

        internal TypedLobby privateLobby = new TypedLobby("privateLobby", LobbyType.Default);
        internal TypedLobby publicLobby = new TypedLobby("publicLobby", LobbyType.Default);

        internal RoomOptions RoomInfo = new RoomOptions();
        internal OpJoinRandomRoomParams ExpectedRandomRoomProperties = new OpJoinRandomRoomParams();

        public delegate void PhotonConnect(bool Success);
        public static PhotonConnect OnPhotonServerConnected;

        public delegate void JoinedLobbyCall(bool Success);
        public static JoinedLobbyCall OnJoinedLobbyCall;

        public delegate void OnPriateRoomListUpdate(bool Success);
        public static OnPriateRoomListUpdate RoomListUpdated;

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

            RoomListUpdated += AfterRoomListUpdate;
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

        public override void OnLeftLobby()
        {
            Debug.Log("Leaved Lobby,.....");
            base.OnLeftLobby();
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

                string randomRoomName = GenerateRandomRoomName() + Random.Range(10000, 100000).ToString();
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

        public void CreatePrivateRoom(float EntryFee)
        {
            Debug.Log("Connected or not " + PhotonNetwork.IsConnected);

            IsPrivateRoomCreater = true;
            IsPrivateRoom = true;
            string randomRoomName = UnityEngine.Random.Range(10001, 99999).ToString();
            StaticData.PrivateRoomRandomCode = randomRoomName;
            RoomInfo = new RoomOptions();
            RoomInfo.MaxPlayers = 2;
            RoomInfo.EmptyRoomTtl = 0;
            RoomInfo.CleanupCacheOnLeave = true;
            RoomInfo.CustomRoomPropertiesForLobby = new string[] { StaticData.ROOMNAME };
            RoomInfo.CustomRoomProperties = new Hashtable { { StaticData.ROOMNAME, "" } };
            RoomInfo.IsVisible = true;
            RoomInfo.IsOpen = true;
            RoomInfo.CustomRoomProperties[StaticData.ROOMNAME] = "";// ProfileManager.Instance.profileData.username;

            PhotonNetwork.JoinOrCreateRoom(randomRoomName, RoomInfo, privateLobby, null);
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
                    UIManager.uiManagerInstance.ShowShareCodePNL(true);
                    //UIController.Instance.ShowLoading(false);
                }
            }
            PlayerStateMode = PlayerState.WaitingForOpponent;
            IsMultiPlayer = true;
            print($"Joined Room.... {PhotonNetwork.CurrentRoom.Name } {IsPrivateRoomCreater}  = Is multiplayer =  { IsMultiPlayer}");
            base.OnJoinedRoom();
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            if (UIManager.uiManagerInstance.pvpModestate == PVPType.Join)
            {
                print(message);
                LeaveLobbyCond();
                UIManager.uiManagerInstance.EnterJoinCode.text = "";
                UIManager.uiManagerInstance.infoText.text = "Game Does not exist";
            }
            base.OnJoinRoomFailed(returnCode, message);
        }


        public void JoinerRoomPropoerties()
        {
            IsPrivateRoomJoin = true;
            RoomInfo = new RoomOptions();
            RoomInfo.MaxPlayers = 2;
            RoomInfo.EmptyRoomTtl = 0;
            RoomInfo.CleanupCacheOnLeave = true;
            RoomInfo.CustomRoomPropertiesForLobby = new string[] { StaticData.ROOMNAME };
            RoomInfo.CustomRoomProperties = new Hashtable { { StaticData.ROOMNAME, "" } };
            RoomInfo.IsVisible = false;

        }
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Debug.Log(IsPrivateRoom);
            if (IsPrivateRoom)
            {


                foreach (var s in roomList)
                {

                    if (s.RemovedFromList)
                    {
                        cachedRoomList.Remove(s.Name);
                    }
                    else
                    {


                        if (cachedRoomList.ContainsKey(s.Name))
                        {
                            cachedRoomList[s.Name] = s.CustomProperties;
                        }
                        else
                        {
                            cachedRoomList.Add(s.Name, s.CustomProperties);
                        }

                    }
                }
                foreach (var a in cachedRoomList)
                {
                    Debug.Log("Room information" + a.Key + ":" + a.Value);
                }

                RoomListUpdated(true);
            }
            base.OnRoomListUpdate(roomList);
        }

        void AfterRoomListUpdate(bool state)
        {
            if (state)
            {
                JoinerRoomPropoerties();
                IsPrivateRoom = true;
                JoinCustomRoom(UIManager.uiManagerInstance.EnterJoinCode.text);

            }
        }

        public void JoinCustomRoom(string roomName)
        {
            PhotonNetwork.JoinRoom(roomName);
        }


        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.IsOpen == true)
            {

                RaiseEventManager.instance.RaiseEVT(StaticData.PUBLIC_GAME_MODE, "StartGame");

                Debug.Log(newPlayer.ToStringFull());
            }

            base.OnPlayerEnteredRoom(newPlayer);
        }


        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            Debug.Log("Room Propoerties Update" + propertiesThatChanged);
            base.OnRoomPropertiesUpdate(propertiesThatChanged);
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



        public void LeaveGameCond()
        {

            Debug.Log(PlayerStateMode);
            Debug.Log("Connected or not" + PhotonNetwork.IsConnected);
            PlayerStateMode = PlayerState.GameEnd;
            UIManager.uiManagerInstance.ShowResultMenu(false, false);

            UIManager.uiManagerInstance.ResultWarningBox.text = "You are Disconnected";
            UIManager.uiManagerInstance.ResultRematch.interactable = false;




            PhotonNetwork.Disconnect();
            SceneManager.LoadScene(0);
        }
        public void LeaveLobbyCond()
        {
            Debug.Log("Connected or not" + PhotonNetwork.IsConnected);
            PhotonNetwork.AutomaticallySyncScene = false;
            PhotonNetwork.Disconnect();
        }


        public override void OnPlayerLeftRoom(Player otherPlayer)
        {

            Debug.Log("Current State=" + PlayerStateMode + "=Left Player=" + otherPlayer);
            Debug.Log("Onplayer left");

            if (PlayerStateMode == PlayerState.GameEnd)
            {
                Debug.Log($"game is end show this function is called {PlayerStateMode}");
                UIManager.uiManagerInstance.ResultRematch.interactable = false;
                UIManager.uiManagerInstance.ResultWarningBox.text = "Other Player Is Disconnected";
            }
            else if (PlayerStateMode == PlayerState.WaitingRematch)
            {
                UIManager.uiManagerInstance.ResultRematch.interactable = false;
                UIManager.uiManagerInstance.RematchPop.SetActive(false);
                UIManager.uiManagerInstance.ResultWarningBox.text = "other player leave the room";
                Debug.Log($"player state ========== {PlayerStateMode}");
            }
            else
            {
                GameUIManager.instance.OtherPlayerLeft.SetActive(true);
                Invoke(nameof(ActivateWinPannel), 3f);
            }

            // if (PlayerStateMode == PlayerState.WaitingRematch)
            // {
            //     Debug.Log($"game is end show this function is called {PlayerStateMode}");
            //     UIManager.uiManagerInstance.ShowResultMenu(false, true);
            //     UIManager.uiManagerInstance.ResultRematch.interactable = false;
            //     UIManager.uiManagerInstance.ResultWarningBox.text = "Other Player Is Disconnected";
            // }

            if (PlayerStateMode == PlayerState.WaitingForOpponent)
            {
                Debug.Log("Waiting for playerr #player left");
                if (UIManager.uiManagerInstance.pvpModestate == PVPType.Host || UIManager.uiManagerInstance.pvpModestate == PVPType.Join)
                {

                    //UIManager.uiManagerInstance.PlayerLeftPrivatePOP.SetActive(true);
                }

                //LeaveLobbyCond();
                //PhotonNetwork.LoadLevel(1);
            }
            base.OnPlayerLeftRoom(otherPlayer);
        }

        void ActivateWinPannel()
        {

            //Invoked in this script
            GameUIManager.instance.WaitingForOther.SetActive(false);
            UIManager.uiManagerInstance.ShowResultMenu(true, false);

            UIManager.uiManagerInstance.ResultRematch.interactable = false;
            UIManager.uiManagerInstance.ResultWarningBox.text = "Other Player disconnect";
        }
    }
}