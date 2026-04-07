using System.Collections.Generic;
using Imui.Core;
using WKLib.API.UI;
using WKLib.Core.UI.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace WKLib.Core.UI;

internal class OverlayState : MonoBehaviour
{
    public static List<PopupSettings> Popups = new List<PopupSettings>();
    
    private Dictionary<GraphicRaycaster, bool> _originalState = new Dictionary<GraphicRaycaster, bool>();
    private CL_GameManager gameManager = null;
    private GraphicRaycaster mainGraphicRaycaster = null;
    
    private bool isOpen = true;

    public bool IsOpen
    {
        get => isOpen;
        set
        {
            if (gameManager == null)
                gameManager = GameObject.Find("GameManager")?.GetComponent<CL_GameManager>();

            var gameIsLoading = gameManager != null && gameManager.loading;
            var playerIsReviving = gameManager != null && gameManager.reviving;

            if (!gameIsLoading
                && !playerIsReviving)
            {
                if (isOpen == value)
                    return;
            }
            else
            {
                value = false;
            }

            isOpen = value;

            var gameIsPaused = gameManager != null && gameManager.isPaused;
            
            var terminalInUse = OS_Manager.inUse;

            var pauseMenuExists = gameManager != null && gameManager.pauseMenu;
            var canPause = gameManager != null && gameManager.canPause;

            if (isOpen)
            {
                _originalState.Clear();

                if (pauseMenuExists)
                {
                    if (gameIsPaused)
                        return;

                    if (!terminalInUse && canPause
                        && !gameIsLoading && !playerIsReviving)
                    {
                        gameManager?.Pause();
                    }
                    else
                    {
                        // Close and show message
                        isOpen = false;

                        PopupSettings newPopup = new PopupSettings();
                        
                        if (terminalInUse)
                        {
                            newPopup.PopupText = "Cannot open overlay in terminal.";
                        }
                        else if (!canPause)
                        {
                            newPopup.PopupText = "Cannot open because game cannot be paused.";
                        }
                        else if (gameIsLoading)
                        {
                            newPopup.PopupText = "Cannot open overlay because the game is loading.";
                        }
                        else if (playerIsReviving)
                        {
                            newPopup.PopupText = "Cannot open overlay because the player is reviving.";
                        }
                        
                        newPopup.TimeTillClose = Time.realtimeSinceStartup + newPopup.PopupTime;
                        
                        Popups.Add(newPopup);
                    }
                }
                else if (mainGraphicRaycaster != null)// We are in the main menu or some ui scene
                {
                    // So disable the ui and override !
                    var graphicRaycasters = FindObjectsOfType<GraphicRaycaster>();
                    foreach (var graphicRaycaster in graphicRaycasters)
                    {
                        if (graphicRaycaster == null)
                            continue;

                        _originalState[graphicRaycaster] = graphicRaycaster.IsActive();

                        graphicRaycaster.enabled = false;
                    }

                    mainGraphicRaycaster.enabled = true;
                }
            }
            else
            {
                //gui?.ResetActiveControl();

                if (pauseMenuExists)
                {
                    if (!gameIsPaused)
                        return;

                    if (!terminalInUse && canPause
                        && !gameIsLoading && !playerIsReviving)
                    {
                        gameManager?.UnPause();
                    }
                }
                else
                {
                    foreach(var originalState in _originalState)
                    {
                        if (originalState.Key == null)
                            continue;

                        originalState.Key.enabled = true;
                    }
                }
            }
        }
    }

    private void OnEnable()
    { 
        mainGraphicRaycaster = gameObject.GetComponent<GraphicRaycaster>();
    }

    //TODO: Change popup window position
    public void Draw(ImGui gui)
    {
        foreach (var popup in Popups)
        {
            if (Time.realtimeSinceStartup < popup.TimeTillClose)
            {
                QuickPopupWindow.Draw(gui, popup.PopupText);
            }
        }
    }
}