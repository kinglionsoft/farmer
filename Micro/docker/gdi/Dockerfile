FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim
RUN apt-get -o Acquire::http::proxy="http://192.168.2.15:10081" update \
    && apt-get -o Acquire::http::proxy="http://192.168.2.15:10081" install -y --allow-unauthenticated \
        libc6-dev \
        libgdiplus \
        libx11-dev \
    && rm -rf /var/lib/apt/lists/*