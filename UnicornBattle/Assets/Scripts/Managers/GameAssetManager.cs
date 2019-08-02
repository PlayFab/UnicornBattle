using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

namespace UnicornBattle.Managers
{

    public class GameAssetManager : MonoBehaviour
    {
        public void Initialize( MainManager p_manager )
        {
            mainManager = p_manager;
        }

        protected MainManager mainManager { get; private set; }



    }
}
