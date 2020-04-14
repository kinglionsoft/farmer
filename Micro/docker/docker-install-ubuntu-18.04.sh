curl -fsSL https://get.docker.com -o get-docker.sh && sudo sh get-docker.sh

# /etc/docker/daemon.json
{
    "insecure-registries" : ["registry.local.com"],
    "registry-mirrors": ["https://registry.docker-cn.com"]
}
# host
172.16.0.249 registry.local.com
172.16.0.21 consul.local.com

# docker login