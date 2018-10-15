using System;
using System.Collections.Generic;
using System.Text;

namespace GtabbersDotNetCore.Bll.Interfaces
{
     internal interface IDriver
    {
        void Initialize(string robot);

        void Quit();
    }
}
