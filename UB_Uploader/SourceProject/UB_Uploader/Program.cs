using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using PlayFab;
using PlayFab.AdminModels;
using Newtonsoft.Json;
using PlayFab.ServerModels;
using CatalogItem = PlayFab.AdminModels.CatalogItem;
using SetTitleDataRequest = PlayFab.AdminModels.SetTitleDataRequest;

namespace UB_Uploader
{
    class Program
    {
        // shared variables
        const string defaultCatalog = "CharacterClasses";
        static bool hitErrors;
        
        // data file locations
        const string titleSettingsPath = "./PlayFabData/TitleSettings.json";
        const string titleDataPath = "./PlayFabData/TitleData.json";
        const string catalogPath = "./PlayFabData/Catalog.json";
        const string dropTablesPath = "./PlayFabData/DropTables.json";
        const string cloudScriptPath = "./PlayFabData/CloudScript.js";
        const string titleNewsPath = "./PlayFabData/TitleNews.json";
        const string statsDefPath = "./PlayFabData/StatisticsDefinitions.json";
        const string storesPath = "./PlayFabData/Stores.json";
        const string cdnAssetsPath = "./PlayFabData/CdnData.json";

        // log file details
        private static FileInfo logFile;
        private static StreamWriter logStream;

       /// <summary>
        /// This app parses the textfiles(defined above) and uploads the contents into a PlayFab title (defined in titleSettingsPath);
       /// </summary>
       /// <param name="args"></param>
       static void Main(string[] args)
        {
            try
            {
                // setup the log file
                logFile = new FileInfo("PreviousUploadLog.txt");
                logStream = logFile.CreateText();

                // get the destination title settings
                if (!GetTitleSettings())
                    throw new Exception("\tFailed to load Title Settings");

                // start uploading
                if (!UploadTitleData())
                    throw new Exception("\tFailed to upload TitleData.");
                
                if (!UploadEconomyData())
                    throw new Exception("\tFailed to upload Economy Data.");

                if (!UploadCloudScript())
                    throw new Exception("\tFailed to upload CloudScript.");

                if (!UploadTitleNews())
                    throw new Exception("\tFailed to upload TitleNews.");

                if (!UploadStatisticDefinitions())
                    throw new Exception("\tFailed to upload Statistics Definitions.");

                if (!UploadCdnAssets())
                    throw new Exception("\tFailed to upload CDN Assets.");

            }
            catch (Exception ex)
            {
                hitErrors = true;
                LogToFile("\tAn unexpected error occurred: " + ex.Message, ConsoleColor.Red);
            }
            finally
            {
                var status = hitErrors ? "ended with errors. See PreviousUploadLog.txt for details" : "ended successfully!";
                var color = hitErrors ? ConsoleColor.Red : ConsoleColor.White;

                LogToFile(string.Format("UB_Uploader.exe {0}", status), color);
                logStream.Close();
                Console.WriteLine("Press return to exit.");
                Console.ReadLine();
            }
        }

       #region Uploading Functions -- these are straightforward calls that push the data to the backend
           static bool UploadEconomyData()
            {
                ////MUST upload these in this order so that the economy data is properly imported: VC -> Catalogs -> DropTables -> Stores
                if (!UploadVC())
                    return false;

                List<CatalogItem> reUploadList = new List<CatalogItem>();
                if(!UploadCatalog(ref reUploadList))
                    return false;

                if (!UploadDropTables())
                    return false;

                if (!UploadStores())
                    return false;

               // workaround for the DropTable conflict
                if (reUploadList.Count > 0)
                {
                    LogToFile("Re-uploading [" + reUploadList.Count + "] CatalogItems due to DropTable conflicts...");
                    UpdateCatalog(reUploadList);
                }
                return true;
            }

