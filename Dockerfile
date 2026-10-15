FROM mcr.microsoft.com/dotnet/sdk:10.0

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    DOTNET_NOLOGO=1

WORKDIR /workspace

CMD ["bash"]
