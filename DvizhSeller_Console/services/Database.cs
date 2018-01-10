using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace DvizhSeller_Console.services
{
    class Database
    {
        string dbConnectionString;
        string dbFile = Properties.Settings.Default.dbFile;
        public SQLiteConnection connection;

        public Database()
        {
            if (!File.Exists(dbFile))
            {
                dbConnectionString = "Data Source=database.db; Version=3;";
                Console.WriteLine("База данных не найдена, проверьте настройки. Подключена временная БД, доступная только для чтения.");
            }
            else
                dbConnectionString = "Data Source=" + dbFile + "; Version=3;";

            connection = new SQLiteConnection(dbConnectionString);
            connection.Open();
        }
    }
}
