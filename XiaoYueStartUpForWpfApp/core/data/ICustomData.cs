using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiaoYueStartUpForWpfApp.core.data
{
    interface ICustomData
    {
        JObject ReadJsonCustomData(string path);
        bool AddJsonCustomData(ref JObject qaJsonObject, ref JToken qasToken, string customQUtf8, string customAUtf8, string dataPath);
        bool UpdateJsonCustomData();
        bool DeleteJsonCustomData();
    }
}
