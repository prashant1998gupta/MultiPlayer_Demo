using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace prashantMultiPlayer
{
    public class RaiseEventManager : MonoBehaviourPunCallbacks, IOnEventCallback
    {


        public static RaiseEventManager instance = null;


        /*  public static RaiseEventManager Instance

          {

              get

              {
                  if (instance == null)

                  {

                      instance = FindObjectOfType<RaiseEventManager>();

                      if (instance == null)

                      {

                          GameObject go = new GameObject();
                          go.name = "RaiseEventManager";

                          instance = go.AddComponent<RaiseEventManager>();

                          DontDestroyOnLoad(go);

                      }

                  }


                  return instance;

              }

          }*/



        void Awake()

        {
            Debug.Log($"onAwake RasieEvent");

            if (instance == null)

            {
                instance = this;

                DontDestroyOnLoad(this.gameObject);

            }

            else
            {
                Destroy(gameObject);

            }
        }



        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
            Debug.Log("this is raiseEventManager OnEnable fnc");
        }
        // Start is called before the first frame update
        void Start()
        {

        }


        public void RaiseEVT(byte Code, string Data)
        {
            RaiseEventOptions receiverOPT = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(Code, Data, receiverOPT, SendOptions.SendReliable);
            Debug.Log($"OnEvent calling in Raise event manager {1}");
        }
        public void RaiseEVT(byte Code, bool state)
        {
            RaiseEventOptions receiverOPT = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
            PhotonNetwork.RaiseEvent(Code, state, receiverOPT, SendOptions.SendReliable);
            Debug.Log("OnEvent calling in Raise event manager 2");
        }
        public void RaiseEVT(byte Code, object Data, bool TF)
        {
            RaiseEventOptions receiverOPT = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
            PhotonNetwork.RaiseEvent(Code, Data, receiverOPT, SendOptions.SendReliable);
            Debug.Log("OnEvent calling in Raise event manager 3");
        }


        public void OnEvent(EventData photonEvent)
        {
            Debug.Log("OnEvent calling in Raise event manager 4");

            if (photonEvent.Code == StaticData.PUBLIC_GAME_MODE)
            {
                Debug.Log("OnEvent calling in Raise event manager 5");

                if (UIManager.uiManagerInstance.pvpModestate == PVPType.Host || UIManager.uiManagerInstance.pvpModestate == PVPType.Join)
                {
                    UIManager.uiManagerInstance.ClosePrivatePanals();
                }


                UIManager.uiManagerInstance.ShowContributionPanel(true);

                object[] data = new object[]
          {
                StaticData.MyProfileName ,
              //StaticData.MyProfileImageUrl
          };

                RaiseEventManager.instance.RaiseEVT(StaticData.PUBLIC_GAME_MODE_SEND_DATA, data, true);



                /*object[] data = new object[2];

                data[0] = StaticData.MyProfileName;
                data[1] = StaticData.MyProfileImageUrl;*/


                Invoke(nameof(DelayStartGame), 1f);
            }

            if (photonEvent.Code == StaticData.PUBLIC_GAME_MODE_SEND_DATA)
            {
                Debug.Log("OnEvent calling in Raise event manager 6");

                object[] ReceiveedData = (object[])photonEvent.CustomData;
                if (ReceiveedData != null)
                {
                    StaticData.OtherUserProfileName = ReceiveedData[0].ToString();
                    Debug.Log(StaticData.OtherUserProfileName + "this is otherusername");
                    //StaticData.OtherProfileImageUrl = ReceiveedData[1].ToString();
                }


            }

            if (photonEvent.Code == StaticData.REMATCH_REQUEST)
            {
                PhotonManager.PlayerStateMode = PlayerState.WaitingRematch;
                UIManager.uiManagerInstance.RematchTexter();
                UIManager.uiManagerInstance.RematchPop.SetActive(true);

            }

            if (photonEvent.Code == StaticData.LEAVE_ROOM_WHILE_WAITING_OPPONENT)
            {

            }

            if (photonEvent.Code == StaticData.ACCEPT_REMATCH)
            {

                UIManager.uiManagerInstance.ResultPannel.SetActive(false);
                UIManager.uiManagerInstance.RematchPop.SetActive(false);
                //UIController.Instance.ShowLoading(true);

                /*if (LoginManager.Instance.CheckSessionExpire())
                {
                    LoginManager.Instance.RunSessionChecker();

                }*/

                RaiseEVT(StaticData.PUBLIC_GAME_MODE, "StartGame");
            }

            if (photonEvent.Code == StaticData.REJECT_REMATCH)
            {
                UIManager.uiManagerInstance.ResultWarningBox.text = "Request Rejected";
                UIManager.uiManagerInstance.ResultRematch.interactable = false;
            }
        }
        public void DelayStartGame()
        {
            //^^^^Invoked
            PhotonManager.instance.StartGameCondition();
            Debug.Log("Game Started Condition");
        }

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }
    }
}