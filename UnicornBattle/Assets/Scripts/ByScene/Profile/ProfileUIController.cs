using System;
using UnityEngine;
using UnityEngine.UI;

public class ProfileUIController : MonoBehaviour
{
    public Transform[] UiObjects;
    public Image[] colorize;

    private bool _isCharacterInventoryLoaded = false;
    private bool _isCharacterDataLoaded = false;

    public void OnEnable()
    {
        PF_Bridge.OnPlayFabCallbackError += HandleCallbackError;
        PF_Bridge.OnPlayfabCallbackSuccess += HandleCallbackSuccess;
    }

    public void OnDisable()
    {
        PF_Bridge.OnPlayFabCallbackError -= HandleCallbackError;
        PF_Bridge.OnPlayfabCallbackSuccess -= HandleCallbackSuccess;
    }

    public void HandleCallbackError(string details, PlayFabAPIMethods method, MessageDisplayStyle style)
    {

    }

    public void HandleCallbackSuccess(string details, PlayFabAPIMethods method, MessageDisplayStyle style)
    {
        switch (method)
        {
            case PlayFabAPIMethods.GetCharacterReadOnlyData:
                _isCharacterDataLoaded = true;
                break;
            case PlayFabAPIMethods.GetCharacterInventory:
                _isCharacterInventoryLoaded = true;
                break;
        }
        CheckToContinue();
    }

    private void CheckToContinue()
    {
        if (_isCharacterDataLoaded && _isCharacterInventoryLoaded)
        {
            PF_PlayerData.PlayerClassTypes ponyType = (PF_PlayerData.PlayerClassTypes)Enum.Parse(typeof(PF_PlayerData.PlayerClassTypes), PF_PlayerData.activeCharacter.baseClass.CatalogCode);

            switch ((int)ponyType)
            {
                case 0:
                    foreach (var item in colorize)
                        item.color = PF_GamePlay.ClassColor1;
                    break;
                case 1:
                    foreach (var item in colorize)
                        item.color = PF_GamePlay.ClassColor2;
                    break;
                case 2:
                    foreach (var item in colorize)
                        item.color = PF_GamePlay.ClassColor3;
                    break;
                default:
                    Debug.LogWarning("Unknown Class type detected...");
                    break;
            }

            PF_PlayerData.UpdateActiveCharacterData();
            foreach (Transform each in UiObjects)
            {
                each.gameObject.SetActive(true); //<---- BOOM Null Ref
                each.BroadcastMessage("Init", SendMessageOptions.DontRequireReceiver);
            }
            // ResetDataChecks
            _isCharacterDataLoaded = false;
            _isCharacterInventoryLoaded = false;
        }
    }

    public void OpenInventory()
    {
        DialogCanvasController.RequestInventoryPrompt(null, DialogCanvasController.InventoryFilters.AllItems);
    }

    public void SwitchToSocialScene()
    {
        DialogCanvasController.RequestSocialPrompt();
    }

    public void OpenStorePicker()
    {
        DialogCanvasController.RequestStore(PF_GameData.StandardStores[0]);
    }
}