           static bool UploadStatisticDefinitions()
           {
                if (string.IsNullOrEmpty(statsDefPath))
                    return false;

                LogToFile("Updating Player Statistics Definitions ...");
                string parsedFile = ParseFile(statsDefPath);

                var statisticDefinitions = JsonConvert.DeserializeObject<List<PlayerStatisticDefinition>>(parsedFile);

               foreach (var item in statisticDefinitions)
               {
                   LogToFile(string.Format("\tUploading: {0}", item.StatisticName));

                   CreatePlayerStatisticDefinitionRequest request = new CreatePlayerStatisticDefinitionRequest()
                   {
                       StatisticName = item.StatisticName,
                       VersionChangeInterval = item.VersionChangeInterval,
                       AggregationMethod = item.AggregationMethod
                   };

                   var createStatTask = PlayFabAdminAPI.CreatePlayerStatisticDefinitionAsync(request);
                   createStatTask.Wait();

                   if (createStatTask.Result.Error != null)
                   {
                       if (createStatTask.Result.Error.Error == PlayFabErrorCode.StatisticNameConflict)
                       {
                           LogToFile(string.Format("\tStatistic Already Exists, Updating values: {0}", item.StatisticName), ConsoleColor.DarkYellow);
                           UpdatePlayerStatisticDefinitionRequest updateRequest = new UpdatePlayerStatisticDefinitionRequest()
                           {
                               StatisticName = item.StatisticName,
                               VersionChangeInterval = item.VersionChangeInterval,
                               AggregationMethod = item.AggregationMethod
                           };

                           var updateStatTask = PlayFabAdminAPI.UpdatePlayerStatisticDefinitionAsync(updateRequest);
                           updateStatTask.Wait();
                           if (updateStatTask.Result.Error != null)
                           {
                               OutputPlayFabError("\t\tStatistics Definition Error: " + item.StatisticName, updateStatTask.Result.Error);
                           }
                           else
                           {
                               LogToFile(string.Format("\t\tStatistics Definition: {0} Updated ", item.StatisticName), ConsoleColor.Green);
                           }
                       }
                       else
                       {
                           OutputPlayFabError("\t\tStatistics Definition Error: " + item.StatisticName, createStatTask.Result.Error);   
                       }
                  
                   }
                   else
                   {
                       LogToFile(string.Format("\t\tStatistics Definition: {0} Created ", item.StatisticName), ConsoleColor.Green);
                   }
               }
               return true;
           }

           static bool UploadTitleNews()
            {
                if (string.IsNullOrEmpty(titleNewsPath))
                    return false;

                LogToFile("Uploading TitleNews...");
                string parsedFile = ParseFile(titleNewsPath);

                var titleNewsItems = JsonConvert.DeserializeObject<List<TitleNewsItem>>(parsedFile);

                foreach (var item in titleNewsItems)
                {
                    LogToFile(string.Format("\tUploading: {0}", item.Title));

                    AddNewsRequest request = new AddNewsRequest()
                    {
                        Title = item.Title,
                        Body = item.Body
                    };

                    var addNewsTask = PlayFabAdminAPI.AddNewsAsync(request);
                    addNewsTask.Wait();

                    if (addNewsTask.Result.Error != null)
                    {
                        OutputPlayFabError("\t\tTitleNews Upload: " + item.Title, addNewsTask.Result.Error);
                    }
                    else
                    {
                        LogToFile(string.Format("\t\t{0} Uploaded.", item.Title), ConsoleColor.Green);
                    }
                }

                return true;
            }

           static bool UploadCloudScript()
            {
                if (string.IsNullOrEmpty(cloudScriptPath))
                    return false;

                LogToFile("Uploading CloudScript...");
                string parsedFile = ParseFile(cloudScriptPath);

                if (parsedFile != null)
                {
                    List<CloudScriptFile> files = new List<CloudScriptFile>();
                    files.Add(new CloudScriptFile()
                    {
                       Filename = "CloudScript.js",
                       FileContents = parsedFile
                    });


                    UpdateCloudScriptRequest request = new UpdateCloudScriptRequest()
                    {
                        Publish = true,
                        Files = files
                    };

                    var updateCloudScriptTask = PlayFabAdminAPI.UpdateCloudScriptAsync(request);
                    updateCloudScriptTask.Wait();

                    if (updateCloudScriptTask.Result.Error != null)
                    {
                        OutputPlayFabError("\tCloudScript Upload Error: ", updateCloudScriptTask.Result.Error);
                        return false;
                    }
                    else
                    {
                        LogToFile("\tUploaded CloudScript!", ConsoleColor.Green);
                        return true;
                    }
                }
                else
                {
                    LogToFile("\tAn error occurred deserializing the CloudScript.js file.", ConsoleColor.Red);
                    return false;
                }
                return true;
            }

