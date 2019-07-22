using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayFab.Internal;
using PlayFab.AdminModels;
using PlayFab;
using System.IO;
using PlayFab.AuthenticationModels;
using System.Net;

namespace UnicornBattleInstaller
{
    public class TitleMigrater
    {
        public delegate void ReportLogtoFile(string info);
        public static event ReportLogtoFile OnLogTofile;

        //ITransportPlugin transport = PluginManager.GetPlugin<ITransportPlugin>(PluginContract.PlayFab_Transport);
        static ISerializerPlugin json = PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer);

        // shared variables
        private static string defaultCatalog = null; // Determined by TitleSettings.json
        private static bool hitErrors;

        // data file locations
        private const string currencyPath = "./PlayFabData/Currency.json";
        private const string titleSettingsPath = "./PlayFabData/TitleSettings.json";
        private const string titleDataPath = "./PlayFabData/TitleData.json";
        private const string catalogPath = "./PlayFabData/Catalog.json";
        private const string catalogPathEvents = "./PlayFabData/CatalogEvents.json";
        private const string dropTablesPath = "./PlayFabData/DropTables.json";
        private const string cloudScriptPath = "./PlayFabData/CloudScript.js";
        private const string titleNewsPath = "./PlayFabData/TitleNews.json";
        private const string statsDefPath = "./PlayFabData/StatisticsDefinitions.json";
        private const string storesPath = "./PlayFabData/Stores.json";
        private const string storesPathEvents = "./PlayFabData/StoresEvents.json";
        private const string cdnAssetsPath = "./PlayFabData/CdnData.json";
        public const string permissionPath = "./PlayFabData/Permissions.json";

        // authentication tokens
        private static string authToken;

        // log file details
        private static FileInfo logFile;
        private static StreamWriter logStream;

        // CDN
        public enum CdnPlatform { Desktop, iOS, Android }
        public static readonly Dictionary<CdnPlatform, string> cdnPlatformSubfolder = new Dictionary<CdnPlatform, string> {
            { CdnPlatform.Desktop, "" },
            { CdnPlatform.iOS, "iOS/" },
            { CdnPlatform.Android, "Android/" },
        };
        public static string cdnPath = "./PlayFabData/AssetBundles/";

        public bool GetTitleSettings(int index)
        {
            var form = FormManager.Get<InstallForm>("InstallForm");
            var title = form.AvailableTitles[index - 1];

            PlayFabSettings.staticSettings.TitleId = title.Id;
            PlayFabSettings.staticSettings.DeveloperSecretKey = title.SecretKey;
            defaultCatalog = "CharacterClasses";
            return true;

            /*
            var parsedFile = ParseFile(titleSettingsPath);

            var titleSettings = json.DeserializeObject<Dictionary<string, string>>(parsedFile);

            if (titleSettings != null &&
                titleSettings.TryGetValue("TitleId", out PlayFabSettings.staticSettings.TitleId) && !string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId) &&
                titleSettings.TryGetValue("DeveloperSecretKey", out PlayFabSettings.staticSettings.DeveloperSecretKey) && !string.IsNullOrEmpty(PlayFabSettings.staticSettings.DeveloperSecretKey) &&
                titleSettings.TryGetValue("CatalogName", out defaultCatalog))
            {
                LogToFile("Setting Destination TitleId to: " + PlayFabSettings.staticSettings.TitleId);
                LogToFile("Setting DeveloperSecretKey to: " + PlayFabSettings.staticSettings.DeveloperSecretKey);
                LogToFile("Setting defaultCatalog name to: " + defaultCatalog);
                return true;
            }

            LogToFile("An error occurred when trying to parse TitleSettings.json", ConsoleColor.Red);
            return false;
            */
        }

