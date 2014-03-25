// XonkiORM
//
// Hugo Gómez Arenas (2014)
// Twitter: @HugoGomezArenas
// Web: http://hugox.me
// Email: hugo.gomeza@outlook.com
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//


using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data .SqlClient;
using MySql.Data;
using MySql.Data.MySqlClient ;

namespace Xonki.ORM
{
    public class DBAccess
    {

        #region "Attributes"


        private MySqlConnection sqlConnection = null;
        private MySqlTransaction sqlTransaction = null;
      
        #endregion


        #region "Contructors"

       

        public DBAccess(MySqlConnection sqlConn)
        {
            sqlConnection = sqlConn;
        }

        public DBAccess(MySqlConnection sqlConn, MySqlTransaction sqlTrans)
        {
            sqlConnection = sqlConn;
            sqlTransaction = sqlTrans;
        }


        #endregion


        #region "Properties"

        public MySqlConnection Connection
        {
            get
            {
                return sqlConnection;
            }
        }


        public MySqlTransaction Transaction
        {
            get
            {
                return sqlTransaction;
            }
        }

        #endregion


        #region "Methods"
       
        public DataSet ExecuteQuery(MySqlCommand sqlCommand)
        {
            DataSet ds = null;

            if (sqlCommand != null)
            {
                ds = new DataSet();

                ds.Locale = CultureInfo.CurrentCulture;
                try
                {
                    sqlCommand.Connection = Connection;
                    sqlCommand.CommandTimeout = 0;

                    if (sqlTransaction != null)
                    {
                        sqlCommand.Transaction = sqlTransaction;
                    }

                    DbDataAdapter sqlDataAdapter = new MySqlDataAdapter();
                    sqlDataAdapter.SelectCommand = sqlCommand;
                    sqlDataAdapter.Fill(ds);


                }
                catch (DbException e)
                {
                    throw new DBAccessException("An error occurred executing the following query:" + Convert.ToString(sqlCommand.CommandText), e);
                }

            }


            return ds;
        }


        public DataSet ExecuteQuery(string sqlQuery)
        {
            DataSet ds = new DataSet();
            MySqlCommand sqlCommand = new MySqlCommand(sqlQuery, Connection);
            ds.Locale = CultureInfo.CurrentCulture;
            try
            {
                //sqlCommand.Connection = Connection;
                //sqlCommand.CommandText = sqlQuery;
                sqlCommand.CommandTimeout = 0;
                if (sqlTransaction != null)
                {
                    sqlCommand.Transaction = sqlTransaction;
                }
                MySqlDataAdapter sqlDataAdapter = new MySqlDataAdapter(sqlCommand);
                //sqlDataAdapter .SelectCommand =sqlCommand;
                sqlDataAdapter.Fill(ds);


            }
            catch (DbException e)
            {
                throw new DBAccessException("An error occurred executing the following query: " + Convert.ToString(sqlCommand.CommandText), e);
            }


            return ds;
        }


        public object ExecuteScalar(MySqlCommand sqlCommand)
        {
            object obj = null;

            try
            {
                sqlCommand.Connection = Connection;
                sqlCommand.CommandTimeout = 0;
                if (sqlTransaction != null)
                {
                    sqlCommand.Transaction = sqlTransaction;
                }
                obj = sqlCommand.ExecuteScalar();
            }
            catch (DbException e)
            {
                throw new DBAccessException("An error occurred executing the following query: " + Convert.ToString(sqlCommand.CommandText), e);
            }

            return obj;
        }


        public object ExecuteScalar(string sqlQuery)
        {
            object obj = null;

            MySqlCommand sqlCommand = new MySqlCommand();

            try
            {
                sqlCommand.Connection = Connection;
                sqlCommand.CommandText = sqlQuery;
                sqlCommand.CommandTimeout = 0;
                if (sqlTransaction != null)
                {
                    sqlCommand.Transaction = sqlTransaction;
                }
                obj = sqlCommand.ExecuteScalar();
            }
            catch (DbException e)
            {
                throw new DBAccessException("An error occurred executing the following query: " + Convert.ToString(sqlCommand.CommandText), e);

            }


            return obj;
        }

      
        public int ExecuteNonQuery(MySqlCommand sqlCommand)
        {
            int iRowsAffected = 0;


            if (sqlCommand != null)
            {
                try
                {
                    sqlCommand.Connection = Connection;
                    sqlCommand.CommandTimeout = 0;
                    if (sqlTransaction != null)
                    {
                        sqlCommand.Transaction = sqlTransaction;
                    }
                    iRowsAffected = sqlCommand.ExecuteNonQuery();
                }
                catch (DbException e)
                {
                    throw new DBAccessException("Ha ocurrido un error al tratar de ejecutar la siguiente consulta: " + Convert.ToString(sqlCommand.CommandText), e);
                }
            }

            return iRowsAffected;
        }

       
        public int ExecuteNonQuery(string sqlQuery)
        {
            int iRowsAffected = 0;

            MySqlCommand sqlCommand = new MySqlCommand();

            try
            {
                sqlCommand.Connection = Connection;
                sqlCommand.CommandText = sqlQuery;
                sqlCommand.CommandTimeout = 0;
                if (sqlTransaction != null)
                {
                    sqlCommand.Transaction = sqlTransaction;
                }
                iRowsAffected = sqlCommand.ExecuteNonQuery();
            }
            catch (DbException e)
            {
                throw new DBAccessException("Ha ocurrido un error  al tratar de ejecutar la siguiente consulta: " + Convert.ToString(sqlCommand.CommandText), e);
            }


            return iRowsAffected;
        }


        #endregion


    }
}
