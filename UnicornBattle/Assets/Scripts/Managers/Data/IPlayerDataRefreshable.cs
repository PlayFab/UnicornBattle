using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnicornBattle.Managers
{
    ///  <summary>
    /// Indicates that this class has refreshable data
    /// </summary>
    interface IPlayerDataRefreshable
    {
        void Refresh(bool p_forceRefresh,
                     System.Action<string> p_onSuccessCallback = null, 
                     System.Action<string> p_onFailureCallback = null);
    }
}