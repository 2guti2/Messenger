﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Log
{
    [Serializable]
    public class LoginEntry : LogEntryAttributes
    {
        public override string ToString()
        {
            return $"{Timestamp}: {ClientUsername} logged in.";
        }
    }
}
