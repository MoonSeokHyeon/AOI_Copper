using MVision.Common.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.MairaDB.DAO
{
    public class SystemOptionDAO
    {
        private readonly SqlDataAccess _dataAccess;
        private readonly string tableName;

        public SystemOptionDAO(SqlDataAccess dataAccess, string tableName)
        {
            this._dataAccess = dataAccess;
            this.tableName = tableName;

            CreateTableIfNotExists();
        }

        // 테이블 생성
        public int CreateTableIfNotExists()
        {
            var Sql = $@"
CREATE TABLE IF NOT EXISTS {this.tableName} (
    ID INT PRIMARY KEY AUTO_INCREMENT,
    Name VARCHAR(50) NOT NULL UNIQUE,
    `Value` BOOLEAN NOT NULL,
    `Desc` VARCHAR(100),
    EditTime DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);
";

            return this._dataAccess.SaveData(Sql, new { });
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
        public IEnumerable<SystemOption> All()
        {
            var sql = $@"
SELECT *
FROM {this.tableName};
";
            return this._dataAccess.LoadData<SystemOption, dynamic>(sql, new { });
        }

        // 이름으로 데이터 조회
        public SystemOption GetByName(string name)
        {
            var sql = $@"
SELECT *
FROM {this.tableName}
WHERE Name = @Name;
";
            return this._dataAccess.LoadData<SystemOption, dynamic>(sql, new { Name = name }).FirstOrDefault();
        }

        // 데이터 추가
        public int Add(SystemOption option)
        {
            var insertSql = $@"
INSERT INTO {this.tableName} (Name, 'Desc', EditTime, Value)
VALUES (@Name, @Desc, @EditTime, @Value);
";
            return this._dataAccess.SaveData(insertSql, option);
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
    (@Name4, @Desc4, @EditTime4, @Value4);
";

            // 파라미터 데이터 준비
            var parameters = new
            {
                Name1 = "SaveOKImage",
                Desc1 = "SaveOKImage",
                EditTime1 = DateTime.Now,
                Value1 = true,
                Name2 = "SaveNGImage",
                Desc2 = "SaveNGImage",
                EditTime2 = DateTime.Now,
                Value2 = true,
                Name3 = "SaveGraphics",
                Desc3 = "SaveGraphics",
                EditTime3 = DateTime.Now,
                Value3 = true,
                Name4 = "UseAlwaysLightOn",
                Desc4 = "UseAlwaysLightOn",
                EditTime4 = DateTime.Now,
                Value4 = true,
            };

            // 데이터 저장
            int ret = this._dataAccess.SaveData(sql, parameters);

            return ret;
        }

        // 데이터 업데이트
        public int Update(SystemOption option)
        {
            var updateSql = $@"
UPDATE {this.tableName}
SET
    Desc = @Desc,
    EditTime = @EditTime,
    Value = @Value
WHERE Name = @Name;
";
            return this._dataAccess.SaveData(updateSql, option);
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
