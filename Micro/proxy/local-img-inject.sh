LOCAL="registry.local.com"

usage() {
    echo "Usage:"
    echo "  pimg [commands] [yaml]"
    echo "Commands:"
    echo "  download   	pull the images which described in k8s YAML and push them to local docker registry"
    echo "  inject    	replace the images which described in k8s YAML to local images"
    echo "  pull    	pull the images which described in k8s YAML from the local docker registry"
    echo "  list        list the images which described in k8s YAML"
    exit 1
}

if [ $# -ne 2 ]; then
    usage
fi

if [ "$1" != "pull" -a "$1" != "inject" -a "$1" != "download" -a "$1" != "list" ]; then
    echo "unkown command: $1"
    usage
fi

command="$1"

if [ "$2" != "-" ]; then
    exec 0<$2;
fi


download() {
	echo "--------------------------------"
    echo "$1"
    
    # https://registry.local.com/api/repositories/gcr.io/linkerd-io/controller/tags/stable-2.3.1
    q=$(echo "$1" | sed 's/:/\/tags\//')
    url="https://$LOCAL/api/repositories/$q"
	echo "url: $url"
    httpCode=$(curl -s -o /dev/null -w "%{http_code}" "$url")
	echo "http code: $httpCode"
    if [ $httpCode -eq 200 ]; then
        echo "$1 is already exists."
    else
        echo "starting pull $1"
        localImg="$LOCAL/$1"
        echo "local image: $localImg"
        docker pull "$1" && docker tag "$1" "$localImg" && docker push "$localImg" && docker rmi -f "$1" && docker rmi -f "$localImg"
    fi
}

pull() {
    echo "--------------------------------"
    echo "$1"
    localImg="$LOCAL/$1"
    echo "local image: $localImg"
    docker pull "$localImg" && docker tag "$localImg" "$img"
}

# read and run 

IFSbak=$IFS
IFS="\n"

while read line
do    
    img=$(echo "$line" | awk '/ image: /{print $2}')
    if [ "$command" == "inject" ]; then
        if [ -n "$img" ]; then
            echo "$line" | sed "s/ image: / image: $LOCAL\//"
        else 
            echo "$line"
        fi

    elif [ -n "$img" ]; then
        if [ "$command" == "pull" ]; then
            pull "$img"
        elif [ "$command" == "download" ]; then 
            download "$img"
        elif [ "$command" == "list" ]; then
            echo  "$img"
        fi
    fi   

done<&0;
IFS=$IFSbak

