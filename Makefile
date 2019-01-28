.PHONY: run

run:
	docker-compose up database backend

fsi: 
	docker-compose run backend \
		bash -c 'dotnet /usr/share/dotnet/sdk/`dotnet --version`/FSharp/fsi.exe --readline+'

db-access:
	docker-compose exec -e COLUMNS="`tput cols`" -e LINES="`tput lines`" database psql -U todo
