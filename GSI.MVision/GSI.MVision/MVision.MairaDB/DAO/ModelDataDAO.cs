using MVision.Common.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.MairaDB.DAO
{
    public class ModelDataDAO
    {
        private readonly SqlDataAccess _dataAccess;
        private readonly string tableName;

        public ModelDataDAO(SqlDataAccess dataAccess, string tableName)
        {
            this._dataAccess = dataAccess;
            this.tableName = tableName;

            CreateTableIfNotExists();
        }
        public int CreateTableIfNotExists()
        {
            // ModelData 테이블 생성
            var modelDataSql = $@"
    CREATE TABLE IF NOT EXISTS {this.tableName} (
        ModelID INT PRIMARY KEY AUTO_INCREMENT,
        ModelName VARCHAR(50),
        Description VARCHAR(255),
        CreateTime DATETIME
    );
    ";

            // SQL 실행
            int ret = this._dataAccess.SaveData(modelDataSql, new { });

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
            var model = new ModelData() { ModelName = "Default", Description = "Default Model", CreateTime = DateTime.Now };

            return Add(model);
        }

        public IEnumerable<ModelData> All()
        {
            var sql = $@"
SELECT *
FROM {this.tableName} 
";
            return this._dataAccess.LoadData<ModelData, dynamic>(sql, new { });

        }

        //24.05.14 Priority 추가
        public int Add(ModelData model)
        {

            var sql = $@"
INSERT INTO {this.tableName}
(ModelID, ModelName, Description, CreateTime)
VALUES(@ModelID, @ModelName, @Description, @CreateTime)
;";

            return this._dataAccess.SaveData(sql, model);
        }

        public int Update(ModelData model)
        {
            var sql = $@"
UPDATE {this.tableName} 
SET TransferState = @TransferState, TransferStartTime = @TransferStartTime, CarrierLoc = @CarrierLoc, DestID = @DestID, PinholeState = @PinholeState
WHERE ModelID = @ModelID
;";
            return this._dataAccess.SaveData(sql, model);
        }

        public int Delete(ModelData model)
        {
            var sql = $@"
DELETE FROM {this.tableName}
WHERE ModelID = @ModelID;
";

            return this._dataAccess.SaveData(sql, new { ModelID = model.ModelID });
        }
    }
}
