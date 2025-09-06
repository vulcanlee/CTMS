using CTMS.Share.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.Business.Helpers;

public class SubjectNoHelper
{
    public string GetHospital(string subjectNo)
    {
        string Hospital = "未知";
        if (subjectNo.Contains(MagicObjectHelper.prefix奇美醫院))
        {
            Hospital = MagicObjectHelper.prefix奇美醫院;
        }
        else if (subjectNo.Contains(MagicObjectHelper.prefix郭綜合醫院))
        {
            Hospital = MagicObjectHelper.prefix郭綜合醫院;
        }
        else if (subjectNo.Contains(MagicObjectHelper.prefix成大醫院))
        {
            Hospital = MagicObjectHelper.prefix成大醫院;
        }
        return Hospital;
    }

    public string GetBloodFilename(string subjectNo, string bloodType)
    {
        string hospital = GetHospital(subjectNo);
        string filename = string.Empty;
        string filenamePostfix = "";
        string filenamePrefix = "";

        switch(hospital)
        {
            case MagicObjectHelper.prefix成大醫院:
                filenamePostfix = "";
                break;
            case MagicObjectHelper.prefix奇美醫院:
                filenamePostfix = "2";
                break;
            case MagicObjectHelper.prefix郭綜合醫院:
                filenamePostfix = "1";
                break;
        }

        if (bloodType == MagicObjectHelper.Blood抽血檢驗血液)
        {
            filenamePrefix = "抽血檢驗血液";
        }else if (bloodType == MagicObjectHelper.Blood抽血檢驗生化)
        {
            filenamePrefix = "抽血檢驗生化";
        }

        filename = $"{filenamePrefix}{filenamePostfix}.json";

        return filename;
    }
}
