﻿/*
ImageGlass Project - Image viewer for Windows
Copyright (C) 2010 - 2023 DUONG DIEU PHAP
Project homepage: https://imageglass.org

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace ImageGlass.Base;

/// <summary>
/// A wrapper of <see cref="WebView2"/> control.
/// </summary>
public class Web2 : WebView2
{
    private bool _darkMode = true;
    private Color _accentColor = Color.FromArgb(28, 146, 255);


    // Properties
    #region Properties

    /// <summary>
    /// Gets, sets dark mode of <see cref="Web2"/>.
    /// </summary>
    public bool DarkMode
    {
        get => _darkMode;
        set
        {
            _darkMode = value;
            _ = SetWeb2DarkModeAsync(value);
        }
    }

    /// <summary>
    /// Gets, sets accent color of <see cref="Web2"/>.
    /// </summary>
    public Color AccentColor
    {
        get => _accentColor;
        set
        {
            _accentColor = value;
            _ = SetWeb2AccentColorAsync(value);
        }
    }

    /// <summary>
    /// Gets, sets campaign for hyperlink url.
    /// </summary>
    public string PageName { get; set; } = "unknown";

    /// <summary>
    /// Gets value indicates that <see cref="Web2"/> is ready to use.
    /// </summary>
    public bool IsWeb2Ready => this.CoreWebView2 != null;

    #endregion // Properties



    // Public events
    #region Public events

    /// <summary>
    /// Occurs when <see cref="Web2"/> is ready to use.
    /// </summary>
    public event EventHandler<EventArgs> Web2Ready;

    /// <summary>
    /// Occurs when <see cref="Web2"/> navigation is completed
    /// </summary>
    public event EventHandler<EventArgs> Web2NavigationCompleted;

    /// <summary>
    /// Occurs when <see cref="Web2"/> receives a message from web view.
    /// </summary>
    public event EventHandler<Web2MessageReceivedEventArgs> Web2MessageReceived;

    /// <summary>
    /// Occurs when <see cref="Web2"/> receives keydown.
    /// </summary>
    public event EventHandler<KeyEventArgs> Web2KeyDown;

    #endregion // Public events



    /// <summary>
    /// Initializes new instance of <see cref="Web2"/>.
    /// </summary>
    public Web2()
    {
        this.DefaultBackgroundColor = Color.Transparent;
        this.WebMessageReceived += Web2_WebMessageReceived;
        this.NavigationCompleted += Web2_NavigationCompleted;
    }



    // Override/ virtual methods
    #region Override/ virtual methods

    protected override void Dispose(bool disposing)
    {
        if (this.CoreWebView2 != null)
        {
            this.CoreWebView2.NewWindowRequested -= CoreWebView2_NewWindowRequested;
        }

        this.WebMessageReceived -= Web2_WebMessageReceived;
        this.NavigationCompleted -= Web2_NavigationCompleted;

        base.Dispose(disposing);
    }


    /// <summary>
    /// Occurs when the <see cref="Web2"/> control is ready.
    /// </summary>
    protected virtual async Task OnWeb2ReadyAsync()
    {
        if (InvokeRequired)
        {
            await Invoke(OnWeb2ReadyAsync);
            return;
        }

        Web2Ready?.Invoke(this, EventArgs.Empty);
        await Task.CompletedTask;
    }


    /// <summary>
    /// Occurs when the <see cref="Web2"/> navigation is completed.
    /// </summary>
    protected virtual async Task OnWeb2NavigationCompleted()
    {
        if (InvokeRequired)
        {
            await Invoke(OnWeb2NavigationCompleted);
            return;
        }

        Web2NavigationCompleted?.Invoke(this, EventArgs.Empty);
        await Task.CompletedTask;
    }


    /// <summary>
    /// Triggers <see cref="Web2MessageReceived"/> event.
    /// </summary>
    protected virtual async Task OnWeb2MessageReceivedAsync(Web2MessageReceivedEventArgs e)
    {
        if (InvokeRequired)
        {
            await Invoke(async delegate
            {
                await OnWeb2MessageReceivedAsync(e);
            });
            return;
        }

        Web2MessageReceived?.Invoke(this, e);
        await Task.CompletedTask;
    }


    /// <summary>
    /// Triggers <see cref="Web2KeyDown"/> event.
    /// </summary>
    protected virtual async Task OnWeb2KeyDownAsync(KeyEventArgs e)
    {
        if (InvokeRequired)
        {
            await Invoke(async delegate
            {
                await OnWeb2KeyDownAsync(e);
            });
            return;
        }

        Web2KeyDown?.Invoke(this, e);
        await Task.CompletedTask;
    }

    #endregion // Override/ virtual methods



    // Public methods
    #region Public methods

    /// <summary>
    /// Ensures <see cref="Web2"/> is ready to work.
    /// </summary>
    public async Task EnsureWeb2Async()
    {
        var options = new CoreWebView2EnvironmentOptions
        {
            AdditionalBrowserArguments = "--disable-web-security --allow-file-access-from-files --allow-file-access",
        };

        try
        {
            var env = await CoreWebView2Environment.CreateAsync(
                userDataFolder: App.ConfigDir(PathType.Dir, "WebUIData"),
                options: options);

            await this.EnsureCoreWebView2Async(env);

            this.CoreWebView2.Settings.IsZoomControlEnabled = false;
            this.CoreWebView2.Settings.IsStatusBarEnabled = false;
            this.CoreWebView2.Settings.IsBuiltInErrorPageEnabled = false;
            this.CoreWebView2.Settings.IsGeneralAutofillEnabled = false;
            this.CoreWebView2.Settings.IsPasswordAutosaveEnabled = false;
            this.CoreWebView2.Settings.IsPinchZoomEnabled = false;
            this.CoreWebView2.Settings.IsSwipeNavigationEnabled = false;
            this.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = false;
            this.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            this.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;

            // DevTools
            this.CoreWebView2.Settings.AreDevToolsEnabled = false;
#if DEBUG
            this.CoreWebView2.Settings.AreDevToolsEnabled = true;
#endif

            this.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;


            await OnWeb2ReadyAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{nameof(Web2)}: Failed to initialize Webview2!\r\n\r\n" +
                $"{ex.Message}\r\n\r\n" +
                $"at {nameof(EnsureWeb2Async)}() method", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }


    /// <summary>
    /// Sets accent color to <see cref="Web2"/> content. Example:
    /// <code>
    /// html style="--Accent: 255 0 0"
    /// </code>
    /// </summary>
    public async Task SetWeb2AccentColorAsync(Color accent)
    {
        if (!this.IsWeb2Ready) return;

        var rgb = $"{accent.R} {accent.G} {accent.B}";
        await this.ExecuteScriptAsync($"""
            document.documentElement.style.setProperty('--Accent', '{rgb}');
            """);
    }


    /// <summary>
    /// Sets dark mode for <see cref="Web2"/> content. Example:
    /// <code>
    /// html color-mode="light"
    /// </code>
    /// </summary>
    public async Task SetWeb2DarkModeAsync(bool darkMode)
    {
        if (!this.IsWeb2Ready) return;

        var colorMode = darkMode ? "dark" : "light";
        await this.ExecuteScriptAsync($"""
            document.documentElement.setAttribute('color-mode', '{colorMode}');
            """);
    }


    /// <summary>
    /// Sets focus mode for <see cref="Web2"/> content. Example:
    /// <code>
    /// html window-focus="true"
    /// </code>
    /// </summary>
    public async Task SetWeb2WindowFocusAsync(bool focus)
    {
        if (!this.IsWeb2Ready) return;

        await this.ExecuteScriptAsync($"""
            document.documentElement.setAttribute('window-focus', '{focus.ToString().ToLower()}');
            """);
    }


    /// <summary>
    /// Sets the visibility of <see cref="Web2"/>.
    /// If the <paramref name="visible"/> is <c>false</c>,
    /// the <see cref="Web2"/> will be also suspended to consume less memory.
    /// </summary>
    public async Task SetWeb2VisibilityAsync(bool visible)
    {
        this.Visible = visible;

        if (!visible)
        {
            await TrySuspendWeb2Async();
        }
    }


    /// <summary>
    /// Tries to suspend the <see cref="Web2"/> instance to consume less memory.
    /// </summary>
    public async Task TrySuspendWeb2Async(int delayMs = 1000)
    {
        await Task.Delay(delayMs);
        if (!this.IsWeb2Ready) return;

        try
        {
            _ = this.CoreWebView2.TrySuspendAsync();
        }
        catch (InvalidOperationException) { }
    }


    /// <summary>
    /// Post a message to web view.
    /// </summary>
    /// <param name="name">Name of the message</param>
    /// <param name="dataJson">Message data</param>
    /// <exception cref="ArgumentException"></exception>
    public void PostWeb2Message(string name, string dataJson)
    {
        if (!this.IsWeb2Ready) return;


        var json = @$"
{{
    ""Name"": ""{name}"",
    ""Data"": {dataJson}
}}";

        try
        {
            this.CoreWebView2.PostWebMessageAsJson(json);
        }
        catch (Exception ex)
        {
            throw new ArgumentException(
                $"{ex.ToString()}\r\n" +
                $"JSON data:\r\n{json}\r\n\r\n" +
                $"Error detail:\r\n", ex);
        }
    }


    /// <summary>
    /// Safely run the action after the current event handler is completed,
    /// to avoid potential reentrancy caused by running a nested message loop
    /// in the WebView2 event handler.
    /// Source: <see href="https://learn.microsoft.com/en-us/microsoft-edge/webview2/concepts/threading-model#reentrancy"/>
    /// </summary>
    public static void SafeRunUi(Action action)
    {
        SynchronizationContext.Current.Post((_) =>
        {
            action();
        }, null);
    }

    #endregion // Public methods



    // Webview2 Events
    #region Webview2 Events

    private void Web2_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
    {
        var msg = BHelper.ParseJson<Web2MessageReceivedEventArgs>(e.WebMessageAsJson);

        if (msg.Name.Equals("KEYDOWN") && !string.IsNullOrWhiteSpace(msg.Data))
        {
#if DEBUG
            if (msg.Data == "ctrl+shift+i")
            {
                this.CoreWebView2.OpenDevToolsWindow();
                return;
            }
#endif

            var hotkey = new Hotkey(msg.Data);

            _ = OnWeb2KeyDownAsync(new KeyEventArgs(hotkey.KeyData));
        }
        else
        {
            _ = OnWeb2MessageReceivedAsync(new Web2MessageReceivedEventArgs(msg.Name.Trim(), msg.Data?.Trim()));
        }
    }


    private async void Web2_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        var darkModeTask = SetWeb2DarkModeAsync(DarkMode);
        var accentColorTask = SetWeb2AccentColorAsync(AccentColor);
        var keyDownTask = ListenToWeb2KeyDownEventAsync();

        await Task.WhenAll(darkModeTask, accentColorTask, keyDownTask);
        await OnWeb2NavigationCompleted();
    }


    private void CoreWebView2_NewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
    {
        e.Handled = true;
        BHelper.OpenUrl(e.Uri, $"app_{PageName}");
    }


    #endregion // Webview2 Events



    // Private methods
    #region Private methods

    /// <summary>
    /// Starts listening to keydown event.
    /// </summary>
    private async Task ListenToWeb2KeyDownEventAsync()
    {
        await this.ExecuteScriptAsync("""
            window.onkeydown = (e) => {
                const ctrl = e.ctrlKey ? 'ctrl' : '';
                const shift = e.shiftKey ? 'shift' : '';
                const alt = e.altKey ? 'alt' : '';
        
                let key = e.key.toLowerCase();
                const keyMaps = {
                    control: '',
                    shift: '',
                    alt: '',
                    arrowleft: 'left',
                    arrowright: 'right',
                    arrowup: 'up',
                    arrowdown: 'down',
                    backspace: 'back',
                };
                if (keyMaps[key] !== undefined) {
                    key = keyMaps[key];
                }
                const keyCombo = [ctrl, shift, alt, key].filter(Boolean).join('+');

                // preserve ESCAPE key for closing HTML5 dialog
                if (keyCombo === 'escape' && document.querySelector('dialog[open]') {
                    return;
                }
        
                console.log('KEYDOWN', keyCombo);
                window.chrome.webview?.postMessage({ Name: 'KEYDOWN', Data: keyCombo });
            }
        """);
    }

    #endregion // Private methods

}
