﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequireJsCore.Configuration
{
    interface IFileReader
    {
        string ReadFile(string path);
    }
}
