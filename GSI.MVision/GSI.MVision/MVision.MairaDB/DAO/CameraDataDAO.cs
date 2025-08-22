using MVision.Common.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.MairaDB.DAO
{
    public class CameraDataDAO
    {
        private readonly SqlDataAccess _dataAccess;
        private readonly string tableName;

        public CameraDataDAO(SqlDataAccess dataAccess, string tableName)
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
    CamID INT NOT NULL,
    BoardId INT NOT NULL,
    PortId INT NOT NULL,
    Width INT NOT NULL,
    Height INT NOT NULL,
    DeviceIpAddress VARCHAR(50) NOT NULL,
    SerialNumber VARCHAR(50) NOT NULL,
    DeviceType VARCHAR(50),
    DigitalShift INT DEFAULT 0,
    GainRaw INT DEFAULT 51,
    ExposureTimeAbs DOUBLE DEFAULT 35000.0,
    Gamma DOUBLE DEFAULT 1.0,
    ImageRotateType INT DEFAULT 0
);
";
            return this._dataAccess.SaveData(createTableSql, new { });
        }

        public int Init()
        {
            var checkSql = $@"SELECT COUNT(1) FROM {this.tableName};";
            var existingDataCount = this._dataAccess.LoadData<int, dynamic>(checkSql, new { }).FirstOrDefault();

            if (existingDataCount > 0) return 0;
            return NewModelAdd();
        }

        // 전체 조회
        public IEnumerable<CameraData> All()
        {
            var sql = $@"SELECT * FROM {this.tableName};";
            return this._dataAccess.LoadData<CameraData, dynamic>(sql, new { });
        }

        public IEnumerable<CalibrationData> LoadByCamId(int camID)
        {
            var sql = $@"
SELECT * FROM {this.tableName}
WHERE CamID = @CamID;
";
            return this._dataAccess.LoadData<CalibrationData, dynamic>(sql, new { CamID = camID });
        }

        // 추가
        public int Add(CameraData camera)
        {
            var insertSql = $@"
INSERT INTO {this.tableName}
(CamID, BoardId, PortId, Width, Height, DeviceIpAddress, SerialNumber, DeviceType, DigitalShift, GainRaw, ExposureTimeAbs, Gamma, ImageRotateType)
VALUES
(@CamID, @BoardId, @PortId, @Width, @Height, @DeviceIpAddress, @SerialNumber, @DeviceType, @DigitalShift, @GainRaw, @ExposureTimeAbs, @Gamma, @ImageRotateType);
";
            return this._dataAccess.SaveData(insertSql, camera);
        }

        // 수정
        public int Update(CameraData camera)
        {
            var updateSql = $@"
UPDATE {this.tableName}
SET
    BoardId = @BoardId,
    PortId = @PortId,
    Width = @Width,
    Height = @Height,
    DeviceIpAddress = @DeviceIpAddress,
    SerialNumber = @SerialNumber,
    DeviceType = @DeviceType,
    DigitalShift = @DigitalShift,
    GainRaw = @GainRaw,
    ExposureTimeAbs = @ExposureTimeAbs,
    Gamma = @Gamma,
    ImageRotateType = @ImageRotateType
WHERE CamID = @CamID;
";
            return this._dataAccess.SaveData(updateSql, camera);
        }

        // 초기값 삽입
        public int NewModelAdd()
        {
            var sql = $@"
INSERT INTO {this.tableName}
(CamID, BoardId, PortId, Width, Height, DeviceIpAddress, SerialNumber, DeviceType, DigitalShift, GainRaw, ExposureTimeAbs, Gamma, ImageRotateType)
VALUES
(@CamID1, @BoardId1, @PortId1, @Width1, @Height1, @DeviceIpAddress1, @SerialNumber1, @DeviceType1, @DigitalShift1, @GainRaw1, @ExposureTimeAbs1, @Gamma1, @ImageRotateType1),
(@CamID2, @BoardId2, @PortId2, @Width2, @Height2, @DeviceIpAddress2, @SerialNumber2, @DeviceType2, @DigitalShift2, @GainRaw2, @ExposureTimeAbs2, @Gamma2, @ImageRotateType2),
(@CamID3, @BoardId3, @PortId3, @Width3, @Height3, @DeviceIpAddress3, @SerialNumber3, @DeviceType3, @DigitalShift3, @GainRaw3, @ExposureTimeAbs3, @Gamma3, @ImageRotateType3),
(@CamID4, @BoardId4, @PortId4, @Width4, @Height4, @DeviceIpAddress4, @SerialNumber4, @DeviceType4, @DigitalShift4, @GainRaw4, @ExposureTimeAbs4, @Gamma4, @ImageRotateType4);
";

            var parameters = new
            {
                CamID1 = 1,
                BoardId1 = 1,
                PortId1 = 1,
                Width1 = 1920,
                Height1 = 1080,
                DeviceIpAddress1 = "192.168.0.101",
                SerialNumber1 = "SN101",
                DeviceType1 = "GigE",
                DigitalShift1 = 0,
                GainRaw1 = 51,
                ExposureTimeAbs1 = 35000.0,
                Gamma1 = 1.0,
                ImageRotateType1 = 0,

                CamID2 = 2,
                BoardId2 = 1,
                PortId2 = 2,
                Width2 = 1920,
                Height2 = 1080,
                DeviceIpAddress2 = "192.168.0.102",
                SerialNumber2 = "SN102",
                DeviceType2 = "GigE",
                DigitalShift2 = 0,
                GainRaw2 = 51,
                ExposureTimeAbs2 = 35000.0,
                Gamma2 = 1.0,
                ImageRotateType2 = 0,

                CamID3 = 3,
                BoardId3 = 2,
                PortId3 = 1,
                Width3 = 1920,
                Height3 = 1080,
                DeviceIpAddress3 = "192.168.0.103",
                SerialNumber3 = "SN103",
                DeviceType3 = "GigE",
                DigitalShift3 = 0,
                GainRaw3 = 51,
                ExposureTimeAbs3 = 35000.0,
                Gamma3 = 1.0,
                ImageRotateType3 = 0,

                CamID4 = 4,
                BoardId4 = 2,
                PortId4 = 2,
                Width4 = 1920,
                Height4 = 1080,
                DeviceIpAddress4 = "192.168.0.104",
                SerialNumber4 = "SN104",
                DeviceType4 = "GigE",
                DigitalShift4 = 0,
                GainRaw4 = 51,
                ExposureTimeAbs4 = 35000.0,
                Gamma4 = 1.0,
                ImageRotateType4 = 0
            };

            return this._dataAccess.SaveData(sql, parameters);
        }

        // 삭제
        public int Delete(CameraData camera)
        {
            var deleteSql = $@"
DELETE FROM {this.tableName}
WHERE CamID = @CamID;
";
            return this._dataAccess.SaveData(deleteSql, new { CamID = camera.CamID });
        }
    }
}