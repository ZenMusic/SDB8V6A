using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SymbolDB
{
   
/**
 * @author David McClanahan
 */


public class ImageInfo
{
    GlobalVars gv;

    private  long serialVersionUID = 1;
    int order;  //////////// or FK SID
    public string name;
    public string fullPath;
    string desc;
    int rating;
    public Rectangle bounds;
    public int x;
    public int y;
    public int xx;
    public int yy;
    public long size; /////// or FK SID
    public int width;
    public int height;
    int anid; //// annotation SID
    int iid;  ////  SID
    int rotation = 0;
    ImageSize imageSize;
    ImageLocation imageLocation;
    Boolean showOriginalPosition;

    public ImageInfo()
    {
        //this(0, "","","",0,0,0,0,0, 0, 0, false, false);
        name = "";
        fullPath = "";
        desc = "";
        imageSize = ImageSize.BESTFIT; // set some default
    }

    public ImageInfo(int orderNo)
    {
        name = "";
        fullPath = "";
        desc = "";
        order = orderNo;
    }

    public ImageInfo(string name, string fullPath, int order)
    {
        this.name = name;
        this.fullPath = fullPath;
        this.desc = null;
        this.rating = 0;
        this.order = order;
        this.x = 0;
        this.y = 0;
        this.xx = 0;
        this.yy = 0;
        this.width = 0;
        this.height = 0;
        this.imageSize = ImageSize.STORED;
        this.showOriginalPosition = false;
    }

    public ImageInfo(string name,
            string fullPath,
            string desc,
            int slot,
            int rating,
            int order,
            int x,
            int y,
            int xx,
            int yy,
            ImageSize imageSize,
            ImageLocation imageLocation,
            int width,
            int height,
            Boolean showOriginalPosition)
    {
        this.name = name;
        this.fullPath = fullPath;
        this.desc = desc;
        this.rating = rating;
        this.order = order;
        this.x = x;
        this.y = y;
        this.xx = xx;
        this.yy = yy;
        this.imageSize = imageSize;
        this.imageLocation = imageLocation;
        this.width = width;
        this.height = height;
        this.showOriginalPosition = showOriginalPosition;
    }

    public ImageInfo(ImageInfo info1)
    {
        this.name = info1.name;
        this.fullPath = info1.fullPath;
        this.desc = info1.desc;
        this.rating = info1.rating;
        this.order = info1.order;
        this.x = info1.x;
        this.y = info1.y;
        this.xx = info1.xx;
        this.yy = info1.yy;
        this.width = info1.width;
        this.height = info1.height;
        this.imageSize = info1.imageSize;
        this.imageLocation = info1.imageLocation;
        this.showOriginalPosition = info1.showOriginalPosition;
    }

    public ImageInfo(FileInfoItem info1)
    {
        this.name = info1.fname;
        this.fullPath = info1.fpath;
        this.desc = "";
        this.rating = 0;
        this.order = 0;
        this.x = 0;
        this.y = 0;
        this.xx = 0;
        this.yy = 0;
        this.width = 0;
        this.height = 0;
        this.size = info1.len;
        this.imageSize = ImageSize.BESTFIT;
        this.imageLocation = ImageLocation.CENTER;
        this.showOriginalPosition = true;
    }

    public ImageInfo(FileInfoItem info1, int x2, int y2, int xx2, int yy2)
    {
        this.name = info1.fname;
        this.fullPath = info1.fpath;
        this.desc = "";
        this.rating = 0;
        this.order = 0;
        this.x = x2;
        this.y = y2;
        this.xx = xx2;
        this.yy = yy2;
        this.width = 0;
        this.height = 0;
        this.size = info1.len;
        this.imageSize = ImageSize.BESTFIT;
        this.imageLocation = ImageLocation.CENTER;
        this.showOriginalPosition = false;
    }
        public FileInfoItem lastFI;
        public ImageInfo(FileInfoItem info1, Rectangle rec)
        {
            if (info1 == null)
                info1 = lastFI;
            else
                lastFI = info1;
            if (info1 != null)
            {
                this.name = info1.fname;
                this.fullPath = info1.fpath;
                this.size = info1.len;
            }
            this.desc = "";
            this.rating = 0;
            this.order = 0;
            this.bounds.X = rec.X;
            this.bounds.Y = rec.Y;
            this.bounds.Width = rec.Width;
            this.bounds.Height = rec.Height;
            this.x = rec.X;
            this.y = rec.Y;
            this.xx = rec.Width;
            this.yy = rec.Height;
            this.width = 0;
            this.height = 0;
//            this.size = info1.len;
            this.imageSize = ImageSize.BESTFIT;
            this.imageLocation = ImageLocation.CENTER;
            this.showOriginalPosition = false;
        }

        /*
        public ImageInfo(IInfoB info1)
        {
            this.name = info1.info.getName();//info.name;
            this.fullPath = info1.info.getFullPath();//.fullPath;
            this.desc = info1.info.getName();//.desc;
            this.rating = 0;
            this.order = info1.info.getIID();//.order;
        }
        
 

    using(var imageStream = File.OpenRead("file"))
    {
            var decoder = BitmapDecoder.Create(imageStream, BitmapCreateOptions.IgnoreColorProfile,
                BitmapCacheOption.Default);
            var height = decoder.Frames[0].PixelHeight;
            var width = decoder.Frames[0].PixelWidth;
    }
    C:/Users/davidmc/Pictures/20180610_144011(0) (002).jpg
        */
        public ImageInfo(string txt)
    {
        name = txt;
    }

    public long writeImageInfo(ImageInfo in2)
    {
        return 0;
    }
    public void CreateDataSet()
    {
      
    }

    public ImageInfo getImageInfo()
    {
        return this;
    }
    public void setFullPath(string fpath)
    {

    }
}}
