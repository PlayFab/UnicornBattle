using System.Collections.Generic;
using System.Linq;
using PlayFab;
using PlayFab.ClientModels;
using UnicornBattle.Controllers;
using UnicornBattle.Models;
using UnityEngine;

namespace UnicornBattle.Managers
{
    /// <summary>
    /// INVENTORY MANAGER
    /// =================
    /// Encapsulates all the Inventory data for a player or character, and controls how the data is accessed
    ///  - in this game, inventory is associated with a player.
    //   - this is a separate class because not all games need an inventory.
    /// </summary>
    public class InventoryManager : DataManager, IPlayerDataRefreshable
    {
        /// <summary>
        /// Initialize the manager
        /// </summary>
        /// <param name="p_manager">ref to the main manager</param>
        public override void Initialize(MainManager p_manager)
        {
            m_inventoryByCategory = new Dictionary<string, UBInventoryCategory>();
            m_playerInventory = new List<UBInventoryItem>();
            m_virtualCurrency = new Dictionary<string, int>();
            m_catalogItems = new Dictionary<string, UBCatalogItem>();

            base.Initialize(p_manager);
        }

        /// <summary>
        /// Refresh the local cache with data from the PlayFab server
        /// - This method will only pull down from the server if the data is out of date OR if the data is dirty
        /// - To force the server to get new data, set p_forceRefresh to true
        /// </summary>
        /// <param name="p_forceRefresh">Should we force the call from the server?</param>
        /// <param name="p_onSuccessCallback">Called if successfully refreshed</param>
        /// <param name="p_onFailureCallback">Called if an error occurred</param>
        public void Refresh(bool p_forceRefresh,
            System.Action<string> p_onSuccessCallback,
            System.Action<string> p_onFailureCallback)
        {
            if (IsInitialized == false)
            {
                Initialize(MainManager.Instance);
            }
            if (p_forceRefresh == false)
            {
                if (IsDataCleanAndFresh)
                {
                    if (null != p_onSuccessCallback)
                        p_onSuccessCallback.Invoke("Data Fresh");
                    return;
                }
            }

            PlayFabClientAPI.GetCatalogItems(
                new GetCatalogItemsRequest { CatalogVersion = GlobalStrings.PrimaryCatalogName },
                (GetCatalogItemsResult result1) =>
                {
                    m_catalogItems.Clear();
                    foreach (var eachItem in result1.Catalog)
                        m_catalogItems[eachItem.ItemId] = new UBCatalogItem(eachItem);

                    PlayFabClientAPI.GetUserInventory(
                        new GetUserInventoryRequest(),
                        (GetUserInventoryResult result2) =>
                        {
                            m_virtualCurrency.Clear();
                            foreach (var pair in result2.VirtualCurrency)
                                m_virtualCurrency.Add(pair.Key, pair.Value);

                            m_playerInventory.Clear();
                            foreach (var eachItem in result2.Inventory)
                                m_playerInventory.Add(new UBInventoryItem(eachItem));

                            populateInventoryByCategory();
                            DataRefreshed();

                            if (null != p_onSuccessCallback)
                                p_onSuccessCallback.Invoke(string.Empty);

                        },
                        (PlayFabError e2) =>
                        {
                            if (null != p_onFailureCallback)
                                p_onFailureCallback.Invoke(e2.ErrorMessage);
                        }
                    );
                },
                (PlayFabError e1) =>
                {
                    if (null != p_onFailureCallback)
                        p_onFailureCallback.Invoke(e1.ErrorMessage);
                }
            );

        }

        /// <summary>
        /// Retrieves the quest items.
        /// </summary>
        /// <param name="p_onSuccessCallback">Called if successful</param>
        /// <param name="p_onFailureCallback">Called if an error occurs</param>
        public void RetrieveQuestItems(
            List<string> itemsFound,
            System.Action<List<UBQuestRewardItem>> p_onSuccessCallback = null,
            System.Action<string> p_onFailureCallback = null)
        {

            var l_request = new ExecuteCloudScriptRequest
            {
            FunctionName = "RetrieveQuestItems",
            FunctionParameter = new { ItemIds = itemsFound },
            };

            PlayFabClientAPI.ExecuteCloudScript(
                l_request,
                (ExecuteCloudScriptResult result) =>
                {
                    if (null != result.Error)
                    {
                        if (null != p_onFailureCallback)
                            p_onFailureCallback.Invoke(result.Error.Message);
                        return;
                    }

                    var JsonUtil = PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer);
                    List<UBQuestRewardItem> ItemsAwarded = JsonUtil.DeserializeObject<List<UBQuestRewardItem>>(result.FunctionResult.ToString());

                    FlagAsDirty();
                    if (null != p_onSuccessCallback)
                        p_onSuccessCallback.Invoke(ItemsAwarded);
                },
                (PlayFabError e) =>
                {
                    if (null != p_onFailureCallback)
                        p_onFailureCallback.Invoke(e.ErrorMessage);
                }
            );
        }

