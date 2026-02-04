using Syncfusion.Drawing;

namespace CTMS.Helper;

public class Icons
{
    public const string Circle_Red = """
<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" aria-hidden="true" role="img">
  <circle cx="12" cy="12" r="12" fill="#ff0000"/>
</svg>

""";

    public static string GetCircleIcon(Syncfusion.Drawing.Color color)
    {
        //紅燈:
        //有新增頁面但尚未填入任何資料
        //黃燈:
        //有新增頁面填入部分資料，完成度50 %
        //綠燈:
        //所有新增頁面皆完成100 %
        //灰燈:
        //尚未新增頁面也沒有填入資料  
        string result = "";
        if (color == Syncfusion.Drawing.Color.Red)
        {
            result = Circle_Red;
        }
        else if (color == Syncfusion.Drawing.Color.Green)
        {
            result = Circle_Red.Replace("#ff0000", "#00ff00");
        }
        else if (color == Syncfusion.Drawing.Color.Yellow)
        {
            result = Circle_Red.Replace("#ff0000", "#ffff00");
        }
        else if (color == Syncfusion.Drawing.Color.Gray)
        {
            result = Circle_Red.Replace("#ff0000", "#808080");
        }
        return result;
    }

    public static string SetCircleColor(string completionPercent)
    {
        string CircleIcon = "";
        if (string.IsNullOrEmpty(completionPercent))
        {
            CircleIcon = CTMS.Helper.Icons.GetCircleIcon(Color.Gray);
        }
        else
        {
            float percent = 0;
            float.TryParse(completionPercent, out percent);
            if (percent < 50)
            {
                CircleIcon = CTMS.Helper.Icons.GetCircleIcon(Color.Red);
            }
            else if (percent < 100)
            {
                CircleIcon = CTMS.Helper.Icons.GetCircleIcon(Color.Yellow);
            }
            else if (percent >= 100)
            {
                CircleIcon = CTMS.Helper.Icons.GetCircleIcon(Color.Green);
            }
        }
        return CircleIcon;
    }
}
