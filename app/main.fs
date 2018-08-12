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

let mutable todos: Todo list = []

//Endpoints
let getEndpoint () = todos
let createEndpoint (c: CreateTodo) = 
    let todo = { 
        id = (todos |> List.tryHead |> Option.map (fun t -> t.id) |> Option.defaultValue 0) + 1 
        message = c.message
        createdAt = System.DateTime.Now
    }
    todos <- todo::todos
    Ok todo

let updateEndpoint (u: UpdateTodo) = 
    let updater = fun (v, l) (t: Todo) -> 
        if t.id = u.id then let r = { t with message = u.message  } in (Some r, r::l) else (v, t::l)
    let updateResult = List.fold updater (None, []) todos
    match updateResult with
    | (None, _) -> Error "Todo not found"
    | (Some t, updatedTodo) ->
        todos <- updatedTodo
        Ok t
    

//Application
let app =
    cors defaultCORSConfig >=>
    choose
        [ GET >=> fetchEndpoint getEndpoint
          POST  >=> endpoint createEndpoint
          PUT >=> endpoint updateEndpoint ]

let config = { defaultConfig with bindings = [HttpBinding.createSimple HTTP "0.0.0.0" 3000]  }
[<EntryPoint>]
let main argv =
    startWebServer config app
    0
