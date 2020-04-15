using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiaoYueStartUpForWpfApp.core.data
{
    internal interface IConfigData: IGeneralData
    {
        void OpenConfigFile();
        bool UpdateConfig(Configuration configFile);
    }
}
