module Main
open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open FSharp.Control.Tasks.V2.ContextInsensitive
open Giraffe

exception StatusError of (int * string)

type CreateTodo = {
    message: string
}

type UpdateTodo = { 
    id: int
    message: string 
}

//Endpoints
let getEndpoint: HttpHandler = 
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            return! json (Todo.getAll()) next ctx
        }

let bind<'T> (ctx: HttpContext) = task {
    try
        return! ctx.BindModelAsync<'T>()
    with
        | _ -> return! (raise (StatusError(400, "Failed to parse body")))
}

let createEndpoint: HttpHandler = 
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let! c = bind<CreateTodo> ctx
            let r = Todo.create (c.message)
            return! json r next ctx
        }

let updateEndpoint: HttpHandler = 
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let! u = bind<UpdateTodo> ctx
            return! json (Todo.update u.id u.message) next ctx
        }

let webApp = choose [
    GET >=> getEndpoint
    POST >=> createEndpoint
    PUT >=> updateEndpoint
]

//Application
let errorHandler (ex : Exception) (logger : ILogger) =
    match ex with
    | :? StatusError as se ->
        let (code, message) = se.Data0
        clearResponse >=> setStatusCode code >=> text message
    | _  -> 
        logger.LogError(EventId(), ex, "An unhandled exception has occurred while executing the request.")
        clearResponse >=> ServerErrors.INTERNAL_ERROR ex.Message

let configureApp (app : IApplicationBuilder) =
    app
        .UseGiraffeErrorHandler(errorHandler)
        .UseGiraffe webApp

let configureServices (services : IServiceCollection) =
    services.AddGiraffe() |> ignore

[<EntryPoint>]
let main _ =
    WebHostBuilder()
        .UseKestrel()
        .Configure(Action<IApplicationBuilder> configureApp)
        .ConfigureServices(configureServices)
        .UseUrls("http://*:3000")
        .Build()
        .Run()
    0
