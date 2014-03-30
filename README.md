XonkiORM
========
XonkiORM it´s a very simple and basic ORM library for .Net and Mono.
Supports SQL Server and MySQL databases.

Example:

For the table called "User" in a MySQL Database:

USER
   id INT NOT NULL AUTO_INCREMENT
   username VARCHAR(50)
   lastname VARCHAR(50)

You could create the following class:


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using Xonki.ORM;


namespace mynamespace
{
    public class User : PersistentDO
    {

        public User(MySqlConnection  connection)
            : base("user", "id", connection)
        {
        }

        public User(MySqlConnection connection, MySqlTransaction  trans)
            : base("user", "id", connection, trans)
        {
        }
        public User(MySqlConnection connection, int id)
            : base("user", "id", connection, id)
        {
            Load(id);
        }

        public User(MySqlConnection connection, MySqlTransaction trans, int id)
            : base("user", "id", connection, trans, id)
        {
            Load(id);
        }

        public string UserName
        {
            get { return Get<string>("username"); }
            set { Set("username", value); }
        }

        public string LastName
        {
            get { return Get<string>("lastname"); }
            set { Set("lastname", value); }
        }
       
    }
}

And you could create a instance for the User class for have the CRUD operations, for example:

MySqlConnection conn = new MySqlConnection(_strConn);
conn.Open();

// For a new User´s record
User user= new User(conn);
user.firstname="Hugo";
user.lastname="Gomez Arenas";
user.Save();

//For update a User's record with the id 35

int idUser=35;
User user= new User(conn,idUser);
user.firstname="Hugo";
user.lastname="Gomez Arenas";
user.Save();

//For delete a User´s  with the id 35

int idUser=35;
User user= new User(conn,idUser);
user.Delete();









