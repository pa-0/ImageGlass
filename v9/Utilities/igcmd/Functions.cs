﻿/*
ImageGlass Project - Image viewer for Windows
Copyright (C) 2010 - 2022 DUONG DIEU PHAP
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

using ImageGlass.Base;
using ImageGlass.Base.WinApi;
using ImageGlass.Settings;

namespace igcmd;

public static class Functions
{
    /// <summary>
    /// Set desktop wallpaper
    /// </summary>
    /// <param name="bmpPath">Full path of BMP file</param>
    /// <param name="styleStr">Wallpaper style, see <see cref="WallpaperStyle"/>.</param>
    /// <returns></returns>
    public static IgExitCode SetDesktopWallpaper(string bmpPath, string styleStr)
    {
        // Get style
        if (Enum.TryParse(styleStr, out WallpaperStyle style))
        {
            var result = DesktopApi.SetWallpaper(bmpPath, style);


            if (result == WallpaperResult.PrivilegesFail)
            {
                return IgExitCode.AdminRequired;
            }
            else if (result == WallpaperResult.Success)
            {
                return IgExitCode.Done;
            }
        }

        return IgExitCode.Error;
    }


    /// <summary>
    /// Sets or unsets app extensions
    /// </summary>
    /// <param name="enable"></param>
    /// <param name="ext">Extensions to proceed. Example: <c>*.png;*.jpg;</c></param>
    /// <returns></returns>
    public static IgExitCode SetAppExtensions(bool enable, string ext = "")
    {
        if (string.IsNullOrEmpty(ext))
        {
            var allExts = Config.AllFormats;

            // Issue #664
            allExts.Remove(".ico");
            ext = Config.GetImageFormats(allExts);
        }


        var result = enable
            ? App.RegisterAppAndExtensions(ext)
            : App.UnregisterAppAndExtensions(ext);
        result.Keys.Clear();

        if (result.IsSuccessful)
        {
            return IgExitCode.Done;
        }
        
        if (!App.IsAdmin)
        {
            return IgExitCode.AdminRequired;
        }

        return IgExitCode.Error;
    }
}