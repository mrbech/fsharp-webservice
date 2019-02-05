module Database
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
