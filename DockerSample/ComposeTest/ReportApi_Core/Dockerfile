#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
ENV LANG C.UTF-8
WORKDIR /app
COPY . /app/
ENTRYPOINT ["dotnet", "ReportAPI.dll"]