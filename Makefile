.PHONY: run

run:
	docker-compose up database backend
fsi:
	docker-compose run fsi
db-access:
	docker-compose exec -e COLUMNS="`tput cols`" -e LINES="`tput lines`" database psql -U todo
