using MVision.Common.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.MairaDB.DAO
{
    public class MachineDataDAO
    {
        private readonly SqlDataAccess _dataAccess;
        private readonly string tableName;

        public MachineDataDAO(SqlDataAccess dataAccess, string tableName)
        {
            this._dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
            this.tableName = string.IsNullOrWhiteSpace(tableName) ? "MachineData" : tableName;

            CreateTableIfNotExists();
        }

        /// <summary>
        /// 테이블 생성 (없으면)
        /// </summary>
        public int CreateTableIfNotExists()
        {
            var createTableSql = $@"
CREATE TABLE IF NOT EXISTS `{this.tableName}` (
    `ID` INT PRIMARY KEY AUTO_INCREMENT,
    `CurrentModelId` INT NOT NULL,
    `MachineName` VARCHAR(100) NOT NULL UNIQUE,
    `EditTime` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
";
            return this._dataAccess.SaveData(createTableSql, new { });
        }

        /// <summary>
        /// 초기 데이터가 없으면 1건 삽입(예: 기본 머신)
        /// </summary>
        public int Init()
        {
            var countSql = $@"SELECT COUNT(1) FROM `{this.tableName}`;";
            var existingCount = this._dataAccess.LoadData<int, dynamic>(countSql, new { }).FirstOrDefault();

            if (existingCount > 0)
                return 0;

            // 필요 시 초기 레코드 정의
            return Add(new MachineData
            {
                CurrentModelId = 1,
                MachineName = "DEFAULT"
            });
        }

        /// <summary>
        /// 전체 조회
        /// </summary>
        public IEnumerable<MachineData> All()
        {
            var sql = $@"
SELECT `CurrentModelId`, `MachineName`
FROM `{this.tableName}`;
";
            return this._dataAccess.LoadData<MachineData, dynamic>(sql, new { });
        }

        /// <summary>
        /// MachineName으로 단건 조회
        /// </summary>
        public MachineData GetByMachineName(string machineName)
        {
            var sql = $@"
SELECT `CurrentModelId`, `MachineName`
FROM `{this.tableName}`
WHERE `MachineName` = @MachineName
LIMIT 1;
";
            return this._dataAccess.LoadData<MachineData, dynamic>(sql, new { MachineName = machineName }).FirstOrDefault();
        }

        /// <summary>
        /// CurrentModelId로 조회 (동일 모델을 쓰는 다수 머신이 있을 수 있어 다건 반환)
        /// </summary>
        public IEnumerable<MachineData> GetByCurrentModelId(int currentModelId)
        {
            var sql = $@"
SELECT `CurrentModelId`, `MachineName`
FROM `{this.tableName}`
WHERE `CurrentModelId` = @CurrentModelId;
";
            return this._dataAccess.LoadData<MachineData, dynamic>(sql, new { CurrentModelId = currentModelId });
        }

        /// <summary>
        /// 추가 (MachineName UNIQUE)
        /// </summary>
        public int Add(MachineData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            var insertSql = $@"
INSERT INTO `{this.tableName}` (`CurrentModelId`, `MachineName`, `EditTime`)
VALUES (@CurrentModelId, @MachineName, @EditTime);
";
            var param = new
            {
                data.CurrentModelId,
                data.MachineName,
                EditTime = DateTime.Now
            };
            return this._dataAccess.SaveData(insertSql, param);
        }

        /// <summary>
        /// 업데이트 (MachineName 기준)
        /// </summary>
        public int Update(MachineData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            var updateSql = $@"
UPDATE `{this.tableName}`
SET
    `CurrentModelId` = @CurrentModelId,
    `EditTime` = @EditTime
WHERE `MachineName` = @MachineName;
";
            var param = new
            {
                data.CurrentModelId,
                data.MachineName,
                EditTime = DateTime.Now
            };
            return this._dataAccess.SaveData(updateSql, param);
        }  
  
    }
}
