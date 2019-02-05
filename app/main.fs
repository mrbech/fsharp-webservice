module Main
open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful
open Suave.RequestErrors
open Suave.CORS
open FSharp.Json

//Utility
let endpoint (endpoint: 'a -> Result<'b, string>) =
    request (fun r -> 
        let input = r.rawForm |> UTF8.toString |> Json.deserialize<'a>
        let output = input |> endpoint
        match output with
        | Ok result -> OK (result |> Json.serialize)
        | Error error -> BAD_REQUEST (error)
    )

let fetchEndpoint (endpoint: Unit -> 'b) =
    request (fun _ -> OK (endpoint() |> Json.serialize))

//Type + Data
type Todo = { 
    id: int
    message: string 
    createdAt: System.DateTime
}

type CreateTodo = {
    message: string
}

type UpdateTodo = { 
    id: int
    message: string 
}

//Endpoints
let getEndpoint () = 
    let ctx = Database.getContext()
    Database.getTodos ctx
        |> List.map (fun t -> t.MapTo<Todo>())

let createEndpoint (c: CreateTodo) = 
    let ctx = Database.getContext()
    let todo = Database.createTodo ctx c.message
    Ok (todo.MapTo<Todo>())

let updateEndpoint (u: UpdateTodo) = 
    let ctx = Database.getContext()
    match (Database.updateTodo ctx u.id u.message) with
    | None -> Error "Todo not found"
    | Some t ->
        Ok (t.MapTo<Todo>())
    

//Application
let app =
    cors defaultCORSConfig >=>
    choose
        [ GET >=> fetchEndpoint getEndpoint
          POST  >=> endpoint createEndpoint
          PUT >=> endpoint updateEndpoint ]

let config = { defaultConfig with bindings = [HttpBinding.createSimple HTTP "0.0.0.0" 3000]  }
[<EntryPoint>]
let main _ =
    startWebServer config app
    0
