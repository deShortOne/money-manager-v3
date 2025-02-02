FROM be-base-image

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /App
COPY --from=be-base-image /App/out .
EXPOSE 8080

ENTRYPOINT ["dotnet", "MoneyTracker.Queries.API.dll"]
