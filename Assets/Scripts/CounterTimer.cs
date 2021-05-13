using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace prashantMultiPlayer
{
    public class CounterTimer : MonoBehaviour
    {

        public static CounterTimer instace;
        bool alert;
        public bool TimeRaiseEventStopper = true;


        [Header("CounterTimer  UI references :")]
        [SerializeField] private Text uiText;
        [SerializeField] private Image textBg;

        public int Duration { get; private set; }

        public bool IsPaused { get; private set; }

        private int remainingDuration;

        // Events --
        private UnityAction onTimerBeginAction;
        private UnityAction<int> onTimerChangeAction;
        private UnityAction onTimerEndAction;
        private UnityAction<bool> onTimerPauseAction;
        private bool isTimerEnd;



        private void Awake()
        {
            Debug.Log("CounterTimer  Awake is callling");

            if (instace == null)

            {
                instace = this;

                // DontDestroyOnLoad(this.gameObject);

            }

            else
            {
                Destroy(gameObject);

            }
        }
        // Use this for initialization
        void Start()
        {
            alert = true;
            isTimerEnd = true;
        }



        // Update is called once per frame
        void Update()
        {
            StaticData.MyStaticTiming = remainingDuration;
            if (StaticData.MyStaticTiming < 10 && alert)
            {
                StartCoroutine(FadeInFadeOut(true));
                alert = false;
            }

            if (StaticData.MyStaticTiming <= 0 && TimeRaiseEventStopper == true)
            {
                StaticData.MyGameFinish = true;
                if (StaticData.OtherGameFinish == false && PhotonManager.PlayerStateMode == PlayerState.Game)
                {
                    Debug.Log("My turn state=" + StaticData.MyGameFinish + "=other Game State=" + StaticData.OtherGameFinish);
                    PhotonManager.PlayerStateMode = PlayerState.GameEnd;
                    GameUIManager.instance.WaitingForOther.SetActive(true);
                }

                GameRasieEventManager.instance.TimeraseEvent();
                TimeRaiseEventStopper = false;
            }
        }

        private IEnumerator FadeInFadeOut(bool fadeAway)
        {
            // fade from opaque to transparent
            if (fadeAway)
            {
                // loop over 1 second backwards

                textBg.color = new Color(255, 0, 0, .4f);
                yield return new WaitForSeconds(1f);
                textBg.color = new Color(255, 0, 0, 1f);


            }


            if (StaticData.MyStaticTiming >= 0 && isTimerEnd)
            {
                Debug.Log(StaticData.MyStaticTiming + " thsis is timer ");
                StartCoroutine(FadeInFadeOut(true));

                if (StaticData.MyStaticTiming == 0)
                {
                    isTimerEnd = false;
                }
            }

        }





        private void OnEnable()
        {
            ResetTimer();
        }

        private void ResetTimer()
        {
            uiText.text = "00:00";
            Duration = remainingDuration = 0;

            onTimerBeginAction = null;
            onTimerChangeAction = null;
            onTimerEndAction = null;
            onTimerPauseAction = null;

            IsPaused = false;
        }

        public void SetPaused(bool paused)
        {
            IsPaused = paused;

            if (onTimerPauseAction != null)
                onTimerPauseAction.Invoke(IsPaused);
        }


        public CounterTimer SetDuration(int seconds)
        {
            Duration = remainingDuration = seconds;
            return this;
        }



        //-- Events ----------------------------------
        public CounterTimer OnBegin(UnityAction action)
        {
            onTimerBeginAction = action;
            return this;
        }

        public CounterTimer OnChange(UnityAction<int> action)
        {
            onTimerChangeAction = action;
            return this;
        }

        public CounterTimer OnEnd(UnityAction action)
        {
            onTimerEndAction = action;
            return this;
        }

        public CounterTimer OnPause(UnityAction<bool> action)
        {
            onTimerPauseAction = action;
            return this;
        }





        public void Begin()
        {
            if (onTimerBeginAction != null)
                onTimerBeginAction.Invoke();

            StopCoroutine("UpdateTimer");
            StartCoroutine(UpdateTimer());
        }


        private IEnumerator UpdateTimer()
        {
            while (remainingDuration > 0)
            {
                if (!IsPaused)
                {
                    if (onTimerChangeAction != null)
                        onTimerChangeAction.Invoke(remainingDuration);

                    UpdateUI(remainingDuration);
                    remainingDuration--;
                }
                yield return new WaitForSeconds(1f);
            }
            End();
        }

        private void UpdateUI(int seconds)
        {
            uiText.text = string.Format("{0:D2}:{1:D2}", seconds / 60, seconds % 60);
        }

        public void End()
        {
            if (onTimerEndAction != null)
                onTimerEndAction.Invoke();
            ResetTimer();
        }


        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }

}