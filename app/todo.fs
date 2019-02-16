module Todo
open System
open OptionUtils
open Npgsql.FSharp

//Type + Data
type Todo = { 
    id: int
    message: string 
    createdAt: System.DateTime
}

let getAll () = 
    Database.conn
    |> Sql.query "SELECT * FROM todos"
    |> Sql.executeTable
    |> Sql.mapEachRow (fun row -> 
        option {
            let! id = Sql.readInt "id" row
            let! message = Sql.readString "message" row
            let! createdAt = Sql.readDate "created_at" row
            return { id = id; message = message; createdAt = createdAt }
        }
    )

let update (id: int) (message: string) =
    Database.conn
    |> Sql.query "UPDATE todos SET message = @message WHERE id = @id RETURNING *"
    |> Sql.parameters [ 
        "message", Sql.Value message
        "id", Sql.Value id ]
    |> Sql.executeTable
    |> Sql.mapEachRow (fun row -> 
        option {
            let! id = Sql.readInt "id" row
            let! message = Sql.readString "message" row
            let! createdAt = Sql.readDate "created_at" row
            return { id = id; message = message; createdAt = createdAt }
        }
    ) |> List.head

let create (message: string) = 
    Database.conn
    |> Sql.query @"INSERT INTO todos (message) VALUES (@message) RETURNING *"
    |> Sql.parameters [ ("@message", Sql.Value message) ]
    |> Sql.executeTable
    |> Sql.mapEachRow (fun row -> 
        option {
            let! id = Sql.readInt "id" row
            let! message = Sql.readString "message" row
            let! createdAt = Sql.readDate "created_at" row
            return { id = id; message = message; createdAt = createdAt }
        }
    ) |> List.head
