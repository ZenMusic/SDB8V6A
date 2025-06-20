using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SymbolDB
{
    public class DisplayMode
{
    public ImageSize imageSize;
    public ImageLocation imageLocation;
    public Boolean locked; // hold the last change
    public Boolean set; // has the user set a value

        

    public DisplayMode ()
    {
        locked = false;
        set = false;
        imageSize = ImageSize.BESTFIT;
        imageLocation = ImageLocation.CENTER;
    }
    public DisplayMode (ImageSize imgs, ImageLocation al, Boolean alocked, Boolean aset)
    {
        imageSize = imgs;
        imageLocation = al;
        locked = alocked;
        set = aset;
    }

    public DisplayMode(ImageSize BESTFIT, ImageLocation CENTER)
    {
        locked = false;
        set = false;
        imageSize = ImageSize.BESTFIT;
        imageLocation = ImageLocation.CENTER;
    }
    public void setDM(DisplayMode im)
    {
        imageSize = im.imageSize;
        imageLocation = im.imageLocation;
        locked = im.locked;
        set = im.set;
    }
    public void setDM(ImageSize size, ImageLocation loc)
    {
        imageSize = size;
        imageLocation = loc;
        locked = false;
        set = false;
    }
}
}
