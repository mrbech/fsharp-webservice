FROM microsoft/dotnet:2.2-sdk

RUN apt-get update
RUN apt-get install -y gnupg git procps

RUN curl -sL https://deb.nodesource.com/setup_11.x | bash -
RUN apt-get install -y nodejs

WORKDIR /
RUN git clone https://github.com/georgewfraser/fsharp-language-server
WORKDIR /fsharp-language-server

RUN npm install
RUN dotnet build -c Release

ARG PROJECT_DIR

ADD . $PROJECT_DIR/fsharp-webservice
WORKDIR $PROJECT_DIR/fsharp-webservice
RUN dotnet restore
