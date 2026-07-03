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
        // 依 prefix 反查為「院區群組層級」：共用 prefix 的分院（柳營奇美）視為其 prefix 擁有者（奇美）。
        return HospitalRegistry.GetOwnerBySubjectNo(subjectNo)?.Prefix ?? "未知";
    }

    public string GetBloodFilename(string subjectNo, string bloodType)
    {
        var owner = HospitalRegistry.GetOwnerBySubjectNo(subjectNo);

        if (bloodType == MagicObjectHelper.Blood抽血檢驗血液)
        {
            return owner?.BloodHematologyFile ?? MagicObjectHelper.成醫抽血檢驗血液File;
        }
        else if (bloodType == MagicObjectHelper.Blood抽血檢驗生化)
        {
            return owner?.BloodBiochemistryFile ?? MagicObjectHelper.成醫抽血檢驗生化File;
        }

        return ".json";
    }
}