        /// <summary>
        /// Get a item by ID
        /// </summary>
        /// <param name="p_itemId">ID</param>
        /// <returns>UBInventoryItem</returns>
        public UBInventoryItem GetItem(string p_itemId)
        {
            var l_item = m_playerInventory.Where((item) => { return item.ItemId == p_itemId; }).FirstOrDefault();
            return l_item;
        }

        /// <summary>
        /// Get an inventory category by type
        /// </summary>
        /// <param name="p_itemType"></param>
        /// <returns>UBInventoryCategory</returns>
        public UBInventoryCategory GetItemCategory(string p_itemType)
        {
            if (m_inventoryByCategory.ContainsKey(p_itemType))
            {
                return m_inventoryByCategory[p_itemType];
            }
            else return null;
        }

        ///  <summary>
        /// Get all inventory categories as a dictionary
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, UBInventoryCategory> GetAllCategories()
        {
            return m_inventoryByCategory;
        }

        /// <summary>
        /// Return number of RemainingUses of an stack of itemIds in your inventory
        /// </summary>
        /// <returns>
        /// -1 => Item does not exist in the inventory
        /// 0 => The item has infinite uses
        /// else, the number of remaining uses
        /// </returns>
        public int CountItemsByID(string p_itemId)
        {
            var output = 0;
            foreach (var eachItem in m_playerInventory)
            {
                if (eachItem.ItemId != p_itemId)
                    continue;
                if (eachItem.RemainingUses == null)
                    return -1; // Unlimited uses
                if (eachItem.RemainingUses.Value > 0) // Non-Positive is probably a PlayFab api error
                    output += eachItem.RemainingUses.Value;
            }
            return output;
        }

        /// <summary>
        /// Count all items in a category
        /// </summary>
        /// <param name="p_itemType"></param>
        /// <returns></returns>
        public int CountItemsInCategory(string p_itemType)
        {
            if (m_inventoryByCategory.ContainsKey(p_itemType))
            {
                var category = m_inventoryByCategory[p_itemType];
                return category.count;
            }
            else return 0;
        }

        /// <summary>
        /// Get the amount of a Virtual Currency in the local cache.  Call Refresh() prior to this method to update the local cache.
        /// </summary>
        /// <param name="p_currencyName">The currency name</param>
        /// <returns>The amount of currency; or -1 if that currency does not exist</returns>
        public int GetCurrencyAmount(string p_currencyName)
        {
            if (null != m_virtualCurrency && m_virtualCurrency.ContainsKey(p_currencyName))
                return m_virtualCurrency[p_currencyName];
            else
                return -1;
        }

        ///  <summary>
        /// Consume an item
        /// </summary>
        /// <param name="p_itemId">item id on the server</param>
        /// <param name="p_onSuccessCallback">Called if successful</param>
        /// <param name="p_onFailureCallback">Called if an error occurs</param>
        public void ConsumeItem(string p_itemId,
            System.Action<string> p_onSuccessCallback = null,
            System.Action<string> p_onFailureCallback = null)
        {
            var request = new ConsumeItemRequest
            {
            ConsumeCount = 1,
            ItemInstanceId = p_itemId
            };
            PlayFabClientAPI.ConsumeItem(
                request,
                (ConsumeItemResult result) =>
                {
                    FlagAsDirty();
                    if (null != p_onSuccessCallback)
                        p_onSuccessCallback.Invoke(string.Empty);
                },
                (PlayFabError e) =>
                {
                    if (null != p_onFailureCallback)
                        p_onFailureCallback.Invoke(e.ErrorMessage);
                }
            );
        }

