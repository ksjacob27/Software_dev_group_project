using UnityEngine;
using MySql.Data.MySqlClient;



namespace DatabaseManager {
    public class DatabaseManager : MonoBehaviour {
        private       string          _conx;
        private       string          _sqlQ;
        private       string          _sql_filePath;
        private       MySqlConnection _db_conx;
        private       MySqlCommand    _db_command;
        private const string          p_DatabaseName_ = "/5Guys-Database.mysql";


        void Start() {
            _sql_filePath = Application.dataPath + p_DatabaseName_;
            _conx = "URI=file:" + _sql_filePath;
            _db_conx = new MySqlConnection(_conx);
            CreateTable();
        }



        private void CreateTable() {
            using (_db_conx = new MySqlConnection(_conx)) {
                _db_conx.Open();
                _db_command = _db_conx.CreateCommand();

                _sqlQ = "CREATE TABLE IF NOT EXISTS ['users'] (" +
                    "[id]           INT             AUTO_INCREMENT          PRIMARY KEY" +
                    "[first_name]   VARCHAR(20)                             NOT NULL" +
                    "[last_name]    VARCHAR(20)                             NOT NULL" +
                    "[username]     VARCHAR(80)     DEFAULT 'player_'       NOT NULL" +
                    "[email]        VARCHAR(255)    UNIQUE                  NOT NULL" +
                    "[password]     VARCHAR(255)    UNIQUE                  NOT NULL" +
                    "[acc_credits]  INT             UNSIGNED DEFAULT '0'    NOT NULL" +
                    ")";
                _db_command.CommandText = _sqlQ;
                _db_command.ExecuteScalar();
                _db_conx.Close();
            }
        }
    }
}
