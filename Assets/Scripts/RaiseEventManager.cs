using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiseEventManager : MonoBehaviourPunCallbacks, IOnEventCallback
{

    public static RaiseEventManager instance;

    private void Awake()
    {
      
        if(instance == null)
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
        Debug.Log("OnEvent calling in Raise event manager");
    }
    public void RaiseEVT(byte Code, bool state)
    {
        RaiseEventOptions receiverOPT = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(Code, state, receiverOPT, SendOptions.SendReliable);
        Debug.Log("OnEvent calling in Raise event manager");
    }
    public void RaiseEVT(byte Code, object Data, bool TF)
    {
        RaiseEventOptions receiverOPT = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(Code, Data, receiverOPT, SendOptions.SendReliable);
        Debug.Log("OnEvent calling in Raise event manager");
    }


    public void OnEvent(EventData photonEvent)
    {
        Debug.Log("OnEvent calling in Raise event manager");

        if (photonEvent.Code == StaticData.PUBLIC_GAME_MODE)
        {
            Debug.Log("OnEvent calling in Raise event manager");

            /*object[] data = new object[]
            {
                StaticData.MyProfileName ,
                StaticData.MyProfileImageUrl
            };*/

            object[] data = new object[2];

            data[0] = StaticData.MyProfileName;
            data[1] = StaticData.MyProfileImageUrl;
            
            RaiseEVT(StaticData.PUBLIC_GAME_MODE, data, true);
            Invoke(nameof(DelayStartGame), 0.35f);
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
