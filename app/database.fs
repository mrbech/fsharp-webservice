module Database
open System
open FSharp.Data.Sql

let [<Literal>] dbVendor = Common.DatabaseProviderTypes.POSTGRESQL
let [<Literal>] connString = "Host=database;Database=todo;Username=todo;Password=todo"

#if BUILD
let [<Literal>] resPath = __SOURCE_DIRECTORY__ + "/temp" 
#else
let [<Literal>] resPath = @"/root/.nuget/packages"
#endif

type sql = SqlDataProvider<
            ConnectionString = connString,
            DatabaseVendor = dbVendor,
            ResolutionPath = resPath,
            IndividualsAmount = 1000,
            UseOptionTypes = true>;;
type Context = sql.dataContext
type Todo = Context.``public.todosEntity``

let getContext() = sql.GetDataContext()

let getTodos (ctx: Context): Todo list = ctx.Public.Todos |> Seq.toList

let createTodo (ctx: Context) (message: string) = 
    let result: Todo = ctx.Public.Todos.``Create(created_at, message)``(DateTime.Now, message)
    ctx.SubmitUpdates()
    result

let updateTodo (ctx: Context) id message =
    let result = query {
        for t: Todo in ctx.Public.Todos do
            where (t.Id = id)
            select (Some t)
            exactlyOneOrDefault
    }
    match result with
    | Some t -> 
            t.Message <- message
            ctx.SubmitUpdates()
            Some(t)
    | None -> None
