#
# *************************************************
# Copyright (c) 2019, Grindrod Bank Limited
# License MIT: https://opensource.org/licenses/MIT
# **************************************************
#

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-alpine AS build
WORKDIR /app/scrubfu

# copy csproj and restore as distinct layers
COPY ./src/ ./src
COPY ./tests ./tests
COPY ./scrubfu.sln ./scrubfu.sln
COPY ./scrubfu.alias ./scrubfu.alias
WORKDIR /app/scrubfu
RUN dotnet restore

WORKDIR /app/scrubfu/src/scrubfu
# add IL Linker package
RUN dotnet add package ILLink.Tasks -v 0.1.5-preview-1841731 -s https://dotnet.myget.org/F/dotnet-core/api/v3/index.json
RUN dotnet publish -c Release -r linux-musl-x64 -o out /p:ShowLinkerSizeComparison=true 

FROM mcr.microsoft.com/dotnet/core/runtime-deps:2.2-alpine AS runtime
WORKDIR /app
COPY --from=build /app/scrubfu/src/scrubfu/out/ ./
ENTRYPOINT ["./scrubfu"]