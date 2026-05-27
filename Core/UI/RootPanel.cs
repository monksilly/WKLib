using Imui.Controls;
using Imui.Core;
using Imui.IO.UGUI;
using ImuiBepInEx.API;
using UnityEngine;
using UnityEngine.SceneManagement;
using WKLib.API;
using WKLib.API.Input;
using WKLib.Core.Classes;
using WKLib.Core.UI.Windows;
using WKLib.Examples.UI;
using static WKLib.Core.Config.ConfigManager;

namespace WKLib.Core.UI;

// Right after InputSystem executes
[DefaultExecutionOrder(-999)]
internal class RootPanel : MonoSingleton<RootPanel>
{
    public ImGui gui = null;
    public ImuiPanel ImuiPanel = null;

    public ThemeController ThemeController = null;
    public OverlayState OverlayState = null;
    
    public bool IsOpen
    {
        get => OverlayState.IsOpen;
        set => OverlayState.IsOpen = value;
    }
    
    private bool isDemoOpen = false;
    
    public override void OnEnable()
    {
        void OnSceneChange(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (AutoCloseOverlay.Value)
                IsOpen = false;
        }
        
        base.OnEnable();

        SceneManager.sceneLoaded -= OnSceneChange;
        SceneManager.sceneLoaded += OnSceneChange;
        
        ImuiPanel.Canvas.hideFlags = HideFlags.HideAndDontSave;
        DontDestroyOnLoad(ImuiPanel.Canvas);
        
        var backend = transform.GetComponent<ImuiUnityGUIBackend>();
        if (gui == null)
            gui = new ImGui(backend, backend);

        OverlayState = gameObject.GetComponent<OverlayState>();
        if (OverlayState == null)
            OverlayState = gameObject.AddComponent<OverlayState>();
        
        ThemeController = gameObject.GetComponent<ThemeController>();
        if (ThemeController == null)
            ThemeController = gameObject.AddComponent<ThemeController>();
        
        ThemeController.SetTheme(gui);
        ModListWindow.Initialize();
    }

    private void Update()
    {
        ThemeController.DetectChanges(gui);

        gui.BeginFrame();
        if (InputUtility.GetKeyDown(OverlayKey.Value))
        {
            IsOpen = !IsOpen;
        }
        
        HandleAPIInput();
        
        // Draw overlay warnings (like not being able to open)
        OverlayState.Draw(gui);

        if (IsOpen)
        {
            DrawRootMenuBar();
        }

        ChangeLogWindow.Draw(gui, IsOpen);
        if (EnableDemoWindow.Value)
        {
            DemoWindow.Draw(gui, ref isDemoOpen);
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

            if (EnableDemoWindow.Value)
                gui.Menu("Open demo menu", ref isDemoOpen);
            
            gui.Separator();

            gui.Menu("Open changelog", ref ChangeLogWindow.isOpen);

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
                ModListWindow.isOpen = false;
                ModListWindow.CloseConfigWindows();
                
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
                
                window.isOpen = false;
            }
        }
    }
}