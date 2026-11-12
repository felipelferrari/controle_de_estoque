FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["controle_de_estoque.csproj", "./"]
RUN dotnet restore "controle_de_estoque.csproj"

COPY . .
RUN dotnet publish "controle_de_estoque.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://0.0.0.0:10000
EXPOSE 10000

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "controle_de_estoque.dll"]
