using MVision.Common.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.MairaDB.DAO
{
    public class UVWDataDAO
    {
        private readonly SqlDataAccess _dataAccess;
        private readonly string tableName;

        public UVWDataDAO(SqlDataAccess dataAccess, string tableName)
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
    UVWPos INT NOT NULL UNIQUE,
    Radius DOUBLE NOT NULL,
    AngleX1 DOUBLE NOT NULL,
    AngleX2 DOUBLE NOT NULL,
    AngleY1 DOUBLE NOT NULL,
    AngleY2 DOUBLE NOT NULL,
    DirR0 TINYINT(1) NOT NULL,
    DirX1 TINYINT(1) NOT NULL,
    DirX2 TINYINT(1) NOT NULL,
    DirY1 TINYINT(1) NOT NULL,
    DirY2 TINYINT(1) NOT NULL
);
";
            return this._dataAccess.SaveData(createTableSql, new { });
        }

        // 초기 데이터 주입 (없을 때만)
        public int Init()
        {
            var checkSql = $@"SELECT COUNT(1) FROM {this.tableName};";
            var existingDataCount = this._dataAccess.LoadData<int, dynamic>(checkSql, new { }).FirstOrDefault();

            if (existingDataCount > 0) return 0;
            return NewModelAdd();
        }

        // 전체 조회
        public IEnumerable<UVWData> All()
        {
            var sql = $@"SELECT * FROM {this.tableName};";
            return this._dataAccess.LoadData<UVWData, dynamic>(sql, new { });
        }

        // UVWPos로 단건 조회
        public UVWData LoadByPos(int uvwPos)
        {
            var sql = $@"
SELECT * FROM {this.tableName}
WHERE UVWPos = @UVWPos;
";
            return this._dataAccess.LoadData<UVWData, dynamic>(sql, new { UVWPos = uvwPos }).FirstOrDefault();
        }

        // 추가
        public int Add(UVWData data)
        {
            var insertSql = $@"
INSERT INTO {this.tableName}
(UVWPos, Radius, AngleX1, AngleX2, AngleY1, AngleY2, DirR0, DirX1, DirX2, DirY1, DirY2)
VALUES
(@UVWPos, @Radius, @AngleX1, @AngleX2, @AngleY1, @AngleY2, @DirR0, @DirX1, @DirX2, @DirY1, @DirY2);
";
            return this._dataAccess.SaveData(insertSql, data);
        }

        // 수정 (UVWPos 기준)
        public int Update(UVWData data)
        {
            var updateSql = $@"
UPDATE {this.tableName}
SET
    Radius = @Radius,
    AngleX1 = @AngleX1,
    AngleX2 = @AngleX2,
    AngleY1 = @AngleY1,
    AngleY2 = @AngleY2,
    DirR0 = @DirR0,
    DirX1 = @DirX1,
    DirX2 = @DirX2,
    DirY1 = @DirY1,
    DirY2 = @DirY2
WHERE UVWPos = @UVWPos;
";
            return this._dataAccess.SaveData(updateSql, data);
        }

        // 초기값 삽입 (UVWPos = 0,1,2,3)
        public int NewModelAdd()
        {
            var sql = $@"
INSERT INTO {this.tableName}
(UVWPos, Radius, AngleX1, AngleX2, AngleY1, AngleY2, DirR0, DirX1, DirX2, DirY1, DirY2)
VALUES
(@UVWPos0, @Radius0, @AngleX10, @AngleX20, @AngleY10, @AngleY20, @DirR00, @DirX10, @DirX20, @DirY10, @DirY20),
(@UVWPos1, @Radius1, @AngleX11, @AngleX21, @AngleY11, @AngleY21, @DirR01, @DirX11, @DirX21, @DirY11, @DirY21),
(@UVWPos2, @Radius2, @AngleX12, @AngleX22, @AngleY12, @AngleY22, @DirR02, @DirX12, @DirX22, @DirY12, @DirY22),
(@UVWPos3, @Radius3, @AngleX13, @AngleX23, @AngleY13, @AngleY23, @DirR03, @DirX13, @DirX23, @DirY13, @DirY23);
";

            // UVWData 기본 생성자의 초기값을 그대로 사용
            var defaults = new UVWData();

            var parameters = new
            {
                // row 0
                UVWPos0 = 1,
                Radius0 = defaults.Radius,
                AngleX10 = defaults.AngleX1,
                AngleX20 = defaults.AngleX2,
                AngleY10 = defaults.AngleY1,
                AngleY20 = defaults.AngleY2,
                DirR00 = defaults.DirR0,
                DirX10 = defaults.DirX1,
                DirX20 = defaults.DirX2,
                DirY10 = defaults.DirY1,
                DirY20 = defaults.DirY2,

                // row 1
                UVWPos1 = 2,
                Radius1 = defaults.Radius,
                AngleX11 = defaults.AngleX1,
                AngleX21 = defaults.AngleX2,
                AngleY11 = defaults.AngleY1,
                AngleY21 = defaults.AngleY2,
                DirR01 = defaults.DirR0,
                DirX11 = defaults.DirX1,
                DirX21 = defaults.DirX2,
                DirY11 = defaults.DirY1,
                DirY21 = defaults.DirY2,

                // row 2
                UVWPos2 = 3,
                Radius2 = defaults.Radius,
                AngleX12 = defaults.AngleX1,
                AngleX22 = defaults.AngleX2,
                AngleY12 = defaults.AngleY1,
                AngleY22 = defaults.AngleY2,
                DirR02 = defaults.DirR0,
                DirX12 = defaults.DirX1,
                DirX22 = defaults.DirX2,
                DirY12 = defaults.DirY1,
                DirY22 = defaults.DirY2,

                // row 3
                UVWPos3 = 4,
                Radius3 = defaults.Radius,
                AngleX13 = defaults.AngleX1,
                AngleX23 = defaults.AngleX2,
                AngleY13 = defaults.AngleY1,
                AngleY23 = defaults.AngleY2,
                DirR03 = defaults.DirR0,
                DirX13 = defaults.DirX1,
                DirX23 = defaults.DirX2,
                DirY13 = defaults.DirY1,
                DirY23 = defaults.DirY2
            };

            return this._dataAccess.SaveData(sql, parameters);
        }

        // 삭제
        public int Delete(UVWData data)
        {
            var deleteSql = $@"
DELETE FROM {this.tableName}
WHERE UVWPos = @UVWPos;
";
            return this._dataAccess.SaveData(deleteSql, new { UVWPos = data.UVWPos });
        }
    }
}
