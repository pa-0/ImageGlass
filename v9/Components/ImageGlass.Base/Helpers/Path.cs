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
using ImageGlass.Base.WinApi;

namespace ImageGlass.Base;


public partial class Helpers
{
    private const string LONG_PATH_PREFIX = @"\\?\";


    /// <summary>
    /// Check if the given path (file or directory) is writable. 
    /// </summary>
    /// <param name="type">Indicates if the given path is either file or directory</param>
    /// <param name="path">Full path of file or directory</param>
    /// <returns></returns>
    public static bool CheckPathWritable(PathType type, string path)
    {
        try
        {
            // If path is file
            if (type == PathType.File)
            {
                using (File.OpenWrite(path)) { }
            }

            // if path is directory
            else
            {
                var isDirExist = Directory.Exists(path);

                if (!isDirExist)
                {
                    Directory.CreateDirectory(path);
                }

                var sampleFile = Path.Combine(path, "test_write_file.temp");

                using (File.Create(sampleFile)) { }
                File.Delete(sampleFile);

                if (!isDirExist)
                {
                    Directory.Delete(path, true);
                }
            }


            return true;
        }
        catch
        {
            return false;
        }
    }


    /// <summary>
    /// Fallout from Issue #530. To handle a long path name (i.e. a file path
    /// longer than MAX_PATH), a magic prefix is sometimes necessary.
    /// </summary>
    public static string PrefixLongPath(string path)
    {
        if (string.IsNullOrEmpty(path)) return string.Empty;

        if (path.Length > 255 && !path.StartsWith(LONG_PATH_PREFIX))
            return LONG_PATH_PREFIX + path;

        return path;
    }

    /// <summary>
    /// Fallout from Issue #530. Specific functions (currently FileWatch)
    /// fail if provided a prefixed file path. In this case, strip the prefix
    /// (see PrefixLongPath above).
    /// </summary>
    public static string DePrefixLongPath(string path)
    {
        if (path.StartsWith(LONG_PATH_PREFIX))
            return path[LONG_PATH_PREFIX.Length..];
        return path;
    }


    /// <summary>
    /// Get distinct directories list from paths list.
    /// </summary>
    /// <param name="pathList">Paths list</param>
    /// <returns></returns>
    public static List<string> GetDistinctDirsFromPaths(IEnumerable<string> pathList)
    {
        if (!pathList.Any())
        {
            return new List<string>();
        }

        var hashedDirsList = new HashSet<string>();

        foreach (var path in pathList)
        {
            if (File.Exists(path))
            {
                string dir;
                if (string.Equals(Path.GetExtension(path), ".lnk", StringComparison.CurrentCultureIgnoreCase))
                {
                    var shortcutPath = FileShortcutApi.GetTargetPathFromShortcut(path);

                    // get the DIR path of shortcut target
                    if (File.Exists(shortcutPath))
                    {
                        dir = Path.GetDirectoryName(shortcutPath) ?? "";
                    }
                    else if (Directory.Exists(shortcutPath))
                    {
                        dir = shortcutPath;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    dir = Path.GetDirectoryName(path) ?? "";
                }

                hashedDirsList.Add(dir);
            }
            else if (Directory.Exists(path))
            {
                hashedDirsList.Add(path);
            }
            else
            {
                continue;
            }
        }

        return hashedDirsList.ToList();
    }


    /// <summary>
    /// Checks whether the input path is a directory
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool IsDirectory(string path)
    {
        var attrs = File.GetAttributes(path);

        return attrs.HasFlag(FileAttributes.Directory);
    }

}