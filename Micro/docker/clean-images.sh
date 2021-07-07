#!/bin/bash

registry='ytzx2020'
keeps=('registry.ytzx.com/java-8:server-jre-8u202-zh-slim')

docker stop $(docker ps -a | grep "Exited" | awk '{print $1 }') 2>/dev/null
docker rm $(docker ps -a | grep "Exited" | awk '{print $1 }') 2>/dev/null
docker rmi $(docker images | grep "none" | awk '{print $3}') 2>/dev/null
docker rmi $(docker images -qa -f dangling=true 2>/dev/null) 2>/dev/null


images=$(docker images | grep $registry | awk '{print $1":"$2}'| sort -r)

# for img in $images; do echo $img;  arr=(${img//:/ }); echo ${arr[0]}; break; done

last=''
for img in $images
do
    echo "image: $img"
    arr=(${img//:/ })
    imgName=${arr[0]}

    if [[ "${keeps[@]}" =~ "${imgName}" ]]; then
        echo "keep"
    elif [ "$last" = "$imgName" ]; then
        echo "remove"
        docker rmi $img
    else
        # keep the latest
        echo "keep"
        last=$imgName
    fi
done