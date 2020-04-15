using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiaoYueStartUpForWpfApp.core.data
{
    internal class LocalConfigData: IConfigData
    {
        /// <summary>
        /// 用于对配置文件(Local.config)的访问操作, 包含客户端的一般用户配置信息
        /// 此类实现于接口IConfigData
        /// </summary>
        public LocalConfigData()
        {
            OpenConfigFile();
        }

        public void OpenConfigFile()
        {
            // 指定打开的配置文件的映射对象
            ExeConfigurationFileMap exeConfigurationFileMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = Environment.CurrentDirectory + "/parameters/settings/local/Local.config"
            };
            // 生成指定对象的Configuration
            Configuration configFile = ConfigurationManager.OpenMappedExeConfiguration(exeConfigurationFileMap, ConfigurationUserLevel.None);

            UpdateConfig(configFile);
        }

        public bool UpdateConfig(Configuration configFile)
        {
            try
            {
                // 更新配置文件
                if (configFile.AppSettings.Settings["FirstStartUp"].Value == "0")
                {
                    configFile.AppSettings.Settings["FirstStartUp"].Value = "1";
                    configFile.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
                }
            }
            catch (ConfigurationErrorsException)
            {
                return false;
            }
            return true;
        }
    }
}
