using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models;

public class MasterRecord
{
    public int Id { get; set; }
    public string Title { get; set; }

    public bool IsExist
    {
        get
        {
            if (string.IsNullOrEmpty(Title))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }
}
