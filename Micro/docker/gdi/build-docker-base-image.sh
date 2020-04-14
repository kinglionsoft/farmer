#!/bin/bash

# Create GDI+\TTF images for .NetCore

images=(runtime:2.1 runtime:2.2 runtime:3.0 aspnet:2.1 aspnet:2.2 aspnet:3.0 aspnet:3.1)
official=mcr.microsoft.com/dotnet/core/
yx=registry.local.com/dotnetcore/
proxy=http://192.168.1.123:11080/

rm -r gdi
mkdir gdi
cp sources.list ./gdi
for img in ${images[*]}
do
    # GDI+
    gdi=$yx$(echo $img | sed 's/:/-gdi:/')
    echo '========================='
    echo building $gdi
    if [[ "$(docker image ls -q $gdi 2> /dev/null)" != "" ]]; then
        echo $gdi exists
    else
        pushd gdi
        tee Dockerfile << EOF
FROM $official$img
ADD sources.list /etc/apt/
RUN apt-get update \
&& apt-get install -y --allow-unauthenticated \
    libc6-dev \
    libgdiplus \
    libx11-dev \
    && rm -rf /var/lib/apt/lists/*
EOF
        docker build -t $gdi .
        rm Dockerfile
        popd
    fi

    # TTF
    echo '========================='
    ttf=$yx$(echo $img | sed 's/:/-ttf:/')
    echo building $ttf
    if [[ "$(docker image ls -q $ttf 2> /dev/null)" != "" ]]; then
        echo $ttf exists
    else
        pushd font
        tee Dockerfile << EOF 
FROM $gdi
COPY ttf/* /usr/share/fonts/winFonts/    
EOF
        docker build -t $ttf .
        rm Dockerfile
        popd
    fi
done

