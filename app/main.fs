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

type CreateTodo = {
    message: string
}

type UpdateTodo = { 
    id: int
    message: string 
}

//Endpoints
let getEndpoint () = Todo.getAll()

let createEndpoint (c: CreateTodo) = Todo.create c.message |> Ok

let updateEndpoint (u: UpdateTodo) = Todo.update u.id u.message |> Ok

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
