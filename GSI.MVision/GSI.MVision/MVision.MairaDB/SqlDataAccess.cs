using Dapper;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVision.MairaDB
{
    public class SqlDataAccess
    {
        public string ConnectionStringName { get; set; } = "Default";

        string connectionString = string.Empty;
        object lockObj = new object();

        //IDbConnection con = null;

        //bool isConnected => this.con == null ? false : this.con.State == ConnectionState.Open;

        public SqlDataAccess(string connectionString)
        {
            this.connectionString = connectionString;

            //! TimeSpan을 Ticks로 저장하기 위한 설정
            //SqlMapper.RemoveTypeMap(typeof(TimeSpan));
            //SqlMapper.RemoveTypeMap(typeof(TimeSpan?));
            //SqlMapper.AddTypeHandler(new TimeSpanToTicksHandler());
        }

        public IEnumerable<T> LoadData<T, U>(string sql, U parameters, string connectionId = "Default")
        {
            using (var con = new MySqlConnection(this.connectionString))
            {
                return con.Query<T>(sql, parameters);
            }
        }

        public IEnumerable<T> LoadData<T, U, F>(string sql, U parameters, Func<T, F, T> map, string splitOn, string connectionId = "Default")
        {
            IEnumerable<T> ret = null;
            using (var con = new MySqlConnection(this.connectionString))
            {
                try
                {
                    ret = con.Query<T, F, T>(sql, map, parameters, splitOn: splitOn);
                }
                catch (Exception ex)
                {
                    //throw new Exception("Data Base 연결 상태를 확인 하세요.");
                }
                return ret;
            }
        }

        public IEnumerable<T> LoadData<T, U, F, F1>(string sql, U parameters, Func<T, F, F1, T> map, string splitOn, string connectionId = "Default")
        {
            IEnumerable<T> ret = null;
            using (var con = new MySqlConnection(this.connectionString))
            {
                try
                {
                    ret = con.Query<T, F, F1, T>(sql, map, parameters, splitOn: splitOn);
                }
                catch (Exception ex)
                {
                    //throw new Exception("Data Base 연결 상태를 확인 하세요.");
                }
                return ret;
            }
        }

        public int SaveData<T>(string sql, T parameter, string connectionId = "Default")
        {
            int ret = 0;
            using (var con = new MySqlConnection(this.connectionString))
            {
                try
                {
                    ret = con.Execute(sql, parameter);
                }
                catch (Exception ex)
                {
                    //throw new Exception("Data Base 연결 상태를 확인 하세요");
                }
                return ret;
            }
        }
    }
}
