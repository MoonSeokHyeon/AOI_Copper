using MVision.Common.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.MairaDB.DAO
{
    public class SystemSettingDAO
    {
        private readonly SqlDataAccess _dataAccess;
        private readonly string tableName;

        public SystemSettingDAO(SqlDataAccess dataAccess, string tableName)
        {
            this._dataAccess = dataAccess;
            this.tableName = tableName;

            CreateTableIfNotExists();
        }

        // 테이블 생성
        public int CreateTableIfNotExists()
        {
            var createTableSql = $@"
CREATE TABLE IF NOT EXISTS {this.tableName} (
    ID INT PRIMARY KEY AUTO_INCREMENT,
    Name VARCHAR(50) NOT NULL UNIQUE,
    `Value` VARCHAR(50),
    `Desc` VARCHAR(100),
    EditTime DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);
";

            return this._dataAccess.SaveData(createTableSql, new { });
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
            return NewModelAdd();
        }

        // 모든 데이터 조회
        public IEnumerable<SystemSetting> All()
        {
            var sql = $@"
SELECT *
FROM {this.tableName};
";
            return this._dataAccess.LoadData<SystemSetting, dynamic>(sql, new { });
        }

        // 이름으로 데이터 조회
        public SystemSetting GetByName(string name)
        {
            var sql = $@"
SELECT *
FROM {this.tableName}
WHERE Name = @Name;
";
            return this._dataAccess.LoadData<SystemSetting, dynamic>(sql, new { Name = name }).FirstOrDefault();
        }

        // 데이터 추가
        public int Add(SystemSetting setting)
        {
            var insertSql = $@"
INSERT INTO {this.tableName} (Name, Value, 'Desc', EditTime)
VALUES (@Name, @Value, @Desc, @EditTime);
";
            return this._dataAccess.SaveData(insertSql, setting);
        }

        public int NewModelAdd()
        {
            // 데이터를 삽입하는 SQL
            var sql = $@"
INSERT INTO {this.tableName} (Name, `Desc`, EditTime, Value)
VALUES
    (@Name1, @Desc1, @EditTime1, @Value1),
    (@Name2, @Desc2, @EditTime2, @Value2),
    (@Name3, @Desc3, @EditTime3, @Value3),
    (@Name4, @Desc4, @EditTime4, @Value4),
    (@Name5, @Desc5, @EditTime5, @Value5)
";

            // 파라미터 데이터 준비
            var parameters = new
            {
                Name1 = "GrabDelayTime",
                Desc1 = "GrabDelayTime",
                EditTime1 = DateTime.Now,
                Value1 = 0,
                Name2 = "LogSaveTime",
                Desc2 = "LogSaveTime",
                EditTime2 = DateTime.Now,
                Value2 = 15,
                Name3 = "LogBackUpDays",
                Desc3 = "LogBackUpDays",
                EditTime3 = DateTime.Now,
                Value3 = 15,
                Name4 = "ImageBackUpDays",
                Desc4 = "ImageBackUpDays",
                EditTime4 = DateTime.Now,
                Value4 = 15,
                Name5 = "RetryCount",
                Desc5 = "RetryCount",
                EditTime5 = DateTime.Now,
                Value5 = 3,
            };

            // 데이터 저장
            int ret = this._dataAccess.SaveData(sql, parameters);

            return ret;
        }

        // 데이터 업데이트
        public int Update(SystemSetting setting)
        {
            var updateSql = $@"
UPDATE {this.tableName}
SET
    Value = @Value,
    Desc = @Desc,
    EditTime = @EditTime
WHERE Name = @Name;
";
            return this._dataAccess.SaveData(updateSql, setting);
        }

        // 데이터 삭제
        public int Delete(string name)
        {
            var deleteSql = $@"
DELETE FROM {this.tableName}
WHERE Name = @Name;
";
            return this._dataAccess.SaveData(deleteSql, new { Name = name });
        }
    }
}
