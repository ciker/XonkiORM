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
using System.Collections;
using System.Globalization;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace Xonki.ORM
{
    public class Persistent
    {

        #region "Private Variables"

        private string _tableName = "";
        private string idField = "";
        private Hashtable values;
        protected MySqlConnection _sqlConnection = null;
        protected MySqlTransaction _sqlTransaction = null;
        public int _idValue;

        #endregion
        #region "Contructors"

        public Persistent(string tableName, string idField, MySqlConnection sqlConn)
        {

            _tableName = tableName;
            _idValue = 0;
            _sqlConnection = sqlConn;

            this.idField = idField;
            values = new Hashtable();

        }

        public Persistent(string tableName, string idField, MySqlConnection sqlConn, int id)
        {

            _tableName = tableName;
            _idValue = id;
            _sqlConnection = sqlConn;

            this.idField = idField;
            values = new Hashtable();

        }


        public Persistent(string tableName, string idField, MySqlConnection sqlConn, MySqlTransaction sqlTrans)
        {

            _tableName = tableName;
            _idValue = 0;
            _sqlConnection = sqlConn;
            _sqlTransaction = sqlTrans;

            this.idField = idField;
            values = new Hashtable();

        }

        public Persistent(string tableName, string idField, MySqlConnection sqlConn, MySqlTransaction sqlTrans, int id)
        {

            _tableName = tableName;
            _idValue = id;
            _sqlConnection = sqlConn;
            _sqlTransaction = sqlTrans;

            this.idField = idField;
            values = new Hashtable();

        }


        #endregion

        #region "Properties"
        public int Id
        {
            get
            {
                return _idValue;
            }
        }

        #endregion

        #region "Object DBAccess"
        private DBAccess _databaseAccessor;

        
        public DBAccess DBAccesor
        {
            get
            {
                if (_databaseAccessor == null)
                {
                    if ((_sqlConnection != null) & (_sqlTransaction == null))
                    {

                        _databaseAccessor = new DBAccess(_sqlConnection);
                    }
                    else if ((_sqlConnection != null) & (_sqlTransaction != null))
                    {
                        _databaseAccessor = new DBAccess(_sqlConnection, _sqlTransaction);
                    }
                }
                return _databaseAccessor;
            }
        }
        #endregion


        #region "Load Methods"
        //Load the Object With a specific ID
        protected void Load(int id)
        {
            _idValue = id;

            string str = "SELECT * FROM " + _tableName + " WHERE " + idField + " =" + _idValue.ToString();

            MySqlCommand sc = new MySqlCommand(str);

            try
            {
                DataTable dataTable = DBAccesor.ExecuteQuery(sc).Tables[0];
                if (dataTable.Rows.Count != 0)
                {
                    Load(dataTable.Rows[0]);
                }
                else
                {
                    throw new Exception("The query has no rows returneds");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(" The persistent object could not be loaded - " + "Table: " + _tableName + ", Field ID: " + idField + ", ID: " + id, ex);
            }


        }
        protected void Load(DataRow dr)
        {
            values = new Hashtable();
            DataColumnCollection columns = dr.Table.Columns;
            foreach (DataColumn column in columns)
            {
                values.Add(column.ColumnName, dr[column]);
            }
            //Take the id field out of the values
            try
            {
                if (values.Contains(idField))
                {
                    try
                    {
                        object value = values[idField];
                        _idValue = int.Parse(value.ToString());
                    }
                    catch
                    {
                        throw new Exception("An attempt was made to cast the value in field " + idField + " from table " + _tableName + " to an invalid type.");
                    }
                }

            }
            catch
            {
                throw new Exception("An attempt was made to cast the value in field " + idField + " from table " + _tableName + " to an invalid type.");
            }

            values.Remove(idField);
        }
        #endregion

        #region "Save and Delete Methods"


        public void Save()
        {

            IDictionaryEnumerator valuesEnumerator = values.GetEnumerator();
            MySqlCommand sc = new MySqlCommand();
          
            while (valuesEnumerator.MoveNext())
            {
                sc.Parameters.AddWithValue("@" + valuesEnumerator.Key.ToString(), valuesEnumerator.Value);
            }

            try
            {
                // If the entity not exist in the database
                if (_idValue == 0)
                {
                    //INSERT
                    string fieldList = "";
                    string valueList = "";
                    valuesEnumerator.Reset();
                    bool first = true;
                    while (valuesEnumerator.MoveNext())
                    {
                        if (first)
                        {
                            first = false;
                        }
                        else
                        {
                            fieldList += ", ";
                            valueList += ", ";
                        }
                        fieldList += valuesEnumerator.Key.ToString();
                        valueList += "@" + valuesEnumerator.Key.ToString();
                    }

                    string sql = "" + "INSERT " + _tableName + " (" + fieldList + ")" + " VALUES (" + valueList + ");" + " SELECT LAST_INSERT_ID();";

                    sc.CommandText = sql;
                    _idValue = int.Parse(DBAccesor.ExecuteScalar(sc).ToString());
                }
                else
                {
                    //UPDATE
                    string setList = "";
                    valuesEnumerator.Reset();
                    bool first = true;
                    while (valuesEnumerator.MoveNext())
                    {
                        if (first)
                        {
                            first = false;
                        }
                        else
                        {
                            setList += ", ";
                        }
                        setList += valuesEnumerator.Key.ToString() + " = @" + valuesEnumerator.Key.ToString();
                    }

                    string sql = "UPDATE " + _tableName + " SET " + setList + " WHERE " + idField + "=" + _idValue.ToString();
                    sc.CommandText = sql;
                    DBAccesor.ExecuteNonQuery(sc);
                }
            }
            catch
            {
                throw new Exception("The persistent data object could not have been saved - " + "Table: " + _tableName + ", Field ID : " + idField);


            }


        }

        public void Delete()
        {

            if (this._sqlConnection is MySqlConnection)
            {
                try
                {
                    string strQuery = "DELETE FROM " + _tableName + " WHERE " + idField + "=" + Id.ToString();
                    DBAccesor.ExecuteNonQuery(strQuery);
                }
                catch (Exception e)
                {
                    throw new Exception("The entity can not be deleted - " + "Table: " + _tableName + ", Id Field: " + idField + ", Id: " + _idValue, e);
                }


            }

        }
        #endregion


        #region "Get and Set Methods"

        protected T Get<T>(string fieldName)
        {
            if (values.Contains(fieldName))
            {
                object value = values[fieldName];

                try
                {
                    //If the value is null in the database, null must be returned
                    if (value == DBNull.Value)
                        value = null;

                    return (T)value;
                }
                catch (InvalidCastException)
                {
                    throw new Exception("An attempt was made to cast the value in field \"" + fieldName + "\" from table \"" + _tableName + "\" to an invalid type.");
                }
            }
            else { }
            throw new Exception("The requested field \"" + fieldName + "\" does not exist in table \"" + _tableName + "\".");
        }

        protected void Set(string fieldName, object value)
        {
            if (value == null)
            {
                values[fieldName] = DBNull.Value;
            }
            else
            {
                values[fieldName] = value;
            }
        }

        #endregion
    }

}