        /// <summary>
        /// Is this Item a Container?
        /// </summary>
        /// <param name="itemId">Item ID</param>
        /// <returns>true, if a container; false otherwise</returns>
        public bool IsContainer(string itemId)
        {
            UBCatalogItem catalogItem;
            // Check for a container
            if (!m_catalogItems.TryGetValue(itemId, out catalogItem) || catalogItem.Container == null)
                return false;

            // Check for empty container
            bool items = catalogItem.Container.ItemContents == null || catalogItem.Container.ItemContents.Count == 0;
            bool vcs = catalogItem.Container.VirtualCurrencyContents == null || catalogItem.Container.VirtualCurrencyContents.Count == 0;
            bool drops = catalogItem.Container.ResultTableContents == null || catalogItem.Container.ResultTableContents.Count == 0;
            return !items || !vcs || !drops;
        }

        /// <summary>
        /// Try to open a container
        /// </summary>
        /// <param name="containerId"></param>
        /// <param name="p_onSuccessCallback"></param>
        /// <param name="p_onFailureCallback"></param>
        public void TryOpenContainer(string containerId,
            System.Action<UnlockContainerItemResult> p_onSuccessCallback = null,
            System.Action<string> p_onFailureCallback = null)
        {
            var request = new UnlockContainerItemRequest { ContainerItemId = containerId };

            PlayFabClientAPI.UnlockContainerItem(request,
                (UnlockContainerItemResult result) =>
                {
                    FlagAsDirty();
                    if (p_onSuccessCallback != null)
                        p_onSuccessCallback.Invoke(result);
                },
                (PlayFabError e) =>
                {
                    if (null != p_onFailureCallback)
                        p_onFailureCallback.Invoke(e.ErrorMessage);
                }
            );
        }

        /// <summary>
        /// Get a Catalog Item by ID
        /// </summary>
        /// <param name="id">Id of the item</param>
        /// <returns>UBCatalogItem</returns>
        public UBCatalogItem GetCatalogItemById(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;
            UBCatalogItem output;
            m_catalogItems.TryGetValue(id, out output);
            return output;
        }

        /// <summary>
        /// Get an Icon ID for an Item
        /// </summary>
        /// <param name="catalogItemId">ID of the item</param>
        /// <param name="iconDefault">Default value if the item is not found</param>
        /// <returns>The Icon ID</returns>
        public string GetIconByItemById(string catalogItemId, string iconDefault = "Default")
        {
            var JsonUtil = PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer);

            var catalogItem = GetCatalogItemById(catalogItemId);
            if (catalogItem == null)
                return null;
            var iconName = iconDefault;
            try
            {
                string temp;
                var kvps = JsonUtil.DeserializeObject<Dictionary<string, string>>(catalogItem.CustomData);
                if (kvps != null && kvps.TryGetValue("icon", out temp))
                    iconName = temp;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
            return iconName;
        }

        internal void LoadInventory(List<ItemInstance> p_inventory)
        {
            m_playerInventory.Clear();
            foreach (var eachItem in p_inventory)
                m_playerInventory.Add(new UBInventoryItem(eachItem));
        }

        internal void LoadVirtualCurrency(Dictionary<string, int> p_currency)
        {
            m_virtualCurrency.Clear();
            foreach (var pair in p_currency)
                m_virtualCurrency.Add(pair.Key, pair.Value);
        }

        /// <summary>
        /// populates the InventoryByCategory list
        /// </summary>
        private void populateInventoryByCategory()
        {
            if (null == m_playerInventory || m_playerInventory.Count == 0)
                return; // nothing to populate

            if (null == m_catalogItems || m_catalogItems.Count == 0)
                return; // again if nothing in this list, then abort

            m_inventoryByCategory.Clear();
            foreach (var item in m_playerInventory)
            {
                if (m_inventoryByCategory.ContainsKey(item.ItemId))
                    continue; // duplicates?

                var catalogItem = GetCatalogItemById(item.ItemId);
                if (catalogItem == null)
                    continue;

                var items = m_playerInventory.FindAll(x => { return x.ItemId.Equals(item.ItemId); });
                var customIcon = GetIconByItemById(catalogItem.ItemId);
                var icon = GameController.Instance.iconManager.GetIconById(customIcon, IconManager.IconTypes.Item);
                m_inventoryByCategory.Add(item.ItemId, new UBInventoryCategory(item.ItemId, catalogItem, items, icon));
            }
        }

        private Dictionary<string, UBInventoryCategory> m_inventoryByCategory;
        private List<UBInventoryItem> m_playerInventory;
        private Dictionary<string, int> m_virtualCurrency;
        private Dictionary<string, UBCatalogItem> m_catalogItems;
    }
}