           static bool GetTitleSettings()
            {
                string parsedFile = ParseFile(titleSettingsPath);

                var titleSettings = JsonConvert.DeserializeObject<Dictionary<string, string>>(parsedFile);

                if (titleSettings != null &&
                    titleSettings.TryGetValue("TitleId", out PlayFabSettings.TitleId) &&
                    titleSettings.TryGetValue("DeveloperSecretKey", out PlayFabSettings.DeveloperSecretKey))
                {
                    LogToFile(string.Format("Setting Destination TitleId to:{0}", PlayFabSettings.TitleId));
                    LogToFile(string.Format("Setting DeveloperSecretKey to:{0}", PlayFabSettings.DeveloperSecretKey));
                    return true;
                }
                else
                {
                    LogToFile("An error occurred when trying to parse TitleSettings.json", ConsoleColor.Red);
                    return false;
                }
            }

           static bool UploadTitleData()
            {
                if (string.IsNullOrEmpty(titleDataPath))
                    return false;

                LogToFile("Uploading Title Data Keys & Values...");
                string parsedFile = ParseFile(titleDataPath);

                var titleDataDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(parsedFile, new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeHtml });
           
               foreach(var kvp in titleDataDict)
               {
                   LogToFile(string.Format("\tUploading: {0}", kvp.Key ));

                   SetTitleDataRequest request = new SetTitleDataRequest()
                   {
                       Key = kvp.Key,
                       Value = kvp.Value
                   };

                   var setTitleDataTask = PlayFabAdminAPI.SetTitleDataAsync(request);
                   setTitleDataTask.Wait();

                   if (setTitleDataTask.Result.Error != null)
                   {
                        OutputPlayFabError("\t\tTitleData Upload: " + kvp.Key, setTitleDataTask.Result.Error);
                   }
                   else
                   {
                        LogToFile(string.Format("\t\t{0} Uploaded.", kvp.Key ), ConsoleColor.Green);
                   }
               }

               return true;
            }

           static bool UploadVC()
            {
                LogToFile("Uploading Virtual Currency Settings...");
                List<VirtualCurrencyData> vcData = new List<VirtualCurrencyData>();

                vcData.Add(new VirtualCurrencyData()
                {
                  CurrencyCode  = "G",
                  DisplayName = "Gold",
                  InitialDeposit = 1000,
                  RechargeMax = 0,
                  RechargeRate = 0
                });

                vcData.Add(new VirtualCurrencyData()
                {
                  CurrencyCode  = "Gm",
                  DisplayName = "Gems",
                  InitialDeposit = 5,
                  RechargeMax = 0,
                  RechargeRate = 0
                });

                vcData.Add(new VirtualCurrencyData()
                {
                  CurrencyCode  = "HT",
                  DisplayName = "Hearts",
                  InitialDeposit = 5,
                  RechargeMax = 5,
                  RechargeRate = 24
                });

                AddVirtualCurrencyTypesRequest request = new AddVirtualCurrencyTypesRequest()
                {
                    VirtualCurrencies = vcData
                };

                var updateVCTask = PlayFabAdminAPI.AddVirtualCurrencyTypesAsync(request);
                updateVCTask.Wait();

                if (updateVCTask.Result.Error != null)
                {
                    OutputPlayFabError("\tVC Upload Error: ", updateVCTask.Result.Error);
                    return false;
                }
                else
                {
                    LogToFile("\tUploaded VC!", ConsoleColor.Green);
                    return true;
                }
 
            }

           static bool UploadCatalog(ref List<CatalogItem> reUploadList)
            {
           
               if (string.IsNullOrEmpty(catalogPath))
                    return false;

                LogToFile("Uploading CatalogItems...");
                string parsedFile = ParseFile(catalogPath);

                var catalogItems = JsonConvert.DeserializeObject<List<CatalogItem>>(parsedFile);
            


                if (catalogItems != null)
                {
                    for (var z = 0; z < catalogItems.Count; z++)
                    {
                        if ((catalogItems[z].Bundle != null && catalogItems[z].Bundle.BundledResultTables != null && catalogItems[z].Bundle.BundledResultTables.Count > 0) || (catalogItems[z].Container != null && catalogItems[z].Container.ResultTableContents != null && catalogItems[z].Container.ResultTableContents.Count > 0))
                        {
                            CatalogItem strip = catalogItems[z];
                            CatalogItem clone = CloneCatalogItemAndStripTables(ref strip);
                            reUploadList.Add(clone);
                        }
                    }

                    if (!UpdateCatalog(catalogItems))
                    {
                        return false;
                    }
                    return true;

                }
                else
                {
                    LogToFile("\tAn error occurred deserializing the Catalog.json file.", ConsoleColor.Red);
                    return false;
                }
            }

