# FSharp Webservice
Attempt at using fsharp for webservices using dotnet core and docker. 

The project will explore:
- Web service libraries ([suave.io](https://suave.io/), ~~[suave
  swagger](https://rflechner.github.io/Suave.Swagger/)~~)
- Database migration and access (Postgres ~~using
  [SQLProvider](https://fsprojects.github.io/SQLProvider/)~~)
- Micro services (in mono project) with shared utility library 

## VIM: FSharp Language Server
This project contains the [FSharp Language
Server](https://github.com/georgewfraser/fsharp-language-server) running inside
docker. The following is an example of using it with vim-lsc, but it should work
with any VIM language server plugin. Theoretically it should also work with
other editors.

Install
[https://github.com/natebosch/vim-lsc](https://github.com/natebosch/vim-lsc) and
configure as you like.

First we need set an environment variable with the absolute path of the parent
directory of this projects directory, example: `PROJECT_DIR=$HOME/projects`

Then add the language server:

```
let g:lsc_server_commands = {'fsharp': 'docker-compose run fsharp-language-server'}
```

As long as you are running `vim` in this projects directory or any of its
subdirectories the language server should work, (this is due to how
docker-compose resolves the docker-compose.yml file).
