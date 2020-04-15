using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiaoYueStartUpForWpfApp.core.data
{
    /// <summary>
    /// 提供的数据库接口。操控持久化数据于服务器
    /// </summary>
    internal interface IDataBaseData: IGeneralData
    {
        object GetDbData();
        bool AddDbData();
        bool AlterDbData();
    }
}
