#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
ENV LANG C.UTF-8
WORKDIR /app
RUN apt-get update && apt-get install -y apt-utils libgdiplus libc6-dev
RUN ln -s /usr/lib/libgdiplus.so /usr/lib/gdiplus.dll
COPY . /app/
ENTRYPOINT ["dotnet", "ReportAPI.dll"]