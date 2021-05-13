using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace prashantMultiPlayer
{
    public class GameRasieEventManager : MonoBehaviourPunCallbacks, IOnEventCallback, ILobbyCallbacks, IInRoomCallbacks
    {

        public static GameRasieEventManager instance;
        bool DeciderBool = true;
        bool TimeDataSent = true;
        private void Awake()
        {
            Debug.Log("GameRaisEventManager Awake");
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
            DeciderBool = true;
            Debug.Log("Decider bool " + DeciderBool);
            PhotonNetwork.AddCallbackTarget(this);
            GameUIManager.ScoreIncrement += ChangeInScore;

            StaticData.My_Current_Score = 0;
            StaticData.Other_Current_Score = 0;
            GameUIManager.instance.myScore.text = StaticData.My_Current_Score.ToString();
            GameUIManager.instance.oppScore.text = StaticData.Other_Current_Score.ToString();
            if (PhotonManager.instance.IsMultiPlayer)
            {
                UIManager.uiManagerInstance.ShowContributionPanel(false);
                PhotonManager.PlayerStateMode = PlayerState.Game;
                GameUIManager.instance.myName.text = StaticData.MyProfileName;
                Debug.Log("Other User name" + StaticData.OtherUserProfileName);
                GameUIManager.instance.oppName.text = StaticData.OtherUserProfileName;

            }
        }

        private void OnDisable()
        {
            GameUIManager.ScoreIncrement += ChangeInScore;
        }

        private void ChangeInScore(bool status)
        {

            if (status == true)
            {
                if (StaticData.MyStaticTiming >= 0)
                {
                    Debug.Log("Data Updated");
                    float Myscore = GameUIManager.instance.sum;
                    GameUIManager.instance.myScore.text = Myscore.ToString();
                    StaticData.My_Current_Score = GameUIManager.instance.sum;
                    object[] SendData = new object[] { Myscore };
                    RaiseEVT(StaticData.SCORE_SHARING, SendData, true);
                }
            }
        }



        public void RaiseEVT(byte Code, object[] Data, bool State)
        {
            RaiseEventOptions EventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
            PhotonNetwork.RaiseEvent(Code, Data, EventOptions, SendOptions.SendReliable);
        }
        public void RaiseEVT(byte Code, string Data)
        {
            RaiseEventOptions EventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(Code, Data, EventOptions, SendOptions.SendReliable);
        }
        public void RaiseEVT(byte Code, bool Data)
        {
            RaiseEventOptions EventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
            PhotonNetwork.RaiseEvent(Code, Data, EventOptions, SendOptions.SendReliable);
        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == StaticData.SCORE_SHARING)
            {
                object[] ReceivedData = (object[])photonEvent.CustomData;
                GameUIManager.instance.oppScore.text = ReceivedData[0].ToString();
                StaticData.Other_Current_Score = int.Parse(ReceivedData[0].ToString());
            }
            else if (photonEvent.Code == StaticData.CHACK_GAME_END)
            {
                Debug.Log("other game finied ");
                bool ReceivedData = (bool)photonEvent.CustomData;
                StaticData.OtherGameFinish = (bool)ReceivedData;
                CheckGameFinish();

            }
            // Marster Decider the result and send raise event to other to show result
            else if (photonEvent.Code == StaticData.WHO_Master_DECIDER)
            {
                DecideWinLoose();
            }
            // afer receive raise event other plyer will call LocalWinDecider() to show result;
            else if (photonEvent.Code == StaticData.WIN_OTHER_DECIDER)
            {
                object[] ReceivedData = (object[])photonEvent.CustomData;
                int Val = (int)ReceivedData[0];
                LocalWinDecider(Val);
            }

        }


        public void TimeraseEvent()
        {
            TimeDataSent = false;
            bool state = true;
            RaiseEVT(StaticData.CHACK_GAME_END, state);
        }



        public void CheckGameFinish()
        {
            Debug.Log("My game finish state" + StaticData.MyGameFinish + "Other Game Finish State" + StaticData.OtherGameFinish);
            if (StaticData.MyGameFinish == true && StaticData.OtherGameFinish == true)
            {
                PhotonManager.PlayerStateMode = PlayerState.GameEnd;
                Debug.Log("My game finish state" + StaticData.MyGameFinish + "Other Game Finish State" + StaticData.OtherGameFinish);
                RaiseEVT(StaticData.WHO_Master_DECIDER, "true");
            }
        }


        void DecideWinLoose()
        {
            PhotonNetwork.AutomaticallySyncScene = false;

            if (PhotonNetwork.IsMasterClient && DeciderBool)
            {
                if (StaticData.My_Current_Score > StaticData.Other_Current_Score)
                {
                    GameUIManager.instance.WaitingForOther.SetActive(false);
                    Debug.Log($"myScore {StaticData.My_Current_Score} ohterScoer {StaticData.Other_Current_Score} that means i am winner");
                    if (UIManager.uiManagerInstance.pvpModestate == PVPType.PublicGame)
                    {
                        // UserDataManager.Instance.AddPvpWinDiamond();
                    }
                    UIManager.uiManagerInstance.ShowResultMenu(true, false);

                    PhotonManager.PlayerStateMode = PlayerState.GameEnd;
                    int youloose = 0;
                    object[] Sent_Data = new object[] { youloose };
                    RaiseEVT(StaticData.WIN_OTHER_DECIDER, Sent_Data, true);
                }
                else if (StaticData.My_Current_Score < StaticData.Other_Current_Score)
                {
                    Debug.Log($"myScore {StaticData.My_Current_Score} ohterScoer {StaticData.Other_Current_Score} that means other win winner");
                    GameUIManager.instance.WaitingForOther.SetActive(false);

                    if (UIManager.uiManagerInstance.pvpModestate == PVPType.PublicGame)
                    {

                    }
                    UIManager.uiManagerInstance.ShowResultMenu(false, false);

                    PhotonManager.PlayerStateMode = PlayerState.GameEnd;

                    int youwin = 1;
                    object[] Sent_Data = new object[] { youwin };
                    RaiseEVT(StaticData.WIN_OTHER_DECIDER, Sent_Data, true);
                }
                else if (StaticData.My_Current_Score == StaticData.Other_Current_Score)
                {
                    Debug.Log($"myScore {StaticData.My_Current_Score} ohterScoer {StaticData.Other_Current_Score} that means match draw winner");
                    GameUIManager.instance.WaitingForOther.SetActive(false);
                    if (UIManager.uiManagerInstance.pvpModestate == PVPType.PublicGame)
                    {

                    }
                    UIManager.uiManagerInstance.ShowResultMenu(false, true);

                    PhotonManager.PlayerStateMode = PlayerState.GameEnd;


                    int draw = 2;
                    object[] Sent_Data = new object[] { draw };
                    RaiseEVT(StaticData.WIN_OTHER_DECIDER, Sent_Data, true);
                }
                DeciderBool = false;
            }

        }


        void LocalWinDecider(int Data)
        {
            PhotonManager.PlayerStateMode = PlayerState.GameEnd;
            if (Data == 1)
            {
                GameUIManager.instance.WaitingForOther.SetActive(false);
                if (UIManager.uiManagerInstance.pvpModestate == PVPType.PublicGame)
                {
                }
                Debug.Log("you win with LocalWinDecider");
                UIManager.uiManagerInstance.ShowResultMenu(true, false);
            }
            if (Data == 0)
            {
                GameUIManager.instance.WaitingForOther.SetActive(false);
                if (UIManager.uiManagerInstance.pvpModestate == PVPType.PublicGame)
                {
                }
                Debug.Log("you loose LocalWinDecider");
                UIManager.uiManagerInstance.ShowResultMenu(false, false);
            }
            if (Data == 2)
            {
                GameUIManager.instance.WaitingForOther.SetActive(false);
                if (UIManager.uiManagerInstance.pvpModestate == PVPType.PublicGame)
                {
                }
                Debug.Log("draww mathcLocalWinDecider");
                UIManager.uiManagerInstance.ShowResultMenu(false, true);
            }
        }


    }
}