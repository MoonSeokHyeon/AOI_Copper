using MVision.Common.DBModel;
using MVision.Common.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.MairaDB.DAO
{
    public class LightDataDAO
    {
        private readonly SqlDataAccess _dataAccess;
        private readonly string tableName;
        private readonly string parentTableName;

        public LightDataDAO(SqlDataAccess dataAccess, string tableName, string parentTableName)
        {
            this._dataAccess = dataAccess;
            this.tableName = tableName;
            this.parentTableName = parentTableName;

            CreateTableIfNotExists();
        }
        public int CreateTableIfNotExists()
        {
            // LightData 테이블 생성
            var Sql = $@"
    CREATE TABLE IF NOT EXISTS {this.tableName} (
        ID INT PRIMARY KEY AUTO_INCREMENT,
        ZoneID INT,
        Quadrant INT,
        Port INT,
        Channel INT,
        Value INT,
        `ChUse` BOOLEAN NOT NULL,
        ModelID INT,
        FOREIGN KEY (ModelID) REFERENCES {this.parentTableName}(ModelID) ON DELETE CASCADE
    );
    ";
            // SQL 실행
            int ret = this._dataAccess.SaveData(Sql, new { });

            return ret;
        }
        public int Init()
        {
            // 테이블에 데이터가 1개라도 있는지 확인
            var checkSql = $@"
SELECT COUNT(1)
FROM {this.tableName};
";

            var existingDataCount = this._dataAccess.LoadData<int, dynamic>(checkSql, new { }).FirstOrDefault();

            if (existingDataCount > 0)
            {
                // 데이터가 하나라도 존재하면 바로 반환
                return 0;
            }

            // 데이터가 없으면 초기 데이터 삽입
            return NewModelAdd("Default");
        }

        // 모든 데이터 조회
        public IEnumerable<LightData> All()
        {
            var sql = $@"
SELECT *
FROM {this.tableName};
";
            return this._dataAccess.LoadData<LightData, dynamic>(sql, new { });
        }

        public int Add(LightData lightData)
        {

            var sql = $@"
INSERT INTO {this.tableName}
(ZoneID, Quadrant, Port, Channel, Value, `ChUse`, ModelName)
VALUES(@ZoneID, @Quadrant, @Port, @Channel, @Value, @ChUse, @ModelName)
;";
            int ret = this._dataAccess.SaveData(sql, lightData);

            return ret;
        }

        public int NewModelAdd(string modelName)
        {
            // ModelName으로 ModelID를 조회
            var modelSql = "SELECT ModelID FROM ModelData WHERE ModelName = @ModelName";
            var modelId = this._dataAccess.LoadData<int, dynamic>(modelSql, new { ModelName = modelName }).FirstOrDefault();

            if (modelId == 0)
            {
                // ModelID를 찾을 수 없는 경우 처리
                return 0; // 실패 시 0 반환 (필요에 따라 예외 처리 가능)
            }

            int ret = 0;

            for (var i = ePort.COM1; i <= ePort.COM4; i++)
            {
                for (var j = eChannel.Ch1; j <= eChannel.Ch8; j++)
                {
                    var sql = $@"
    INSERT INTO {this.tableName} (ZoneID, Quadrant, Port, Channel, Value, `ChUse`, ModelID)
    VALUES
        (@ZoneID, @Quadrant, @Port, @Channel, @Value, @ChUse, @ModelID);

    ";

                    // 파라미터 데이터 준비
                    var parameters = new
                    {
                        ZoneId = 1,
                        Quadrant = 1,
                        Port = i,
                        Channel = j,
                        Value = 10,
                        ChUse = true,
                        ModelID = modelId
                    };

                    // 데이터 저장
                    ret += this._dataAccess.SaveData(sql, parameters);
                }
            }

            return ret;
        }

        public int ModelDelete(string modelName)
        {
            var modelSql = "SELECT ModelID FROM " + this.parentTableName + " WHERE ModelName = @ModelName";
            var modelId = this._dataAccess.LoadData<int, dynamic>(modelSql, new { ModelName = modelName }).FirstOrDefault();

            if (modelId == 0)
            {
                // ModelID를 찾을 수 없는 경우 처리
                return 0; // 실패 시 0 반환 (필요에 따라 예외 처리 가능)
            }

            var deleteSql = $@"
DELETE FROM {this.tableName}
WHERE ModelID = @ModelID;
";

            return this._dataAccess.SaveData(deleteSql, new { ModelID = modelId });
        }
    }
}
