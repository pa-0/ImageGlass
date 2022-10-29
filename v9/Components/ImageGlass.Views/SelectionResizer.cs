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

namespace ImageGlass.Views;

public class SelectionResizer
{
    /// <summary>
    /// Gets, sets the type of the resizer
    /// </summary>
    public SelectionResizerType Type { get; set; }


    /// <summary>
    /// Gets the cursor of the resizer
    /// </summary>
    public Cursor Cursor => Type switch
    {
        SelectionResizerType.TopLeft => Cursors.SizeNWSE,
        SelectionResizerType.Top => Cursors.SizeNS,
        SelectionResizerType.TopRight => Cursors.SizeNESW,
        SelectionResizerType.Right => Cursors.SizeWE,
        SelectionResizerType.BottomRight => Cursors.SizeNWSE,
        SelectionResizerType.Bottom => Cursors.SizeNS,
        SelectionResizerType.BottomLeft => Cursors.SizeNESW,
        SelectionResizerType.Left => Cursors.SizeWE,
        _ => Cursors.Default,
    };


    /// <summary>
    /// Gets, sets the rectangle region of the resizer
    /// </summary>
    public RectangleF Region { get; set; }


    /// <summary>
    /// Initialize a new <see cref="SelectionResizer"/> instance.
    /// </summary>
    public SelectionResizer(SelectionResizerType position, RectangleF region)
    {
        Type = position;
        Region = region;
    }
}


public enum SelectionResizerType
{
    TopLeft = 0,
    Top = 1 << 1,
    TopRight = 1 << 2,
    Right = 1 << 3,
    BottomRight = 1 << 4,
    Bottom = 1 << 5,
    BottomLeft = 1 << 6,
    Left = 1 << 7,
}