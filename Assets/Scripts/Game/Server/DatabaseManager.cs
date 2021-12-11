using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;



namespace DatabaseNetwork {

    public class DatabaseManager : DifficultyManager {
        private static string          _conxId;
        private static string          _sqlQ;
        private static string          _sql_filePath;
        private static UnityWebRequest _DB_CONX_URI;
        private static UnityWebRequest _DB_CONX_URL;
        private const  string          p_DatabaseName_ = "/5Guys-Database.mysql";

        /// <summary>Dictionary of all server connections, with _conxId as key</summary>
        // public static Dictionary<int, NetworkConnectionToClient> connections = new Dictionary<int, NetworkConnectionToClient>();
        
        private static bool _ONLINE;
        public         bool Online { get { return _ONLINE; } }
        private static bool _OFFLINE;
        public         bool Offline { get { return _OFFLINE; } }
        private static bool _Active;
        public         bool Active { get { return _Active; } }


        /// <summary>
        /// 
        /// </summary>
        public static void StartConnection() {
            _ONLINE = true;
            _OFFLINE = false;
        }

        
        /// <summary>
        /// Shuts down the server connection.
        /// <remarks>https://github.com/vis2k/Mirror/blob/8b363d11e9ff2d53878838efe61c0debf8240488/Assets/Mirror/Runtime/NetworkServer.cs#L154</remarks>
        /// </summary>
        public static void KillConnection() {
            _ONLINE = false;
            _OFFLINE = true;
        }


        /// <summary>
        /// 
        /// </summary>
        private static void InsertNew() {
            // _sql_filePath = Application.dataPath + p_DatabaseName_;
            // _conx = "URI=file:" + _sql_filePath;
            // _db_conx = new MySqlConnection(_conx);
            // CreateTable();
        }



        /*private static void CreateTable() {
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
        }*/
    }
}
