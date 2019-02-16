module Database
open System
open Npgsql.FSharp

let connString = 
    Sql.host "database"
    |> Sql.port 5432
    |> Sql.username "todo"
    |> Sql.password "todo"
    |> Sql.database "todo"
    |> Sql.str
    |> (+) """
    Application Name = app;
    Pooling=true;
    Minimum Pool Size = 3;
    Maximum Pool Size = 5;
    """

let conn = Sql.connect connString
