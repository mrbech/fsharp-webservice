run:
	docker-compose up

fsi: 
	docker-compose run backend \
		dotnet /usr/share/dotnet/sdk/2.1.301/FSharp/fsi.exe --readline+ \
		--reference:/src/app/obj/Debug/netcoreapp2.1/app.dll
