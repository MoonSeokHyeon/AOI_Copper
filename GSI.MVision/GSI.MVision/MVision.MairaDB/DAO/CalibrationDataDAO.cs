using MVision.Common.DBModel;
using MVision.Common.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.MairaDB.DAO
{
    public class CalibrationDataDAO
    {
        private readonly SqlDataAccess _dataAccess;
        private readonly string tableName;
        private readonly string parentTableName;

        public CalibrationDataDAO(SqlDataAccess dataAccess, string tableName, string parentTableName)
        {
            this._dataAccess = dataAccess;
            this.tableName = tableName;
            this.parentTableName = parentTableName;

            CreateTableIfNotExists();
        }

        // 테이블 생성
        public int CreateTableIfNotExists()
        {
            var createTableSql = $@"
CREATE TABLE IF NOT EXISTS {this.tableName} (
    ID INT PRIMARY KEY AUTO_INCREMENT,
    ModelID INT NOT NULL,
    CamId INT NOT NULL,
    ExecuteZoneId INT NOT NULL,
    Quadrant INT NOT NULL,
    FindType INT NOT NULL,
    Score DOUBLE NOT NULL,
    AxisCenterXY_X DOUBLE,
    AxisCenterXY_Y DOUBLE,
    XAxisStartXY_X DOUBLE,
    XAxisStartXY_Y DOUBLE,
    XAxisEndXY_X DOUBLE,
    XAxisEndXY_Y DOUBLE,
    YAxisStartXY_X DOUBLE,
    YAxisStartXY_Y DOUBLE,
    YAxisEndXY_X DOUBLE,
    YAxisEndXY_Y DOUBLE,
    FixPosXY_X DOUBLE,
    FixPosXY_Y DOUBLE,
    ResolutionXY_X DOUBLE,
    ResolutionXY_Y DOUBLE,
    CurrentPosCamXY_X DOUBLE,
    CurrentPosCamXY_Y DOUBLE,
    CurrentRobotPos_X DOUBLE,
    CurrentRobotPos_Y DOUBLE,
    CurrentRobotPos_T DOUBLE,
    CurrentUVRWPos_U DOUBLE,
    CurrentUVRWPos_V DOUBLE,
    CurrentUVRWPos_R DOUBLE,
    CurrentUVRWPos_W DOUBLE,
    BasePosXY_X DOUBLE,
    BasePosXY_Y DOUBLE,
    RefPosXY1_X DOUBLE,
    RefPosXY1_Y DOUBLE,
    RefPosXY2_X DOUBLE,
    RefPosXY2_Y DOUBLE,
    MovingPitch_X DOUBLE,
    MovingPitch_Y DOUBLE,
    MovingPitch_T DOUBLE,
    FOREIGN KEY (ModelID) REFERENCES {this.parentTableName}(ModelID) ON DELETE CASCADE
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
            return NewModelAdd("Default");
        }

        // 모든 데이터 조회
        public IEnumerable<CalibrationData> All()
        {
            var sql = $@"
SELECT *
FROM {this.tableName};
";
            return this._dataAccess.LoadData<CalibrationData, dynamic>(sql, new { });
        }

        // CamId로 데이터 조회 (기존 시그니처/이름 유지)
        public IEnumerable<CalibrationData> LoadByCamId(int modelId)
        {
            var sql = $@"
SELECT *
FROM {this.tableName}
WHERE ModelID = @ModelID;
";
            return this._dataAccess.LoadData<CalibrationData, dynamic>(sql, new { ModelID = modelId });
        }

        // 데이터 추가
        public int Add(CalibrationData calibration)
        {
            var insertSql = $@"
INSERT INTO {this.tableName}
(
    ModelID, 
    CamId, 
    ExecuteZoneId, 
    Quadrant, 
    FindType, 
    Score, 
    AxisCenterXY_X, AxisCenterXY_Y,
    XAxisStartXY_X, XAxisStartXY_Y, 
    XAxisEndXY_X, XAxisEndXY_Y, 
    YAxisStartXY_X, YAxisStartXY_Y,
    YAxisEndXY_X, YAxisEndXY_Y, 
    FixPosXY_X, FixPosXY_Y, 
    ResolutionXY_X, ResolutionXY_Y,
    CurrentPosCamXY_X, CurrentPosCamXY_Y,
    CurrentRobotPos_X, CurrentRobotPos_Y, CurrentRobotPos_T,
    CurrentUVRWPos_U, CurrentUVRWPos_V, CurrentUVRWPos_R, CurrentUVRWPos_W,
    BasePosXY_X, BasePosXY_Y,
    RefPosXY1_X, RefPosXY1_Y,
    RefPosXY2_X, RefPosXY2_Y,
    MovingPitch_X, MovingPitch_Y, MovingPitch_T
)
VALUES
(
    @ModelID, 
    @CamId,
    @ExecuteZoneId,
    @Quadrant, 
    @FindType, 
    @Score, 
    @AxisCenterXY.X, @AxisCenterXY.Y,
    @XAxisStartXY.X, @XAxisStartXY.Y, 
    @XAxisEndXY.X, @XAxisEndXY.Y, 
    @YAxisStartXY.X, @YAxisStartXY.Y,
    @YAxisEndXY.X, @YAxisEndXY.Y, 
    @FixPosXY.X, @FixPosXY.Y,
    @ResolutionXY.X, @ResolutionXY.Y, 
    @CurrentPosCamXY.X, @CurrentPosCamXY.Y,
    @CurrentRobotPos.X, @CurrentRobotPos.Y, @CurrentRobotPos.T,
    @CurrentUVRWPos.U, @CurrentUVRWPos.V, @CurrentUVRWPos.R, @CurrentUVRWPos.W,
    @BasePosXY.X, @BasePosXY.Y,
    @RefPosXY1.X, @RefPosXY1.Y,
    @RefPosXY2.X, @RefPosXY2.Y,
    @MovingPitch.X, @MovingPitch.Y, @MovingPitch.T
);
";

            return this._dataAccess.SaveData(insertSql, calibration);
        }

        // 데이터 업데이트
        public int Update(CalibrationData calibration)
        {
            var updateSql = $@"
UPDATE {this.tableName}
SET
    ModelID = @ModelID,
    CamId = @CamId,
    ExecuteZoneId = @ExecuteZoneId,
    Quadrant = @Quadrant,
    FindType = @FindType,
    Score = @Score,
    AxisCenterXY_X = @AxisCenterXY.X,
    AxisCenterXY_Y = @AxisCenterXY.Y,
    XAxisStartXY_X = @XAxisStartXY.X,
    XAxisStartXY_Y = @XAxisStartXY.Y,
    XAxisEndXY_X = @XAxisEndXY.X,
    XAxisEndXY_Y = @XAxisEndXY.Y,
    YAxisStartXY_X = @YAxisStartXY.X,
    YAxisStartXY_Y = @YAxisStartXY.Y,
    YAxisEndXY_X = @YAxisEndXY.X,
    YAxisEndXY_Y = @YAxisEndXY.Y,
    FixPosXY_X = @FixPosXY.X,
    FixPosXY_Y = @FixPosXY.Y,
    ResolutionXY_X = @ResolutionXY.X,
    ResolutionXY_Y = @ResolutionXY.Y,
    CurrentPosCamXY_X = @CurrentPosCamXY.X,
    CurrentPosCamXY_Y = @CurrentPosCamXY.Y,
    CurrentRobotPos_X = @CurrentRobotPos.X,
    CurrentRobotPos_Y = @CurrentRobotPos.Y,
    CurrentRobotPos_T = @CurrentRobotPos.T,
    CurrentUVRWPos_U = @CurrentUVRWPos.U,
    CurrentUVRWPos_V = @CurrentUVRWPos.V,
    CurrentUVRWPos_R = @CurrentUVRWPos.R,
    CurrentUVRWPos_W = @CurrentUVRWPos.W,
    BasePosXY_X = @BasePosXY.X,
    BasePosXY_Y = @BasePosXY.Y,
    RefPosXY1_X = @RefPosXY1.X,
    RefPosXY1_Y = @RefPosXY1.Y,
    RefPosXY2_X = @RefPosXY2.X,
    RefPosXY2_Y = @RefPosXY2.Y,
    MovingPitch_X = @MovingPitch.X,
    MovingPitch_Y = @MovingPitch.Y,
    MovingPitch_T = @MovingPitch.T
WHERE ID = @ID;
";

            return this._dataAccess.SaveData(updateSql, calibration);
        }

        public int NewModelAdd(string modelName)
        {
            // ModelName으로 ModelID를 조회
            var modelSql = "SELECT ModelID FROM " + this.parentTableName + " WHERE ModelName = @ModelName";
            var modelId = this._dataAccess.LoadData<int, dynamic>(modelSql, new { ModelName = modelName }).FirstOrDefault();

            if (modelId == 0)
            {
                // ModelID를 찾을 수 없는 경우 처리
                return 0; // 실패 시 0 반환 (필요에 따라 예외 처리 가능)
            }

            int ret = 0;

            for (var i = eExecuteZoneID.Main1_TARGET; i <= eExecuteZoneID.TAPE_FEEDER2; i++)
            {
                for (var j = eQuadrant.FirstQuadrant; j <= eQuadrant.FourthQuadrant; j++)
                {
                    // SQL 구문 준비
                    var sql = $@"
INSERT INTO {this.tableName} (
    ModelID, CamId, ExecuteZoneId, Quadrant, FindType, Score,
    AxisCenterXY_X, AxisCenterXY_Y,
    XAxisStartXY_X, XAxisStartXY_Y, XAxisEndXY_X, XAxisEndXY_Y,
    YAxisStartXY_X, YAxisStartXY_Y, YAxisEndXY_X, YAxisEndXY_Y,
    FixPosXY_X, FixPosXY_Y,
    ResolutionXY_X, ResolutionXY_Y,
    CurrentPosCamXY_X, CurrentPosCamXY_Y,
    CurrentRobotPos_X, CurrentRobotPos_Y, CurrentRobotPos_T,
    CurrentUVRWPos_U, CurrentUVRWPos_V, CurrentUVRWPos_R, CurrentUVRWPos_W,
    BasePosXY_X, BasePosXY_Y,
    RefPosXY1_X, RefPosXY1_Y, RefPosXY2_X, RefPosXY2_Y,
    MovingPitch_X, MovingPitch_Y, MovingPitch_T
)
VALUES
(
    @ModelID, @CamId, @ExecuteZoneId, @Quadrant, @FindType, @Score,
    @AxisCenterXY_X, @AxisCenterXY_Y,
    @XAxisStartXY_X, @XAxisStartXY_Y, @XAxisEndXY_X, @XAxisEndXY_Y,
    @YAxisStartXY_X, @YAxisStartXY_Y, @YAxisEndXY_X, @YAxisEndXY_Y,
    @FixPosXY_X, @FixPosXY_Y,
    @ResolutionXY_X, @ResolutionXY_Y,
    @CurrentPosCamXY_X, @CurrentPosCamXY_Y,
    @CurrentRobotPos_X, @CurrentRobotPos_Y, @CurrentRobotPos_T,
    @CurrentUVRWPos_U, @CurrentUVRWPos_V, @CurrentUVRWPos_R, @CurrentUVRWPos_W,
    @BasePosXY_X, @BasePosXY_Y,
    @RefPosXY1_X, @RefPosXY1_Y, @RefPosXY2_X, @RefPosXY2_Y,
    @MovingPitch_X, @MovingPitch_Y, @MovingPitch_T
);

";

                    // 파라미터 데이터 준비
                    var parameters = new
                    {
                        ModelID = modelId,

                        CamId = 1,
                        ExecuteZoneId = i,
                        Quadrant = j,
                        FindType = 1,
                        Score = 0.0,
                        AxisCenterXY_X = 0.0,
                        AxisCenterXY_Y = 0.0,
                        XAxisStartXY_X = 0.0,
                        XAxisStartXY_Y = 0.0,
                        XAxisEndXY_X = 0.0,
                        XAxisEndXY_Y = 0.0,
                        YAxisStartXY_X = 0.0,
                        YAxisStartXY_Y = 0.0,
                        YAxisEndXY_X = 0.0,
                        YAxisEndXY_Y = 0.0,
                        FixPosXY_X = 0.0,
                        FixPosXY_Y = 0.0,
                        ResolutionXY_X = 0.0,
                        ResolutionXY_Y = 0.0,
                        CurrentPosCamXY_X = 0.0,
                        CurrentPosCamXY_Y = 0.0,
                        CurrentRobotPos_X = 0.0,
                        CurrentRobotPos_Y = 0.0,
                        CurrentRobotPos_T = 0.0,
                        CurrentUVRWPos_U = 0.0,
                        CurrentUVRWPos_V = 0.0,
                        CurrentUVRWPos_R = 0.0,
                        CurrentUVRWPos_W = 0.0,
                        BasePosXY_X = 0.0,
                        BasePosXY_Y = 0.0,
                        RefPosXY1_X = 0.0,
                        RefPosXY1_Y = 0.0,
                        RefPosXY2_X = 0.0,
                        RefPosXY2_Y = 0.0,
                        MovingPitch_X = 0.0,
                        MovingPitch_Y = 0.0,
                        MovingPitch_T = 0.0,
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

        // 데이터 삭제
        public int Delete(int id)
        {
            var deleteSql = $@"
DELETE FROM {this.tableName}
WHERE ID = @ID;
";

            return this._dataAccess.SaveData(deleteSql, new { ID = id });
        }
    }
}