        #region Uploading Functions -- these are straightforward calls that push the data to the backend
        public async Task<bool> UploadEconomyData()
        {
            ////MUST upload these in this order so that the economy data is properly imported: VC -> Catalogs -> DropTables -> Catalogs part 2 -> Stores
            if (!await UploadVc())
                return false;

            if (string.IsNullOrEmpty(catalogPath))
                return false;

            LogToFile("Uploading CatalogItems...");

            // now go through the catalog; split into two parts. 
            var reUploadList = new List<CatalogItem>();
            var parsedFile = ParseFile(catalogPath);

            var catalogWrapper = json.DeserializeObject<CatalogWrapper>(parsedFile);
            if (catalogWrapper == null)
            {
                LogToFile("An error occurred deserializing the Catalog.json file.", ConsoleColor.Red);
                return false;
            }
            for (var z = 0; z < catalogWrapper.Catalog.Count; z++)
            {
                if (catalogWrapper.Catalog[z].Bundle != null || catalogWrapper.Catalog[z].Container != null)
                {
                    var original = catalogWrapper.Catalog[z];
                    var strippedClone = CloneCatalogItemAndStripTables(original);

                    reUploadList.Add(original);
                    catalogWrapper.Catalog.Remove(original);
                    catalogWrapper.Catalog.Add(strippedClone);
                }
            }

            if (! await UpdateCatalog(catalogWrapper.Catalog, defaultCatalog, true))
                return false;

            if (! await UploadDropTables())
                return false;

            if (! await UploadStores(storesPath, defaultCatalog))
                return false;

            // workaround for the DropTable conflict
            if (reUploadList.Count > 0)
            {
                LogToFile("Re-uploading [" + reUploadList.Count + "] CatalogItems due to DropTable conflicts...");
                if (! await UpdateCatalog(reUploadList, defaultCatalog, true))
                    return false;
            }
            return true;
        }

        public async Task<bool> UploadEventData()
        {
            if (string.IsNullOrEmpty(catalogPathEvents))
                return false;

            LogToFile("Uploading Event Items...");
            var parsedFile = ParseFile(catalogPathEvents);
            var catalogWrapper = json.DeserializeObject<CatalogWrapper>(parsedFile);
            if (catalogWrapper == null)
            {
                LogToFile("An error occurred deserializing the CatalogEvents.json file.", ConsoleColor.Red);
                return false;
            }

            if (! await UpdateCatalog(catalogWrapper.Catalog, "Events", false))
                return false;

            LogToFile("Uploaded Event Catalog!", ConsoleColor.Green);

            if (! await UploadStores(storesPathEvents, "Events"))
                return false;

            return true;
        }

        public async Task<bool> UploadStatisticDefinitions()
        {
            if (string.IsNullOrEmpty(statsDefPath))
                return false;

            LogToFile("Updating Player Statistics Definitions ...");
            var parsedFile = ParseFile(statsDefPath);

            var statisticDefinitions = json.DeserializeObject<List<PlayerStatisticDefinition>>(parsedFile);

            foreach (var item in statisticDefinitions)
            {
                LogToFile("Uploading: " + item.StatisticName);

                var request = new CreatePlayerStatisticDefinitionRequest()
                {
                    StatisticName = item.StatisticName,
                    VersionChangeInterval = item.VersionChangeInterval,
                    AggregationMethod = item.AggregationMethod
                };

                var createStatTask = await PlayFabAdminAPI.CreatePlayerStatisticDefinitionAsync(request);

                if (createStatTask.Error != null)
                {
                    if (createStatTask.Error.Error == PlayFabErrorCode.StatisticNameConflict)
                    {
                        LogToFile("Statistic Already Exists, Updating values: " + item.StatisticName, ConsoleColor.DarkYellow);
                        var updateRequest = new UpdatePlayerStatisticDefinitionRequest()
                        {
                            StatisticName = item.StatisticName,
                            VersionChangeInterval = item.VersionChangeInterval,
                            AggregationMethod = item.AggregationMethod
                        };

                        var updateStatTask = await PlayFabAdminAPI.UpdatePlayerStatisticDefinitionAsync(updateRequest);
                        
                        if (updateStatTask.Error != null)
                            OutputPlayFabError("Statistics Definition Error: " + item.StatisticName, updateStatTask.Error);
                        else
                            LogToFile("Statistics Definition:" + item.StatisticName + " Updated", ConsoleColor.Green);
                    }
                    else
                    {
                        OutputPlayFabError("Statistics Definition Error: " + item.StatisticName, createStatTask.Error);
                    }
                }
                else
                {
                    LogToFile("Statistics Definition: " + item.StatisticName + " Created", ConsoleColor.Green);
                }
            }
            return true;
        }

