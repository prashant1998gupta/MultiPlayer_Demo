using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace mmocircles
{

    public class AvataarChange : MonoBehaviour
    {
        public static AvataarChange instance;
        public GameObject CurrentImage;
        public GameObject[] AllObject;
        string[] RandomName = new string[] { "Tourname ", "All star", "Denial", "Evin" , "김승환", "kbetw", "이동수", "CHICBOSS" };
        public bool IsActive;

        // Start is called before the first frame update
        void OnEnable()
        {
            instance = this;
            IsActive = false;
            CurrentImage.SetActive(true);
            StartCoroutine(Change());
        }

        IEnumerator Change()
        {
            yield return new WaitForSeconds(0.1f);
            CurrentImage.SetActive(false);
            
            CurrentImage = AllObject[Random.Range(0, AllObject.Length)];
            string RandName = RandomName[Random.Range(0, RandomName.Length)];
            UIManager.uiManagerInstance.waitingLoadingOtherPlayerName.text = RandName;
            CurrentImage.SetActive(true);
            if (!IsActive)
            {
                if(this.gameObject.name == "MaskSearch")
                {
                    StartCoroutine(Change());
                }
                
            }
            if (IsActive)
            {

              
                //MainOponentImage.sprite = StaticData.OponentGlobalSprite;
                //UIController.Instance.ContributionOponentProfileName.text = StaticData.OtherUserProfileName; 
            }
         
        }

    }
}