           static bool UploadDropTables()
            {
                if (string.IsNullOrEmpty(dropTablesPath))
                    return false;

               LogToFile("Uploading DropTables...");
               string parsedFile = ParseFile(dropTablesPath);

               var dtDict = JsonConvert.DeserializeObject<Dictionary<string, RandomResultTableListing>>(parsedFile);
               if (dtDict != null)
                { 
                    List<RandomResultTable> dropTables = new List<RandomResultTable>();
                    foreach (var kvp in dtDict)
                    {
                        dropTables.Add(new RandomResultTable()
                        {
                            TableId = kvp.Value.TableId,
                            Nodes = kvp.Value.Nodes
                        });
                    }

                    UpdateRandomResultTablesRequest request = new UpdateRandomResultTablesRequest()
                    {
                        CatalogVersion = defaultCatalog,
                        Tables = dropTables
                    };

                    var updateResultTableTask = PlayFabAdminAPI.UpdateRandomResultTablesAsync(request);
                    updateResultTableTask.Wait();

                    if (updateResultTableTask.Result.Error != null)
                    {
                        OutputPlayFabError("\tDropTable Upload Error: ", updateResultTableTask.Result.Error);
                        return false;
                    }
                    else
                    {
                        LogToFile("\tUploaded DropTables!", ConsoleColor.Green);
                        return true;
                    }
                }
                else
                {
                    LogToFile("\tAn error occurred deserializing the DropTables.json file.", ConsoleColor.Red);
                    return false;
                }
            }

           static bool UploadStores()
            {
                if (string.IsNullOrEmpty(storesPath))
                    return false;

                LogToFile("Uploading Stores...");
                string parsedFile = ParseFile(storesPath);

                var storesDict = JsonConvert.DeserializeObject<Dictionary<string, List<StoreItem>>>(parsedFile);

                foreach (var kvp in storesDict)
                {
                    LogToFile(string.Format("\tUploading: {0}", kvp.Key));

                    UpdateStoreItemsRequest request = new UpdateStoreItemsRequest()
                    {
                        CatalogVersion = defaultCatalog,
                        StoreId = kvp.Key,
                        Store = kvp.Value
                    };

                    var updateStoresTask = PlayFabAdminAPI.SetStoreItemsAsync(request);
                    updateStoresTask.Wait();

                    if (updateStoresTask.Result.Error != null)
                    {
                        OutputPlayFabError("\t\tStore Upload: " + kvp.Key, updateStoresTask.Result.Error);
                    }
                    else
                    {
                        LogToFile(string.Format("\t\tStore: {0} Uploaded.", kvp.Key), ConsoleColor.Green);
                    }
                }
                return true;
            }
           
           static bool UploadCdnAssets()
           {
               if (string.IsNullOrEmpty(cdnAssetsPath))
                   return false;

               LogToFile("Uploading CDN AssetBundles...");
               string parsedFile = ParseFile(cdnAssetsPath);

               var assetData = JsonConvert.DeserializeObject<List<CdnAssetData>>(parsedFile);

               if (assetData != null)
               {
                   foreach (var item in assetData)
                   {
                       string key = string.Format("{0}{1}/{2}", item.Platform == "Desktop" ? "" : item.Platform + "/", item.Key,
                           item.Name);
                       string path = item.Path + item.Name;
                       UploadAsset(key, path);
                   }
               }
               else
               {
                   LogToFile("\tAn error occurred deserializing CDN Assets: ", ConsoleColor.Red);
                   return false;
               }
               return true;
           }
       #endregion

       #region Helper Functions -- these functions help the main uploading functions
           static void LogToFile(string content, ConsoleColor color = ConsoleColor.White)
           {
               Console.ForegroundColor = color;
               Console.WriteLine(content);
               logStream.WriteLine(content);

               Console.ForegroundColor = ConsoleColor.White;
            }

