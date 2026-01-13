using iTunesSyncer.Core.DataSet;

namespace iTunesSyncer.Core.Config
{
    /* ConfigPropertyのアップデート手順
     * 
     * Step1：ConfigProperty_XXXの最新を作成する
     *          名前を変えて、ひとつ前のバージョンからのデータ引継ぎ処理を実装すること
     * Step2：ConfigManagerBaseに最新のConfigPropertyクラスを指定する
     * Step2：ConfigManagerコンストラクタに最新のConfigPropertyクラスを追記する
     * 
     */

    // ここ継承に最新のConfigPropertyクラスを指定する
    public class ConfigManager : ConfigManagerBase<ConfigProperty_1_3_0>
    {
        private static ConfigManager _instance = null;
        public static ConfigManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ConfigManager();
                return _instance;
            }
        }

        private ConfigManager()
        {
            // ここに歴代のConfigPropertyクラスを追記していく
            ConfigPropertyTypeList.Add(typeof(ConfigProperty_1_2_0));
            ConfigPropertyTypeList.Add(typeof(ConfigProperty_1_3_0));

            Load();
        }
    }
}
