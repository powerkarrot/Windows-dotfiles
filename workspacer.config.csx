#r "C:\Users\p_kar\workspacer\workspacer.Shared.dll"
#r "C:\Users\p_kar\workspacer\plugins\workspacer.Bar\workspacer.Bar.dll"
#r "C:\Users\p_kar\workspacer\plugins\workspacer.ActionMenu\workspacer.ActionMenu.dll"
#r "C:\Users\p_kar\workspacer\plugins\workspacer.FocusIndicator\workspacer.FocusIndicator.dll"
#r "C:\Users\p_kar\workspacer\plugins\workspacer.Gap\workspacer.Gap.dll"
#r "C:\Users\p_kar\workspacer\plugins\workspacer.TitleBar\workspacer.TitleBar.dll"

using System.Runtime.InteropServices;
using System.Diagnostics;
using System;
using System.Linq;
using System.IO;
using workspacer;
using workspacer.Bar;
using workspacer.Bar.Widgets;
using workspacer.ActionMenu;
using workspacer.FocusIndicator;
using workspacer.Gap;
using workspacer.TitleBar;


/*
 * TODO: 
 *  alt-right klick resize
 *  alt-middle move
 *  --> float windows and keep them on top
 *  
 *  transparency with alpha value
 *  
 *  calendar on click date
 *  
 *  active workspace is bold
 * 
 */
Action<IConfigContext> doConfig = (IConfigContext context) =>
{
    /* Variables */
    var fontSize = 12;
    var barHeight = 19;
    var fontName = "Cascadia Code PL";
    var fontStyle = "Regular";
    var background = new Color(0, 0, 0, 0);
    //var foreground = new Color(255, 0, 162, 255);
    var foreground = new Color(255, 255, 0, 0);



    var monitors = context.MonitorContainer.GetAllMonitors();
    //context.AddBar();  
    var sticky = new StickyWorkspaceContainer(context, StickyWorkspaceIndexMode.Local);
    context.WorkspaceContainer = sticky;

    //WorkspaceWidget ww = new WorkspaceWidget();
    //ww.WorkspaceHasFocusColor = Color.Black;

    context.AddBar(new BarPluginConfig()
    {
        BarTitle = "workspacer.Bar",
        FontSize = fontSize,
        FontName = fontName,
        FontStyle = fontStyle,

        RightWidgets = () => new IBarWidget[] { new PerformanceMonitor(), new TextWidget("|"), new TimeWidget(1000, "HH:mm:ss dd MMM yy"), new TextWidget("|"), new BatteryWidget("Bold") },

        LeftWidgets = () => new IBarWidget[]
        {
            new WorkspaceWidget() {
                WorkspaceHasFocusColor = Color.Black
            },
            new TextWidget(": "),
            new TitleWidget() {
                IsShortTitle = true
            }
        },

        DefaultWidgetBackground = background,
        DefaultWidgetForeground = foreground,

        Transparent = true,
    });


    context.AddFocusIndicator(new FocusIndicatorPluginConfig()
    {
        BorderColor = Color.Lime,
        TimeToShow = 150,
    });

    var actionMenu = context.AddActionMenu(new ActionMenuPluginConfig()
    {
        Foreground = Color.Blue,
    });

    var gap = 20;
    context.AddGap(
        new GapPluginConfig()
        {
            InnerGap = gap,
            OuterGap = gap / 2,
            Delta = gap / 2,
        }
    );

    var titleBarPluginConfig = new TitleBarPluginConfig(new TitleBarStyle(showTitleBar: false, showSizingBorder: false));
    context.AddTitleBar(titleBarPluginConfig);

    if (monitors.Length > 0)
    {
        sticky.CreateWorkspaces(monitors[0], "Code");
        sticky.CreateWorkspaces(monitors[1], "Web", "Chat", "Media", "Stuff");
    }
    else
    {
        sticky.CreateWorkspaces("Web", "Code", "Chat", "Media", "Stuff");
    }


    context.WindowRouter.AddRoute((window) => window.Title.Contains("Visual Studio") ? context.WorkspaceContainer["Code"] : null);
    context.WindowRouter.AddRoute((window) => window.Title.Contains("Sublime") ? context.WorkspaceContainer["Code"] : null);
    context.WindowRouter.AddRoute((window) => window.Title.Contains("JetBrains Rider") ? context.WorkspaceContainer["Code"] : null);
    context.WindowRouter.RouteProcessName("JetBrains Rider", "Code");

    context.WindowRouter.AddRoute((window) => window.Title.Contains("Spotify") ? context.WorkspaceContainer["Media"] : null);

    context.WindowRouter.AddRoute((window) => window.Title.Contains("Opera") ? context.WorkspaceContainer["Web"] : null);

    context.WindowRouter.AddRoute((window) => window.Title.Contains("Discord") ? context.WorkspaceContainer["Chat"] : null);
    context.WindowRouter.AddRoute((window) => window.Title.Contains("Zoom") ? context.WorkspaceContainer["Chat"] : null);
    context.WindowRouter.AddRoute((window) => window.Title.Contains("Skype") ? context.WorkspaceContainer["Chat"] : null);

    // filters, workspacer will ignore windows with this names (I recommend ignoring fullscreen applications)
    context.WindowRouter.AddFilter((window) => !window.Title.Contains("Netflix"));
    context.WindowRouter.AddFilter((window) => !window.Title.Contains("Popcorn Time"));
    context.WindowRouter.AddFilter((window) => !window.Title.Contains("Picture in Picture"));


    // if true, windows have to be manually raised when switching layouts
    context.CanMinimizeWindows = false;

    // keybinds
    KeyModifiers mod = KeyModifiers.Alt;

    // default keybindings: https://github.com/rickbutton/workspacer/blob/master/src/workspacer/Keybinds/KeybindManager.cs

    //unsuscribe defaults that conflict with other software or that I don't use
    context.Keybinds.Unsubscribe(mod, Keys.O); //I'm mapping Alt + O to open PowerToys Run
    context.Keybinds.Unsubscribe(mod | KeyModifiers.LShift, Keys.O); //not going to use
    context.Keybinds.Unsubscribe(mod | KeyModifiers.LShift, Keys.I); //not going to use

    //suscribe
    context.Keybinds.Subscribe(mod | KeyModifiers.LShift, Keys.G, () => System.Diagnostics.Process.Start("explorer.exe", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)));
    //context.Keybinds.Subscribe(mod | KeyModifiers.LShift, Keys.Enter, () => System.Diagnostics.Process.Start(Environment.ExpandEnvironmentVariables(@"%ProgramFiles%\Alacritty\alacritty.exe"));
};

return doConfig;