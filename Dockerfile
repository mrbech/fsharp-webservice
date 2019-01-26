FROM fsharp:10.2.1-netcore

RUN apt-get update
RUN apt-get install -y gnupg git

RUN curl -sL https://deb.nodesource.com/setup_11.x | bash -
RUN apt-get install -y nodejs

WORKDIR /
RUN git clone https://github.com/mrbech/fsharp-language-server
WORKDIR /fsharp-language-server

RUN npm install
RUN dotnet build -c Release

ADD . /home/mrb/projects/fsharp-webservice
WORKDIR /home/mrb/projects/fsharp-webservice/app
RUN dotnet build
