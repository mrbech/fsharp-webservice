module Migrate
open Npgsql
open System.Threading

let rec connect retry =
    try
        let connString = "Host=database;Username=todo;Password=todo;Database=todo"
        let conn = new NpgsqlConnection(connString)
        conn.Open()
        conn
    with
        | e -> if retry > 0 then 
                Thread.Sleep 1000
                connect (retry - 1) else raise e

let migrate () =
    printfn "waiting for database"
    use conn = connect 5
    printfn "migrating"
    let cmd = new NpgsqlCommand("""
        CREATE TABLE IF NOT EXISTS todos(
            id serial primary key,
            message text NOT NULL,
            created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT now()
        )
    """, conn)
    cmd.ExecuteNonQuery()