        public async Task<bool> UploadTitleNews()
        {
            if (string.IsNullOrEmpty(titleNewsPath))
                return false;

            LogToFile("Uploading TitleNews...");
            var parsedFile = ParseFile(titleNewsPath);

            var titleNewsItems = json.DeserializeObject<List<PlayFab.ServerModels.TitleNewsItem>>(parsedFile);

            foreach (var item in titleNewsItems)
            {
                LogToFile("Uploading: " + item.Title);

                var request = new AddNewsRequest()
                {
                    Title = item.Title,
                    Body = item.Body
                };

                var addNewsTask = await PlayFabAdminAPI.AddNewsAsync(request);

                if (addNewsTask.Error != null)
                    OutputPlayFabError("TitleNews Upload: " + item.Title, addNewsTask.Error);
                else
                    LogToFile("" + item.Title + " Uploaded.", ConsoleColor.Green);
            }

            return true;
        }

        // retrieves and stores an auth token
        // returns false if it fails
        public async Task<bool> GetAuthToken()
        {
            var entityTokenRequest = new GetEntityTokenRequest();
            var authTask = await PlayFabAuthenticationAPI.GetEntityTokenAsync(entityTokenRequest);

            if (authTask.Error != null)
            {
                OutputPlayFabError("Error retrieving auth token: ", authTask.Error);
                return false;
            }
            else
            {
                authToken = authTask.Result.EntityToken;
                LogToFile("Auth token retrieved.", ConsoleColor.Green);
            }
            return true;
        }

        public async Task<bool> UploadCloudScript()
        {
            if (string.IsNullOrEmpty(cloudScriptPath))
                return false;

            LogToFile("Uploading CloudScript...");
            var parsedFile = ParseFile(cloudScriptPath);

            if (parsedFile == null)
            {
                LogToFile("An error occurred deserializing the CloudScript.js file.", ConsoleColor.Red);
                return false;
            }
            var files = new List<CloudScriptFile> {
                new CloudScriptFile
                {
                    Filename = "CloudScript.js",
                    FileContents = parsedFile
                }
            };

            var request = new UpdateCloudScriptRequest()
            {
                Publish = true,
                Files = files
            };

            var updateCloudScriptTask = await PlayFabAdminAPI.UpdateCloudScriptAsync(request);

            if (updateCloudScriptTask.Error != null)
            {
                OutputPlayFabError("CloudScript Upload Error: ", updateCloudScriptTask.Error);
                return false;
            }

            LogToFile("Uploaded CloudScript!", ConsoleColor.Green);
            return true;
        }

        public async Task<bool> UploadTitleData()
        {
            if (string.IsNullOrEmpty(titleDataPath))
                return false;

            LogToFile("Uploading Title Data Keys & Values...");
            var parsedFile = ParseFile(titleDataPath);
            var titleDataDict = json.DeserializeObject<Dictionary<string, string>>(parsedFile);

            foreach (var kvp in titleDataDict)
            {
                LogToFile("Uploading: " + kvp.Key);

                var request = new SetTitleDataRequest()
                {
                    Key = kvp.Key,
                    Value = kvp.Value
                };

                var setTitleDataTask = await PlayFabAdminAPI.SetTitleDataAsync(request);

                if (setTitleDataTask.Error != null)
                    OutputPlayFabError("TitleData Upload: " + kvp.Key, setTitleDataTask.Error);
                else
                    LogToFile("" + kvp.Key + " Uploaded.", ConsoleColor.Green);
            }

            return true;
        }

        public async Task<bool> UploadVc()
        {
            LogToFile("Uploading Virtual Currency Settings...");
            var parsedFile = ParseFile(currencyPath);
            var vcData = json.DeserializeObject<List<VirtualCurrencyData>>(parsedFile);
            var request = new AddVirtualCurrencyTypesRequest
            {
                VirtualCurrencies = vcData
            };

            var updateVcTask = await PlayFabAdminAPI.AddVirtualCurrencyTypesAsync(request);

            if (updateVcTask.Error != null)
            {
                OutputPlayFabError("VC Upload Error: ", updateVcTask.Error);
                return false;
            }

            LogToFile("Uploaded VC!", ConsoleColor.Green);
            return true;
        }

