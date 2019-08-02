using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnicornBattle.Controllers;
using UnicornBattle.Managers.Auth;
using UnityEngine;

namespace UnicornBattle.Managers
{
    public class MainManager : Singleton<MainManager>
        {
            [SerializeField] private AuthenticationManager m_authManager;
            [SerializeField] private PlayerManager m_playerManager;
            [SerializeField] private GameDataManager m_gameDataManager;
            [SerializeField] private CharacterManager m_characterManager;
            [SerializeField] private InventoryManager m_inventoryManager;
            [SerializeField] private FriendsManager m_friendsManager;
            [SerializeField] private IconManager m_iconManager;
            [SerializeField] private LeaderboardManager m_leaderboardManager;
            [SerializeField] private PromotionsManager m_PromotionsManager;
            [SerializeField] private StoreManager m_StoreManager;
            //[SerializeField] private GameAssetManager m_gameAssetManager;

            private MainManager() { }

            public override void Awake()
            {
                DontDestroyOnLoad(this.gameObject);
                base.Awake(); // singleton Awake()

                m_iconManager = GameController.Instance.iconManager;

                if (null == m_authManager)
                {
#if UNITY_ANDROID || UNITY_IOS
                    m_authManager = new FacebookAuthManager();
#else
                    m_authManager = new PlayFabAuthManager();
#endif
                }

                // title Data Managers
                m_TitleDataServices = new List<ITitleDataLoadable>();

                if (null == m_gameDataManager)
                    m_gameDataManager = GetComponent<GameDataManager>();
                if (null != m_gameDataManager)
                {
                    m_gameDataManager.Initialize(this);
                    m_TitleDataServices.Add(m_gameDataManager);
                }

                // player data managers
                m_PlayerDataServices = new List<IPlayerDataRefreshable>();

                if (null == m_playerManager)
                    m_playerManager = GetComponent<PlayerManager>();
                if (null != m_playerManager)
                {
                    m_playerManager.Initialize(this);
                    m_PlayerDataServices.Add(m_playerManager);
                }

                if (null == m_characterManager)
                    m_characterManager = GetComponent<CharacterManager>();
                if (null != m_characterManager)
                {
                    m_characterManager.Initialize(this);
                    m_PlayerDataServices.Add(m_characterManager);
                    m_TitleDataServices.Add(m_characterManager);
                }
                if (null != m_inventoryManager)
                    m_inventoryManager = GetComponent<InventoryManager>();
                if (null != m_inventoryManager)
                {
                    m_inventoryManager.Initialize(this);
                    m_PlayerDataServices.Add(m_inventoryManager);
                }

                if (null == m_friendsManager)
                    m_friendsManager = GetComponent<FriendsManager>();
                if (null != m_friendsManager)
                    m_friendsManager.Initialize(this);
                if (null == m_leaderboardManager)
                    m_leaderboardManager = GetComponent<LeaderboardManager>();
                if (null != m_leaderboardManager)
                    m_leaderboardManager.Initialize(this);

                if (null == m_PromotionsManager)
                    m_PromotionsManager = GetComponent<PromotionsManager>();
                if (null != m_PromotionsManager)
                {
                    m_PromotionsManager.Initialize(this);
                    m_PlayerDataServices.Add(m_PromotionsManager);
                }
                if (null == m_StoreManager)
                    m_StoreManager = GetComponent<StoreManager>();
                if (null != m_StoreManager)
                {
                    m_StoreManager.Initialize(this);
                    m_TitleDataServices.Add(m_StoreManager);
                }

                // if (null == m_gameAssetManager)
                //     m_gameAssetManager = GetComponent<GameAssetManager>();
                // if (null != m_gameAssetManager)
                //     m_gameAssetManager.Initialize(this);
            }

            public void RefreshTitleData(System.Action p_onSuccessCallback,
                System.Action<string> p_onFailureCallback)
            {
                // get title
                var request = new GetTitleDataRequest { Keys = GlobalStrings.InitTitleKeys };

                PlayFabClientAPI.GetTitleData(request,
                    (GetTitleDataResult result) =>
                    {

                        LoadTitleData(result.Data);

                        if (null != p_onSuccessCallback)
                            p_onSuccessCallback.Invoke();
                    },
                    (PlayFabError e) =>
                    {
                        if (null != p_onFailureCallback)
                            p_onFailureCallback.Invoke(e.ErrorMessage);
                    }
                );
            }

            public void LoadTitleData(Dictionary<string, string> p_titleData)
            {
                // pass title data to all handlers
                foreach (ITitleDataLoadable l_handler in m_TitleDataServices)
                {
                    l_handler.LoadTitleData(p_titleData);
                }
            }

            public void RefreshAll(bool p_forceRefresh,
                System.Action<string> p_onSuccessCallback,
                System.Action<string> p_onFailureCallback)
            {
                refreshAll_implementation(0, p_forceRefresh, p_onSuccessCallback, p_onFailureCallback);
            }

            private void refreshAll_implementation(int p_index,
                bool p_forceRefresh,
                System.Action<string> p_onSuccessCallback,
                System.Action<string> p_onFailureCallback)
            {
                if (p_index >= m_PlayerDataServices.Count)
                {
                    if (null != p_onSuccessCallback)
                        p_onSuccessCallback.Invoke(" ");
                    return;
                }

                IPlayerDataRefreshable l_mgr = m_PlayerDataServices[p_index];

                l_mgr.Refresh(p_forceRefresh,
                    (string success) =>
                    {
                        refreshAll_implementation(++p_index, p_forceRefresh, p_onSuccessCallback, p_onFailureCallback);
                    },
                    p_onFailureCallback
                );
            }

            public AuthenticationManager getAuthManager() { return m_authManager; }
            public PlayerManager getPlayerManager() { return m_playerManager; }
            public GameDataManager getGameDataManager() { return m_gameDataManager; }
            public CharacterManager getCharacterManager() { return m_characterManager; }
            public InventoryManager getInventoryManager() { return m_inventoryManager; }
            public FriendsManager getFriendsManager() { return m_friendsManager; }
            public IconManager getIconManager() { return m_iconManager; }
            public LeaderboardManager getLeaderboardManager() { return m_leaderboardManager; }
            public PromotionsManager getPromotionsManager() { return m_PromotionsManager; }
            public StoreManager getStoreManager() { return m_StoreManager; }
            //public GameAssetManager getGameAssetManager() { return m_gameAssetManager; }

            private List<ITitleDataLoadable> m_TitleDataServices;
            private List<IPlayerDataRefreshable> m_PlayerDataServices;
        }

}