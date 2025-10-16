using CTMS.Share.Extensions;
using CTMS.Share.Helpers;

namespace CTMS.DataModel.Models.ClinicalInformation;

public class SignatureNode : ICloneable
{
    public int SignatureId { get; set; }
    public string SignatureName { get; set; }
    public string SignatureDate { get; set; }

    public SignatureNode Clone()
    {
        var result = ((ICloneable)this).Clone() as SignatureNode;
        return result;
    }
    object ICloneable.Clone()
    {
        return this.MemberwiseClone();
    }

}