        public async Task<bool> UploadDropTables()
        {
            if (string.IsNullOrEmpty(dropTablesPath))
                return false;

            LogToFile("Uploading DropTables...");
            var parsedFile = ParseFile(dropTablesPath);

            var dtDict = json.DeserializeObject<Dictionary<string, RandomResultTableListing>>(parsedFile);
            if (dtDict == null)
            {
                LogToFile("An error occurred deserializing the DropTables.json file.", ConsoleColor.Red);
                return false;
            }

            var dropTables = new List<RandomResultTable>();
            foreach (var kvp in dtDict)
            {
                dropTables.Add(new RandomResultTable()
                {
                    TableId = kvp.Value.TableId,
                    Nodes = kvp.Value.Nodes
                });
            }

            var request = new UpdateRandomResultTablesRequest()
            {
                CatalogVersion = defaultCatalog,
                Tables = dropTables
            };

            var updateResultTableTask = await PlayFabAdminAPI.UpdateRandomResultTablesAsync(request);

            if (updateResultTableTask.Error != null)
            {
                OutputPlayFabError("DropTable Upload Error: ", updateResultTableTask.Error);
                return false;
            }

            LogToFile("Uploaded DropTables!", ConsoleColor.Green);
            return true;
        }

        public async Task<bool> UploadStores(string filePath, string catalogName)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            LogToFile("Uploading Stores...");
            var parsedFile = ParseFile(filePath);

            var storesList = json.DeserializeObject<List<StoreWrapper>>(parsedFile);

