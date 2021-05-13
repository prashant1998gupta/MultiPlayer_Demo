using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace prashantMultiPlayer
{
    public class DontDestroyOnLoad : MonoBehaviour
    {

        public static DontDestroyOnLoad instance;
        private void Awake()
        {
            Debug.Log("GameUiManager Awake calling");
            if (instance == null)
            {

                instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }
            DontDestroyOnLoad(this.gameObject);
        }

        private void Start()
        {

        }
    }
}