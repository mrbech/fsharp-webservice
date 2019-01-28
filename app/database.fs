module Database
open FSharp.Data.Sql

let [<Literal>] dbVendor = Common.DatabaseProviderTypes.POSTGRESQL
let [<Literal>] connString = "Host=database;Database=todo;Username=todo;Password=todo"
let [<Literal>] resPath = @"/root/.nuget/packages"

type sql = SqlDataProvider<
            ConnectionString = connString,
            DatabaseVendor = dbVendor,
            ResolutionPath = resPath,
            IndividualsAmount = 1000,
            UseOptionTypes = true>;;
type Context = sql.dataContext
type Todo = Context.``public.todosEntity``

let getTodos (ctx: Context) = ctx.Public.Todos |> Seq.toList