            foreach (var eachStore in storesList)
            {
                LogToFile("Uploading: " + eachStore.StoreId);

                var request = new UpdateStoreItemsRequest
                {
                    CatalogVersion = catalogName,
                    StoreId = eachStore.StoreId,
                    Store = eachStore.Store,
                    MarketingData = eachStore.MarketingData
                };

                var updateStoresTask = await PlayFabAdminAPI.SetStoreItemsAsync(request);

                if (updateStoresTask.Error != null)
                    OutputPlayFabError("Store Upload: " + eachStore.StoreId, updateStoresTask.Error);
                else
                    LogToFile("Store: " + eachStore.StoreId + " Uploaded. ", ConsoleColor.Green);
            }
            return true;
        }

        public async Task<bool> UploadPolicy(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            LogToFile("Uploading Policy...");
            var parsedFile = ParseFile(filePath);

            var permissionList = json.DeserializeObject<List<PlayFab.ProfilesModels.EntityPermissionStatement>>(parsedFile);
            var request = new PlayFab.ProfilesModels.SetGlobalPolicyRequest
            {
                Permissions = permissionList
            };

            var setPermissionTask = await PlayFab.PlayFabProfilesAPI.SetGlobalPolicyAsync(request);
            
            if (setPermissionTask.Error != null)
                OutputPlayFabError("Set Permissions: ", setPermissionTask.Error);
            else
                LogToFile("Permissions uploaded... ", ConsoleColor.Green);

            return true;
        }

        public async Task<bool> UploadCdnAssets()
        {
            var tdParsedFile = ParseFile(titleDataPath);
            var titleDataDict = json.DeserializeObject<Dictionary<string, string>>(tdParsedFile);

            if (string.IsNullOrEmpty(cdnAssetsPath))
                return false;

            LogToFile("Uploading CDN AssetBundles...");
            var parsedFile = ParseFile(cdnAssetsPath);
            var bundleNames = json.DeserializeObject<List<string>>(parsedFile); // TODO: This could probably just read the list of files from the directory

            if (bundleNames != null)
            {
                foreach (var bundleName in bundleNames)
                {
                    foreach (CdnPlatform eachPlatform in Enum.GetValues(typeof(CdnPlatform)))
                    {
                        var key = cdnPlatformSubfolder[eachPlatform] + bundleName;
                        var path = cdnPath + key;
                        await UploadAsset(key, path);
                    }
                }
            }
            else
            {
                LogToFile("An error occurred deserializing CDN Assets: ", ConsoleColor.Red);
                return false;
            }
            return true;
        }
        #endregion

        #region Helper Functions -- these functions help the main uploading functions
        static void LogToFile(string content, ConsoleColor color = ConsoleColor.White)
        {
            /*
            Console.ForegroundColor = color;
            Console.WriteLine(content);
            logStream.WriteLine(content);

            Console.ForegroundColor = ConsoleColor.White;
            */
            OnLogTofile?.Invoke(content);
        }

        static void OutputPlayFabError(string context, PlayFabError err)
        {
            hitErrors = true;
            LogToFile("An error occurred during: " + context, ConsoleColor.Red);

            var details = string.Empty;
            if (err.ErrorDetails != null)
            {
                foreach (var kvp in err.ErrorDetails)
                {
                    details += (kvp.Key + ": ");
                    foreach (var eachIssue in kvp.Value)
                        details += (eachIssue + ", ");
                    details += "\n";
                }
            }

            LogToFile(string.Format("[{0}] -- {1}: {2} ", err.Error, err.ErrorMessage, details), ConsoleColor.Red);
        }

        static string ParseFile(string path)
        {
            var s = File.OpenText(path);
            var contents = s.ReadToEnd();
            s.Close();
            return contents;
        }

        static CatalogItem CloneCatalogItemAndStripTables(CatalogItem strip)
        {
            if (strip == null)
                return null;

            return new CatalogItem
            {
                ItemId = strip.ItemId,
                ItemClass = strip.ItemClass,
                CatalogVersion = strip.CatalogVersion,
                DisplayName = strip.DisplayName,
                Description = strip.Description,
                VirtualCurrencyPrices = strip.VirtualCurrencyPrices,
                RealCurrencyPrices = strip.RealCurrencyPrices,
                Tags = strip.Tags,
                CustomData = strip.CustomData,
                Consumable = strip.Consumable,
                Container = null,//strip.Container, // Clearing this is the point
                Bundle = null,//strip.Bundle, // Clearing this is the point
                CanBecomeCharacter = strip.CanBecomeCharacter,
                IsStackable = strip.CanBecomeCharacter,
                IsTradable = strip.IsTradable,
                ItemImageUrl = strip.ItemImageUrl
            };
        }

        public async Task<bool> UpdateCatalog(List<CatalogItem> catalog, string catalogName, bool isDefault)
        {
            var request = new UpdateCatalogItemsRequest
            {
                CatalogVersion = catalogName,
                Catalog = catalog,
                SetAsDefaultCatalog = isDefault
            };

            var updateCatalogItemsTask = await PlayFabAdminAPI.UpdateCatalogItemsAsync(request);

            if (updateCatalogItemsTask.Error != null)
            {
                OutputPlayFabError("Catalog Upload Error: ", updateCatalogItemsTask.Error);
                return false;
            }

            LogToFile("Uploaded Catalog!", ConsoleColor.Green);
            return true;
        }

        public async Task<bool> UploadAsset(string key, string path)
        {
            var request = new GetContentUploadUrlRequest()
            {
                Key = key,
                ContentType = "application/x-gzip"
            };

            LogToFile("Fetching CDN endpoint for " + key);
            var getContentUploadTask = await PlayFabAdminAPI.GetContentUploadUrlAsync(request);

            if (getContentUploadTask.Error != null)
            {
                OutputPlayFabError("Acquire CDN URL Error: ", getContentUploadTask.Error);
                return false;
            }

            var destUrl = getContentUploadTask.Result.URL;
            LogToFile("Acquired CDN Address: " + key, ConsoleColor.Green);

            byte[] fileContents = File.ReadAllBytes(path);

            return PutFile(key, destUrl, fileContents);
        }

        private static bool PutFile(string key, string url, byte[] payload)
        {
            LogToFile("Starting HTTP PUT for: " + key);

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "PUT";
            request.ContentType = "application/x-gzip";

            if (payload != null)
            {
                var dataStream = request.GetRequestStream();
                dataStream.Write(payload, 0, payload.Length);
                dataStream.Close();
            }
            else
            {
                LogToFile("ERROR: Byte array was empty or null", ConsoleColor.Red);
                return false;
            }

            var response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                LogToFile("HTTP PUT Successful:" + key, ConsoleColor.Green);
                return true;
            }
            else
            {
                LogToFile(string.Format("ERROR: Asset:{0} -- Code:[{1}] -- Msg:{2}", url, response.StatusCode, response.StatusDescription), ConsoleColor.Red);
                return false;
            }
        }
        #endregion

    }


    public class CatalogWrapper
    {
        public string CatalogVersion;
        public List<CatalogItem> Catalog;
    }

    public class StoreWrapper
    {
        public string StoreId;
        public List<StoreItem> Store;
        public StoreMarketingModel MarketingData;
    }
}
