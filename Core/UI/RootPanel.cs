using Imui.Controls;
using Imui.Core;
using Imui.Examples;
using Imui.IO.UGUI;
using UnityEngine;
using UnityEngine.SceneManagement;
using WKLib.API;
using WKLib.API.Input;
using WKLib.Core.Classes;
using WKLib.Core.UI.Windows;
using static WKLib.Core.Config.ConfigManager;

namespace WKLib.Core.UI;

internal class RootPanel : MonoSingleton<RootPanel>
{
    public ImGui gui = null;
    private Canvas canvas = null;

    private ThemeController themeController = null;
    private OverlayState overlayState = null;
    
    public bool IsOpen
    {
        get => overlayState.IsOpen;
        set => overlayState.IsOpen = value;
    }
    
    private bool isDemoOpen = false;
    
    public override void OnEnable()
    {
        void OnSceneChange(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (AutoCloseOverlay)
                IsOpen = false;
        }
        
        base.OnEnable();

        SceneManager.sceneLoaded -= OnSceneChange;
        SceneManager.sceneLoaded += OnSceneChange;

        //TODO: Better way for finding the parent object for DontDestroyOnLoad
        canvas = transform.parent.GetComponent<Canvas>();
        canvas.gameObject.hideFlags = HideFlags.HideAndDontSave;
        
        DontDestroyOnLoad(canvas.gameObject);
        
        var backend = transform.GetComponent<ImuiUnityGUIBackend>();
        if (gui == null)
            gui = new ImGui(backend, backend);

        overlayState = gameObject.GetComponent<OverlayState>();
        if (overlayState == null)
            overlayState = gameObject.AddComponent<OverlayState>();
        
        themeController = gameObject.GetComponent<ThemeController>();
        if (themeController == null)
            themeController = gameObject.AddComponent<ThemeController>();
        
        themeController.SetTheme(gui);
    }

    private void Update()
    {
        themeController.DetectChanges(gui);

        gui.BeginFrame();

        if (OverlayKey.Value.IsPressed(gui))
        {
            IsOpen = !IsOpen;
        }
        
        InputUtility.HandleInput(gui);
        HandleAPIInput();
        
        // Draw overlay warnings (like not being able to open)
        overlayState.Draw(gui);

        if (IsOpen)
        {
            DrawRootMenuBar();
        }
        
        ConfigWindow.Draw(gui, IsOpen);
        if (EnableDemoWindow)
        {
            ImDemoWindow.Draw(gui, ref isDemoOpen);
        }
        ModListWindow.Draw(gui, IsOpen);
        
        DrawAPIWindows();

        // Check if enter is down
        if (InputUtility.GetKeyDown(KeyCode.Return))
        {
            gui.ResetActiveControl();
        }

        gui.EndFrame();
        gui.Render();
    }

    private void DrawRootMenuBar()
    {
        gui.BeginMenuBar();

        if (gui.BeginMenu("General"))
        {
            gui.Menu("Open mod list", ref ModListWindow.isOpen);
            gui.Menu("Open config menu", ref ConfigWindow.isOpen);

            if (EnableDemoWindow)
                gui.Menu("Open demo menu", ref isDemoOpen);

            //TODO: Implement save
            if (gui.Menu("Try save"))
            {
                
            }

            if (gui.Menu("Try load"))
            {
                
            }
            
            gui.Separator();

            if (gui.Menu("Close menu"))
            {
                IsOpen = false;
            }

            gui.EndMenu();
        }

        if (gui.BeginMenu("Windows"))
        {
            if (gui.Menu("Close all windows"))
            {
                ConfigWindow.isOpen = false;
                ModListWindow.isOpen = false;

                CloseAPIWindows();
            }

            gui.EndMenu();
        }

        gui.EndMenuBar();
    }
    
    private void DrawAPIWindows()
    {
        foreach (var API in WKLibAPI.internalAPIs)
        {
            if (API == null)
                continue;

            foreach (var window in API.Windows)
            {
                if (window == null)
                    continue;

                window.Draw(gui, IsOpen);
            }
        }   
    }
    
    private void HandleAPIInput()
    {
        foreach (var API in WKLibAPI.internalAPIs)
        {
            if (API == null)
                continue;

            foreach (var window in API.Windows)
            {
                if (window == null)
                    continue;

                window.HandleInput(gui);
            }
        }
    }
    
    private void CloseAPIWindows()
    {
        foreach (var API in WKLibAPI.internalAPIs)
        {
            if (API == null)
                continue;

            foreach (var window in API.Windows)
            {
                if (window == null)
                    continue;

                if (!window.isMainConfigWindow)
                    continue;
                        
                window.isOpen = false;
            }
        }
    }   
}