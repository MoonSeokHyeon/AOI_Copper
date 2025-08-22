using MVision.Common.DBModel;
using MVision.MairaDB.DAO;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MVision.MairaDB
{
    public class MariaManager
    {
        public MachineDataDAO MachineData { get; set; }
        public ModelDataDAO ModelData { get; set; }
        public CameraDataDAO CameraData { get; set; }
        public LightDataDAO LightData { get; set; }
        public LightControllerDataDAO LightControllerData { get; set; }
        public CalibrationDataDAO CalibrationData { get; set; }
        public SystemOptionDAO SystemOptionData { get; set; }
        public SystemSettingDAO SystemSettingData { get; set; }
        public AlignLogDataDAO AlignLogData { get; set; }
        public UVWDataDAO UVWData { get; set; }

        public MariaManager()
        {
            EnsureDatabaseExists();

            var access = new SqlDataAccess("server=127.0.0.1;port=3306;database=VISION;uid=root;password=admin;allow user variables=true; default command timeout=3");

            this.MachineData = new MachineDataDAO(access, "MachineData");

            this.CameraData = new CameraDataDAO(access, "CameraData");
            this.LightControllerData = new LightControllerDataDAO(access, "ControllerData");
            this.UVWData = new UVWDataDAO(access, "UVWData");

            this.SystemOptionData = new SystemOptionDAO(access, "SystemOptionData");
            this.SystemSettingData = new SystemSettingDAO(access, "SystemSettingData");
            this.AlignLogData = new AlignLogDataDAO(access, "AlignLogData");

            this.ModelData = new ModelDataDAO(access, "ModelData");
            this.CalibrationData = new CalibrationDataDAO(access, "CalibrationData", "ModelData");
            this.LightData = new LightDataDAO(access, "LightData", "ModelData");
        }

        public void Init()
        {
            MachineData.Init();

            CameraData.Init();
            LightControllerData.Init();
            UVWData.Init();

            SystemSettingData.Init();
            SystemOptionData.Init();
            AlignLogData.Init();
            
            ModelData.Init();
            LightData.Init();
            CalibrationData.Init();

        }

        private void EnsureDatabaseExists()
        {
            // DB가 없을 때는 database 파라미터를 넣지 말고 서버에만 접속
            const string serverConn = "server=127.0.0.1;port=3306;uid=root;password=admin;allow user variables=true;default command timeout=3";
            const string dbName = "VISION";

            using (var conn = new MySqlConnection(serverConn))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    // MariaDB/MySQL 모두에서 동작 가능한 문자셋/콜레이션
                    cmd.CommandText = $"CREATE DATABASE IF NOT EXISTS `{dbName}` CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci";
                    cmd.ExecuteNonQuery();
                }
            }
        }

    }
}
