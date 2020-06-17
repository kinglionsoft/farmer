docker stop $(docker ps -a | grep "Exited" | awk '{print $1 }')
docker rm $(docker ps -a | grep "Exited" | awk '{print $1 }') 
docker rmi $(docker images | grep "none" | awk '{print $3}')  

docker rmi $(docker images registry.local.com/test/file-translate:1.[0-2]  | awk '{print $3'})
docker rmi registry.local.com/test/file-store:latest

docker rmi $(docker images registry.local.com/test/log-host:*  -q )

docker rmi $(docker images grpc-* -q )

docker rmi $(docker images -qa -f dangling=true 2>/dev/null) 2>/dev/null

docker rmi $(docker images istio/*  -q )