           static void OutputPlayFabError(string context, PlayFabError err)
            {
                hitErrors = true; 
               LogToFile(string.Format("\t An error occurred during: {0}", context), ConsoleColor.Red);

                string details = string.Empty;
                if (err.ErrorDetails != null)
                {
                    foreach (var kvp in err.ErrorDetails)
                    {
                        details += (kvp.Value + "\n");
                    }
                }

               LogToFile(string.Format("\t\t[{0}] -- {1}: {2} ", err.Error, err.ErrorMessage, details), ConsoleColor.Red);
            }

           static string ParseFile(string path)
            {
                StreamReader s = File.OpenText(path);
                string contents = s.ReadToEnd();
                s.Close();
                return  contents;
            }

            static CatalogItem CloneCatalogItemAndStripTables(ref CatalogItem strip)
            {
                if (strip != null)
                {
                    CatalogItem clone = new CatalogItem()
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
                        Container = strip.Container,
                        Bundle = strip.Bundle,
                        CanBecomeCharacter = strip.CanBecomeCharacter,
                        IsStackable = strip.CanBecomeCharacter,
                        IsTradable = strip.IsTradable,
                        ItemImageUrl = strip.ItemImageUrl
                    };

                    strip.Bundle = null;
                    strip.Container = null;
                    return clone;
                }
                else
                {
                    return null;
                }
                // remove bundles with DT's on the initial upload

            }
            
            static bool UpdateCatalog(List<CatalogItem> catalog)
            {
                UpdateCatalogItemsRequest request = new UpdateCatalogItemsRequest()
                {
                    CatalogVersion = defaultCatalog,
                    Catalog = catalog
                };

                var updateCatalogItemsTask = PlayFabAdminAPI.UpdateCatalogItemsAsync(request);
                updateCatalogItemsTask.Wait();

                if (updateCatalogItemsTask.Result.Error != null)
                {
                    OutputPlayFabError("\tCatalog Upload Error: ", updateCatalogItemsTask.Result.Error);
                    return false;
                }
                else
                {
                    LogToFile("\tUploaded Catalog!", ConsoleColor.Green);
                    return true;
                }
            }

            static bool UploadAsset(string key, string path)
            {
                string destURL = string.Empty;
                GetContentUploadUrlRequest request = new GetContentUploadUrlRequest()
                {
                    Key = key,
                    ContentType = "application/x-gzip"
                };

                LogToFile("\tFetching CDN endpoint for " + key);
                var getContentUploadTask = PlayFabAdminAPI.GetContentUploadUrlAsync(request);
                getContentUploadTask.Wait();

                if (getContentUploadTask.Result.Error != null)
                {
                    OutputPlayFabError("\t\tAcquire CDN URL Error: ", getContentUploadTask.Result.Error);
                    return false;
                }
                else
                {
                    destURL = getContentUploadTask.Result.Result.URL;
                    LogToFile("\t\tAcquired CDN Address: " + key, ConsoleColor.Green);

                    byte[] fileContents = File.ReadAllBytes(path);

                    return PutFile(key, destURL, fileContents); ;
                }
            }

            static bool PutFile(string key, string url, byte[] payload)
            {
                LogToFile("\t\tStarting HTTP PUT for: " + key);

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "PUT";
                request.ContentType = "application/x-gzip";

                if (payload != null)
                {
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(payload, 0, payload.Length);
                    dataStream.Close();
                }
                else
                {
                    LogToFile("\t\t\tERROR: Byte array was empty or null", ConsoleColor.Red);
                    return false;
                }
                
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    LogToFile("\t\t\tHTTP PUT Successful:" + key, ConsoleColor.Green);
                    return true;
                }
                else
                {
                    LogToFile(string.Format("\t\t\tERROR: Asset:{0} -- Code:[{1}] -- Msg:{2}", url, response.StatusCode, response.StatusDescription), ConsoleColor.Red);
                    return false;
                }   
            }
       #endregion
    }
}


/// <summary>
/// Used as a helper class for deserializing CDN JSON.
/// </summary>
public class CdnAssetData
{
    public string Name { get; set; }
    public string Key { get; set; }
    public string Path { get; set; }
    public string Platform { get; set; }
}
