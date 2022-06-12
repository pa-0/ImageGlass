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
using ImageMagick;
using System.Drawing.Imaging;

namespace ImageGlass.Base.Photoing.Codecs;


/// <summary>
/// Contains image data and metadata to pass to frontend.
/// </summary>
public class IgImgData : IDisposable
{

    #region IDisposable Disposing

    private bool _isDisposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed)
            return;

        if (disposing)
        {
            // Free any other managed objects here.
            Image?.Dispose();
            Image = null;

            ExifProfile = null;
            ColorProfile = null;
        }

        // Free any unmanaged objects here.
        _isDisposed = true;
    }

    public virtual void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~IgImgData()
    {
        Dispose(false);
    }

    #endregion


    public Bitmap? Image { get; set; } = null;
    public int FrameCount { get; set; } = 0;
    public IExifProfile? ExifProfile { get; set; } = null;
    public IColorProfile? ColorProfile { get; set; } = null;


    public IgImgData() { }


    /// <summary>
    /// Initializes <see cref="IgImgData"/> instance
    /// with <see cref="IgMagickReadData"/> value.
    /// </summary>
    /// <param name="data"></param>
    public IgImgData(IgMagickReadData data)
    {
        FrameCount = data.FrameCount;
        ColorProfile = data.ColorProfile;
        ExifProfile = data.ExifProfile;

        if (data.MultiFrameImage != null)
        {
            // convert WEBP to GIF for animation
            if (data.Extension.Equals(".WEBP", StringComparison.InvariantCultureIgnoreCase))
            {
                Image = data.MultiFrameImage.ToBitmap(ImageFormat.Gif);
            }
            else
            {
                Image = data.MultiFrameImage.ToBitmap();
            }
        }
        else
        {
            Image = data.SingleFrameImage?.ToBitmap();
        }
    }
}
