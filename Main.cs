/// <summary>
/// Updates slide count and UI text boxes to reflect current image list state
/// </summary>
public void RefreshSlideCounters()
{
    if (gv.imageFileList1 != null)
    {
        try
        {
            gv.slideCount1 = gv.imageFileList1.getImageFileListLength();
            tbMaxSlideNumber.Text = string.Format("{0:###,###,###}", gv.slideCount1 - 1);
            gv.nextIdx = gv.nextIdx >= gv.slideCount1 ? gv.slideCount1 - 1 : gv.nextIdx;
            tbSlideNumber.Text = string.Format("{0:###,###,##0}", gv.nextIdx);
            tbImageNumber.Text = tbSlideNumber.Text;
        }
        catch (Exception ex)
        {
            gv.debug.w($"RefreshSlideCounters error: {ex.Message}");
        }
    }
}