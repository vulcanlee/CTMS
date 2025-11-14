using CTMS.DataModel.Models.ClinicalInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models;

public class signature婦產科放射科Model
{
    public List<SignatureNode> signature婦產科 { get; set; } = new();
    public List<SignatureNode> signature放射科 { get; set; } = new();
}
