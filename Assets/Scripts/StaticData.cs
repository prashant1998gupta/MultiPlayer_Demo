using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace prashantMultiPlayer
{
    public class StaticData : MonoBehaviour
    {
        public static StaticData Instance;



        public static string ROOMNAME = "UserName";


        public static string USERNAME = "UserName";
        public static string USERID = "UserID";
        public static string SCORE = "Score";
        public static string RESULT = "Result";


        public static string ClientId = "163";
        public static string ServerId = "164";

        // Room Amounts and codes
        public static float Room_Amount;
        public static float Room_Name;
        public static float Free_Room_Amount = 1;
        public static double UserCurrentSGC;
        public static string PrivateRoomRandomCode;


        //User Profile Data
        public static string MyProfileName;
        public static string MyProfileImageUrl;
        public static string OtherProfileImageUrl;
        public static string OtherUserProfileName;

        //Scoring In Multiplayer
        public static int My_Current_Score = 0;
        public static int Other_Current_Score = 0;
        public static int MyStaticTiming = 30;
        public static bool MyGameFinish = false;
        public static bool OtherGameFinish = false;


        public const byte SCORE_SHARING = 101;
        public const byte CHACK_GAME_END = 102;
        public const byte WHO_Master_DECIDER = 103;
        public const byte WIN_OTHER_DECIDER = 104;
        public const byte REMATCH_REQUEST = 105;
        public const byte ACCEPT_REMATCH = 106;
        public const byte REJECT_REMATCH = 107;
        public const byte LEAVE_ROOM_WHILE_WAITING_OPPONENT = 110;
        public const byte PUBLIC_GAME_MODE_SEND_DATA = 108;
        public const byte PUBLIC_GAME_MODE = 109;


        public Sprite DefaultUserSprite;
        public Color HighlightedColor;
        public Sprite guestUser;
        public static Sprite MyGlobalSprite { get; set; }
        public static Sprite OponentGlobalSprite { get; set; }
        public static string MyDisplayName;
        private static int random;

        private void Awake()
        {


            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        private void Start()
        {
            MyGlobalSprite = DefaultUserSprite;
            OponentGlobalSprite = guestUser;
            if (PlayerPrefs.HasKey("MyProfileName"))
            {
                MyProfileName = PlayerPrefs.GetString("MyProfileName");
            }
            else
            {

                PlayerPrefs.SetString("MyProfileName", MyName());
                MyProfileName = PlayerPrefs.GetString("MyProfileName");
            }


        }


        public string MyName()
        {
            random = Random.Range(1000, 5000);
            return $"Gaust" + random;
        }
    }
}