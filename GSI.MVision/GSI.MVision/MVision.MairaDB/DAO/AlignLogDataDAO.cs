using MVision.Common.DBModel;
using MVision.Common.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace MVision.MairaDB.DAO
{
    public class AlignLogDataDAO
    {
        private readonly SqlDataAccess _dataAccess;
        private readonly string tableName;

        public AlignLogDataDAO(SqlDataAccess dataAccess, string tableName)
        {
            this._dataAccess = dataAccess;
            this.tableName = tableName;

            CreateTableIfNotExists();
        }

        public int CreateTableIfNotExists()
        {
            var createTableSql = $@"
CREATE TABLE IF NOT EXISTS {this.tableName} (
    ID INT PRIMARY KEY AUTO_INCREMENT,
    ZoneID INT NOT NULL,
    Quadrant INT NOT NULL,
    CreateDate DATETIME NOT NULL,
    RevX DOUBLE, RevY DOUBLE, RevT DOUBLE,
    TargetX DOUBLE, TargetY DOUBLE, TargetT DOUBLE,
    ObjectX DOUBLE, ObjectY DOUBLE, ObjectT DOUBLE,
    OffsetX DOUBLE, OffsetY DOUBLE, OffsetT DOUBLE,
    ObjWidthTop DOUBLE,
    ObjWidthBottom DOUBLE,
    ObjHeightLeft DOUBLE,
    ObjHeightRight DOUBLE,
    TarWidthTop DOUBLE,
    TarWidthBottom DOUBLE,
    TarHeightLeft DOUBLE,
    TarHeightRight DOUBLE,
    IsSuccess BOOLEAN
);";
            return this._dataAccess.SaveData(createTableSql, new { });
        }

        public int Init()
        {
            var checkSql = $@"
SELECT COUNT(1)
FROM {this.tableName};";

            var existingDataCount = this._dataAccess.LoadData<int, dynamic>(checkSql, new { }).FirstOrDefault();

            if (existingDataCount > 0)
                return 0;

            return NewModelAdd();
        }

        public IEnumerable<AlignLogData> LoadByZoneId(int zoneId)
        {
            var sql = $@"
SELECT *
FROM {this.tableName}
WHERE ZoneID = @ZoneID;";
            return this._dataAccess.LoadData<AlignLogData, dynamic>(sql, new { ZoneID = zoneId });
        }

        public int Add(AlignLogData data)
        {
            var insertSql = $@"
INSERT INTO {this.tableName}
(ZoneID, Quadrant, CreateDate,
 RevX, RevY, RevT,
 TargetX, TargetY, TargetT,
 ObjectX, ObjectY, ObjectT,
 OffsetX, OffsetY, OffsetT,
 ObjWidthTop, ObjWidthBottom, ObjHeightLeft, ObjHeightRight,
 TarWidthTop, TarWidthBottom, TarHeightLeft, TarHeightRight,
 IsSuccess)
VALUES
(@ZoneID, @Quadrant, @CreateDate,
 @RevX, @RevY, @RevT,
 @TargetX, @TargetY, @TargetT,
 @ObjectX, @ObjectY, @ObjectT,
 @OffsetX, @OffsetY, @OffsetT,
 @ObjWidthTop, @ObjWidthBottom, @ObjHeightLeft, @ObjHeightRight,
 @TarWidthTop, @TarWidthBottom, @TarHeightLeft, @TarHeightRight,
 @IsSuccess);";

            var param = new
            {
                data.ZoneID,
                Quadrant = (int)data.Quadrant,
                data.CreateDate,
                RevX = data.RevXYT.X,
                RevY = data.RevXYT.Y,
                RevT = data.RevXYT.T,
                TargetX = data.TargetXYT.X,
                TargetY = data.TargetXYT.Y,
                TargetT = data.TargetXYT.T,
                ObjectX = data.ObjectXYT.X,
                ObjectY = data.ObjectXYT.Y,
                ObjectT = data.ObjectXYT.T,
                OffsetX = data.OffsetXYT.X,
                OffsetY = data.OffsetXYT.Y,
                OffsetT = data.OffsetXYT.T,
                data.ObjWidthTop,
                data.ObjWidthBottom,
                data.ObjHeightLeft,
                data.ObjHeightRight,
                data.TarWidthTop,
                data.TarWidthBottom,
                data.TarHeightLeft,
                data.TarHeightRight,
                data.IsSuccess
            };

            return this._dataAccess.SaveData(insertSql, param);
        }

        public int Update(AlignLogData data, int id)
        {
            var updateSql = $@"
UPDATE {this.tableName}
SET
    ZoneID = @ZoneID,
    Quadrant = @Quadrant,
    CreateDate = @CreateDate,
    RevX = @RevX, RevY = @RevY, RevT = @RevT,
    TargetX = @TargetX, TargetY = @TargetY, TargetT = @TargetT,
    ObjectX = @ObjectX, ObjectY = @ObjectY, ObjectT = @ObjectT,
    OffsetX = @OffsetX, OffsetY = @OffsetY, OffsetT = @OffsetT,
    ObjWidthTop = @ObjWidthTop,
    ObjWidthBottom = @ObjWidthBottom,
    ObjHeightLeft = @ObjHeightLeft,
    ObjHeightRight = @ObjHeightRight,
    TarWidthTop = @TarWidthTop,
    TarWidthBottom = @TarWidthBottom,
    TarHeightLeft = @TarHeightLeft,
    TarHeightRight = @TarHeightRight,
    IsSuccess = @IsSuccess
WHERE ID = @ID;";

            var param = new
            {
                ID = id,
                data.ZoneID,
                Quadrant = (int)data.Quadrant,
                data.CreateDate,
                RevX = data.RevXYT.X,
                RevY = data.RevXYT.Y,
                RevT = data.RevXYT.T,
                TargetX = data.TargetXYT.X,
                TargetY = data.TargetXYT.Y,
                TargetT = data.TargetXYT.T,
                ObjectX = data.ObjectXYT.X,
                ObjectY = data.ObjectXYT.Y,
                ObjectT = data.ObjectXYT.T,
                OffsetX = data.OffsetXYT.X,
                OffsetY = data.OffsetXYT.Y,
                OffsetT = data.OffsetXYT.T,
                data.ObjWidthTop,
                data.ObjWidthBottom,
                data.ObjHeightLeft,
                data.ObjHeightRight,
                data.TarWidthTop,
                data.TarWidthBottom,
                data.TarHeightLeft,
                data.TarHeightRight,
                data.IsSuccess
            };

            return this._dataAccess.SaveData(updateSql, param);
        }

        public int NewModelAdd()
        {
            var sql = $@"
INSERT INTO {this.tableName}
(
    ZoneID, Quadrant, CreateDate,
    RevX, RevY, RevT,
    TargetX, TargetY, TargetT,
    ObjectX, ObjectY, ObjectT,
    OffsetX, OffsetY, OffsetT,
    ObjWidthTop, ObjWidthBottom, ObjHeightLeft, ObjHeightRight,
    TarWidthTop, TarWidthBottom, TarHeightLeft, TarHeightRight,
    IsSuccess
)
VALUES
(
    @ZoneID, @Quadrant, @CreateDate,
    @RevX, @RevY, @RevT,
    @TargetX, @TargetY, @TargetT,
    @ObjectX, @ObjectY, @ObjectT,
    @OffsetX, @OffsetY, @OffsetT,
    @ObjWidthTop, @ObjWidthBottom, @ObjHeightLeft, @ObjHeightRight,
    @TarWidthTop, @TarWidthBottom, @TarHeightLeft, @TarHeightRight,
    @IsSuccess
);";

            var parameters = new
            {
                ZoneID = 1,
                Quadrant = (int)Common.Shared.eQuadrant.FirstQuadrant,
                CreateDate = DateTime.Now,
                RevX = 0.0,
                RevY = 0.0,
                RevT = 0.0,
                TargetX = 0.0,
                TargetY = 0.0,
                TargetT = 0.0,
                ObjectX = 0.0,
                ObjectY = 0.0,
                ObjectT = 0.0,
                OffsetX = 0.0,
                OffsetY = 0.0,
                OffsetT = 0.0,
                ObjWidthTop = 0.0,
                ObjWidthBottom = 0.0,
                ObjHeightLeft = 0.0,
                ObjHeightRight = 0.0,
                TarWidthTop = 0.0,
                TarWidthBottom = 0.0,
                TarHeightLeft = 0.0,
                TarHeightRight = 0.0,
                IsSuccess = true
            };

            return this._dataAccess.SaveData(sql, parameters);

        }

        public int Delete(int id)
        {
            var deleteSql = $@"
DELETE FROM {this.tableName}
WHERE ID = @ID;";
            return this._dataAccess.SaveData(deleteSql, new { ID = id });
        }
    }
}
