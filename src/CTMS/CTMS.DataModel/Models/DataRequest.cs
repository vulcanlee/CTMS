﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTMS.DataModel.Models;

//
// 摘要:
//     Defines the members of the query.
//
// 備註:
//     DataManagerRequest is used to model bind posted data at server side.
public class DataRequest
{
    //     Specifies the records to skip.
    public int Skip { get; set; }

    //     Specifies the records to take.
    public int Take { get; set; }

    //     Sepcifies that the count is required in response.
    public bool RequiresCounts { get; set; }

    //     Specifies the search criteria.
    public string Search { get; set; }
}
