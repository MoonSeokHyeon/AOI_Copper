using MVision.Common.DBModel;
using MVision.Common.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.MairaDB.DAO
{
    public class LightControllerDataDAO
    {
        private readonly SqlDataAccess _dataAccess;
        private readonly string tableName;

        public LightControllerDataDAO(SqlDataAccess dataAccess, string tableName)
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
    Port INT NOT NULL,
    BaudRate INT NOT NULL,
    Parity INT NOT NULL,
    DataBits INT NOT NULL,
    StopBits INT NOT NULL,
    MaxChannel INT NOT NULL,
    MaxVolume INT NOT NULL
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
        public IEnumerable<LightControllerData> All()
        {
            var sql = $@"
SELECT *
FROM {this.tableName};
";
            return this._dataAccess.LoadData<LightControllerData, dynamic>(sql, new { });
        }

        public IEnumerable<LightControllerData> LoadByPortId(int portNumber)
        {
            var sql = $@"
SELECT *
FROM {this.tableName}
WHERE PortNumber = @PortNumber;
";
            return this._dataAccess.LoadData<LightControllerData, dynamic>(sql, new { PortNumber = portNumber });
        }

        // 데이터 추가
        public int Add(LightControllerData controller)
        {
            var insertSql = $@"
INSERT INTO {this.tableName}
(Port, BaudRate, Parity, DataBits, StopBits, MaxChannel, MaxVolume)
VALUES
(@Port, @BaudRate, @Parity, @DataBits, @StopBits, @MaxChannel, @MaxVolume);
";
            return this._dataAccess.SaveData(insertSql, controller);
        }

        // 데이터 업데이트
        public int Update(LightControllerData controller)
        {
            var updateSql = $@"
UPDATE {this.tableName}
SET
    Port = @Port,
    BaudRate = @BaudRate,
    Parity = @Parity,
    DataBits = @DataBits,
    StopBits = @StopBits,
    MaxChannel = @MaxChannel,
    MaxVolume = @MaxVolume
WHERE ID = @ID;
";
            return this._dataAccess.SaveData(updateSql, controller);
        }

        public int NewModelAdd()
        {
            // 데이터를 삽입하는 SQL
            var sql = $@"
INSERT INTO {this.tableName} (Port, BaudRate, Parity, DataBits, StopBits, MaxChannel, MaxVolume)
VALUES
    (@Port1, @BaudRate1, @Parity1, @DataBits1, @StopBits1, @MaxChannel1, @MaxVolume1),
    (@Port2, @BaudRate2, @Parity2, @DataBits2, @StopBits2, @MaxChannel2, @MaxVolume2),
    (@Port3, @BaudRate3, @Parity3, @DataBits3, @StopBits3, @MaxChannel3, @MaxVolume3),
    (@Port4, @BaudRate4, @Parity4, @DataBits4, @StopBits4, @MaxChannel4, @MaxVolume4);
";

            // 파라미터 데이터 준비
            var parameters = new
            {
                Port1 = ePort.COM1,
                BaudRate1 = 9600,
                Parity1 = 0,
                DataBits1 = 8,
                StopBits1 = 1,
                MaxChannel1 = eChannel.Ch8,
                MaxVolume1 = 255,
                Port2 = ePort.COM2,
                BaudRate2 = 9600,
                Parity2 = 0,
                DataBits2 = 8,
                StopBits2 = 1,
                MaxChannel2 = eChannel.Ch8,
                MaxVolume2 = 255,
                Port3 = ePort.COM3,
                BaudRate3 = 9600,
                Parity3 = 0,
                DataBits3 = 8,
                StopBits3 = 1,
                MaxChannel3 = eChannel.Ch8,
                MaxVolume3 = 255,
                Port4 = ePort.COM4,
                BaudRate4 = 9600,
                Parity4 = 0,
                DataBits4 = 8,
                StopBits4 = 1,
                MaxChannel4 = eChannel.Ch8,
                MaxVolume4 = 255,
            };

            // 데이터 저장
            int ret = this._dataAccess.SaveData(sql, parameters);

            return ret;
        }

        // 데이터 삭제
        public int Delete(LightControllerData controller)
        {
            var deleteSql = $@"
DELETE FROM {this.tableName}
WHERE PortNumber = @PortNumber;
";
            return this._dataAccess.SaveData(deleteSql, new { PortNumber = controller.Port });
        }
    }
}
