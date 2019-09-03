using System.Collections.Generic;
using PlayFab;
using UnityEngine;

namespace UnicornBattle.Managers
{
    /// <summary>
    /// Abstract DataManager class that all data manager classes inherit from
    /// </summary>
    public abstract class DataManager : MonoBehaviour
    {
        public virtual void Initialize(MainManager p_manager)
        {
            m_mainManager = p_manager;
            m_LastRefreshTime = new System.DateTime();
            m_isInitialized = true;
        }

        /// <summary>
        /// Gets the Main Manager ref
        /// </summary>
        /// <returns>MainManager</returns>
        protected MainManager mainManager
        {
            get
            {
                if (null == m_mainManager)
                {
                    m_mainManager = MainManager.Instance;
                }
                return m_mainManager;
            }
        }

        protected bool IsInitialized { get { return m_isInitialized; } }

        protected bool IsDataCleanAndFresh
        {
            get
            {
                System.TimeSpan l_span = (System.DateTime.Now - m_LastRefreshTime);
                return (m_localCacheIsDirty == false) && (l_span.TotalSeconds < k_STALE_DATA_TIME);
            }
        }

        protected void DataRefreshed()
        {
            m_localCacheIsDirty = false;
            m_LastRefreshTime = System.DateTime.Now;
        }

        protected void FlagAsDirty()
        {
            m_localCacheIsDirty = true;
        }

        protected void ExtractJsonTitleData<T>(Dictionary<string, string> resultData, string titleKey, ref T output)
        {
            var JsonUtil = PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer);

            string json;
            if (!resultData.TryGetValue(titleKey, out json))
                Debug.LogError("Failed to load titleData: " + titleKey);
            try
            {
                output = JsonUtil.DeserializeObject<T>(resultData[titleKey]);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to load titleData: " + titleKey);
                Debug.LogException(e);
            }
        }

        private MainManager m_mainManager;
        private System.DateTime m_LastRefreshTime;
        private readonly double k_STALE_DATA_TIME = 600;
        private bool m_isInitialized = false;
        private bool m_localCacheIsDirty = false;
    }

}